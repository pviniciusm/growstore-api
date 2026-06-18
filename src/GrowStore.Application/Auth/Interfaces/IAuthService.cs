namespace GrowStore.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> GenerateTestToken();
    }
}
