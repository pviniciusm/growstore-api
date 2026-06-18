using GrowStore.Application.Auth.Interfaces;

namespace GrowStore.Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        //private readonly IAccountRepository _accountRepository;

        public AuthService(/*IAccountRepository accountRepository*/)
        {
            //_accountRepository = accountRepository;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            //var account = await _accountRepository.GetByEmailAsync(email);

            //if (account is null)
            //{
            //    throw new UnauthorizedAccessException();
            //}

            return "token";
        }

        public async Task<string> GenerateTestToken()
        {
            return "test-token";
        }
    }
}
