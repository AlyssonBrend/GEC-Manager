# GEC Manager

Sistema de gestão de equipes e tarefas em .NET fullstack: API REST + frontend Blazor WebAssembly, com chat em tempo real por projeto via SignalR.

## Stack

| Projeto | Tecnologia | Papel |
|---|---|---|
| `GECManager.API` | ASP.NET Core 8 + EF Core (SQLite) + JWT | API REST, SignalR hub, migrações |
| `GECManager.Web` | Blazor WebAssembly | SPA de gestão (dashboard, projetos, kanban, chat) |
| `GECManager.Shared` | Class library | Models e DTOs compartilhados entre API e Web |
| `GECManager.Mobile` | .NET MAUI | Planejado — ainda é o template inicial |

## Funcionalidades

- **Auth**: registro e login com JWT; senhas com BCrypt em tabela separada (`UserSecrets`).
- **Papéis**: `Admin`, `Leader`, `Member`.
  - O **primeiro usuário registrado vira Admin** (bootstrap); os demais entram como Member.
  - Admin promove/rebaixa papéis em `PUT /api/users/{id}`; auto-promoção é bloqueada.
  - Admin gerencia tudo; Leader cria projetos e gerencia **os próprios** projetos/tarefas; Member move o status das tarefas **atribuídas a ele**.
- **Projetos e tarefas**: CRUD com kanban (Pendente → Em andamento → Concluído).
- **Dashboard**: contadores por status + "minhas tarefas".
- **Chat por projeto**: SignalR (`/hubs/chat`), autenticado por JWT.

## Como rodar

```bash
# 1. Chave JWT (uma vez, na pasta GECManager.API)
cd GECManager.API
dotnet user-secrets set Jwt:Key "<valor aleatório com 32+ caracteres>"

# 2. API (porta 7000/7001) — aplica as migrações no primeiro start
dotnet run

# 3. Web (porta 5093/7045), em outro terminal
cd ../GECManager.Web
dotnet run
```

Em produção, defina `Jwt__Key` como variável de ambiente. A chave nunca deve ser commitada.

> Migrações: o banco agora é criado por `Database.Migrate()`. Se você tinha um `gecmanager.db` antigo (criado por `EnsureCreated`), apague-o antes de rodar.

## Endpoints principais

- `POST /api/auth/register` · `POST /api/auth/login`
- `GET|POST|PUT|DELETE /api/projects` (mutações: Admin/Leader dono)
- `GET|POST|PUT|DELETE /api/tasks`, `PATCH /api/tasks/{id}/status`, `PATCH /api/tasks/{id}/assign`
- `GET /api/users` (Admin/Leader) · `PUT /api/users/{id}` · `DELETE /api/users/{id}` (Admin)
- `GET /api/dashboard`
- Swagger em `/swagger` (dev)

## Roadmap

- App mobile (.NET MAUI) consumindo a mesma API
- Persistência do histórico de chat
- Notificações push
