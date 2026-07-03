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