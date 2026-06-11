# 📌 API GrowStore

API RESTful desenvolvida como solução para o Desafio Back-End GrowStore, com o objetivo de fornecer uma estrutura completa para gerenciamento do ecossistema GrowStore, seguindo boas práticas de desenvolvimento, Clean Architecture e persistência de dados em banco relacional.

## 📋 Sobre o Projeto

A API será construída para atender os requisitos do desafio proposto na trilha de Desenvolvimento Web Back-End com Node.js e C#, disponibilizando endpoints para gerenciamento das entidades do sistema e permitindo operações completas de cadastro, consulta, atualização e remoção de dados.

O projeto segue os princípios de desenvolvimento de APIs RESTful, buscando manter o código organizado, escalável e de fácil manutenção.

## 🎯 Objetivos
Desenvolver uma API RESTful completa.
Aplicar conceitos de Clean Architecture.
Realizar persistência de dados utilizando banco relacional.
Implementar validações de negócio.
Utilizar boas práticas de desenvolvimento e versionamento.
Aplicar autenticação e autorização utilizando JWT.

## ⚠️ Importante — Critérios de Avaliação

Este projeto foi desenvolvido considerando os critérios definidos no desafio técnico da GrowStore. A tabela abaixo apresenta os aspectos que serão avaliados e seus respectivos pesos na nota final.

| Critério                     | Peso    | Descrição                                                                                       |
| ---------------------------- | ------- | ----------------------------------------------------------------------------------------------- |
| 🚀 Funcionalidade            | **30%** | Todos os endpoints funcionam conforme a especificação proposta.                                 |
| 🏛 Arquitetura e Organização | **20%** | Separação de responsabilidades, aplicação de padrões de projeto, princípios SOLID e Clean Code. |
| 🔐 Segurança                 | **15%** | Implementação de autenticação JWT, proteção de rotas e criptografia de senhas.                  |
| 🗄 Banco de Dados            | **15%** | Modelagem adequada, migrations, relacionamentos e consultas corretas.                           |
| 🧪 Testes Automatizados      | **10%** | Cobertura e qualidade dos testes unitários.                                                     |
| 📖 Documentação              | **5%**  | Qualidade do README, Swagger/OpenAPI e organização do repositório.                              |
| 🎯 Apresentação              | **5%**  | Clareza na demonstração das funcionalidades e das decisões técnicas adotadas.                   |

### 📌 Observação

Durante o desenvolvimento, todas as funcionalidades e decisões arquiteturais foram planejadas com base nesses critérios, buscando garantir qualidade, organização, segurança e aderência às boas práticas de desenvolvimento Back-End.


## 🛠 Tecnologias Utilizadas

### 🔹 Back-End

* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* AutoMapper
* FluentValidation

### 🗄 Banco de Dados

* PostgreSQL/SQL Server

### 🔐 Segurança

* JWT Bearer Authentication
* Controle de acesso baseado em permissões

### 📖 Documentação

* Swagger / OpenAPI

### 🐳 Infraestrutura

* Docker
* Docker Compose

## Requisitos
### Funcionais
## ✅ Requisitos Funcionais

| Módulo                     | Requisito                                   | Status |
| -------------------------- | ------------------------------------------- | ------ |
| 👤 Autenticação e Usuários | Cadastro de usuário com senha criptografada | ⬜      |
| 👤 Autenticação e Usuários | Login com JWT                               | ⬜      |
| 👤 Autenticação e Usuários | Refresh Token                               | ⬜      |
| 👤 Autenticação e Usuários | Perfil do usuário autenticado               | ⬜      |
| 👤 Autenticação e Usuários | Proteção de rotas privadas                  | ⬜      |
| 📦 Produtos e Categorias   | CRUD de categorias                          | ⬜      |
| 📦 Produtos e Categorias   | CRUD de produtos                            | ⬜      |
| 📦 Produtos e Categorias   | Suporte a variações de produtos             | ⬜      |
| 📦 Produtos e Categorias   | Filtros por categoria, preço e nome         | ⬜      |
| 📦 Produtos e Categorias   | Paginação de resultados                     | ⬜      |
| 🛒 Carrinho de Compras     | Adicionar item ao carrinho                  | ⬜      |
| 🛒 Carrinho de Compras     | Atualizar quantidade de item                | ⬜      |
| 🛒 Carrinho de Compras     | Remover item do carrinho                    | ⬜      |
| 🛒 Carrinho de Compras     | Visualizar carrinho com subtotal            | ⬜      |
| 📋 Pedidos                 | Criar pedido a partir do carrinho           | ⬜      |
| 📋 Pedidos                 | Listar pedidos do usuário                   | ⬜      |
| 📋 Pedidos                 | Consultar detalhes do pedido                | ⬜      |
| 📋 Pedidos                 | Atualizar status do pedido                  | ⬜      |
| ⚙️ Administração           | Gerenciamento de estoque                    | ⬜      |
| ⚙️ Administração           | Controle de acesso por roles                | ⬜      |

## ✅ Requisitos Não Funcionais

| Categoria           | Requisito                                                                   | Status |
| ------------------- | --------------------------------------------------------------------------- | ------ |
| 🏛 Arquitetura      | Utilizar arquitetura em camadas (API, Application, Domain e Infrastructure) | ⬜      |
| 🏛 Arquitetura      | Aplicar princípios SOLID e Clean Code                                       | ⬜      |
| 🏛 Arquitetura      | Garantir separação de responsabilidades entre as camadas                    | ⬜      |
| 🔐 Segurança        | Criptografar senhas dos usuários                                            | ⬜      |
| 🔐 Segurança        | Implementar autenticação baseada em JWT                                     | ⬜      |
| 🔐 Segurança        | Implementar autorização baseada em Roles e Claims                           | ⬜      |
| 🔐 Segurança        | Não expor senhas, tokens ou chaves sensíveis no repositório                 | ⬜      |
| 🗄 Banco de Dados   | Utilizar migrations para versionamento do banco de dados                    | ⬜      |
| 🗄 Banco de Dados   | Garantir integridade dos relacionamentos entre entidades                    | ⬜      |
| 🧪 Qualidade        | Implementar no mínimo 10 testes unitários                                   | ⬜      |
| 📖 Documentação     | Disponibilizar documentação da API com Swagger/Scalar                       | ⬜      |
| 📖 Documentação     | Manter README atualizado e completo                                         | ⬜      |
| 🐳 Implantação      | Permitir execução completa através de Docker Compose                        | ⬜      |
| ⚙️ Configuração     | Utilizar gerenciamento seguro de configurações e segredos                   | ⬜      |
| 🚀 Manutenibilidade | Código organizado, modular e de fácil manutenção                            | ⬜      |

### Legenda

* ✅ Concluído
* 🟨 Em desenvolvimento
* ⬜ Não iniciado

## 📚 Documentação do Projeto

Durante o desenvolvimento da API GrowStore foram elaborados diagramas para auxiliar na modelagem do sistema, definição dos relacionamentos entre entidades e levantamento dos requisitos funcionais.

| Documento                           | Descrição                                                                          | Link                                                                                             |
| ----------------------------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| 📦 Diagrama de Classes              | Representação das entidades do sistema, seus atributos, métodos e relacionamentos. | [Visualizar](https://drive.google.com/file/d/1Qi9HGSDAyXAxpbs1Xzoj-64lgRuUs0ZC/view)             |
| 🗄 Diagrama de Relacionamento (DER) | Modelagem do banco de dados e relacionamentos entre tabelas.                       | [Visualizar](https://drive.google.com/file/d/1sqXaFEx4V_c9VBCbBvxw7ojnmNyd4hLC/view?usp=sharing) |
| 🎭 Diagrama de Casos de Uso         | Representação das interações entre usuários e funcionalidades do sistema.          | [Visualizar](https://drive.google.com/file/d/1eheFZk7SeDogCvrKj-yLddsR5xnrPmp-/view?usp=sharing) |


### 🚧 Status do Projeto

O desenvolvimento da API GrowStore encontra-se em andamento. As funcionalidades estão sendo implementadas gradualmente seguindo os requisitos propostos no desafio, com foco em qualidade de código, boas práticas de arquitetura, segurança e escalabilidade.

As tabelas de acompanhamento presentes neste repositório são atualizadas conforme cada requisito é concluído, permitindo acompanhar a evolução do projeto de forma transparente.

