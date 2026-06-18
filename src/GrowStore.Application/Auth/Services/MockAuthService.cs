using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrowStore.Application.Auth.Interfaces;

namespace GrowStore.Application.Auth.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<string> LoginAsync(string email, string password)
        {
            return Task.FromResult("mock-jwt-token");
        }

        public Task<string> GenerateTestToken()
        {
            return Task.FromResult("mock-test-token");
        }
    }
}