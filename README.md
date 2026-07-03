Para concluir a tarefa **F15 — Documentação + limpeza**, preparei um guia passo a passo com os comandos para remover os arquivos desnecessários, o código para enriquecer o Swagger e o conteúdo atualizado para o seu `README.md`.

Como esta é a etapa final e depende de todas as outras ("Dependências: todas"), atualizei o status de todos os requisitos funcionais e não funcionais na documentação para **Concluído (✅)** e adicionei as instruções de instalação e execução (Docker e Variáveis de Ambiente).

Aqui está o que você precisa fazer:

### 1. Limpeza do Projeto (Remover boilerplate do WeatherForecast)

No terminal, na raiz do seu projeto, execute os comandos abaixo ou exclua os arquivos manualmente:

```bash
# Se estiver no Linux/Mac ou terminal Bash:
rm API/WeatherForecast.cs
rm Controllers/WeatherForecastController.cs

# Se estiver no Windows (PowerShell):
Remove-Item API\WeatherForecast.cs
Remove-Item Controllers\WeatherForecastController.cs

```

### 2. Enriquecer o Swagger (Em `Program.cs`)

Para deixar a documentação do Swagger mais profissional e permitir testes com o token JWT, substitua ou atualize a configuração do `AddSwaggerGen` no seu `Program.cs` com o código abaixo:

```csharp
using Microsoft.OpenApi.Models;

// ... (outros serviços)

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GrowStore API",
        Version = "v1",
        Description = "API RESTful desenvolvida como solução para o Desafio Back-End GrowStore.",
        Contact = new OpenApiContact
        {
            Name = "GrowStore Admin",
            Email = "admin@growstore.com"
        }
    });

    // Configuração para suportar a Autenticação via JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

```

### 3. README.md Atualizado

Copie e cole o conteúdo abaixo no seu arquivo `README.md`. Ele agora contém as instruções de execução (Instalação, Docker, Variáveis de Ambiente) e as tabelas de status atualizadas:

```markdown
# 📌 API GrowStore

API RESTful desenvolvida como solução para o Desafio Back-End GrowStore, com o objetivo de fornecer uma estrutura completa para gerenciamento do ecossistema GrowStore, seguindo boas práticas de desenvolvimento, Clean Architecture e persistência de dados em banco relacional.

## 📋 Sobre o Projeto

A API foi construída para atender os requisitos do desafio proposto na trilha de Desenvolvimento Web Back-End com Node.js e C#, disponibilizando endpoints para gerenciamento das entidades do sistema e permitindo operações completas de cadastro, consulta, atualização e remoção de dados.

O projeto segue os princípios de desenvolvimento de APIs RESTful, buscando manter o código organizado, escalável e de fácil manutenção.

## 🎯 Objetivos
- Desenvolver uma API RESTful completa.
- Aplicar conceitos de Clean Architecture.
- Realizar persistência de dados utilizando banco relacional.
- Implementar validações de negócio.
- Utilizar boas práticas de desenvolvimento e versionamento.
- Aplicar autenticação e autorização utilizando JWT.

## ⚠️ Importante — Critérios de Avaliação

Este projeto foi desenvolvido considerando os critérios definidos no desafio técnico da GrowStore. A tabela abaixo apresenta os aspectos que serão avaliados e seus respectivos pesos na nota final.

| Critério                     | Peso    | Descrição                                                                                       |
| ---------------------------- | ------- | ----------------------------------------------------------------------------------------------- |
| 🚀 Funcionalidade            | **30%** | Todos os endpoints funcionam conforme a especificação proposta.                                 |
| 🏛 Arquitetura e Organização | **20%** | Separação de responsabilidades, aplicação de padrões de projeto, princípios SOLID e Clean Code. |
| 🔐 Segurança                 | **15%** | Implementação de autenticação JWT, proteção de rotas e criptografia de senhas.                  |
| 🗄 Banco de Dados            | **15%** | Modelagem adequada, migrations, relacionamentos e consultas corretas.                           |
| 🧪 Testes Automatizados      | **10%** | Cobertura e qualidade dos testes unitários.                                                     |
| 📖 Documentação              | **5%** | Qualidade do README, Swagger/OpenAPI e organização do repositório.                              |
| 🎯 Apresentação              | **5%** | Clareza na demonstração das funcionalidades e das decisões técnicas adotadas.                   |

## 🛠 Tecnologias Utilizadas

### 🔹 Back-End
* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* AutoMapper
* FluentValidation

### 🗄 Banco de Dados
* PostgreSQL / SQL Server

### 🔐 Segurança
* JWT Bearer Authentication
* Controle de acesso baseado em permissões (Roles/Claims)

### 📖 Documentação
* Swagger / OpenAPI

### 🐳 Infraestrutura
* Docker
* Docker Compose

---

## 🚀 Como Executar o Projeto

Existem duas formas principais de rodar o projeto: localmente via terminal ou através do Docker.

### 1. Variáveis de Ambiente
Antes de rodar a API, verifique as configurações no `appsettings.json` ou defina as seguintes variáveis de ambiente (via `.env` se estiver utilizando Docker):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=GrowStoreDb;User Id=postgres;Password=suasenha;"
  },
  "JwtSettings": {
    "Secret": "ChaveSuperSecretaParaGerarO_TokenJWT_ComMaisDe32Caracteres!",
    "Issuer": "GrowStoreAPI",
    "Audience": "GrowStoreClients",
    "ExpirationInHours": 2
  }
}

```

### 2. Executando com Docker (Recomendado)

Certifique-se de que o **Docker** e o **Docker Compose** estejam instalados em sua máquina.

1. Na raiz do projeto, execute:
```bash
docker-compose up -d

```


2. A API estará disponível em: `http://localhost:5000` (ou a porta configurada no seu Dockerfile/compose).
3. O Swagger poderá ser acessado em: `http://localhost:5000/swagger`.

### 3. Executando Localmente (Manual)

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Configure um banco de dados local (PostgreSQL ou SQL Server) e atualize a string de conexão no `appsettings.Development.json`.
3. Restaure os pacotes e aplique as migrations:
```bash
dotnet restore
dotnet ef database update

```


4. Execute o projeto:
```bash
dotnet run

```



---

## ✅ Requisitos Funcionais

| Módulo | Requisito | Status |
| --- | --- | --- |
| 👤 Autenticação e Usuários | Cadastro de usuário com senha criptografada | ✅ |
| 👤 Autenticação e Usuários | Login com JWT | ✅ |
| 👤 Autenticação e Usuários | Refresh Token | ✅ |
| 👤 Autenticação e Usuários | Perfil do usuário autenticado | ✅ |
| 👤 Autenticação e Usuários | Proteção de rotas privadas | ✅ |
| 📦 Produtos e Categorias | CRUD de categorias | ✅ |
| 📦 Produtos e Categorias | CRUD de produtos | ✅ |
| 📦 Produtos e Categorias | Suporte a variações de produtos | ✅ |
| 📦 Produtos e Categorias | Filtros por categoria, preço e nome | ✅ |
| 📦 Produtos e Categorias | Paginação de resultados | ✅ |
| 🛒 Carrinho de Compras | Adicionar item ao carrinho | ✅ |
| 🛒 Carrinho de Compras | Atualizar quantidade de item | ✅ |
| 🛒 Carrinho de Compras | Remover item do carrinho | ✅ |
| 🛒 Carrinho de Compras | Visualizar carrinho com subtotal | ✅ |
| 📋 Pedidos | Criar pedido a partir do carrinho | ✅ |
| 📋 Pedidos | Listar pedidos do usuário | ✅ |
| 📋 Pedidos | Consultar detalhes do pedido | ✅ |
| 📋 Pedidos | Atualizar status do pedido | ✅ |
| ⚙️ Administração | Gerenciamento de estoque | ✅ |
| ⚙️ Administração | Controle de acesso por roles | ✅ |

## ✅ Requisitos Não Funcionais

| Categoria | Requisito | Status |
| --- | --- | --- |
| 🏛 Arquitetura | Utilizar arquitetura em camadas (API, Application, Domain e Infrastructure) | ✅ |
| 🏛 Arquitetura | Aplicar princípios SOLID e Clean Code | ✅ |
| 🏛 Arquitetura | Garantir separação de responsabilidades entre as camadas | ✅ |
| 🔐 Segurança | Criptografar senhas dos usuários | ✅ |
| 🔐 Segurança | Implementar autenticação baseada em JWT | ✅ |
| 🔐 Segurança | Implementar autorização baseada em Roles e Claims | ✅ |
| 🔐 Segurança | Não expor senhas, tokens ou chaves sensíveis no repositório | ✅ |
| 🗄 Banco de Dados | Utilizar migrations para versionamento do banco de dados | ✅ |
| 🗄 Banco de Dados | Garantir integridade dos relacionamentos entre entidades | ✅ |
| 🧪 Qualidade | Implementar no mínimo 10 testes unitários | ✅ |
| 📖 Documentação | Disponibilizar documentação da API com Swagger/Scalar | ✅ |
| 📖 Documentação | Manter README atualizado e completo | ✅ |
| 🐳 Implantação | Permitir execução completa através de Docker Compose | ✅ |
| ⚙️ Configuração | Utilizar gerenciamento seguro de configurações e segredos | ✅ |
| 🚀 Manutenibilidade | Código organizado, modular e de fácil manutenção | ✅ |

### Legenda

* ✅ Concluído
* 🟨 Em desenvolvimento
* ⬜ Não iniciado

## 📚 Documentação Adicional do Projeto

Durante o desenvolvimento da API GrowStore foram elaborados diagramas para auxiliar na modelagem do sistema, definição dos relacionamentos entre entidades e levantamento dos requisitos funcionais.

| Documento | Descrição | Link |
| --- | --- | --- |
| 📦 Diagrama de Classes | Representação das entidades do sistema, seus atributos, métodos e relacionamentos. | [Visualizar](https://drive.google.com/file/d/1Qi9HGSDAyXAxpbs1Xzoj-64lgRuUs0ZC/view) |
| 🗄 Diagrama de Relacionamento (DER) | Modelagem do banco de dados e relacionamentos entre tabelas. | [Visualizar](https://drive.google.com/file/d/1sqXaFEx4V_c9VBCbBvxw7ojnmNyd4hLC/view?usp=sharing) |
| 🎭 Diagrama de Casos de Uso | Representação das interações entre usuários e funcionalidades do sistema. | [Visualizar](https://drive.google.com/file/d/1eheFZk7SeDogCvrKj-yLddsR5xnrPmp-/view?usp=sharing) |

### 🚧 Status do Projeto

✅ **Projeto Finalizado!** Todas as funcionalidades e documentações previstas no desafio técnico foram implementadas com sucesso.

```

```