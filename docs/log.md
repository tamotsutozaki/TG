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

## 11. Módulo de Solicitações

O módulo de solicitações constitui o núcleo do sistema, pois representa o fluxo central de trabalho do laboratório: o recebimento de uma requisição de exame, seu acompanhamento até a conclusão e a disponibilização do laudo. Por envolver múltiplas entidades relacionadas, é o módulo mais complexo do backend.

### 11.1 Padrão Busca ou Cria

Ao criar uma solicitação, o sistema precisa identificar o médico veterinário solicitante, o tutor do animal e o paciente. Para evitar que o usuário precise navegar por telas de cadastro separadas antes de abrir uma solicitação, foi adotado o padrão "busca ou cria" para as três entidades:

- O **VetSolicitante** é identificado pelo número e estado do CRMV. Se já existir um registro com esse CRMV no banco, ele é reutilizado; caso contrário, é criado no ato da solicitação.
- O **Tutor** é identificado pelo telefone. Se já houver um tutor com esse número cadastrado, ele é reutilizado; caso contrário, é criado.
- O **Paciente** é identificado pelo nome combinado com o id do tutor. Se o tutor já possuir um animal com aquele nome, o registro existente é reutilizado; caso contrário, um novo paciente é criado.

Esse fluxo permite que o médico veterinário submeta uma solicitação completa em uma única operação, sem cadastros prévios obrigatórios.

### 11.2 Geração do Código Público

No momento da criação da solicitação, o sistema gera automaticamente um código público alfanumérico de oito caracteres, utilizando um conjunto de caracteres sem ambiguidade visual (sem a letra O, sem o número 0, sem a letra I, etc.). O código é verificado contra o banco de dados antes de ser atribuído, garantindo unicidade. Esse código é o único dado necessário para que o tutor consulte o status do exame na página pública.

### 11.3 Cálculo da Data Estimada de Conclusão

A data estimada de conclusão é calculada automaticamente no momento da criação da solicitação, somando ao instante atual o prazo estimado em dias configurado no tipo de exame correspondente. Esse valor é exibido na página pública de consulta e pode ser atualizado pelo patologista se necessário.

### 11.4 Atualização de Status e Histórico

A atualização de status de uma solicitação é realizada por meio do endpoint `PUT /api/solicitacoes/{id}/status`. A cada mudança, um registro de `HistoricoStatus` é criado automaticamente, armazenando o status anterior, o status novo, o momento da alteração, o id do patologista responsável e uma observação opcional. Isso garante rastreabilidade completa do ciclo de vida de cada exame.

O patologista autenticado é identificado por meio da interface `ICurrentUser`, implementada pela classe `CurrentUser` na camada de Infrastructure. Essa classe lê o claim de identificador do usuário presente no token JWT da requisição atual, por meio do `IHttpContextAccessor` do ASP.NET Core.

### 11.5 Desconto Automático de Estoque

Quando o status de uma solicitação é atualizado para `Concluido`, o `SolicitacaoService` aciona automaticamente a lógica de desconto de estoque. O serviço carrega os insumos configurados para o tipo de exame da solicitação e deduz as quantidades correspondentes do estoque atual de cada insumo. Caso a quantidade atual de um insumo seja insuficiente, o valor é reduzido até zero sem gerar erro — a gestão de reposição é responsabilidade do patologista, que é alertado pelo campo `EmEstoqueBaixo` na listagem de insumos.

### 11.6 Consulta Pública por Código

O endpoint `GET /api/solicitacoes/consulta/{codigo}` é o único endpoint não autenticado do sistema (decorado com `[AllowAnonymous]`). Ele aceita o código público gerado na criação da solicitação e retorna um `ConsultaPublicaDto` com informações mínimas: código, status atual, tipo de exame, nome do paciente, data de criação e data estimada de conclusão. Nenhum dado sensível do tutor, do médico veterinário ou da descrição clínica é exposto nesse endpoint.

### 11.7 Endpoints

Os endpoints do módulo de solicitações são:

- `GET /api/solicitacoes` — lista todas as solicitações com dados resumidos (autenticado)
- `GET /api/solicitacoes/{id}` — retorna os detalhes completos de uma solicitação, incluindo histórico de status (autenticado)
- `POST /api/solicitacoes` — cria uma nova solicitação com busca ou criação automática de vet, tutor e paciente (autenticado)
- `PUT /api/solicitacoes/{id}/status` — atualiza o status da solicitação e registra no histórico (autenticado)
- `GET /api/solicitacoes/consulta/{codigo}` — consulta pública por código único (sem autenticação)

---

## 12. Módulo de Laudos e Geração de PDF

O módulo de laudos cobre a etapa final do ciclo de vida de uma solicitação: a emissão do documento de diagnóstico pelo patologista e sua disponibilização para download em formato PDF. A emissão do laudo marca a conclusão do exame e dispara automaticamente o desconto de estoque dos insumos consumidos.

### 12.1 Decisão de Arquitetura: Desconto de Estoque

Durante a implementação deste módulo, identificou-se que o desconto de estoque pertence semanticamente à emissão do laudo — o estoque é consumido quando o exame é de fato realizado e documentado — e não à simples atualização de status. Portanto, o desconto de estoque foi movido do `SolicitacaoService` para o `LaudoService`, tornando a emissão do laudo a única operação que efetivamente desconta o estoque. A atualização manual de status via `SolicitacaoService.UpdateStatusAsync` permanece disponível para correções e movimentações intermediárias, sem efeito sobre o estoque.

### 12.2 Geração de PDF com QuestPDF

Para a geração dos laudos em formato PDF, foi adotada a biblioteca **QuestPDF**, de licença MIT, que oferece uma API fluente e fortemente tipada para composição de documentos em .NET. A biblioteca elimina a necessidade de templates HTML ou conversões intermediárias, permitindo definir o layout do documento diretamente em código C#.

A implementação seguiu o padrão Clean Architecture: a interface `IPdfGenerator` foi definida na camada Application (em `Application/Common/`) e a implementação concreta `QuestPdfGenerator` foi criada na camada Infrastructure (em `Infrastructure/ExternalServices/`). A interface recebe um objeto `LaudoPdfData` com todos os dados necessários para compor o documento, sem expor nenhum detalhe de implementação às camadas superiores.

O PDF gerado possui a seguinte estrutura: cabeçalho com título e identificação do laboratório; seção de identificação contendo o código público, tipo de exame e datas; tabela com dados do paciente (nome, espécie, raça, sexo, idade, peso e tutor); dados do médico veterinário solicitante com o número do CRMV; caixa de conteúdo do laudo com o texto redigido pelo patologista; e rodapé com a identificação e assinatura do patologista responsável. O PDF é gerado sob demanda a cada requisição, sem armazenamento em disco ou banco de dados, o que simplifica a infraestrutura e elimina a necessidade de um serviço de armazenamento de arquivos durante o desenvolvimento.

### 12.3 Fluxo de Emissão do Laudo

Ao criar um laudo via `POST /api/laudos`, o `LaudoService` executa as seguintes operações em uma única transação: verificação de que a solicitação existe e ainda não possui laudo; criação da entidade Laudo com o conteúdo fornecido pelo patologista, o id do patologista autenticado e o instante de emissão; atualização do status da solicitação para `Concluido` com registro automático de `HistoricoStatus`; e desconto dos insumos configurados para o tipo de exame a partir do estoque atual.

### 12.4 Endpoints

- `POST /api/laudos` — cria o laudo, conclui a solicitação e desconta estoque (autenticado)
- `GET /api/laudos/{id}` — retorna os dados do laudo (autenticado)
- `GET /api/laudos/solicitacao/{solicitacaoId}` — retorna o laudo de uma solicitação específica (autenticado)
- `GET /api/laudos/{id}/pdf` — gera e retorna o laudo em PDF para download (autenticado)

O endpoint de download retorna o arquivo diretamente no corpo da resposta com o tipo MIME `application/pdf` e nome de arquivo padronizado `laudo-{id}.pdf`, permitindo que o navegador ou o frontend inicie o download diretamente.

---

## 13. Integração com a API Google Gemini

A integração com o Google Gemini representa o diferencial tecnológico central deste trabalho: a capacidade de criar solicitações de exame a partir de documentos enviados como imagem, PDF ou áudio, sem necessidade de digitação manual por parte do médico veterinário solicitante.

### 13.1 Funcionamento Geral

O fluxo de uso da funcionalidade é o seguinte: o médico veterinário acessa o formulário de nova solicitação e, em vez de preencher os campos manualmente, faz o upload de uma guia de solicitação (foto, PDF ou gravação de voz). O frontend envia esse arquivo ao endpoint `POST /api/gemini/extrair`. O backend recebe o arquivo, converte-o para base64, monta a requisição para a API do Gemini e retorna ao frontend os dados extraídos em formato estruturado. O frontend então preenche automaticamente os campos do formulário com esses dados, permitindo que o usuário revise e confirme antes de submeter a solicitação.

### 13.2 Comunicação com a API do Gemini

A comunicação com a API do Gemini é feita via HTTP diretamente, por meio do `IHttpClientFactory` do ASP.NET Core, sem dependência de SDK de terceiros. A requisição utiliza o endpoint de geração de conteúdo da API REST do Gemini (`v1beta/models/gemini-1.5-flash:generateContent`), enviando o arquivo como dado inline codificado em base64 junto ao prompt de instrução.

O parâmetro `responseMimeType = "application/json"` é passado na configuração de geração, instruindo o modelo a retornar diretamente um objeto JSON válido, sem texto adicional nem formatação Markdown, o que elimina a necessidade de pós-processamento da resposta.

O modelo utilizado por padrão é o **Gemini 1.5 Flash**, otimizado para tarefas de extração de dados — rápido, econômico e com excelente desempenho em documentos de layout variável. O modelo pode ser alterado via configuração sem mudança de código.

### 13.3 Prompt de Extração

O prompt enviado ao Gemini instrui o modelo a comportar-se como assistente de laboratório veterinário e a extrair dezesseis campos estruturados da guia de solicitação: dados do médico veterinário (nome, CRMV, e-mail, telefone), dados do tutor (nome, telefone, e-mail), dados do paciente (nome, espécie, raça, sexo, idade, peso), tipo de exame e descrição clínica. Campos não identificados no documento são retornados como `null`.

A temperatura de geração foi configurada em `0.1`, valor próximo de zero que torna o modelo mais determinístico e literal, adequado para tarefas de extração de informações onde criatividade é indesejada.

### 13.4 Tipos de Arquivo Suportados

O endpoint aceita os seguintes tipos MIME: imagens nos formatos JPEG, PNG, WebP e HEIC; documentos PDF; e arquivos de áudio nos formatos MP3, MPEG, WAV, OGG e WebM. O tamanho máximo de arquivo aceito é de 10 MB, limite compatível com o tier gratuito da API do Gemini e suficiente para guias de solicitação veterinária em todos os formatos suportados.

### 13.5 Segurança da Chave de API

A chave da API do Gemini é armazenada exclusivamente em variável de ambiente ou no arquivo `appsettings.Development.json` (ignorado pelo controle de versão). O arquivo `appsettings.json` presente no repositório contém apenas a estrutura de configuração com a chave em branco. Em produção no Azure App Service, a chave será fornecida via variável de ambiente configurada no painel do serviço.

### 13.6 Arquitetura da Integração

A integração segue o padrão Clean Architecture: a interface `IGeminiService` foi definida em `Application/Common/`, abstraindo o contrato de extração. A implementação concreta `GeminiService` reside em `Infrastructure/ExternalServices/`, contendo toda a lógica de comunicação HTTP, construção da requisição e deserialização da resposta. A classe `DadosExtraidos` é uma classe interna ao `GeminiService`, utilizada apenas para mapear o JSON com chaves em snake_case retornado pelo modelo para o DTO público `ExtrairSolicitacaoDto`, que é retornado ao frontend com as chaves em camelCase.

---

## 14. Configuração do Frontend Angular

### 14.1 Instalação do Angular Material

A biblioteca Angular Material foi adicionada ao projeto por meio do comando `ng add @angular/material`, que realizou automaticamente a configuração do tema visual, a importação das animações assíncronas e a atualização dos arquivos `angular.json`, `index.html` e `styles.scss`. O tema escolhido foi o `indigo-pink`, paleta padrão do Material Design com azul índigo como cor primária, adequada ao contexto de sistema de informação profissional.

### 14.2 Estrutura de Pastas

O projeto Angular foi organizado em três grandes grupos: `core/` com os elementos transversais da aplicação (serviços, guards, interceptors e modelos de dados), `features/` com os componentes de cada funcionalidade em subpastas, e `layout/` com o shell da aplicação autenticada. Todos os componentes foram criados como standalone components, padrão do Angular 17+ que elimina a necessidade de NgModules.

### 14.3 Autenticação no Frontend

O `AuthService` gerencia o estado de autenticação utilizando Signals do Angular. O token JWT recebido após o login é armazenado no `localStorage` e os dados do usuário são mantidos em memória via signal. O serviço expõe os signals `isLoggedIn` e `currentUser`, consumidos pelo layout e pelo guard.

O `authInterceptor` é uma função interceptora funcional que adiciona automaticamente o cabeçalho `Authorization: Bearer {token}` a todas as requisições HTTP. O `authGuard` protege todas as rotas internas, redirecionando para `/login` quando não há token válido.

### 14.4 Roteamento com Lazy Loading

Todos os componentes são carregados sob demanda (lazy loading), reduzindo o bundle inicial. As rotas públicas (`/login` e `/consulta/:codigo`) são acessíveis sem autenticação. Todas as rotas internas são filhas do `MainLayoutComponent` e protegidas pelo `authGuard`.

### 14.5 Layout Principal

O layout autenticado utiliza o `MatSidenav` para compor uma barra lateral fixa com os links de navegação (Solicitações, Tipos de Exame, Estoque) e um rodapé com o nome do patologista e botão de logout. A área de conteúdo renderiza o componente da rota ativa via `<router-outlet>`.

### 14.6 Página de Login

Tela pública com formulário reativo centralizado sobre fundo gradiente índigo. Inclui campos de e-mail e senha com validação, alternância de visibilidade da senha, feedback de erro e spinner de carregamento durante o processo de autenticação.

### 14.7 Página de Consulta Pública

A página `/consulta/:codigo` exibe informações mínimas de uma solicitação (status, tipo de exame, nome do paciente, datas) sem expor dados sensíveis, em conformidade com a LGPD.

---

## 15. Módulo de Solicitações (Frontend)

### 15.1 Serviços HTTP

Foram criados cinco serviços Angular em `core/services/`: `SolicitacaoService`, `TipoExameService`, `InsumoService`, `LaudoService` e `GeminiService`. Todos utilizam `HttpClient` e são fornecidos na raiz da aplicação. O token JWT é adicionado automaticamente pelo interceptor configurado anteriormente.

### 15.2 Listagem de Solicitações

A página de listagem exibe todas as solicitações em uma `MatTable` com as colunas: código público, tipo de exame, paciente, veterinário, status (chip colorido por estado) e data de criação. O clique em qualquer linha navega para o detalhe. Um estado vazio é exibido quando não há solicitações.

### 15.3 Nova Solicitação com Extração por IA

A página de nova solicitação organiza o formulário em seções: método de entrada, exame, médico veterinário, tutor e paciente. O método de entrada é selecionado por `MatButtonToggleGroup` com as opções Manual, Foto da Guia, PDF e Áudio.

Quando o usuário escolhe um método baseado em arquivo e clica em "Extrair com IA", o arquivo é enviado ao endpoint `/api/gemini/extrair`. Os dados retornados são mapeados automaticamente para os controles do formulário reativo — incluindo a correspondência por substring entre o tipo de exame extraído e os tipos cadastrados. O usuário revisa os dados pré-preenchidos antes de submeter.

### 15.4 Detalhe da Solicitação

A página de detalhe apresenta: timeline visual de status com indicadores circulares, dados do paciente e tutor, dados do exame e veterinário, seção de laudo e histórico de mudanças de status. O patologista pode avançar o status diretamente na página com observação opcional.

### 15.5 Emissão de Laudo e Download de PDF

Quando não há laudo, o botão "Emitir Laudo" exibe um formulário de texto para o patologista redigir o conteúdo. Ao confirmar, o backend cria o laudo, transiciona para Concluído e desconta o estoque. Se o laudo já existe, um botão "Baixar PDF" chama `/api/laudos/{id}/pdf`, recebe o blob e aciona o download via `URL.createObjectURL`.

---

## 16. Módulos de Tipos de Exame e Insumos (Frontend)

### 16.1 Padrão de Dialog no Angular Material

Ambas as páginas utilizam o `MatDialog` do Angular Material para operações de criação, edição e ajuste — padrão adequado para formulários curtos que não justificam navegação para uma página separada. Os componentes de dialog foram definidos no mesmo arquivo do componente principal, aproveitando o padrão de múltiplos componentes por arquivo permitido em Angular standalone. Cada dialog recebe e retorna dados por meio de `MAT_DIALOG_DATA` e `MatDialogRef`, mantendo a comunicação desacoplada do componente pai.

### 16.2 Tipos de Exame

A página de tipos de exame exibe todos os tipos cadastrados em uma tabela Material com colunas de nome, prazo estimado, status (Ativo/Inativo) e ações. As ações disponíveis por linha são: gerenciar templates (abre dialog de templates), editar (abre dialog de criação/edição pré-preenchido) e desativar (soft delete).

O dialog de criação/edição contém os campos nome, descrição (opcional) e prazo estimado em dias, com validação de campos obrigatórios.

O dialog de templates exibe todos os templates existentes para o tipo de exame (em ordem decrescente de versão) e permite adicionar um novo template por meio de um textarea. O versionamento é automático no backend — cada novo template recebe o número da versão anterior mais um.

### 16.3 Insumos e Controle de Estoque

A página de insumos exibe todos os itens de estoque ativos com as colunas: nome, unidade de medida, quantidade atual, quantidade mínima, status e ações. Linhas de insumos com estoque abaixo do mínimo recebem destaque visual (fundo amarelo claro) e um ícone de alerta, tornando imediata a identificação de itens que precisam de reposição.

As ações disponíveis são: ajustar quantidade (abre dialog com a quantidade atual pre-carregada para correção manual), editar cadastro e desativar. O dialog de ajuste de quantidade permite ao patologista registrar entradas de novos itens no estoque ou corrigir divergências após inventário físico, sem afetar a configuração do insumo (nome, unidade, mínimo).

---

<!-- Novas seções serão adicionadas aqui conforme o desenvolvimento avança -->
