# CAPÍTULO II

## 3 METODOLOGIA

### 3.1 Natureza da pesquisa

Além da natureza experimental, apresentada na seção 1.4, a pesquisa pode ser classificada quanto aos objetivos e quanto aos procedimentos técnicos, conforme Gil (2002). Quanto aos objetivos, tem caráter exploratório, por buscar maior familiaridade com um problema ainda pouco estudado no contexto dos laboratórios veterinários brasileiros, e descritivo, ao registrar as percepções dos usuários sobre a solução desenvolvida. Quanto aos procedimentos técnicos, combina três abordagens: a pesquisa bibliográfica, empregada na fundamentação teórica e no levantamento dos trabalhos relacionados; a pesquisa experimental, que consistiu em desenvolver o sistema, introduzindo a digitalização do fluxo e a entrada multimodal de dados por IA, e observar seus efeitos em testes controlados; e o levantamento, realizado por meio de um questionário aplicado a potenciais usuários.

Como não houve grupo de controle nem medição do processo anterior à adoção do sistema, os efeitos sobre o esforço operacional e a transparência foram observados pela percepção da profissional colaboradora e dos participantes do questionário, e não por medições diretas de tempo. Já o comportamento do sistema e a qualidade da extração de dados foram verificados de forma objetiva, por meio de testes automatizados e de medições repetidas.

### 3.2 Padrões para a pesquisa experimental

As tecnologias empregadas no desenvolvimento estão resumidas no @qua:tecnologias. Na camada de *backend*, utilizou-se a plataforma .NET 10, com a linguagem C# e o ASP.NET Core, para construir uma API REST responsável pelas regras de negócio, pela persistência, pela autenticação e pela integração com a API do Google Gemini. A persistência foi implementada com o sistema gerenciador de banco de dados SQL Server, acessado por meio do mapeamento objeto-relacional (*Object-Relational Mapping* – ORM) do Entity Framework Core. Na camada de *frontend*, utilizou-se o Angular 18 com a biblioteca de componentes Angular Material, na forma de uma SPA responsiva.

{{quadro id="tecnologias" titulo="Tecnologias utilizadas no desenvolvimento" fonte="Elaborado pelo autor (2026)." larguras="3,5.5,7.5"}}
| Camada | Tecnologia | Uso no projeto |
| *Frontend* | Angular 18, Angular Material e TypeScript | Interface responsiva, rotas protegidas por perfil e formulários reativos |
| *Backend* | .NET 10, ASP.NET Core e C# | API REST, regras de negócio, autenticação e autorização |
| Persistência | SQL Server (LocalDB no desenvolvimento) e Entity Framework Core 10 | Banco relacional, migrações e filtros de isolamento por laboratório |
| Segurança | JWT e BCrypt.Net | Autenticação sem estado e armazenamento seguro de senhas |
| Documentos | QuestPDF | Geração do laudo em PDF |
| Inteligência artificial | API do Google Gemini (modelo gemini-3.8-flash) | Extração de dados de foto, PDF e áudio da guia |
| Testes | Python e Playwright | Testes automatizados da API e da interface no navegador |
| Versionamento | Git | Controle de versões do código-fonte |
| Hospedagem (prevista) | Microsoft Azure (programa Azure for Students) | Publicação do *frontend*, da API e do banco de dados |
{{/quadro}}

Em relação ao planejamento inicial do projeto de trabalho de graduação, que previa o uso de Next.js no *frontend* e de PostgreSQL no banco de dados, duas mudanças foram feitas no início do desenvolvimento. O Angular foi adotado no lugar do Next.js pela coesão com o ecossistema Microsoft do *backend* e por sua estrutura, que já traz injeção de dependências, roteamento e formulários; como a aplicação funciona como SPA e a página pública é simples, os recursos de renderização no servidor do Next.js deixaram de ser necessários. O SQL Server substituiu o PostgreSQL pela integração madura com o Entity Framework Core e pela possibilidade de hospedagem gratuita no Azure SQL por meio do programa Azure for Students.

A integração com o Gemini foi feita exclusivamente no *backend*: o *frontend* envia o arquivo à API do sistema, que o repassa ao Gemini com a chave de acesso guardada na configuração do servidor, de modo que essa chave nunca é exposta ao navegador. O modelo utilizado é definido na configuração da aplicação, o que facilitou sua substituição quando a versão inicialmente adotada foi descontinuada pelo fornecedor. Os trabalhos relacionados do Capítulo I serviram de referência para as funcionalidades contempladas e para os critérios de avaliação descritos na seção 3.5.

### 3.3 Experimento de pesquisa

O experimento de pesquisa consistiu no desenvolvimento do sistema em incrementos, abordagem em que o *software* é construído em partes funcionais sucessivas, cada uma acrescentando novas capacidades às anteriores (PRESSMAN; MAXIM, 2016). O desenvolvimento ocorreu entre agosto e outubro de 2026, com cada incremento registrado no controle de versão. As principais etapas foram:

a) levantamento de requisitos com a médica veterinária patologista colaboradora, que contribuiu para a definição dos campos da solicitação e do fluxo de status do exame;
b) modelagem de dados e organização dos projetos de *frontend* e de *backend*;
c) implementação da autenticação e dos cadastros de tipos de exame, modelos de laudo e insumos;
d) implementação do fluxo de solicitação, do histórico de status e da consulta pública por código;
e) implementação da emissão de laudos, da geração do PDF e do desconto automático de estoque;
f) integração com a API do Google Gemini para a extração de dados de foto, PDF e áudio;
g) construção das telas em Angular e refinamentos, como a página inicial pública, o carregamento do modelo de laudo no editor e o painel de indicadores;
h) redesenho do sistema para atender vários laboratórios;
i) testes automatizados, correções e preparação da avaliação com usuários.

O redesenho para vários laboratórios surgiu durante o desenvolvimento, motivado por três necessidades. A primeira foi controlar quem pode criar um laboratório no sistema, evitando cadastros indevidos: a criação passou a depender de um convite gerado por um administrador. A segunda foi permitir que mais de um laboratório use a mesma instalação, com os dados de cada um isolados dos demais. A terceira foi refletir a realidade dos médicos veterinários, que costumam trabalhar com mais de um laboratório: cada veterinário passou a ter uma única conta, vinculada aos laboratórios que aprovarem seu pedido.

A partir do levantamento inicial e dessas decisões, os requisitos foram classificados em funcionais, que descrevem o que o sistema deve fazer, e não funcionais, que descrevem restrições e atributos de qualidade (PRESSMAN; MAXIM, 2016). O @qua:rf e o @qua:rnf apresentam os requisitos implementados.

{{quadro id="rf" titulo="Requisitos funcionais" fonte="Elaborado pelo autor (2026)." larguras="1.6,14.4"}}
| Código | Descrição |
| RF01 | Permitir que o administrador gere convites de uso único e com prazo de validade para o cadastro de laboratórios, e que os revogue. |
| RF02 | Permitir o cadastro de um laboratório somente a partir de um convite válido, com nome, Cadastro Nacional da Pessoa Jurídica (CNPJ), endereço, contatos e responsável técnico. |
| RF03 | Permitir que o administrador ative e desative contas de laboratórios e de veterinários, com efeito imediato. |
| RF04 | Permitir que o laboratório cadastre tipos de exame, com prazo estimado, e modelos de laudo versionados. |
| RF05 | Permitir o cadastro de insumos, o ajuste de quantidades e a definição dos insumos consumidos por tipo de exame. |
| RF06 | Permitir que o médico veterinário crie sua conta, com CRMV, e peça vínculo a laboratórios, por busca ou por link. |
| RF07 | Permitir que o laboratório aprove, recuse ou desfaça vínculos com veterinários. |
| RF08 | Permitir o registro de solicitações de exame pelo laboratório e por veterinários vinculados, de forma manual ou a partir de foto, PDF ou áudio da guia. |
| RF09 | Extrair com IA os dados da guia enviada e preencher o formulário para revisão antes do envio. |
| RF10 | Gerar um código público único para cada solicitação e calcular a data estimada de conclusão. |
| RF11 | Permitir que o laboratório atualize o status da solicitação, registrando o histórico das mudanças. |
| RF12 | Permitir a emissão do laudo a partir do modelo do tipo de exame, concluindo a solicitação e descontando os insumos do estoque. |
| RF13 | Gerar o laudo em PDF com os dados do laboratório e do responsável técnico. |
| RF14 | Permitir a consulta pública do status do exame pelo código, sem *login*, exibindo apenas dados mínimos. |
| RF15 | Apresentar ao laboratório indicadores de solicitações por status, prazos vencidos e estoque baixo. |
| RF16 | Permitir que cada usuário mantenha seus dados de perfil e altere sua senha. |
{{/quadro}}

{{quadro id="rnf" titulo="Requisitos não funcionais" fonte="Elaborado pelo autor (2026)." larguras="1.6,14.4"}}
| Código | Descrição |
| RNF01 | Isolamento: os dados de um laboratório não podem ser lidos nem alterados por outro laboratório. |
| RNF02 | Segurança: senhas armazenadas com *hash* bcrypt, autenticação por JWT com validade de 8 horas e autorização por perfil em todas as rotas protegidas da API. |
| RNF03 | Privacidade: a consulta pública não exibe dados pessoais de tutores nem de veterinários, em atenção à LGPD. |
| RNF04 | Responsividade: a interface deve funcionar em computadores e em celulares. |
| RNF05 | Entrada multimodal: aceitar arquivos de até 10 MB nos formatos JPEG, PNG, WebP, HEIC, PDF, MP3, WAV, OGG e WebM. |
| RNF06 | Configuração segura: chaves de assinatura e de acesso à IA mantidas fora do código-fonte, na configuração do ambiente. |
| RNF07 | Portabilidade: execução em ambiente local e publicação em nuvem sem alteração do código, apenas da configuração. |
| RNF08 | Rastreabilidade: datas armazenadas em UTC (*Coordinated Universal Time*) e exibidas no horário local; cada laudo guarda o responsável técnico vigente na emissão. |
{{/quadro}}

### 3.4 Diagramas

Os diagramas a seguir apresentam a estrutura e o comportamento do sistema desenvolvido. A @fig:arquitetura mostra a arquitetura geral, com a separação entre o *frontend*, executado no navegador, o *backend*, que expõe a API REST, o banco de dados e os serviços externos. O *frontend* se comunica com o *backend* por meio do protocolo *Hypertext Transfer Protocol Secure* (HTTPS), trocando dados em JSON, e cada requisição autenticada leva o *token* JWT do usuário.

{{figura id="arquitetura" arquivo="diagrama-arquitetura.png" largura="15.5" titulo="Arquitetura geral do sistema" fonte="Elaborado pelo autor (2026)."}}

O *backend* foi dividido em quatro projetos, segundo a arquitetura limpa (MARTIN, 2019). O LabPat.Domain reúne as entidades e as interfaces dos repositórios e não depende de nenhum outro projeto. O LabPat.Application concentra os serviços com as regras de negócio e os objetos de transferência de dados (*Data Transfer Objects* – DTO), organizados por funcionalidade. O LabPat.Infrastructure implementa os detalhes técnicos: o acesso ao banco, a segurança, a geração do PDF e a chamada ao Gemini. O LabPat.Api contém os controladores e a configuração da aplicação. A @fig:camadas mostra a direção das dependências entre esses projetos.

{{figura id="camadas" arquivo="diagrama-camadas.png" largura="14" titulo="Dependências entre os projetos do *backend*" fonte="Elaborado pelo autor (2026)."}}

A @fig:casos-uso apresenta o diagrama de casos de uso, elaborado com a *Unified Modeling Language* (UML). Há quatro atores: o administrador, que controla a entrada de laboratórios e as contas; o laboratório, representado pelo patologista, que configura e opera o fluxo; o médico veterinário, que envia solicitações aos laboratórios a que está vinculado; e o tutor, que apenas consulta o andamento do exame. O Google Gemini aparece como sistema externo associado à extração de dados, que estende o caso de uso de registro da solicitação. A emissão do laudo inclui, obrigatoriamente, o desconto dos insumos do estoque.

{{figura id="casos-uso" arquivo="diagrama-casos-uso.png" largura="15.5" titulo="Diagrama de casos de uso" fonte="Elaborado pelo autor (2026)."}}

A @fig:estados apresenta o ciclo de vida de uma solicitação, com cinco estados: Solicitado, Aguardando amostra, Amostra recebida, Em análise e Concluído. O laboratório só pode avançar para as etapas seguintes à atual, inclusive mais de uma de uma vez, mas não pode retroceder; essa regra é aplicada tanto na interface quanto na API. Cada mudança gera um registro de histórico com o estado anterior, o novo estado, a data, o usuário responsável e uma observação opcional. A conclusão ocorre exclusivamente com a emissão do laudo, que pode ser feita a partir dos estados Amostra recebida ou Em análise; depois de concluída, a solicitação não muda mais de estado.

{{figura id="estados" arquivo="diagrama-estados.png" largura="16" titulo="Diagrama de estados da solicitação de exame" fonte="Elaborado pelo autor (2026)."}}

A @fig:adesao mostra o fluxo de adesão de laboratórios e veterinários. O administrador gera um convite e o envia ao laboratório, que se cadastra pelo link recebido. O laboratório, por sua vez, compartilha com os veterinários um link de vínculo. O veterinário cria sua conta, que já nasce ativa, e pede vínculo ao laboratório; somente após a aprovação ele pode enviar solicitações àquele laboratório.

{{figura id="adesao" arquivo="diagrama-adesao.png" largura="16" titulo="Fluxo de adesão de laboratórios e veterinários" fonte="Elaborado pelo autor (2026)."}}

A @fig:er apresenta o modelo de dados, em notação de entidade-relacionamento. As entidades com a coluna LaboratorioId pertencem a um laboratório e são filtradas automaticamente, conforme a abordagem de banco compartilhado descrita na seção 2.3.3. A tabela EXAME_INSUMO, com chave composta, registra a quantidade de cada insumo consumida por tipo de exame, e o índice único em SolicitacaoId garante no máximo um laudo por solicitação. Para não sobrecarregar o diagrama, foram omitidas as ligações do laboratório com tutores, pacientes, veterinários solicitantes e solicitações, representadas apenas pela coluna LaboratorioId, e as ligações com o usuário que registrou cada solicitação, emitiu cada laudo ou alterou cada status.

{{figura id="er" arquivo="diagrama-er.png" largura="15.5" titulo="Modelo de dados (diagrama entidade-relacionamento)" fonte="Elaborado pelo autor (2026)."}}

Por fim, a @fig:sequencia detalha a extração multimodal. O usuário escolhe o arquivo da guia e o *frontend* o envia à API, que valida o perfil do usuário, o tipo e o tamanho do arquivo e o repassa ao Gemini em base64, acompanhado de instruções que descrevem os campos esperados. A resposta, em JSON, é convertida pela API e devolvida ao *frontend*, que preenche o formulário. A solicitação só é criada depois que o usuário revisa e confirma os dados.

{{figura id="sequencia" arquivo="diagrama-sequencia.png" largura="15" titulo="Sequência da extração de dados com IA" fonte="Elaborado pelo autor (2026)."}}

### 3.5 Critérios para avaliação

Para avaliar se os objetivos foram atingidos, foram adotados quatro mecanismos complementares, escolhidos para reduzir o viés do autor e oferecer uma leitura mais imparcial dos resultados.

O primeiro são os testes funcionais automatizados, que verificam o comportamento do sistema do ponto de vista do usuário, sem depender da estrutura interna do código (PRESSMAN; MAXIM, 2016). Foram escritos roteiros que exercitam a API e a interface no navegador, e o critério adotado foi a aprovação de todas as verificações na versão final do sistema.

O segundo é a medição da extração multimodal. A mesma guia fictícia foi enviada nos três formatos de entrada (foto, PDF e áudio), em execuções repetidas, e para cada execução foram contados os campos extraídos corretamente, entre os 16 solicitados ao modelo, e o tempo de resposta.

O terceiro é a validação com a médica veterinária patologista colaboradora, que, como usuária representativa, utiliza o sistema em uma sessão guiada por um roteiro de tarefas da rotina do laboratório, com observação do uso e uma entrevista ao final. São avaliados a adequação dos campos ao seu fluxo real, a clareza da interface, a utilidade da extração por IA e a percepção de redução de esforço em relação ao processo atual.

O quarto é a aplicação de um questionário *on-line* (Google Forms) a potenciais usuários, patologistas e médicos veterinários, após o uso do sistema. Trata-se de uma amostra não probabilística, por conveniência, formada por profissionais da rede de contatos da colaboradora e do autor. O questionário utiliza a escala de Likert (1932), com cinco pontos, de "discordo totalmente" a "concordo totalmente", e suas afirmações foram organizadas a partir dos construtos do Modelo de Aceitação de Tecnologia (*Technology Acceptance Model* – TAM), de Davis (1989): utilidade percebida, facilidade de uso percebida e intenção de uso. Foram acrescentadas afirmações sobre a extração por IA e sobre a transparência e a privacidade da consulta pública. As respostas são analisadas pela proporção de concordância (notas 4 e 5) e pela mediana, agrupadas por construto. A participação é voluntária e anônima: o formulário apresenta os objetivos da pesquisa e pede o consentimento do participante, sem coletar nome, e-mail ou outros dados que o identifiquem. O questionário completo encontra-se no Apêndice A.

Esses mecanismos se relacionam aos critérios definidos no projeto: a redução percebida do esforço operacional do patologista e a transparência percebida por solicitantes e tutores são avaliadas pela validação e pelo questionário; a padronização das informações, pelos requisitos implementados e pelos testes; e a qualidade da extração de dados pelo Gemini, pela medição descrita acima. Como indicado na seção 3.1, a redução de esforço é avaliada pela percepção dos usuários, e não pela cronometragem do processo antes e depois do uso do sistema.
