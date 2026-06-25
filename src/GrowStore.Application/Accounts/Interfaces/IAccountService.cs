using System;
using System.Threading.Tasks;
using GrowStore.Application.Accounts.DTOs;

namespace GrowStore.Application.Accounts.Interfaces;

public interface IAccountService
{
    Task<ResponseAccountDto> RegisterAsync(CreateAccountDto request);
    Task<string> LoginAsync(string email, string password);
    Task<ResponseAccountDto> GetProfileAsync(Guid accountId);
}