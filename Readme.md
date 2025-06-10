# Itaú Test Application

Este projeto é uma solução proposta para o teste técnico do Itaú Unibanco. Ele simula uma plataforma de controle de investimentos de renda variável, com funcionalidades como:

- Registro de operações de compra/venda de ativos  
- Cálculo de preço médio e posição consolidada  
- Integração com cotações via Kafka  
- Exposição de APIs REST  
- Testes unitários e práticas modernas de desenvolvimento

---

## Estrutura da Solução

- `ItauTest.Models`: Entidades do domínio  
- `ItauTest.Data`: Repositórios e DbContext EF Core  
- `ItauTest.Interfaces`: Interfaces do projeto  
- `ItauTest.Services`: Regras de negócio  
- `ItauTest.WebApi`: API RESTful em ASP.NET Core  
- `ItauTest.Tests`: Testes unitários com xUnit  
- `ItauTest.KafkaWorker`: Worker Service .NET para consumir cotações via Kafka

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Docker + Docker Compose](https://www.docker.com/)
- Visual Studio 2022 ou VS Code (opcional)

---

## Setup do Projeto

### Banco de Dados

1. **Instalar EF Core CLI (caso ainda não tenha):**

```bash
dotnet tool install --global dotnet-ef
```

2. **Gerar e aplicar migrations** (caso ainda não tenha a pasta `Migrations` no projeto):

```bash
dotnet ef migrations add InitialCreate --project ItauTest.Data --startup-project ItauTest.WebApi
```

3. **Aplicar migrations para criar o banco:**

```bash
dotnet ef database update --project ItauTest.Data --startup-project ItauTest.WebApi
```

4. **Inserir dados iniciais no MySQL:**

```sql
USE itau_test;

-- Usuários (senha: 12345)
INSERT INTO usuario (nome, email, senha_hash) VALUES
('João da Silva', 'joao@email.com','AQAAAAIAAYagAAAAEFFVuNTLnsGjteR83XDEWqVWeJ9Xu6u9IgQa9ehOxIJNJPm7isY1oMvRHJerqOVc7w=='),
('Maria Oliveira', 'maria@email.com', 'AQAAAAIAAYagAAAAEFFVuNTLnsGjteR83XDEWqVWeJ9Xu6u9IgQa9ehOxIJNJPm7isY1oMvRHJerqOVc7w==');

-- Ativos
INSERT INTO ativo (nome, tipo) VALUES
('PETR4', 'Ação'),
('VALE3', 'Ação');

-- Operações
INSERT INTO operacao (id_usuario, ativo_id, quantidade, preco_unitario, data_hora, corretagem)
VALUES (1, 1, 100, 50.1234, NOW(), 10.0000),
       (1, 1, 200, 55.6789, NOW(), 15.0000);

-- Cotações
INSERT INTO cotacao (ativo_id, preco_unitario, data_hora)
VALUES (1, 60.0000, NOW());

-- Verificação
SELECT * FROM usuario;
SELECT * FROM ativo;
SELECT * FROM operacao;
SELECT * FROM cotacao;
```

5. **Configurar string de conexão:**

No arquivo `ItauTest.WebApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;user=root;password=suasenha;database=itau_test;"
}
```

---

## Iniciando a Aplicação

### 🌐 API Web

No Visual Studio, selecione `ItauTest.WebApi` como projeto de inicialização e execute com IIS Express.

A API estará disponível em:

```
https://localhost:44390/swagger
```

---

### Kafka - Subindo com Docker

Dentro do diretório `ItauTest.KafkaWorker`:

```bash
docker-compose up -d
```

Verifique se os serviços `zookeeper` e `kafka` subiram corretamente:

```bash
docker ps
```

---

### Iniciar o Worker (Consumer)

```bash
cd ItauTest.KafkaWorker
dotnet run
```

Esse serviço consumirá cotações de um tópico Kafka e atualizará as posições.

---

## Executar Testes

```bash
dotnet test ItauTest.Tests
```

---

## Endpoints REST Principais

| Método | Rota                                                               | Descrição                                         |
|--------|--------------------------------------------------------------------|--------------------------------------------------|
| GET    | `/api/Operacao/average-price/{idUsuario}/{ativoId}`               | Retorna o preço médio de um ativo para o usuário |
| GET    | `/api/Posicao/{idUsuario}`                                        | Retorna a posição geral do usuário               |
| POST   | `/api/Operacao`                                                   | Registra uma nova operação                       |
| GET    | `/api/Cotacao/{ativoId}`                                          | Retorna a última cotação do ativo                |

---

## Documentação OpenAPI

Disponível automaticamente via Swagger em:

```
https://localhost:44390/swagger
```

---

## Observações

- As cotações são consumidas via Kafka no tópico configurado.
- O Kafka pode ser customizado editando `appsettings.json` no projeto `ItauTest.KafkaWorker`.
- O consumo é resiliente e utiliza políticas de **retry** e **idempotência**.
- A aplicação segue boas práticas de injeção de dependência, separation of concerns e async/await com EF Core.

---
