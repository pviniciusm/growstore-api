using GrowStore.Application.Users.DTOs;

namespace GrowStore.Application.Users.Interfaces
{
    public interface IUserService
    {
        Task<ResponseUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<ResponseUserDto> GetUserByIdAsync(Guid id);
        Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task DeleteUserAsync(Guid id);
    }
}
