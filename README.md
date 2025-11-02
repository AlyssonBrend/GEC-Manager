“GEC Manager”

Um sistema unificado de gestão de equipes e tarefas com foco em produtividade, compatível com:
Web (gestão e dashboards),
Desktop (Windows) (controle administrativo),
Android (app móvel para membros da equipe).

🧱 Arquitetura Geral (Fullstack .NET)

Backend:
ASP.NET Core 8 (ou 9 preview) – API RESTful.
Entity Framework Core – ORM para banco de dados (SQL Server / SQLite).
Identity + JWT – autenticação e autorização.
AutoMapper / FluentValidation – mapeamento e validação.
SignalR – notificações em tempo real (mudanças de status, mensagens, etc).

Frontend Web:
Blazor WebAssembly (rodando no navegador, totalmente .NET).
→ Compartilha classes e models com o backend via projeto Shared.
Aplicativo Android:
.NET MAUI – cria app Android (e também iOS se quiser depois).
→ Pode usar os mesmos ViewModels e Models do Blazor e Backend.
→ Consome a mesma API REST.

Desktop (Windows):
    Também com .NET MAUI (modo Desktop) ou WPF + WebView2.
    → Pode exibir uma interface similar à WebApp, com recursos adicionais (admin, relatórios offline, etc).

Banco de Dados:
    SQL Server Express ou SQLite (modo leve).
        → Com EF Core Migration para versionamento do esquema.

Funcionalidades Propostas

    Autenticação & Permissões
        Login com JWT.
        Perfis: Administrador,Líder, Membro.

    Gestão de Projetos e Tarefas
        CRUD de projetos e tarefas.
        Status (Em andamento, Concluído, Pendente).
        Atribuição de tarefas a usuários.
    Chat interno (SignalR)
        Comunicação em tempo real entre membros do projeto.
    Notificações push (MAUI + Firebase)
        Alerta de novas tarefas ou alterações.
    Dashboard (Web e Desktop)
        Gráficos com Blazor Charts.
        Progresso de equipes e métricas.
Modo Offline (Mobile e Desktop)
        Sincronização com API quando voltar a ter conexão.
Fluxo Geral
1-Usuário faz login → API gera JWT.
2-API expõe endpoints REST (/api/tasks, /api/users, /api/projects).
3-Frontend (Blazor Web, MAUI Android, ou Desktop) consome esses endpoints.
4-SignalR envia notificações de mudanças em tempo real.
5-Dados sincronizados via EF Core + SQLite local (em mobile).
