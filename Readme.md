````markdown
# Itaú Test Application

Este projeto simula uma plataforma de controle de investimentos de renda variável, com funcionalidades como:

- Registro de operações de compra/venda de ativos
- Cálculo de preço médio e posição consolidada
- Integração com cotações via Kafka
- Exposição de APIs REST
- Testes unitários e práticas modernas de desenvolvimento

---

## Estrutura da Solução

- **ItauTest.Models**: Entidades do domínio
- **ItauTest.Data**: Repositórios e DbContext com EF Core
- **ItauTest.Interfaces**: Interfaces do projeto
- **ItauTest.Services**: Regras de negócio
- **ItauTest.WebApi**: API RESTful em ASP.NET Core
- **ItauTest.Tests**: Testes unitários com xUnit
- **ItauTest.KafkaWorker**: Worker Service .NET para consumir cotações via Kafka

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Docker + Docker Compose](https://www.docker.com/)
- Visual Studio 2022 ou VS Code (opcional)

---

## Setup do Projeto

### Banco de Dados

1. **Instalar EF Core CLI (caso ainda não tenha)**

   ```bash
   dotnet tool install --global dotnet-ef
```

2. **Gerar e aplicar migrations (caso ainda não tenha a pasta `Migrations`)**

   ```bash
   dotnet ef migrations add InitialCreate --project ItauTest.Data --startup-project ItauTest.WebApi
   ```

3. **Aplicar migrations para criar o banco**

   ```bash
   dotnet ef database update --project ItauTest.Data --startup-project ItauTest.WebApi
   ```

4. **Inserir dados iniciais no MySQL**

   Execute o seguinte código SQL no banco de dados MySQL:

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

5. **Configurar string de conexão**

   No arquivo `ItauTest.WebApi/appsettings.json`, configure a string de conexão com o MySQL:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;user=root;password=suasenha;database=itau_test;"
   }
   ```

---

## Iniciando a Aplicação

### 🌐 API Web

No Visual Studio, selecione o projeto `ItauTest.WebApi` como projeto de inicialização e execute com IIS Express.

A API estará disponível em:

```
https://localhost:44390/swagger
```

---

### Kafka

#### Pré-requisitos

Antes de iniciar o Kafka e o consumer, certifique-se de que:

* O Docker Desktop está rodando.
* O servidor MySQL está rodando.
* A string de conexão no `appsettings.json` do projeto `ItauTest.KafkaWorker` está correta.

#### Subindo Kafka com Docker

Dentro do diretório `ItauTest.KafkaWorker`, execute:

```bash
docker-compose up -d
```

Verifique se os serviços `zookeeper` e `kafka` subiram corretamente com o comando:

```bash
docker ps
```

#### Iniciar o Consumer

Dentro do diretório `ItauTest.KafkaWorker`, execute:

```bash
dotnet run
```

Esse serviço consumirá cotações de um tópico Kafka e atualizará as posições.

#### Iniciar o Producer

Para enviar mensagens para a fila Kafka, você pode usar a API do projeto `ItauTest.WebApi`.

1. Inicie a API no Visual Studio, selecionando `ItauTest.WebApi` como projeto de inicialização e executando com IIS Express.

2. Publique uma nova cotação enviando uma requisição POST para o endpoint `https://localhost:44390/api/Cotacao/publicar` com o seguinte comando curl:

   ```bash
   curl -X 'POST' \
     'https://localhost:44390/api/Cotacao/publicar' \
     -H 'accept: */*' \
     -H 'Content-Type: application/json' \
     -d '{
       "ativoCodigo": "VALE3",
       "precoUnitario": 50,
       "dataHora": "2025-06-10T11:23:38.852Z"
     }'
   ```

---

## Executar Testes

Para rodar os testes unitários, execute o seguinte comando:

```bash
dotnet test ItauTest.Tests
```

---

## Endpoints REST Principais

| Método | Rota                                                | Descrição                                        |
| ------ | --------------------------------------------------- | ------------------------------------------------ |
| GET    | `/api/Operacao/average-price/{idUsuario}/{ativoId}` | Retorna o preço médio de um ativo para o usuário |
| GET    | `/api/Posicao/{idUsuario}`                          | Retorna a posição geral do usuário               |
| POST   | `/api/Operacao`                                     | Registra uma nova operação                       |
| GET    | `/api/Cotacao/{ativoId}`                            | Retorna a última cotação do ativo                |

---

## Documentação OpenAPI

A documentação da API está disponível automaticamente via Swagger:

```
https://localhost:44390/swagger
```

---

## Observações

* As cotações são consumidas via Kafka no tópico configurado.
* O Kafka pode ser customizado editando o `appsettings.json` no projeto `ItauTest.KafkaWorker`.
* A aplicação segue boas práticas de injeção de dependência, separação de responsabilidades e uso de async/await com EF Core.

---

### Notas Adicionais

* Certifique-se de que a string de conexão no `appsettings.json` do projeto `ItauTest.WebApi` está correta, pois a API acessa diretamente o banco de dados.
* O endpoint POST `/api/Cotacao/publicar` não está listado na tabela de endpoints principais, mas está disponível para publicar cotações no Kafka via producer.


