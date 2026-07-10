# PotyRetiradasApi

API REST desenvolvida em **ASP.NET Core (.NET 10)** para gerenciar as retiradas mensais de produtos por colaboradores da **Bebidas Poty**. Cada colaborador pode retirar produtos dentro dos limites (mínimo/máximo) configurados mensalmente por unidade.

---

## Sumário

- [Visão Geral](#visão-geral)
- [Tecnologias e Dependências](#tecnologias-e-dependências)
- [Arquitetura](#arquitetura)
- [Entidades e Banco de Dados](#entidades-e-banco-de-dados)
- [Endpoints da API](#endpoints-da-api)
- [Autenticação](#autenticação)
- [Configuração](#configuração)
- [Executando o Projeto](#executando-o-projeto)
- [Segurança](#segurança)

---

## Visão Geral

O sistema controla o processo de **retiradas mensais de produtos** (ex.: bebidas) por colaboradores distribuídos em diferentes unidades da empresa. As principais funcionalidades são:

- Autenticação de usuários via JWT
- Cadastro e gestão de unidades, colaboradores, tipos de colaborador, produtos e usuários do sistema
- Configuração mensal de limites (mínimo e máximo) de retirada por produto por unidade
- Registro de retiradas mensais com os produtos e quantidades retiradas
- Upload de imagens de produtos no **Azure Blob Storage**
- Documentação interativa via **Swagger UI**

O front-end associado está em: `https://potyretiradas.bebidaspoty.com.br`

---

## Tecnologias e Dependências

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 (SQL Server) |
| Banco de dados | SQL Server (Azure) |
| Autenticação | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Hash de senha | `Microsoft.Extensions.Identity.Core` (PasswordHasher) |
| Armazenamento de arquivos | Azure Blob Storage (`Azure.Storage.Blobs`) |
| Documentação | OpenAPI + Swagger UI (`Swashbuckle.AspNetCore.SwaggerUI`) |

---

## Arquitetura

O projeto segue o padrão **Repository + Service**, com separação clara de responsabilidades:

```
Controller  →  Service  →  Repository  →  DbContext (EF Core)  →  SQL Server
```

### Camadas

| Pasta | Responsabilidade |
|---|---|
| `Controller/` | Recebe requisições HTTP, delega para o Service e retorna respostas |
| `Services/` | Contém a lógica de negócio |
| `Services/Interfaces/` | Contratos das interfaces de serviço |
| `Repositories/` | Acesso a dados via Entity Framework Core |
| `Repositories/Interfaces/` | Contratos das interfaces de repositório |
| `Entities/` | Entidades mapeadas pelo EF Core (tabelas do banco) |
| `Dtos/` | Objetos de transferência de dados (requests/responses) |
| `Data/` | `DbContext` e configuração do modelo do banco |
| `Configurations/` | Classes de opções fortemente tipadas (`JwtOptions`, `AzureStorageOptions`) |

Todos os serviços e repositórios são registrados com **ciclo de vida Scoped** (`AddScoped`) na injeção de dependência.

---

## Entidades e Banco de Dados

O banco de dados `PotyRetiradas` contém as seguintes tabelas, mapeadas via EF Core:

### `Unidades`
Representa as unidades/filiais da empresa.

| Campo | Tipo | Descrição |
|---|---|---|
| `UnidadeID` | `int` (PK) | Identificador da unidade |
| `Descricao` | `varchar(200)` | Nome da unidade |
| `Status` | `bit` | Ativo/Inativo |

### `Tipos`
Define os tipos de colaborador e a quantidade de retirada associada.

| Campo | Tipo | Descrição |
|---|---|---|
| `TipoID` | `varchar(50)` (PK) | Identificador do tipo |
| `Descricao` | `varchar(200)` | Descrição do tipo |
| `QuantidadeRetirada` | `int` | Quantidade de itens que o tipo pode retirar |

### `Colaboradores`
Colaboradores que realizam as retiradas.

| Campo | Tipo | Descrição |
|---|---|---|
| `ColaboradorID` | `varchar(50)` (PK) | Identificador do colaborador |
| `CodigoAlternativo` | `int?` | Código alternativo (ex.: matrícula) |
| `Nome` | `varchar(200)` | Nome completo |
| `UnidadeID` | `int` (FK) | Unidade do colaborador |
| `Status` | `bit` | Ativo/Inativo |
| `TipoID` | `varchar(50)` (FK) | Tipo do colaborador |

### `Produtos`
Produtos disponíveis para retirada.

| Campo | Tipo | Descrição |
|---|---|---|
| `ProdutoID` | `varchar(8)` (PK) | Código do produto |
| `Descricao` | `varchar(200)` | Descrição do produto |
| `Status` | `bit` | Ativo/Inativo |
| `CodigoBarras` | `varchar(50)?` | Código de barras |
| `Imagem` | `varchar` | URL da imagem no Azure Blob Storage |

### `Usuarios`
Usuários que operam o sistema (não são os colaboradores).

| Campo | Tipo | Descrição |
|---|---|---|
| `UsuarioID` | `varchar(50)` (PK) | Identificador do usuário |
| `Nome` | `varchar(200)` | Nome completo |
| `Usuario1` | `varchar` | Login do usuário |
| `SenhaHash` | `varchar` | Hash da senha (ASP.NET Core Identity) |
| `Administrador` | `bit` | É administrador? |
| `Status` | `bit` | Ativo/Inativo |
| `UnidadeID` | `int` (FK) | Unidade do usuário |

### `RetiradasMensais`
Registro de cada retirada mensal realizada por um colaborador.

| Campo | Tipo | Descrição |
|---|---|---|
| `RetiradaMensalID` | `varchar(50)` (PK) | Identificador da retirada |
| `AnoMes` | `varchar(6)` | Período no formato `AAAAMM` |
| `UnidadeID` | `int` (FK) | Unidade onde ocorreu |
| `ColaboradorID` | `varchar(50)` (FK) | Colaborador que retirou |
| `UsuarioID` | `varchar(50)` (FK) | Usuário operador que registrou |
| `Operador` | `varchar` | Nome do operador |
| `DataHora` | `datetime` | Data e hora da retirada |
| `RetiradoPor` | `varchar?` | Quem retirou (caso seja outra pessoa) |

### `RetiradasMensaisProdutos`
Itens (produtos e quantidades) de cada retirada mensal.

| Campo | Tipo | Descrição |
|---|---|---|
| `RetiradaMensalProdutoID` | `varchar(50)` (PK) | Identificador do item |
| `RetiradaMensalID` | `varchar(50)` (FK) | Retirada à qual pertence |
| `ProdutoID` | `varchar(8)` (FK) | Produto retirado |
| `Quantidade` | `int` | Quantidade retirada |

### `ConfiguracoesMensaisProdutos`
Define limites de retirada por produto por unidade em cada mês.

| Campo | Tipo | Descrição |
|---|---|---|
| `ConfiguracaoMensalID` | `varchar(50)` (PK) | Identificador da configuração |
| `AnoMes` | `varchar(6)` | Período no formato `AAAAMM` |
| `UnidadeID` | `int` (FK) | Unidade |
| `ProdutoID` | `varchar(8)` (FK) | Produto |
| `Minimo` | `int` | Quantidade mínima permitida |
| `Maximo` | `int` | Quantidade máxima permitida |

---

## Endpoints da API

A documentação completa e interativa está disponível no Swagger UI em `/swagger` quando a aplicação está em execução.

### Autenticação

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `POST` | `/api/auth/login` | Realiza login e retorna JWT | Não |

### Unidades

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/unidades` | Lista todas as unidades | Sim |
| `GET` | `/api/unidades/{unidadeId}` | Obtém unidade por ID | Sim |
| `POST` | `/api/unidades` | Cria uma nova unidade | Sim |
| `PUT` | `/api/unidades/{unidadeId}` | Atualiza uma unidade | Sim |
| `DELETE` | `/api/unidades/{unidadeId}` | Desativa uma unidade | Sim |

### Colaboradores

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/colaboradores` | Lista todos os colaboradores | Sim |
| `GET` | `/api/colaboradores/{colaboradorId}` | Obtém colaborador por ID | Sim |
| `POST` | `/api/colaboradores` | Cria um novo colaborador | Sim |
| `PUT` | `/api/colaboradores/{colaboradorId}` | Atualiza um colaborador | Sim |
| `DELETE` | `/api/colaboradores/{colaboradorId}` | Desativa um colaborador | Sim |

### Tipos

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/tipos` | Lista todos os tipos | Sim |
| `GET` | `/api/tipos/{tipoId}` | Obtém tipo por ID | Sim |
| `POST` | `/api/tipos` | Cria um novo tipo | Sim |
| `PUT` | `/api/tipos/{tipoId}` | Atualiza um tipo | Sim |
| `DELETE` | `/api/tipos/{tipoId}` | Remove um tipo | Sim |

### Produtos

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/produtos` | Lista todos os produtos | Não* |
| `GET` | `/api/produtos/{produtoId}` | Obtém produto por ID | Não* |
| `POST` | `/api/produtos` | Cria um novo produto | Não* |
| `PUT` | `/api/produtos/{produtoId}` | Atualiza um produto | Não* |
| `DELETE` | `/api/produtos/{produtoId}` | Desativa um produto | Não* |

> *O `[Authorize]` está comentado no `ProdutosController`. Recomenda-se reativá-lo em produção.

### Usuários

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/usuarios` | Lista todos os usuários | Sim |
| `GET` | `/api/usuarios/{usuarioId}` | Obtém usuário por ID | Sim |
| `POST` | `/api/usuarios` | Cria um novo usuário | Sim |
| `PUT` | `/api/usuarios/{usuarioId}` | Atualiza um usuário | Sim |
| `DELETE` | `/api/usuarios/{usuarioId}` | Desativa um usuário | Sim |

### Retiradas Mensais

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/retiradas-mensais` | Lista todas as retiradas | Sim |
| `GET` | `/api/retiradas-mensais/{retiradaMensalId}` | Obtém retirada por ID | Sim |
| `GET` | `/api/retiradas-mensais/ano-mes/{anoMes}` | Lista retiradas por período | Sim |
| `POST` | `/api/retiradas-mensais` | Registra uma nova retirada | Sim |
| `DELETE` | `/api/retiradas-mensais/{retiradaMensalId}` | Remove uma retirada | Sim |

### Configurações Mensais de Produtos

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| `GET` | `/api/configuracoes-mensais-produtos` | Lista todas as configurações | Sim |
| `GET` | `/api/configuracoes-mensais-produtos/{id}` | Obtém configuração por ID | Sim |
| `POST` | `/api/configuracoes-mensais-produtos` | Cria uma configuração | Sim |
| `PUT` | `/api/configuracoes-mensais-produtos/{id}` | Atualiza uma configuração | Sim |
| `DELETE` | `/api/configuracoes-mensais-produtos/{id}` | Remove uma configuração | Sim |

---

## Autenticação

A API utiliza **JWT Bearer Token**.

**Fluxo:**

1. O cliente faz `POST /api/auth/login` com `{ "usuario": "...", "senha": "..." }`
2. A API valida as credenciais e retorna um token JWT
3. O cliente inclui o token em todas as requisições protegidas no header:
   ```
   Authorization: Bearer <token>
   ```

**Configurações JWT (`appsettings.json`):**

| Chave | Descrição |
|---|---|
| `Jwt:Issuer` | Emissor do token (`PotyRetiradasApi`) |
| `Jwt:Audience` | Audiência do token (`PotyRetiradasApp`) |
| `Jwt:Key` | Chave secreta de assinatura (mínimo 32 caracteres) |
| `Jwt:ExpirationMinutes` | Tempo de expiração em minutos (padrão: `480`) |

---

## Configuração

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "RetiradasDb": "<connection-string-do-sql-server>"
  },
  "Jwt": {
    "Issuer": "PotyRetiradasApi",
    "Audience": "PotyRetiradasApp",
    "Key": "<chave-secreta-minimo-32-caracteres>",
    "ExpirationMinutes": 480
  },
  "AzureStorage": {
    "ConnectionString": "<connection-string-do-azure-storage>",
    "ContainerName": "potyretiradas"
  }
}
```

### CORS

A política de CORS `PotyRetiradasWeb` permite requisições apenas de:
```
https://potyretiradas.bebidaspoty.com.br
```

---

## Executando o Projeto

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server acessível com o banco `PotyRetiradas`
- Conta Azure Storage (para upload de imagens)

### Passos

```bash
# Restaurar dependências
dotnet restore

# Executar em modo desenvolvimento
dotnet run
```

A API estará disponível em `https://localhost:{porta}`.  
O Swagger UI estará em `https://localhost:{porta}/swagger`.

---

## Segurança

> **ATENÇÃO**: O arquivo `appsettings.json` atualmente contém credenciais em texto puro (senha do banco, chave JWT, chave do Azure Storage). Isso representa um risco de segurança grave, especialmente se o repositório for público.

**Recomendações:**

- Mover segredos para **variáveis de ambiente** ou **Azure Key Vault**
- Usar **User Secrets** (`dotnet user-secrets`) em desenvolvimento:
  ```bash
  dotnet user-secrets set "ConnectionStrings:RetiradasDb" "<sua-connection-string>"
  dotnet user-secrets set "Jwt:Key" "<sua-chave-secreta>"
  dotnet user-secrets set "AzureStorage:ConnectionString" "<sua-connection-string>"
  ```
- Adicionar `appsettings.json` ao `.gitignore` e usar um `appsettings.example.json` com valores fictícios no repositório
- Reativar o `[Authorize]` no `ProdutosController`
