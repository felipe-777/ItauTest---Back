# Itaú Test Solution

## Visão Geral
Esta é uma solução para o teste técnico do Itaú, implementando uma aplicação para gerenciar investimentos de renda variável. A solução é dividida em projetos para facilitar manutenção e testes.

## Estrutura do Projeto
- **ItauTest.Data**: Contém o DbContext e repositórios para acesso ao banco de dados MySQL.
- **ItauTest.Models**: Contém as entidades `Operacao`, `Cotacao` e `Posicao`.
- **ItauTest.Services**: Contém a lógica de negócios, como cálculo de preço médio.
- **ItauTest.WebApi**: Contém as APIs RESTful.
- **ItauTest.Tests**: Contém testes unitários com xUnit.

## Pré-requisitos
- .NET 8.0 SDK
- MySQL 8.0
- Visual Studio 2022

## Configuração
1. Instale o MySQL 8.0 e crie o banco `itau_test`.
2. Execute o script SQL para criar as tabelas.
3. Configure a string de conexão em `appsettings.json` no projeto `ItauTest.WebApi`.
4. Restaure os pacotes NuGet: `dotnet restore`.
5. Execute a aplicação: `dotnet run` no projeto `ItauTest.WebApi`.
6. Execute os testes: `dotnet test` no projeto `ItauTest.Tests`.

## Endpoints
- `GET /api/Operacao/average-price/{idUsuario}/{ativoId}`: Retorna o preço médio de um ativo para um usuário.