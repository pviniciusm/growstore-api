using System;
using System.Threading.Tasks;
using GrowStore.Application.Accounts.DTOs; 

namespace GrowStore.Application.Accounts;

public class AccountService
{
    public AccountService()
    {
        // Futuramente, injetaremos IAccountRepository, IPasswordHasher, ITokenService 
    }

    public async Task<ResponseAccountDto> RegisterAsync(CreateAccountDto request)
    {
        // TODO: Implementar lógica de cadastro 
        // 1. Verificar se e-mail já existe
        // 2. Criptografar a senha
        // 3. Salvar no banco de dados
        // 4. Retornar os dados convertidos para ResponseAccountDto
        throw new NotImplementedException("Cadastro ainda não implementado.");
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        // TODO: Implementar lógica de login
        // 1. Buscar usuário por e-mail
        // 2. Validar o hash da senha
        // 3. Gerar e retornar o Token JWT
        throw new NotImplementedException("Login ainda não implementado.");
    }

    public async Task<ResponseAccountDto> GetProfileAsync(Guid accountId)
    {
        // TODO: Implementar busca de perfil
        // 1. Buscar dados da Account
        // 2. Converter para ResponseAccountDto e retornar
        throw new NotImplementedException("Busca de perfil ainda não implementada.");
    }
}