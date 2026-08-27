# Registro de Desenvolvimento — TG

Este documento registra de forma narrativa e cronológica tudo que foi desenvolvido durante o Trabalho de Graduação. Seu propósito é servir como base para a redação do capítulo de desenvolvimento da documentação oficial, com formatação ABNT. O texto já está redigido em estilo acadêmico formal para facilitar essa transição.

---

## 1. Visão Geral da Solução

O sistema desenvolvido neste trabalho consiste em uma plataforma web responsiva voltada à gestão do fluxo completo de trabalho de laboratórios de patologia veterinária, abrangendo desde o recebimento de solicitações de exames até a emissão e disponibilização dos laudos ao médico veterinário solicitante e ao tutor do animal.

A solução foi concebida para atender a um único laboratório, conforme escopo definido no Projeto de Trabalho de Graduação, e estruturada de modo que sua arquitetura permita expansão futura sem necessidade de reescrita significativa. O sistema contempla três perfis de interação distintos: o patologista, único usuário autenticado da plataforma, que gerencia o fluxo interno de exames, laudos e estoque; o médico veterinário solicitante, que submete solicitações de exames e consulta o status de suas requisições sem necessidade de cadastro formal; e o tutor do animal, que pode consultar o andamento de um exame por meio de um código público único, sem acesso a informações sensíveis, em conformidade com os princípios da Lei Geral de Proteção de Dados (LGPD).

---

## 2. Definição da Stack Tecnológica

A definição das tecnologias utilizadas no projeto foi orientada por critérios de coesão entre as camadas do sistema, maturidade das ferramentas, disponibilidade de recursos de hospedagem gratuita e adequação ao escopo do trabalho.

### 2.1 Frontend — Angular

Para o desenvolvimento da interface do usuário, foi adotado o framework Angular, mantido pela Google. A escolha do Angular em substituição ao Next.js, tecnologia inicialmente considerada durante a elaboração do PTG, foi motivada pela maior coesão com o ecossistema Microsoft, ao qual o backend pertence. O Angular oferece uma estrutura fortemente opinada, com módulos, serviços e injeção de dependências nativos, o que favorece a organização do código em projetos de médio porte. O projeto foi criado com suporte a roteamento habilitado e SCSS como pré-processador de estilos, sem renderização do lado do servidor (SSR), uma vez que a aplicação opera como SPA (Single Page Application) e não apresenta requisitos de SEO que justificassem a complexidade adicional do SSR.

### 2.2 Backend — .NET 10 com C#

A camada de backend foi desenvolvida utilizando a plataforma .NET na versão 10, com a linguagem C#. A escolha do .NET se deve à sua robustez, tipagem estática, desempenho elevado e ao amplo suporte ao desenvolvimento de APIs REST por meio do ASP.NET Core. A plataforma oferece ferramentas maduras para autenticação, injeção de dependências, ORM e testes, características essenciais para um sistema com o perfil proposto. A API foi estruturada utilizando controllers, padrão que oferece maior organização e clareza de responsabilidades em comparação com as Minimal APIs, abordagem mais recente do ecossistema .NET adequada a projetos de menor escala.

### 2.3 Banco de Dados — SQL Server com Entity Framework Core

Para a persistência de dados, foi adotado o SQL Server como sistema gerenciador de banco de dados relacional, acessado por meio do ORM (Object-Relational Mapper) Entity Framework Core. A escolha pelo SQL Server, em detrimento de alternativas como PostgreSQL, foi motivada pela coesão com o stack Microsoft — .NET e SQL Server são tecnologias desenvolvidas pela mesma empresa e apresentam integração nativa e madura, especialmente por meio do Entity Framework Core, que oferece suporte pleno ao SQL Server incluindo tipos de dados específicos, migrations e scaffolding. Para o ambiente de desenvolvimento local, foi utilizado o SQL Server LocalDB, componente que acompanha o Visual Studio e permite a criação e gerenciamento de bancos de dados relacionais sem a necessidade de instalação de uma instância completa do SQL Server. As tabelas e estruturas do banco podem ser visualizadas diretamente pelo SQL Server Object Explorer, ferramenta integrada ao Visual Studio, acessível pelo menu View.

### 2.4 Inteligência Artificial — Google Gemini API

Para o módulo de extração multimodal de dados a partir de guias de solicitação enviadas como imagem, PDF ou áudio, será utilizada a API do Google Gemini, modelo de linguagem de grande escala multimodal desenvolvido pela Google. A integração com o Gemini será realizada exclusivamente na camada de backend, por meio de chamadas HTTP autenticadas com chave de API armazenada em variável de ambiente. Dessa forma, a chave de API nunca é exposta ao cliente, e o custo de uso da inteligência artificial é absorvido pelo laboratório, sem qualquer repasse ao médico veterinário solicitante. O tier gratuito da API do Gemini oferece cotas suficientes para o volume de requisições esperado em um laboratório de pequeno a médio porte.

### 2.5 Hospedagem — Microsoft Azure

A hospedagem da solução completa será realizada na plataforma Microsoft Azure, por meio do programa Azure for Students, que disponibiliza créditos e recursos gratuitos mediante comprovação de vínculo institucional. A escolha pelo Azure é coerente com o stack Microsoft adotado no projeto e permite hospedar os três componentes da solução sem custo adicional: o frontend Angular no Azure Static Web Apps (tier gratuito permanente), o backend .NET no Azure App Service (tier F1 gratuito), e o banco de dados SQL Server no Azure SQL Database (tier serverless gratuito, com 100.000 vCore-segundos e 32 GB de armazenamento por mês). Durante o desenvolvimento, toda a aplicação é executada localmente, com a migração para a nuvem prevista para a fase final do projeto, após validação completa das funcionalidades.

---

## 3. Arquitetura do Sistema

### 3.1 Arquitetura Geral

O sistema é estruturado em três camadas principais: o frontend Angular, que se comunica com o backend por meio de requisições HTTP à API REST; o backend .NET, responsável pela lógica de negócio, autenticação, integração com serviços externos e acesso ao banco de dados; e o banco de dados SQL Server, que persiste todas as informações do sistema. A integração com a API do Google Gemini ocorre de forma lateral ao backend, sendo acionada apenas quando o médico veterinário solicitante envia um arquivo (imagem, PDF ou áudio) para criação automatizada de uma solicitação. A comunicação entre frontend e backend segue o protocolo HTTP com respostas no formato JSON, e a autenticação é realizada por meio de tokens JWT (JSON Web Token).

### 3.2 Arquitetura do Backend — Clean Architecture

Para a organização interna do backend, foi adotada a Clean Architecture, padrão arquitetural que propõe a separação do código em camadas concêntricas com direções de dependência bem definidas. A adoção desse padrão foi motivada pela clareza de responsabilidades que ele proporciona: cada parte do código tem um papel explícito e bem delimitado, o que facilita a compreensão, manutenção e evolução do sistema. Dentro de cada camada, o código é organizado por funcionalidade (feature), de modo que tudo relacionado a uma determinada área do sistema — como solicitações de exames ou laudos — esteja agrupado, em vez de disperso em pastas genéricas de ViewModels, Services e afins.

A solução foi dividida em quatro projetos distintos dentro de uma única solução .NET:

**LabPat.Domain** é a camada mais interna da arquitetura e não possui dependências externas. Nela estão definidas as entidades de domínio, que representam os conceitos centrais do negócio, bem como as interfaces que definem os contratos dos repositórios e do padrão Unit of Work. Por não depender de nenhuma outra camada, o Domain pode ser testado e evoluído de forma isolada.

**LabPat.Application** depende apenas do Domain e concentra a lógica de negócio da aplicação. É organizada em subpastas por feature: cada funcionalidade do sistema possui sua própria pasta contendo os DTOs (objetos de transferência de dados), os modelos de entrada (InputModels), a interface do serviço e a implementação do serviço. Essa organização por feature elimina a necessidade de navegar entre múltiplas pastas genéricas para compreender o fluxo de uma funcionalidade específica.

**LabPat.Infrastructure** depende do Domain e do Application e é responsável por tudo que envolve detalhes técnicos e infraestrutura: a implementação dos repositórios definidos no Domain, o contexto de banco de dados (AppDbContext) com suas migrations, e os serviços externos como a integração com o Google Gemini e a geração de PDFs dos laudos.

**LabPat.Api** é a camada mais externa e depende do Application e do Infrastructure. Contém os controllers da API REST, que são intencionalmente finos — sua única responsabilidade é receber requisições HTTP, chamar o serviço adequado da camada Application e retornar a resposta. O Program.cs desta camada é responsável pelo registro de todas as dependências no container de IoC (Inversion of Control) nativo do ASP.NET Core, associando cada interface à sua implementação concreta.

Essa organização garante que a direção das dependências sempre aponte para dentro: Api depende de Application e Infrastructure; Application depende de Domain; Infrastructure depende de Domain; Domain não depende de ninguém. Isso significa que o domínio e as regras de negócio são completamente independentes de frameworks, bancos de dados ou interfaces externas.

Os pacotes NuGet instalados na solução são: `Microsoft.EntityFrameworkCore.SqlServer` e `Microsoft.EntityFrameworkCore.Tools` no projeto Infrastructure, para acesso ao SQL Server e suporte às migrations por linha de comando; `Microsoft.EntityFrameworkCore.Design` no projeto Api, necessário para que o projeto de startup seja reconhecido pelo CLI do Entity Framework durante a execução de migrations; e `Microsoft.AspNetCore.Authentication.JwtBearer` no projeto Api, para suporte à autenticação via tokens JWT.

---

## 4. Modelagem do Banco de Dados

### 4.1 Critérios de Modelagem

A modelagem do banco de dados foi realizada com base no levantamento de requisitos do sistema e nas especificidades do fluxo laboratorial de patologia veterinária descrito no PTG. Os principais critérios adotados foram: fidelidade ao processo real de trabalho do laboratório, aderência à LGPD na definição de quais dados seriam expostos publicamente, e suporte ao desconto automático de estoque vinculado à conclusão de exames.

### 4.2 Entidades e Seus Papéis

**Usuario** representa o patologista responsável pelo laboratório, único ator com acesso autenticado ao sistema. Armazena nome, e-mail, hash da senha e status de ativação da conta. O e-mail possui restrição de unicidade no banco de dados.

**VetSolicitante** representa o médico veterinário que solicita os exames. Diferentemente do Usuario, o VetSolicitante não possui credenciais de acesso ao sistema — ele é identificado pelo número de registro no Conselho Regional de Medicina Veterinária (CRMV) e pelo estado de emissão. O cadastro ocorre automaticamente no momento da primeira solicitação, por meio do padrão "busca ou cria": caso já exista um registro com o mesmo CRMV, ele é reutilizado; caso contrário, um novo registro é criado.

**Tutor** representa o proprietário do animal paciente. Assim como o VetSolicitante, o tutor não possui conta no sistema e é cadastrado no momento da solicitação. Seu papel no sistema é estritamente passivo: recebe o código público de consulta e acessa a página de status sem necessidade de autenticação.

**Paciente** representa o animal submetido ao exame. Está vinculado a um tutor e armazena informações como espécie, raça, sexo, idade e peso. A relação entre tutor e paciente é de um para muitos, pois um mesmo tutor pode ter vários animais cadastrados.

**TipoExame** representa as categorias de exames oferecidos pelo laboratório, como citologia, histopatologia, hemograma e análises bioquímicas. Cada tipo de exame possui um prazo estimado de conclusão em dias e pode ter um ou mais templates de laudo associados.

**TemplateLaudo** armazena o conteúdo base do laudo para um determinado tipo de exame. O campo de versão permite que alterações futuras no template não afetem laudos já emitidos, pois cada laudo emitido registra seu conteúdo de forma independente.

**Insumo** representa um item do estoque do laboratório, como reagentes, lâminas, frascos e demais materiais consumíveis. Possui campos para quantidade atual, quantidade mínima de alerta e unidade de medida.

**ExameInsumo** é a tabela de junção que representa a relação muitos-para-muitos entre TipoExame e Insumo. Além das chaves estrangeiras, armazena a quantidade do insumo consumida por execução daquele tipo de exame. Essa estrutura permite que, ao concluir um exame, o sistema desconte automaticamente do estoque os insumos configurados para aquele tipo, sem intervenção manual do patologista.

**Solicitacao** é a entidade central do sistema, representando uma requisição de exame desde sua criação até a conclusão. Armazena um código público alfanumérico único, gerado automaticamente no momento da criação, que é utilizado pelo tutor para consultar o status do exame sem necessidade de login. Registra também o método de entrada da solicitação — Manual, Imagem, PDF ou Áudio — e a URL do arquivo enviado, quando aplicável.

**Laudo** representa o documento de diagnóstico emitido pelo patologista ao concluir a análise de uma amostra. Possui relação um-para-um com a Solicitacao, garantida por índice único no banco de dados. Armazena o conteúdo do laudo, a URL do arquivo PDF gerado e a referência ao patologista responsável pela emissão.

**HistoricoStatus** registra cada mudança de status ocorrida em uma Solicitacao ao longo do seu ciclo de vida. Armazena o status anterior, o status novo, a data e hora da alteração, o usuário responsável pela mudança e uma observação opcional. Essa entidade garante rastreabilidade completa do processo e permite que o patologista visualize o histórico de cada exame.

### 4.3 Fluxo de Status da Solicitação

O ciclo de vida de uma solicitação de exame é representado por cinco estados sequenciais: **Solicitado**, estado inicial criado no momento do recebimento da requisição pelo sistema; **AguardandoAmostra**, indicando que a solicitação foi processada mas a amostra física ainda não chegou ao laboratório; **AmostraRecebida**, confirmando o recebimento físico da amostra; **EmAnalise**, sinalizando que o patologista iniciou a análise laboratorial; e **Concluido**, estado final atingido com a emissão do laudo, momento em que o desconto automático de estoque é acionado.

### 4.4 Decisões de Modelagem

Duas decisões de modelagem merecem destaque por seu impacto nas funcionalidades do sistema.

A primeira é a geração do `CodigoPublico` na entidade Solicitacao. Esse campo recebe um valor alfanumérico curto e único, gerado automaticamente pelo backend no momento da criação da solicitação. O código é compartilhado pelo médico veterinário com o tutor do animal e permite que este consulte o status do exame em uma página pública, sem autenticação. A página exibe apenas informações não sensíveis — tipo de exame, status atual e data estimada de conclusão — em aderência aos princípios de minimização de dados da LGPD.

A segunda é a integração entre estoque e exames por meio da tabela ExameInsumo. Ao configurar um tipo de exame, o patologista associa a ele os insumos consumidos em cada execução e a respectiva quantidade. Quando uma solicitação desse tipo de exame é concluída e o laudo é emitido, o sistema percorre automaticamente os registros de ExameInsumo correspondentes e deduz as quantidades do estoque, mantendo-o atualizado sem necessidade de registro manual pelo patologista.

---

## 5. Implementação das Entidades do Domínio

As entidades do domínio foram implementadas no projeto LabPat.Domain, na pasta `Entities/`, e constituem a representação em código das tabelas e relacionamentos definidos na modelagem. Optou-se por criar uma classe base abstrata, denominada `EntityBase`, que centraliza os campos comuns a todas as entidades: a chave primária inteira auto-incrementada (`Id`) e o campo de data de criação (`CriadoEm`), inicializado automaticamente com a data e hora UTC do momento de criação do objeto. Essa abordagem elimina a duplicação desses campos em cada entidade individualmente.

As propriedades de navegação (referências entre entidades) foram declaradas com inicialização em valores não nulos, seguindo as convenções do C# 8+ com tipos de referência anuláveis habilitados. Propriedades obrigatórias são inicializadas com `string.Empty` ou com `null!` para referências a entidades relacionadas, enquanto propriedades opcionais são declaradas com o operador `?`. Coleções de navegação são inicializadas com a sintaxe de coleção vazia `[]`, disponível a partir do C# 12.

Os enumeradores do sistema foram definidos na pasta `Enums/` do projeto Domain: `StatusSolicitacao` com os cinco estados do ciclo de vida do exame; `MetodoEntrada` com as quatro formas de criação de solicitação suportadas pelo sistema; e `SexoPaciente` com as opções de sexo biológico do animal.

---

## 6. Configuração do Contexto de Banco de Dados

O contexto de banco de dados foi implementado por meio da classe `AppDbContext`, localizada no projeto LabPat.Infrastructure, na pasta `Data/`. A classe herda de `DbContext` do Entity Framework Core e recebe as opções de configuração por injeção de dependência, por meio do construtor primário do C# 12. Cada entidade do domínio foi exposta como uma propriedade `DbSet<T>`, permitindo que o Entity Framework Core mapeie cada classe para sua respectiva tabela no banco de dados.

As configurações específicas de mapeamento foram definidas no método `OnModelCreating`, seguindo o padrão do Entity Framework Core. As configurações estabelecidas incluem: a chave primária composta da entidade ExameInsumo, formada pelos campos `TipoExameId` e `InsumoId`; o índice único sobre o campo `CodigoPublico` da entidade Solicitacao, garantindo que cada solicitação possua um código de consulta pública exclusivo; o índice único sobre o campo `SolicitacaoId` da entidade Laudo, assegurando a relação um-para-um entre laudo e solicitação; o índice único sobre o campo `Email` da entidade Usuario; e a precisão explícita de todos os campos do tipo `decimal` do sistema — `PesoKg` com precisão (6, 2), e os campos de quantidade de Insumo e ExameInsumo com precisão (10, 3) — necessária para evitar truncamentos silenciosos de valores no SQL Server.

O padrão Unit of Work foi implementado por meio da classe `UnitOfWork`, que encapsula a chamada ao método `SaveChangesAsync` do contexto. Essa abstração permite que os serviços da camada Application solicitem a persistência de múltiplas operações em uma única transação, sem depender diretamente do `AppDbContext`.

---

## 7. Migrations e Banco de Dados Local

As migrations do Entity Framework Core foram utilizadas para geração automatizada do schema do banco de dados a partir das entidades de domínio, eliminando a necessidade de criação manual de scripts SQL. A migration inicial, denominada `InitialCreate`, foi gerada por meio da CLI do Entity Framework Core com o seguinte comando:

```
dotnet ef migrations add InitialCreate --project backend/LabPat.Infrastructure --startup-project backend/LabPat.Api
```

O parâmetro `--project` aponta para o projeto que contém o `AppDbContext` (Infrastructure), enquanto `--startup-project` indica o projeto de entrada da aplicação (Api), necessário para que o EF Core carregue as configurações de connection string definidas no `appsettings`.

Após a geração da migration, o banco de dados foi criado e o schema aplicado com o comando:

```
dotnet ef database update --project backend/LabPat.Infrastructure --startup-project backend/LabPat.Api
```

O banco de dados `LabPat` foi criado no SQL Server LocalDB, instância leve do SQL Server distribuída junto ao Visual Studio, adequada para ambientes de desenvolvimento local. A connection string utilizada em ambiente de desenvolvimento é armazenada no arquivo `appsettings.Development.json`, que é ignorado pelo controle de versão para evitar exposição de credenciais. Em ambiente de produção, a connection string apontará para o Azure SQL Database, sem necessidade de alteração no código — apenas na configuração do ambiente.

A migration `InitialCreate` gerou as seguintes tabelas no banco: `Usuarios`, `VetsSolicitantes`, `Tutores`, `Pacientes`, `TiposExame`, `TemplatesLaudo`, `Insumos`, `ExameInsumos`, `Solicitacoes`, `Laudos` e `HistoricoStatus`, além da tabela de controle interno `__EFMigrationsHistory`, utilizada pelo Entity Framework Core para rastrear quais migrations já foram aplicadas ao banco.

---

## 8. Autenticação do Patologista

A autenticação do patologista foi implementada por meio do padrão JWT (JSON Web Token), amplamente adotado em APIs REST modernas por sua natureza stateless: o servidor não armazena sessões, e cada requisição carrega em seu cabeçalho o token que comprova a identidade e as permissões do usuário autenticado.

### 8.1 Organização em Camadas

A implementação da autenticação respeitou rigorosamente a separação de responsabilidades definida pela Clean Architecture. Na camada de domínio, foi adicionada a interface `IUsuarioRepository`, que estende o repositório genérico `IRepository<Usuario>` com o método `GetByEmailAsync`, utilizado para localizar o usuário pelo e-mail informado no login.

Na camada de aplicação, foram criadas duas interfaces de suporte no namespace `Application.Common`: `IPasswordHasher`, responsável por abstrair o mecanismo de hashing e verificação de senhas, e `ITokenGenerator`, responsável por abstrair a geração do token JWT. Essa separação garante que a camada de aplicação não dependa diretamente de bibliotecas de infraestrutura como BCrypt ou as classes de JWT do .NET — ela conhece apenas os contratos.

A feature de autenticação foi organizada na pasta `Application/Features/Auth/`, contendo: `LoginInput`, record que representa os dados de entrada do login (e-mail e senha); `AuthDto`, record que representa a resposta bem-sucedida do login (token, nome e e-mail do usuário); `IAuthService`, interface do serviço; e `AuthService`, implementação do serviço de autenticação. O `AuthService` recebe por injeção de dependência o `IUsuarioRepository`, o `IPasswordHasher` e o `ITokenGenerator`, buscando o usuário pelo e-mail, verificando a senha e, em caso de sucesso, delegando a geração do token ao `ITokenGenerator`.

Na camada de infraestrutura, as implementações concretas foram criadas na pasta `Infrastructure/Security/`: `BcryptPasswordHasher`, que utiliza a biblioteca BCrypt.Net-Next para realizar o hashing e a verificação de senhas com o algoritmo BCrypt; e `JwtTokenGenerator`, que utiliza a biblioteca `System.IdentityModel.Tokens.Jwt` para gerar tokens assinados com o algoritmo HMAC-SHA256, incluindo nos claims do token o identificador, o e-mail e o nome do patologista. As configurações do token — emissor, audiência, tempo de expiração e chave secreta — são lidas do arquivo de configuração via `IConfiguration`, permitindo que valores sensíveis, como a chave secreta, sejam definidos por variáveis de ambiente em produção sem alteração de código.

### 8.2 Endpoint de Login

O endpoint de autenticação foi exposto por meio do `AuthController`, localizado na camada de Api. O controller responde requisições `POST` na rota `/api/auth/login`, recebendo o `LoginInput` no corpo da requisição. Em caso de credenciais inválidas, retorna o status HTTP 401 (Unauthorized) com uma mensagem de erro genérica — sem indicar se o e-mail ou a senha estão incorretos, por razão de segurança. Em caso de sucesso, retorna o status HTTP 200 com o `AuthDto` contendo o token e os dados básicos do usuário.

### 8.3 Configuração do JWT no Pipeline

O `Program.cs` foi atualizado para configurar corretamente o pipeline de autenticação JWT do ASP.NET Core. Os parâmetros de validação do token definem que o servidor deve verificar o emissor, a audiência, o tempo de vida e a assinatura do token em toda requisição recebida. A chave secreta utilizada para assinar e verificar os tokens é lida da configuração e nunca é exposta em código-fonte ou no repositório.

### 8.4 Registro de Dependências

O registro de todas as dependências foi centralizado no `Program.cs` da Api, vinculando cada interface à sua implementação concreta no container de IoC do ASP.NET Core: `IUsuarioRepository` → `UsuarioRepository`; `IUnitOfWork` → `UnitOfWork`; `IPasswordHasher` → `BcryptPasswordHasher`; `ITokenGenerator` → `JwtTokenGenerator`; `IAuthService` → `AuthService`. Todos os registros utilizam o ciclo de vida `Scoped`, adequado para operações que devem ser criadas e destruídas a cada requisição HTTP.

### 8.5 Seed de Dados para Desenvolvimento

Para viabilizar os testes durante o desenvolvimento, foi criada a classe estática `DbSeeder`, localizada em `Infrastructure/Seeding/`. Ao inicializar a aplicação em ambiente de desenvolvimento, o `Program.cs` verifica se existe algum usuário cadastrado no banco e, caso não exista, insere automaticamente um usuário padrão com e-mail `admin@labpat.com` e senha `Admin@123`, com a senha armazenada como hash BCrypt. Esse comportamento ocorre apenas em ambiente de desenvolvimento, sendo completamente desabilitado em produção.

### 8.6 Segurança das Configurações

A chave secreta do JWT, essencial para a integridade dos tokens, é armazenada exclusivamente no arquivo `appsettings.Development.json`, que é ignorado pelo sistema de controle de versão por meio do `.gitignore`. O arquivo `appsettings.json`, presente no repositório, contém apenas a estrutura de configuração sem valores sensíveis. Em produção, a chave será fornecida por variável de ambiente configurada no Azure App Service, seguindo a prática recomendada de separação entre configuração e código.

---

## 9. Módulo de Tipos de Exame e Templates de Laudo

O módulo de tipos de exame representa o primeiro cadastro configurável do sistema e constitui um pré-requisito para a criação de solicitações, uma vez que toda solicitação deve estar vinculada a um tipo de exame previamente cadastrado pelo patologista. Tipos de exame típicos incluem citologia, histopatologia, hemograma, análises bioquímicas, urinálise e parasitologia, entre outros conforme a oferta do laboratório.

### 9.1 Repositório Base Genérico

Antes de implementar o repositório específico de tipos de exame, foi criada a classe abstrata `RepositoryBase<T>`, localizada em `Infrastructure/Repositories/`. Essa classe implementa a interface genérica `IRepository<T>` e centraliza as operações CRUD comuns a todos os repositórios: busca por id, listagem completa, adição, atualização e remoção. Todos os repositórios específicos do sistema herdam de `RepositoryBase<T>` e adicionam apenas os métodos que são particulares a cada entidade, eliminando a duplicação de código. O `UsuarioRepository` foi refatorado para utilizar essa classe base na mesma oportunidade.

### 9.2 Organização da Feature

A feature de tipos de exame foi implementada na pasta `Application/Features/TiposExame/`, seguindo o mesmo padrão de organização por funcionalidade adotado no módulo de autenticação. Os arquivos criados foram:

- `TipoExameDto` e `TipoExameDetalhadoDto`: records que representam as respostas da API. O DTO simples retorna os campos básicos do tipo de exame; o DTO detalhado inclui também a lista de templates de laudo associados.
- `TemplateLaudoDto`: record de resposta para templates individuais, contendo id, conteúdo, versão e data de criação.
- `CreateTipoExameInput` e `UpdateTipoExameInput`: records que representam os dados de entrada para criação e atualização, respectivamente.
- `CreateTemplateLaudoInput`: record de entrada para adição de um novo template a um tipo de exame existente.
- `ITipoExameService` e `TipoExameService`: interface e implementação do serviço.

### 9.3 Lógica de Negócio

O `TipoExameService` implementa as operações de listagem, busca por id, criação, atualização e exclusão lógica de tipos de exame, além da adição de templates de laudo. A exclusão é implementada como soft delete: o campo `Ativo` é alterado para `false`, preservando o histórico de solicitações já vinculadas ao tipo de exame sem remover o registro do banco.

A adição de templates implementa versionamento automático: ao adicionar um novo template a um tipo de exame, o serviço identifica a versão mais alta entre os templates já existentes e atribui ao novo template o número subsequente. Isso garante que a evolução dos templates seja rastreável e que laudos já emitidos não sejam afetados por alterações futuras no conteúdo padrão.

### 9.4 Repositório e Endpoints

O `ITipoExameRepository` define, além dos métodos herdados do repositório genérico, dois métodos específicos: `GetAllAtivosAsync`, que retorna apenas tipos de exame com `Ativo = true` ordenados por nome; e `GetByIdComTemplatesAsync`, que utiliza o método `Include` do Entity Framework Core para carregar a lista de templates junto ao tipo de exame em uma única consulta ao banco, evitando o problema de N+1 queries.

Os endpoints REST expostos pelo `TiposExameController` são:

- `GET /api/tipos-exame` — lista todos os tipos de exame ativos
- `GET /api/tipos-exame/{id}` — retorna um tipo de exame com seus templates
- `POST /api/tipos-exame` — cria um novo tipo de exame
- `PUT /api/tipos-exame/{id}` — atualiza um tipo de exame existente
- `DELETE /api/tipos-exame/{id}` — realiza soft delete do tipo de exame
- `POST /api/tipos-exame/{id}/templates` — adiciona um novo template de laudo ao tipo de exame

Todos os endpoints exigem autenticação JWT, declarada pelo atributo `[Authorize]` no nível do controller, garantindo que apenas o patologista autenticado possa realizar operações de cadastro.

---

## 10. Módulo de Insumos e Controle de Estoque

O módulo de insumos gerencia o estoque de materiais consumíveis do laboratório, como reagentes, lâminas, frascos e demais itens utilizados na execução dos exames. Sua implementação estabelece tanto o cadastro individual dos insumos quanto o vínculo entre insumos e tipos de exame, base para o desconto automático de estoque ao concluir uma solicitação.

### 10.1 Feature de Insumos

A feature foi implementada na pasta `Application/Features/Insumos/`, seguindo o padrão estabelecido nos módulos anteriores. O `InsumoDto` inclui, além dos campos cadastrais, o campo calculado `EmEstoqueBaixo`, que retorna verdadeiro quando a quantidade atual é inferior à quantidade mínima configurada. Esse campo permite que a interface exiba alertas visuais ao patologista sem necessidade de lógica adicional no frontend.

O serviço `InsumoService` implementa as operações de listagem, busca, criação, atualização e exclusão lógica (soft delete via campo `Ativo`). Inclui também a operação `AjustarQuantidadeAsync`, que permite ao patologista registrar manualmente a quantidade atual de um insumo — útil para entrada de novos itens no estoque ou correção de divergências após inventário. O desconto automático ao concluir exames, por sua vez, é responsabilidade do módulo de laudos e ocorre sem intervenção manual.

Os endpoints expostos pelo `InsumosController` são:

- `GET /api/insumos` — lista todos os insumos ativos
- `GET /api/insumos/{id}` — retorna um insumo específico
- `POST /api/insumos` — cadastra um novo insumo (quantidade inicial zero)
- `PUT /api/insumos/{id}` — atualiza nome, unidade de medida, quantidade mínima e status
- `DELETE /api/insumos/{id}` — soft delete do insumo
- `PATCH /api/insumos/{id}/quantidade` — ajuste manual da quantidade atual

### 10.2 Vínculo entre Insumos e Tipos de Exame

A configuração de quais insumos são consumidos por cada tipo de exame é gerenciada por meio de dois endpoints adicionados ao `TiposExameController`:

- `POST /api/tipos-exame/{id}/insumos` — vincula um insumo a um tipo de exame com a quantidade consumida por execução; se o vínculo já existir, atualiza a quantidade (operação de upsert)
- `DELETE /api/tipos-exame/{id}/insumos/{insumoId}` — remove o vínculo entre insumo e tipo de exame

Essa operação de upsert simplifica a experiência do patologista ao configurar insumos: ele não precisa verificar se um vínculo já existe antes de salvar — o sistema trata automaticamente os dois casos.

O `TipoExameDetalhadoDto` foi atualizado para incluir a lista de insumos vinculados ao tipo de exame, de modo que ao consultar um tipo de exame pelo id, o patologista visualiza em uma única resposta os templates de laudo e os insumos configurados.

### 10.3 Atualização do Repositório de Tipos de Exame

O método de busca por id do `TipoExameRepository` foi atualizado para `GetByIdComDetalhesAsync`, carregando em uma única consulta os templates de laudo e os vínculos com insumos, incluindo os dados completos de cada insumo por meio do método `ThenInclude` do Entity Framework Core. Isso garante que todas as informações necessárias para exibir ou manipular um tipo de exame estejam disponíveis sem consultas adicionais ao banco.

---

<!-- Novas seções serão adicionadas aqui conforme o desenvolvimento avança -->
