using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Users.DTOs;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<ResponseUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = User.Create(createUserDto.Name, createUserDto.Cpf, createUserDto.BirthDate, createUserDto.Role);
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
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
                return Result.Failure(
                    Error.NotFound("User.NotFound", $"User not found by ID: {id}"));

            user.Update(
                updateUserDto.Name,
                updateUserDto.Cpf,
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