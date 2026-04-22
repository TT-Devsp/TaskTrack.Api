# TaskTrack API - Execucao Local

Este projeto tem dois modos de execucao local:

1. Modo rapido (tudo em Docker)
2. Modo debug (breakpoints no VS Code ou Visual Studio)

## Pre-requisitos

Obrigatorio para qualquer modo:

- Docker Desktop (Windows/macOS) ou Docker Engine + Docker Compose (Linux)
- Git

Obrigatorio para debug com breakpoints:

- .NET SDK 9.0
- VS Code com extensao C# ou Visual Studio 2022+

## Primeiro passo (igual para todos)

1. Clone o repositorio:

```bash
git clone https://github.com/TT-Devsp/TaskTrack.Api.git
cd TaskTrack.Api
```

2. (Opcional) Crie um arquivo `.env` para customizar portas/credenciais locais:

```bash
cp .env.example .env
```

No Windows PowerShell, se preferir:

```powershell
Copy-Item .env.example .env
```

## Modo 1 - Rapido (API + banco em container)

Suba tudo:

```bash
docker compose up -d
```

Abra o Swagger:

```text
http://localhost:8080/swagger
```

Detalhes:

- O servico `postgres` sobe o banco
- O servico `api` builda e sobe a API
- As migrations sao aplicadas automaticamente no startup da API

Quando voce alterar Dockerfile/dependencias da imagem da API, rode com rebuild:

```bash
docker compose up --build -d
```

## Modo 2 - Debug com breakpoints (VS Code ou Visual Studio)

Use este modo quando quiser depurar codigo linha a linha.

### 1) Suba somente o banco

```bash
docker compose up -d postgres
```

### 2) Restaure os pacotes

```bash
dotnet restore TaskTrack.Api.slnx
```

### 3) Rode a API local (fora do container)

```bash
dotnet run --project TaskTrack.Api
```

As migrations tambem serao aplicadas automaticamente no startup da API.

### 4) Debug no VS Code

1. Abra a pasta do repositorio no VS Code
2. Instale a extensao oficial C# (se ainda nao tiver)
3. Abra o arquivo que deseja depurar e clique na margem esquerda para criar breakpoint
4. Pressione `F5`
5. Se o VS Code perguntar o perfil, selecione o projeto da API (`TaskTrack.Api`)
6. Execute uma requisicao (Swagger/Postman) para atingir o endpoint e parar no breakpoint

### 5) Debug no Visual Studio

1. Abra a solucao `TaskTrack.Api.slnx`
2. Defina `TaskTrack.Api` como Startup Project
3. Configure o profile para `Development` (quando necessario)
4. Coloque breakpoints no controller/service desejado
5. Pressione `F5` (ou botao Start)
6. Execute uma requisicao para atingir o endpoint e parar no breakpoint

## Comandos uteis

Subir tudo em container:

```bash
docker compose up -d
```

Subir tudo com rebuild da imagem da API (quando necessario):

```bash
docker compose up --build -d
```

Ver logs da API em container:

```bash
docker compose logs -f api
```

Ver logs do banco:

```bash
docker compose logs -f postgres
```

Parar ambiente mantendo dados:

```bash
docker compose down
```

Parar ambiente e resetar banco local:

```bash
docker compose down -v
```

## Variaveis de ambiente do Compose

Voce pode sobrescrever no arquivo `.env`:

- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `POSTGRES_PORT`
- `API_PORT`

Padroes atuais estao em `.env.example`.

## Troubleshooting

- Porta ocupada (5432 ou 8080):
  - Ajuste `POSTGRES_PORT` e/ou `API_PORT` no `.env`.

- Breakpoint nao para:
  - Confirme que a API esta rodando localmente (modo debug), nao no container da API.
  - Confirme que o projeto iniciado e `TaskTrack.Api`.
  - Rebuild com `dotnet build TaskTrack.Api.slnx`.

- API nao sobe por falha no banco:
  - Verifique `docker compose logs -f postgres`.
  - Verifique credenciais no `.env`.
