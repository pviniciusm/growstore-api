using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Settings;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Microsoft.Extensions.Options;

namespace GrowStore.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        if (await _accountRepository.EmailExistsAsync(request.Email))
        {
            return Result<AuthResponseDto>.Failure(
                Error.Conflict("Account.EmailAlreadyExists", "Email already registered."));
        }

        // Todo novo cadastro entra como CUSTOMER por padrao.
        var user = User.Create(request.Name, request.Cpf, request.BirthDate, UserRole.CUSTOMER);
        await _userRepository.AddAsync(user);

        var passwordHash = _passwordHasher.Hash(request.Password);
        var account = new Account(user.Id, request.Email, passwordHash);
        await _accountRepository.AddAsync(account);

        var response = await BuildAuthResponseAsync(account, user);
        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var account = await _accountRepository.GetByEmailAsync(request.Email);

        if (account is null || !_passwordHasher.Verify(request.Password, account.Password))
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var user = await _userRepository.GetUserByIdAsync(account.UserId);

        if (user is null)
        {
            return Result<AuthResponseDto>.Failure(
                Error.NotFound("User.NotFound", "User not found for this account."));
        }

        var response = await BuildAuthResponseAsync(account, user);
        return Result<AuthResponseDto>.Success(response);
    }

    public async Task<Result<AuthResponseDto>> RefreshAsync(RefreshTokenRequestDto request)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (stored is null || !stored.IsActive)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));
        }

        stored.Revoke();
        await _refreshTokenRepository.UpdateAsync(stored);

        var account = await _accountRepository.GetByIdAsync(stored.AccountId);
        var user = account is null ? null : await _userRepository.GetUserByIdAsync(account.UserId);

        if (account is null || user is null)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token."));
        }

        var response = await BuildAuthResponseAsync(account, user);
        return Result<AuthResponseDto>.Success(response);
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(Account account, User user)
    {
        var accessToken = await _tokenService.GenerateToken(new TokenRequestDto
        {
            AccountId = account.Id,
            UserId = user.Id,
            Email = account.Email,
            Role = user.Role
        });

        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken(
            account.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccountId = account.Id,
            UserId = user.Id,
            Name = user.Name,
            Email = account.Email,
            Role = user.Role
        };
    }
}