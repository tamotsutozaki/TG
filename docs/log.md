# Log de progresso — TG

Registro macro de tudo que foi desenvolvido durante o TG. Serve como base para construir a documentação oficial.

---

## Estrutura inicial do projeto

- Repositório criado no GitHub: https://github.com/tamotsutozaki/TG
- Estrutura de pastas definida: `frontend/` (Angular), `backend/` (.NET C#), `docs/`
- Stack definida: Angular + .NET C# + SQL Server + Google Gemini API + Azure (Azure for Students)
- Decisão: sistema desenvolvido para atender um único laboratório (escopo do TG conforme PTG)
- Decisão: SQL Server escolhido por coesão com o stack .NET/Microsoft; hospedagem via Azure SQL Database (free tier)
- Decisão: Angular escolhido no lugar de Next.js por maior coesão com o ecossistema .NET

---

## Modelagem do banco de dados

Definição das entidades e relacionamentos do sistema antes do início da implementação.

**Entidades definidas:**
- `Usuario` — patologista autenticado (único ator com login no sistema)
- `VetSolicitante` — médico veterinário solicitante, sem conta/login, identificado por CRMV
- `Tutor` — dono do animal, sem conta/login, acessa apenas via código público
- `Paciente` — animal vinculado a um tutor
- `TipoExame` — tipo de exame laboratorial (ex: citologia, histopatologia)
- `TemplateLaudo` — template de laudo vinculado a um tipo de exame, com versionamento
- `Insumo` — item de estoque do laboratório
- `ExameInsumo` — tabela de junção N:M entre TipoExame e Insumo, com quantidade consumida por exame
- `Solicitacao` — solicitação de exame, com código público único para consulta externa
- `Laudo` — laudo emitido pelo patologista, relação 1:1 com Solicitacao
- `HistoricoStatus` — auditoria de todas as mudanças de status de uma solicitação

**Decisões de modelagem:**
- Tutor e VetSolicitante não possuem autenticação; são cadastrados no momento da solicitação pelo fluxo "busca ou cria"
- `CodigoPublico` na Solicitacao é gerado automaticamente e usado pelo tutor para consultar status sem login
- Ao emitir o laudo (status → Concluído), o sistema desconta automaticamente do estoque os insumos configurados para aquele tipo de exame via tabela `ExameInsumo`
- `TemplateLaudo` possui campo `Versao` para preservar laudos antigos caso o template seja alterado
- Chave do Google Gemini armazenada em variável de ambiente no backend (não no banco)

**Fluxo de status da Solicitacao:**
`Solicitado → AguardandoAmostra → AmostraRecebida → EmAnalise → Concluido`

---

## Configuração do projeto Angular (frontend)

- Projeto criado com Angular CLI 18 na pasta `frontend/`
- Configurações: routing habilitado, SCSS como pré-processador, sem SSR
- Ambiente de desenvolvimento local na porta `http://localhost:4200`

---

## Configuração do backend .NET — Clean Architecture

Estrutura do backend definida com 4 projetos seguindo Clean Architecture com organização por feature:

- `LabPat.Domain` — entidades de domínio, interfaces de repositório e enums; sem dependências externas
- `LabPat.Application` — features organizadas por funcionalidade (DTOs, InputModels, interfaces e implementações de serviços)
- `LabPat.Infrastructure` — implementações dos repositórios, AppDbContext, migrations e serviços externos (Gemini, PDF)
- `LabPat.Api` — controllers (finos, apenas orquestram chamadas aos serviços), Program.cs com registro de IoC

**Referências entre projetos:**
- Application → Domain
- Infrastructure → Domain + Application
- Api → Application + Infrastructure

**Pacotes NuGet instalados:**
- `Microsoft.EntityFrameworkCore.SqlServer` (Infrastructure) — ORM com suporte a SQL Server
- `Microsoft.EntityFrameworkCore.Tools` (Infrastructure) — ferramentas de migration via CLI
- `Microsoft.EntityFrameworkCore.Design` (Api) — suporte ao startup project para o CLI do EF
- `Microsoft.AspNetCore.Authentication.JwtBearer` (Api) — autenticação JWT

**Configurações do Program.cs (Api):**
- CORS configurado para aceitar requisições do Angular (`http://localhost:4200`)
- EF Core registrado com connection string via `appsettings`
- Autenticação JWT Bearer registrada

---

## Entidades do domínio (LabPat.Domain)

Criação de todas as entidades, enums e interfaces base do domínio.

**Entidades criadas** em `LabPat.Domain/Entities/`:
`EntityBase`, `Usuario`, `Tutor`, `Paciente`, `VetSolicitante`, `TipoExame`, `TemplateLaudo`, `Insumo`, `ExameInsumo`, `Solicitacao`, `Laudo`, `HistoricoStatus`

**Enums criados** em `LabPat.Domain/Enums/`:
- `StatusSolicitacao`: Solicitado, AguardandoAmostra, AmostraRecebida, EmAnalise, Concluido
- `MetodoEntrada`: Manual, Imagem, PDF, Audio
- `SexoPaciente`: Macho, Femea, NaoInformado

**Interfaces criadas** em `LabPat.Domain/Interfaces/`:
- `IRepository<T>` — contrato genérico de repositório (GetById, GetAll, Add, Update, Remove)
- `IUnitOfWork` — contrato de persistência (CommitAsync)

---

## Banco de dados — migration inicial e LocalDB

- `AppDbContext` configurado em `LabPat.Infrastructure/Data/` com todos os `DbSet` das entidades
- Configurações explícitas no `OnModelCreating`: índice único em `CodigoPublico`, relação 1:1 em `Laudo`, chave composta em `ExameInsumo`, email único em `Usuario`, precisão definida para todos os campos `decimal`
- Migration `InitialCreate` gerada e aplicada
- Banco de dados `LabPat` criado no SQL Server LocalDB para desenvolvimento local
- Connection string de desenvolvimento configurada em `appsettings.Development.json` (ignorado pelo git)
- Para visualizar o banco: Visual Studio → `View > SQL Server Object Explorer` → `(localdb)\MSSQLLocalDB` → `Databases` → `LabPat`

---

<!-- novas entradas abaixo -->
