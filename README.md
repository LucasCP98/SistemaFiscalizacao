# Sistema de Fiscalização Municipal

API REST para gerenciamento de denúncias e processos de fiscalização municipal, desenvolvida com C# e ASP.NET Core.

## Sobre o projeto

O **Sistema de Fiscalização Municipal** tem como objetivo digitalizar e organizar o fluxo de atendimento de denúncias relacionadas a possíveis irregularidades urbanas, permitindo que as informações sejam registradas e consultadas de forma estruturada.

A proposta é centralizar as informações e acompanhar as etapas do processo de fiscalização, desde o registro da denúncia até sua resolução, contribuindo para a rastreabilidade e a organização dos procedimentos administrativos.

O projeto está em desenvolvimento e será construído de forma incremental, utilizando uma arquitetura em camadas para facilitar a manutenção, os testes e a evolução da aplicação.

## Funcionalidades

### Implementadas

* Cadastro de denúncias.
* Listagem de denúncias cadastradas.
* Consulta de denúncia por ID.
* Persistência de dados utilizando Entity Framework Core e SQL Server.
* Documentação e testes manuais dos endpoints por meio do Swagger ou Scalar.

### Planejadas

* Geração de protocolo para as denúncias.
* Gerenciamento de usuários e permissões.
* Atribuição de denúncias aos fiscais.
* Registro e acompanhamento de vistorias.
* Emissão de notificações e controle de prazos.
* Registro de autos de infração, recursos e cobranças.
* Histórico de movimentações e auditoria.
* Interface web para cidadãos, atendentes e fiscais.
* Backend for Frontend (BFF) para integração com o frontend.

## Tecnologias utilizadas

* **C# / ASP.NET Core:** desenvolvimento da API REST.
* **Entity Framework Core:** mapeamento objeto-relacional e acesso aos dados.
* **SQL Server:** banco de dados relacional.
* **Docker:** execução do banco de dados em ambiente de contêiner.
* **Swagger-Scalar / OpenAPI:** documentação e testes dos endpoints.
* **Git / GitHub:** versionamento do código-fonte.

## Arquitetura

O backend está organizado em camadas, separando as responsabilidades para facilitar a manutenção e a evolução do sistema.

| Projeto                              | Responsabilidade                                                |
| ------------------------------------ | --------------------------------------------------------------- |
| `SistemaFiscalizacao.API`            | Controllers, configuração da API e Swagger/Scalar.                     |
| `SistemaFiscalizacao.Application`    | DTOs, serviços e regras de aplicação.                           |
| `SistemaFiscalizacao.Domain`         | Entidades e regras de domínio.                                  |
| `SistemaFiscalizacao.Infrastructure` | Persistência, Entity Framework Core e acesso ao banco de dados. |

Essa organização busca reduzir o acoplamento entre as camadas e facilitar a implementação de novas funcionalidades.

## Endpoints disponíveis

A API disponibiliza atualmente os seguintes endpoints para gerenciamento de denúncias.

| Método | Endpoint              | Descrição                       |
| ------ | --------------------- | ------------------------------- |
| `POST` | `/api/denuncias`      | Cadastra uma denúncia.          |
| `GET`  | `/api/denuncias`      | Lista as denúncias cadastradas. |
| `GET`  | `/api/denuncias/{id}` | Consulta uma denúncia pelo ID.  |

### Exemplo de requisição

Cadastro de uma denúncia:

```http
POST /api/denuncias
Content-Type: application/json
```

```json
{
  "descricao": "Construção irregular em um terreno",
  "canal": "Site",
  "anonima": true
}
```

Os campos do exemplo correspondem ao DTO de criação atual e poderão evoluir conforme as regras de negócio forem implementadas.

## Como executar o projeto

### Pré-requisitos

* .NET SDK compatível com o `TargetFramework` definido no projeto.
* Docker Desktop.
* SQL Server.
* Git.

### 1. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd SistemaFiscalizacao
```

### 2. Configurar a conexão com o banco de dados

Configure a connection string `DefaultConnection` no `appsettings.json` ou, preferencialmente, por meio de variáveis de ambiente ou User Secrets.

Não publique senhas ou outras credenciais no repositório.

### 3. Executar as migrations

Na raiz da solução, execute os comandos utilizando os projetos de Infrastructure e API:

```bash
dotnet ef database update \
  --project backend/SistemaFiscalizacao.Infrastructure \
  --startup-project backend/SistemaFiscalizacao.API
```

> Os caminhos acima pressupõem que a solução e os projetos estejam organizados nessa estrutura. Ajuste-os caso estejam diferentes no seu ambiente.

### 4. Executar a API

```bash
dotnet run --project backend/SistemaFiscalizacao.API
```

Utilize a URL e a porta informadas no terminal para acessar a aplicação.

### 5. Acessar o Swagger ou Scalar

Com a API em execução, acesse a rota `/swagger` ou `/scalar` na URL indicada pelo terminal para consultar a documentação e testar os endpoints disponíveis.

## Próximas etapas

O desenvolvimento seguirá uma abordagem incremental:

1. Consolidar a estrutura do backend e as regras de negócio das denúncias.
2. Implementar testes unitários e melhorar as validações.
3. Desenvolver o módulo de vistorias.
4. Implementar notificações, prazos e autos de infração.
5. Adicionar autenticação, autorização e auditoria.
6. Desenvolver o BFF e a interface web.

## Objetivo de desenvolvimento

Além de solucionar um problema de organização do processo de fiscalização municipal, o projeto é uma oportunidade prática de aplicar conceitos de desenvolvimento backend, arquitetura em camadas, APIs REST, persistência de dados, testes automatizados e boas práticas de engenharia de software.
