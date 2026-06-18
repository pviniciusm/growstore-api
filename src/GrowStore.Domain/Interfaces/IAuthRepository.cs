namespace GrowStore.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> LoginAsync(string email, string password);
    }
}
