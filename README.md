# Fiap Vehicle Sales API

API backend desenvolvida para o Tech Challenge da Pós Tech FIAP — Fase 3 — curso SOAT.

O projeto simula uma plataforma para uma empresa de revenda de veículos automotores. A solução permite cadastrar veículos para venda, editar veículos, listar veículos disponíveis, listar veículos vendidos e realizar a compra de veículos por compradores cadastrados.

A autenticação e autorização dos compradores foi implementada de forma separada da API principal, utilizando Keycloak, conforme a proposta do desafio de manter os dados dos clientes separados dos dados transacionais de vendas.

---

## Índice

* [Sobre o projeto](#sobre-o-projeto)
* [Funcionalidades](#funcionalidades)
* [Tecnologias utilizadas](#tecnologias-utilizadas)
* [Arquitetura](#arquitetura)
* [Estrutura da solução](#estrutura-da-solução)
* [Como rodar localmente](#como-rodar-localmente)
* [Configuração do Keycloak](#configuração-do-keycloak)
* [Como gerar token JWT](#como-gerar-token-jwt)
* [Endpoints principais](#endpoints-principais)
* [Fluxo de compra](#fluxo-de-compra)
* [Modelagem do banco](#modelagem-do-banco)
* [Testes automatizados](#testes-automatizados)
* [CI/CD](#cicd)
* [Como testar no Swagger](#como-testar-no-swagger)

---

## Sobre o projeto

A empresa de revenda de veículos automotores precisa de uma plataforma web para vender veículos pela internet.

Como o time de frontend será responsável pela interface, este projeto entrega a API backend responsável pelas regras de negócio e persistência dos dados.

A API permite:

* Cadastrar veículos para venda;
* Editar dados de veículos;
* Listar veículos disponíveis;
* Listar veículos vendidos;
* Realizar a compra de um veículo;
* Permitir compra apenas para usuários autenticados;
* Separar autenticação/autorização dos dados transacionais.

---

## Funcionalidades

### Veículos

* Cadastro de veículo com:

  * Marca;
  * Modelo;
  * Ano;
  * Cor;
  * Preço.

* Edição dos dados do veículo.

* Listagem de veículos disponíveis para venda.

* Listagem de veículos vendidos.

* Ordenação das listagens por preço crescente.

### Compras

* Compra de veículo por usuário autenticado.
* Validação se o veículo existe.
* Validação se o veículo está disponível.
* Registro da venda.
* Alteração automática do status do veículo para vendido.

### Autenticação e autorização

* Autenticação separada usando Keycloak.
* Usuários e roles gerenciados fora da API principal.
* A API valida tokens JWT emitidos pelo Keycloak.
* A venda armazena apenas o identificador do comprador vindo do token.

---

## Tecnologias utilizadas

* .NET 10
* C#
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Docker
* Docker Compose
* Keycloak
* Swagger
* xUnit
* GitHub Actions
* Clean Architecture
* SOLID
* DDD simplificado

---

## Arquitetura

O projeto utiliza Clean Architecture com separação em camadas.

A ideia principal é proteger as regras de negócio e manter a aplicação organizada, simples e fácil de manter.

![Arquitetura da solução](docs/images/arquitetura.png)

### Domain

Camada responsável pelas entidades e regras de negócio.

Contém:

* Entidades;
* Enums;
* Exceptions de domínio;
* Regras como:

  * veículo não pode ter preço inválido;
  * veículo vendido não pode ser editado;
  * veículo vendido não pode ser comprado novamente.

### Application

Camada responsável pelos casos de uso da aplicação.

Contém:

* DTOs;
* Interfaces de repositório;
* Casos de uso;
* Mappers;
* Exceptions da aplicação.

Exemplos de casos de uso:

* Criar veículo;
* Editar veículo;
* Listar veículos disponíveis;
* Listar veículos vendidos;
* Comprar veículo.

### Infrastructure

Camada responsável por detalhes externos.

Contém:

* DbContext;
* Configurações do Entity Framework;
* Repositórios;
* Migrations;
* Implementação das interfaces da camada Application.

### API

Camada responsável por expor os endpoints HTTP.

Contém:

* Controllers;
* Configuração do Swagger;
* Configuração de autenticação JWT;
* Configuração de autorização;
* Injeção de dependência.

---

## Estrutura da solução

```text
Fiap.VehicleSales
│
├── src
│   ├── Fiap.VehicleSales.Api
│   ├── Fiap.VehicleSales.Application
│   ├── Fiap.VehicleSales.Domain
│   └── Fiap.VehicleSales.Infrastructure
│
├── tests
│   ├── Fiap.VehicleSales.UnitTests
│   └── Fiap.VehicleSales.IntegrationTests
│
├── config
│   └── keycloak
│       └── fiap-vehicle-sales-realm.json
│
├── docker-compose.yml
├── README.md
└── .github
    └── workflows
        └── ci.yml
```

---

## Como rodar localmente

### Pré-requisitos

Antes de executar o projeto, é necessário ter instalado:

* .NET 10 SDK, caso queira executar os testes fora do Docker;
* Docker;
* Docker Compose;
* Git.

---

### Clonar o repositório

```bash
git clone https://github.com/AdamMachado/fiap-vehicle-sales.git
cd fiap-vehicle-sales
```

---

### Subir os containers

Execute:

```bash
docker compose up -d --build
```

Esse comando irá subir:

* API de venda de veículos;
* Banco PostgreSQL da API;
* Banco PostgreSQL do Keycloak;
* Keycloak.

---

### Aplicar migrations

As migrations do Entity Framework são aplicadas automaticamente durante a
inicialização da API. Em uma instalação nova, o banco e as tabelas são criados
sem a necessidade de executar `dotnet ef database update`.

Após iniciar a aplicação, acesse o Swagger:

```text
http://localhost:5000/swagger
```

### Reiniciar o ambiente do zero

Para remover os containers e apagar os volumes dos dois bancos:

```bash
docker compose down --volumes --remove-orphans
docker compose up -d --build
```

> Atenção: essa operação apaga definitivamente veículos, vendas, usuários e
> configurações persistidas no ambiente Docker local.

---

## Configuração do Keycloak

O realm `fiap-vehicle-sales`, o client `vehicle-sales-api`, as roles e os
usuários de demonstração são importados automaticamente na primeira
inicialização do Keycloak pelo Docker Compose.

Acesse o Keycloak:

```text
http://localhost:8080
```

Credenciais administrativas:

```text
Usuário: admin
Senha: admin
```

### Usuários de demonstração

Administrador:

```text
Username: admin@test.com
Password: 123456
Role: admin
```

Comprador:

```text
Username: buyer@test.com
Password: 123456
Role: buyer
```

Essas credenciais são destinadas exclusivamente ao ambiente local de
demonstração e devem ser substituídas em qualquer ambiente publicado.

---

## Como gerar token JWT

### Token do comprador

PowerShell:

```powershell
$buyerTokenResponse = Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:8080/realms/fiap-vehicle-sales/protocol/openid-connect/token" `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
    client_id = "vehicle-sales-api"
    username = "buyer@test.com"
    password = "123456"
    grant_type = "password"
  }

$buyerTokenResponse.access_token
```

---

### Token do administrador

PowerShell:

```powershell
$adminTokenResponse = Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:8080/realms/fiap-vehicle-sales/protocol/openid-connect/token" `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
    client_id = "vehicle-sales-api"
    username = "admin@test.com"
    password = "123456"
    grant_type = "password"
  }

$adminTokenResponse.access_token
```

---

### Usar token no Swagger

No Swagger, clique em:

```text
Authorize
```

Informe o token no formato:

```text
seu_token_aqui
```

Informe somente o token. O Swagger adiciona o prefixo `Bearer`
automaticamente.

---

## Endpoints principais

### Veículos

#### Cadastrar veículo

```http
POST /api/vehicles
```

Permissão:

```text
admin
```

Body:

```json
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2022,
  "color": "Prata",
  "price": 120000
}
```

---

#### Editar veículo

```http
PUT /api/vehicles/{id}
```

Permissão:

```text
admin
```

Body:

```json
{
  "brand": "Toyota",
  "model": "Corolla XEI",
  "year": 2022,
  "color": "Preto",
  "price": 118000
}
```

---

#### Buscar veículo por ID

```http
GET /api/vehicles/{id}
```

Permissão:

```text
Público
```

---

#### Listar veículos disponíveis

```http
GET /api/vehicles/available
```

Permissão:

```text
Público
```

A listagem retorna veículos disponíveis ordenados por preço crescente.

---

#### Listar veículos vendidos

```http
GET /api/vehicles/sold
```

Permissão:

```text
Público
```

A listagem retorna veículos vendidos ordenados por preço crescente.

---

### Vendas

#### Comprar veículo

```http
POST /api/sales
```

Permissão:

```text
buyer
```

Body:

```json
{
  "vehicleId": "id-do-veiculo"
}
```

A API identifica o comprador pelo token JWT enviado no header Authorization.

---

## Fluxo de compra

O fluxo de compra funciona da seguinte forma:

```text
1. O comprador é cadastrado no Keycloak.
2. O comprador faz login e obtém um token JWT.
3. O comprador envia o token para a API.
4. A API valida o token.
5. A API extrai o identificador do comprador pelo claim sub.
6. O comprador informa o ID do veículo desejado.
7. A API verifica se o veículo existe.
8. A API verifica se o veículo está disponível.
9. A API cria uma venda.
10. A API salva o BuyerId na tabela Sales.
11. A API altera o status do veículo para Sold.
12. O veículo deixa de aparecer na listagem de disponíveis.
13. O veículo passa a aparecer na listagem de vendidos.
```

---

## Estratégia de autenticação

A autenticação foi implementada de forma separada utilizando Keycloak.

O Keycloak possui banco próprio e é responsável por:

* Cadastro dos usuários;
* Login;
* Senhas;
* Roles;
* Emissão de tokens JWT.

A API principal não armazena dados sensíveis do comprador, como nome, e-mail ou senha.

Na tabela de vendas, a API salva apenas o identificador do comprador recebido no token JWT.

Exemplo:

```text
BuyerId = claim sub do token
```

Essa separação mantém os dados de autenticação fora da base transacional da aplicação.

---

## Regras de autorização

| Endpoint                    | Permissão |
| --------------------------- | --------- |
| POST /api/vehicles          | admin     |
| PUT /api/vehicles/{id}      | admin     |
| GET /api/vehicles/{id}      | público   |
| GET /api/vehicles/available | público   |
| GET /api/vehicles/sold      | público   |
| POST /api/sales             | buyer     |

---

## Modelagem do banco

### Vehicles

Tabela responsável por armazenar os veículos.

| Campo     | Tipo      | Descrição                |
| --------- | --------- | ------------------------ |
| Id        | uuid      | Identificador do veículo |
| Brand     | varchar   | Marca do veículo         |
| Model     | varchar   | Modelo do veículo        |
| Year      | int       | Ano do veículo           |
| Color     | varchar   | Cor do veículo           |
| Price     | decimal   | Preço do veículo         |
| Status    | int       | Status do veículo        |
| CreatedAt | timestamp | Data de criação          |
| UpdatedAt | timestamp | Data de atualização      |

Status possíveis:

```text
1 = Available
2 = Sold
```

---

### Sales

Tabela responsável por armazenar as vendas realizadas.

| Campo     | Tipo      | Descrição                                 |
| --------- | --------- | ----------------------------------------- |
| Id        | uuid      | Identificador da venda                    |
| VehicleId | uuid      | Identificador do veículo vendido          |
| BuyerId   | varchar   | Identificador do comprador vindo do token |
| Price     | decimal   | Preço no momento da venda                 |
| SaleDate  | timestamp | Data da venda                             |
| Status    | int       | Status da venda                           |

Status possíveis:

```text
1 = Completed
2 = Canceled
```

---

## Testes automatizados

O projeto possui testes automatizados com xUnit.

Para executar:

```bash
dotnet test
```

Atualmente, a suíte possui 32 testes unitários e 7 testes de integração,
cobrindo:

* Deve criar veículo com dados válidos;
* Não deve criar veículo com preço inválido;
* Não deve editar veículo vendido;
* Deve comprar veículo disponível;
* Não deve comprar veículo já vendido;
* Deve listar veículos disponíveis ordenados por preço;
* Deve listar veículos vendidos ordenados por preço.

---

## CI/CD

O projeto utiliza GitHub Actions para validação automática.

O pipeline executa:

```text
Restore
Build
Test
Upload dos resultados de teste
Docker build
Validação do ambiente completo com Docker Compose
Validação da autenticação no Keycloak
Publicação da imagem no GitHub Container Registry
```

O workflow é executado em Pull Requests e pushes para a branch `master`.
Também pode ser iniciado manualmente pelo GitHub Actions. Em Pull Requests e
execuções manuais, todo o ambiente é construído e validado, mas a imagem não é
publicada. Em pushes para `master`, depois de todas as validações, a imagem é
publicada no GHCR com as tags do commit e `latest`:

```text
ghcr.io/adammachado/fiap-vehicle-sales-api:<commit-sha>
ghcr.io/adammachado/fiap-vehicle-sales-api:latest
```

O pipeline utiliza o `GITHUB_TOKEN` fornecido pelo GitHub Actions e a
permissão `packages: write`. O deploy da imagem em um ambiente público ainda
não faz parte deste workflow.

---

## Como testar no Swagger

### 1. Subir infraestrutura

```bash
docker compose up -d --build
```

---

### 2. Acessar Swagger

```text
http://localhost:5000/swagger
```

### 3. Gerar token de admin

Use o token do usuário:

```text
admin@test.com
```

---

### 4. Autorizar no Swagger

Clique em:

```text
Authorize
```

Informe:

```text
token_do_admin
```

---

### 5. Cadastrar veículo

```http
POST /api/vehicles
```

---

### 6. Gerar token de comprador

Use o token do usuário:

```text
buyer@test.com
```

---

### 7. Comprar veículo

```http
POST /api/sales
```

---

### 8. Listar veículos disponíveis

```http
GET /api/vehicles/available
```

O veículo comprado não deve aparecer mais.

---

### 9. Listar veículos vendidos

```http
GET /api/vehicles/sold
```

O veículo comprado deve aparecer como vendido.

---

## Demonstração esperada no vídeo

O vídeo de demonstração deve apresentar:

```text
1. Infraestrutura subindo com Docker Compose.
2. Keycloak funcionando.
3. Cadastro ou exibição do usuário comprador.
4. Geração do token JWT.
5. Cadastro de veículo pela API.
6. Listagem de veículos disponíveis.
7. Compra do veículo autenticado como comprador.
8. Listagem de veículos vendidos.
9. Execução dos testes.
10. Pipeline de CI/CD no GitHub Actions.
```

---

## Decisões técnicas

### Por que Clean Architecture?

Foi utilizada para separar responsabilidades e evitar acoplamento entre regras de negócio, banco de dados e API.

### Por que Keycloak?

Foi utilizado para manter autenticação e autorização separadas da solução principal.

### Por que PostgreSQL?

Foi escolhido por ter boa integração com Docker, Entity Framework Core e Keycloak.

### Por que salvar apenas BuyerId?

Para manter os dados pessoais do comprador fora da base transacional da API. A API salva apenas a referência do comprador autenticado.

### Por que não implementar pagamento?

O desafio não exige integração com gateway de pagamento. Por isso, a compra é efetivada no momento em que o comprador autenticado confirma a aquisição do veículo.

---

## Autor

Projeto desenvolvido para o Tech Challenge — Pós Tech FIAP — Fase 3 — SOAT.
