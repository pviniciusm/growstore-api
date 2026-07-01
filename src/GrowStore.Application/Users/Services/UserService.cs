using FluentValidation;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Shared.Rules;
using GrowStore.Application.Users.DTOs;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateUserDto> _createUserValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;

        public UserService(
            IUserRepository userRepository,
            IValidator<CreateUserDto> createUserValidator,
            IValidator<UpdateUserDto> updateUserValidator)
        {
            _userRepository = userRepository;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
        }

        public async Task<Result<ResponseUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<ResponseUserDto>.Failure(
                    Error.Validation("User.Validation", errorMessage));
            }

            var formattedCpf = UserValidationRules.FormatCpf(createUserDto.Cpf);
            var user = User.Create(createUserDto.Name, formattedCpf, createUserDto.BirthDate, createUserDto.Role);
            await _userRepository.AddAsync(user);

            return Result<ResponseUserDto>.Success(MapToResponseModel(user));
        }

        public async Task<Result<ResponseUserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
                return Result<ResponseUserDto>.Failure(
                    Error.NotFound("User.NotFound", $"User not found by ID: {id}"));

            return Result<ResponseUserDto>.Success(MapToResponseModel(user));
        }

        public async Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure(
                    Error.Validation("User.Validation", errorMessage));
            }

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
                return Result.Failure(
                    Error.NotFound("User.NotFound", $"User not found by ID: {id}"));

            var formattedCpf = UserValidationRules.FormatCpf(updateUserDto.Cpf);

            user.Update(
                updateUserDto.Name,
                formattedCpf,
                updateUserDto.BirthDate,
                updateUserDto.Role
            );

            await _userRepository.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
                return Result.Failure(
                    Error.NotFound("User.NotFound", $"User not found by ID: {id}"));

            await _userRepository.DeleteAsync(id);

            return Result.Success();
        }

        private static ResponseUserDto MapToResponseModel(User user)
        {
            return new ResponseUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Cpf = user.Cpf,
                BirthDate = user.BirthDate,
                Role = user.Role,
            };
        }
    }
}