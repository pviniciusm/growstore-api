using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Common.Results;

namespace GrowStore.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<AuthResponseDto>> RefreshAsync(RefreshTokenRequestDto request);
}