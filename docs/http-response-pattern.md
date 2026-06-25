# Padrao de respostas HTTP

Este projeto usa um padrao reutilizavel para separar erros de aplicacao de respostas HTTP.

Resumo da regra:

- A camada `Application` retorna `Result` ou `Result<T>`.
- A camada `Application` nao conhece HTTP, controllers, `ActionResult` ou `ProblemDetails`.
- A camada `API` converte `Result` em resposta HTTP usando `ToActionResult`.
- Excecoes inesperadas sao tratadas pelo `ExceptionHandlingMiddleware`.

## Arquivos principais

| Arquivo | Responsabilidade |
| --- | --- |
| `src/GrowStore.Application/Common/Results/Result.cs` | Representa sucesso ou falha de uma operacao. |
| `src/GrowStore.Application/Common/Errors/Error.cs` | Representa o erro de negocio/aplicacao. |
| `src/GrowStore.Application/Common/Errors/ErrorType.cs` | Define a categoria do erro. |
| `src/GrowStore.API/Extensions/ResultExtensions.cs` | Converte `Result` em resposta HTTP. |
| `src/GrowStore.API/Middlewares/ExceptionHandlingMiddleware.cs` | Padroniza respostas para excecoes nao tratadas. |

## Como usar na Application

Services devem retornar `Result<T>` quando precisam devolver dados:

```csharp
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;

public async Task<Result<ResponseUserDto>> GetByIdAsync(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);

    if (user is null)
    {
        return Result<ResponseUserDto>.Failure(
            Error.NotFound("User.NotFound", "User not found."));
    }

    var response = new ResponseUserDto
    {
        Id = user.Id,
        Name = user.Name,
        Cpf = user.Cpf,
        BirthDate = user.BirthDate,
        Role = user.Role
    };

    return Result<ResponseUserDto>.Success(response);
}
```

Services devem retornar `Result` quando nao precisam devolver dados:

```csharp
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;

public async Task<Result> DeleteAsync(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);

    if (user is null)
    {
        return Result.Failure(
            Error.NotFound("User.NotFound", "User not found."));
    }

    await _userRepository.DeleteAsync(user);

    return Result.Success();
}
```

## Como usar no controller

Controllers devem receber o `Result` do service e converter usando `ToActionResult(this)`.

```csharp
using GrowStore.API.Extensions;
using Microsoft.AspNetCore.Mvc;

[HttpGet("{id}")]
public async Task<ActionResult<ResponseUserDto>> GetById(Guid id)
{
    var result = await _userService.GetByIdAsync(id);

    return result.ToActionResult(this);
}
```

Use `[ProducesResponseType]` para documentar no Swagger/OpenAPI quais respostas o endpoint pode retornar:

```csharp
using GrowStore.API.Extensions;
using GrowStore.Application.Users.DTOs;
using Microsoft.AspNetCore.Mvc;

[HttpGet("{id:guid}")]
[ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ResponseUserDto>> GetById(Guid id)
{
    var result = await _userService.GetByIdAsync(id);

    return result.ToActionResult(this);
}
```

Neste exemplo:

- `200 OK` retorna `ResponseUserDto`.
- `400 Bad Request` retorna `ProblemDetails`, por exemplo para `Error.Validation`.
- `404 Not Found` retorna `ProblemDetails`, por exemplo para `Error.NotFound`.

Para operacoes sem retorno:

```csharp
using GrowStore.API.Extensions;
using Microsoft.AspNetCore.Mvc;

[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<ActionResult> Delete(Guid id)
{
    var result = await _userService.DeleteAsync(id);

    return result.ToActionResult(this);
}
```

## Mapeamento HTTP

| `ErrorType` | Status HTTP | Quando usar |
| --- | --- | --- |
| `Validation` | `400 Bad Request` | Dados de entrada invalidos. |
| `NotFound` | `404 Not Found` | Recurso nao encontrado. |
| `Conflict` | `409 Conflict` | Conflito com estado atual, como CPF ou email ja cadastrado. |
| `Unauthorized` | `401 Unauthorized` | Usuario nao autenticado. |
| `Forbidden` | `403 Forbidden` | Usuario autenticado sem permissao. |
| `Failure` | `500 Internal Server Error` | Falha inesperada ou erro generico. |

## Formato de erro

Erros sao retornados no formato `ProblemDetails`:

```json
{
  "type": "https://httpstatuses.com/404",
  "title": "Resource not found",
  "status": 404,
  "detail": "User not found.",
  "code": "User.NotFound"
}
```

O campo `code` deve ser estavel e especifico, porque pode ser usado por front-end, testes e documentacao.

## Convencao para codigos de erro

Use o formato:

```text
<Entity>.<ErrorName>
```

Exemplos:

```text
User.NotFound
User.CpfAlreadyExists
Account.EmailAlreadyExists
Product.OutOfStock
Cart.Empty
Order.InvalidStatus
```

## Boas praticas

- Retorne `Result<T>.Success(value)` quando a operacao concluiu com dados.
- Retorne `Result.Success()` quando a operacao concluiu sem dados.
- Retorne `Result<T>.Failure(error)` ou `Result.Failure(error)` para erros esperados.
- Use `DomainException` apenas para proteger invariantes das entidades.
- Nao retorne `ActionResult`, `ProblemDetails` ou status HTTP na camada `Application`.
- Nao retorne sempre `200 OK` com `success: false`; o status HTTP deve representar o resultado real.
- Nao exponha entidades diretamente em controllers; prefira DTOs de resposta.

## Excecoes

O middleware `ExceptionHandlingMiddleware` trata:

- `DomainException` como `400 Bad Request`.
- Qualquer outra excecao como `500 Internal Server Error`.

Erros esperados de fluxo, como recurso inexistente ou conflito de cadastro, devem usar `Result` em vez de excecao.
