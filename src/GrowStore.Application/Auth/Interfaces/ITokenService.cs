using GrowStore.Application.Auth.DTOs;

namespace GrowStore.Application.Auth.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(TokenRequestDto tokenRequestDto);
    }
}
