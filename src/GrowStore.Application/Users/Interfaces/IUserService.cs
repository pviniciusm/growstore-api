using GrowStore.Application.Common.Results;
using GrowStore.Application.Users.DTOs;

namespace GrowStore.Application.Users.Interfaces
{
    public interface IUserService
    {
        Task<Result<ResponseUserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result<ResponseUserDto>> GetUserByIdAsync(Guid id);
        Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<Result> DeleteUserAsync(Guid id);
    }
}