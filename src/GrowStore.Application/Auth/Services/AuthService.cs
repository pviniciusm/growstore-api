using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Auth.Services
{
    public class AuthService
    {
        private readonly IAccountRepository _accountRepository;

        public AuthService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
        
        //var account = await _accountRepository.GetAccountByEmailAsync(email);

        // if (account is null)
        //     throw new UnauthorizedAccessException();
            return "token";
        }

        public async Task<string> GenerateTestToken()
        {
            // Implementação
            return "test-token";
        }
    }
}