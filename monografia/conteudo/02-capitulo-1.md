# CAPÍTULO I

## 2 FUNDAMENTAÇÃO TEÓRICA

### 2.1 Conceitos-chave

Para embasar esta pesquisa, optou-se por organizar este capítulo em duas partes. Primeiramente, apresentam-se os conceitos-chave que referenciam o trabalho: sistemas de informação laboratorial, aplicações web com arquitetura cliente-servidor, inteligência artificial generativa multimodal e patologia clínica veterinária. Na segunda parte, apresenta-se um conjunto de trabalhos acadêmicos e de produtos comerciais relacionados ao tema, desenvolvidos nos últimos anos.

### 2.2 Sistemas de informação laboratorial (LIS)

Os Sistemas de Informação Laboratorial são plataformas de *software* projetadas para apoiar os processos de trabalho de laboratórios clínicos, gerenciando o ciclo completo do exame: a requisição, a identificação da amostra, o processamento, a análise, o resultado, o laudo e a entrega ao solicitante. Sepulveda e Young (2013) descrevem um conjunto de funcionalidades esperadas de um LIS moderno, que vão do registro do pedido e do acompanhamento das amostras até a liberação e a comunicação dos resultados.

Marletta *et al.* (2025), em estudo comparativo sobre os LIS utilizados em laboratórios de patologia na Itália, destacam que o fluxo laboratorial se divide nas fases pré-analítica, analítica e pós-analítica e que esses sistemas são ferramentas essenciais para garantir rastreabilidade, eficiência e segurança dos dados no diagnóstico. Os autores apontam ainda que a escolha do sistema adequado tem impacto direto na qualidade diagnóstica e na segurança operacional do laboratório.

Embora o conceito de LIS seja consolidado na medicina humana, sua aplicação à medicina veterinária é menos difundida. Como mostra o levantamento da seção 2.6, boa parte das soluções veterinárias disponíveis concentra-se na gestão de clínicas, e as voltadas a laboratórios priorizam o laudo e a entrega de resultados, com pouca automação na etapa de abertura da solicitação.

### 2.3 Aplicações web e arquitetura cliente-servidor

Aplicações web são sistemas de *software* acessados por meio de navegadores, o que elimina a necessidade de instalação local pelo usuário. Sua arquitetura mais comum é a cliente-servidor: o navegador (cliente) faz requisições, geralmente pelo protocolo *Hypertext Transfer Protocol* (HTTP), a um servidor remoto, que processa as regras de negócio e devolve respostas, frequentemente no formato *JavaScript Object Notation* (JSON), para que a interface seja montada dinamicamente. Essa arquitetura oferece vantagens relevantes para o caso deste trabalho: atualizações centralizadas, acesso a partir de qualquer dispositivo com navegador e baixa barreira de entrada para usuários ocasionais, como médicos veterinários solicitantes e tutores, que acessam o sistema apenas pontualmente.

#### 2.3.1 API REST e aplicação de página única

O estilo arquitetural *Representational State Transfer* (REST), definido por Fielding (2000), organiza a comunicação entre cliente e servidor em torno de recursos identificados por endereços, manipulados por uma interface uniforme e por interações sem estado, em que cada requisição carrega toda a informação necessária para ser atendida. Uma API REST expõe, assim, os recursos do sistema, como solicitações, laudos e tipos de exame, para qualquer cliente capaz de enviar requisições HTTP.

No lado do cliente, a abordagem de aplicação de página única (*Single Page Application* – SPA) carrega a interface uma única vez no navegador e, a partir daí, troca apenas dados com o servidor, atualizando a tela sem recarregar a página. O Angular, *framework* mantido pela Google, oferece uma estrutura completa para esse tipo de aplicação, com componentes, roteamento, formulários reativos e injeção de dependências (GOOGLE, 2026a).

#### 2.3.2 Arquitetura limpa

Martin (2019) propõe a arquitetura limpa como forma de separar as regras de negócio dos detalhes técnicos, como o banco de dados, os *frameworks* e a interface com o usuário. A ideia central é a regra de dependência: o código é organizado em camadas, e as dependências devem apontar sempre para dentro, em direção às regras de negócio, que não conhecem os detalhes externos. Com isso, é possível trocar o banco de dados ou a forma de entrega da aplicação sem reescrever o núcleo do sistema, além de facilitar os testes e a manutenção.

#### 2.3.3 Multilocação

A multilocação (*multi-tenancy*) é um princípio de arquitetura associado ao modelo *Software as a Service* (SaaS), em que vários clientes, chamados locatários, compartilham a mesma instância da aplicação e do banco de dados, aproveitando a economia de escala, enquanto cada um tem a percepção de usar um sistema exclusivo (BEZEMER; ZAIDMAN, 2010). O principal cuidado nesse modelo é garantir que os dados de um locatário não sejam vistos por outro.

A documentação do Entity Framework Core descreve três abordagens para implementar a multilocação: um banco de dados por locatário, um esquema por locatário e um banco compartilhado, em que cada tabela recebe uma coluna com o identificador do locatário. Nesta última, um filtro global de consulta pode ser aplicado automaticamente a todas as consultas, evitando que o desenvolvedor escreva, por descuido, um código que acesse dados de outro cliente (MICROSOFT, 2022).

#### 2.3.4 Autenticação e proteção de dados

Em APIs REST, a autenticação costuma ser feita por *tokens*. O *JSON Web Token* (JWT) é um formato compacto para representar declarações (*claims*) trocadas entre duas partes, assinado digitalmente para garantir sua integridade (JONES; BRADLEY; SAKIMURA, 2015). Como o próprio *token* carrega a identidade e o perfil do usuário, o servidor não precisa manter sessões em memória. Para o armazenamento de senhas, o algoritmo bcrypt aplica sal e um fator de custo ajustável, que torna cada tentativa de adivinhação deliberadamente lenta e pode ser aumentado à medida que o *hardware* evolui (PROVOS; MAZIÈRES, 1999).

A falha de controle de acesso ocupa a primeira posição da lista OWASP Top 10 de 2021, que reúne os riscos de segurança mais críticos em aplicações web (OWASP FOUNDATION, 2021). Em um sistema multilocatário, esse risco se traduz na possibilidade de um usuário acessar dados de outro laboratório, o que exige verificações de permissão em todas as requisições, e não apenas na interface.

No campo da privacidade, a LGPD estabelece, entre os princípios do tratamento de dados pessoais, o da necessidade, que limita o tratamento ao mínimo necessário para a realização de suas finalidades (BRASIL, 2018). Esse princípio orientou o desenho da consulta pública por código deste trabalho, que exibe apenas o andamento do exame, sem dados pessoais de tutores ou de veterinários.

### 2.4 Inteligência artificial generativa multimodal

Modelos de linguagem de grande escala (*Large Language Models* – LLM) são redes neurais profundas treinadas com grandes volumes de texto, capazes de gerar conteúdo em linguagem natural de forma contextual. A evolução dessa tecnologia deu origem aos modelos multimodais (*Multimodal Large Language Models* – MLLM), capazes de interpretar não apenas texto, mas também imagens, áudio e vídeo em uma única requisição. A família Gemini, do Google, foi treinada desde o início de forma conjunta com dados de diferentes modalidades, o que lhe permite raciocinar sobre texto, imagem, áudio e vídeo (GEMINI TEAM, 2023).

Para a extração de dados de documentos, essa capacidade muda a abordagem tradicional. No reconhecimento óptico de caracteres (*Optical Character Recognition* – OCR), a imagem é convertida em texto e, em seguida, são necessárias regras específicas para localizar cada campo, o que depende do *layout* do documento. Um modelo multimodal, por sua vez, recebe o documento e uma instrução em linguagem natural: segundo a documentação da API, os modelos Gemini processam arquivos PDF com visão nativa, considerando o contexto do documento inteiro, e podem extrair as informações em formatos estruturados (GOOGLE, 2026b); a resposta pode, ainda, ser configurada para seguir um esquema JSON (GOOGLE, 2026c). Isso dispensa a criação de regras de extração para cada modelo de guia, característica relevante em um cenário em que cada clínica utiliza um formulário diferente.

Esses modelos, contudo, têm limitações. Pal e Sankarasubbu (2024) avaliaram o Gemini em tarefas médicas e constataram que ele fica atrás de modelos como o MedPaLM 2 e o GPT-4 em precisão diagnóstica e que é suscetível a alucinações, isto é, a respostas incorretas apresentadas com confiança. Por esse motivo, neste trabalho o Gemini é empregado apenas para a extração de dados cadastrais da guia, nunca para o diagnóstico, e os dados extraídos sempre passam pela revisão do usuário antes de a solicitação ser criada.

### 2.5 Patologia clínica veterinária e fluxo laboratorial

A patologia veterinária é a área da medicina veterinária dedicada ao estudo das doenças dos animais, de suas causas e das alterações que provocam em células, tecidos e órgãos (SANTOS; ALESSI, 2016). Na prática laboratorial, engloba especialidades como a citologia, a histopatologia, a hematologia, a bioquímica clínica, a urinálise e a parasitologia. Em um laboratório de patologia veterinária, o fluxo típico de um exame envolve a recepção da solicitação, o recebimento da amostra biológica, sua identificação e preparo, a execução do procedimento técnico, a análise pelo patologista, a redação do laudo e a disponibilização do resultado ao solicitante.

Cada uma dessas etapas envolve informações que precisam ser registradas, transmitidas e arquivadas: dados do paciente (espécie, raça, idade, sexo e peso), dados do tutor, dados do médico veterinário solicitante, incluindo seu número de registro no Conselho Regional de Medicina Veterinária (CRMV), a descrição clínica do caso, o tipo de exame, o prazo estimado, o andamento e, ao final, o laudo emitido pelo patologista.

No Brasil, a Resolução nº 1.374/2020 do Conselho Federal de Medicina Veterinária (CFMV) dispõe sobre a responsabilidade técnica, as atividades clínico-laboratoriais e o funcionamento dos laboratórios clínicos de diagnóstico veterinário e dos laboratórios de patologia veterinária, e trata a realização de exames laboratoriais em amostras animais como atividade privativa do médico veterinário (CFMV, 2020). Por isso, cada laboratório atua sob a responsabilidade técnica de um profissional inscrito no CRMV, razão pela qual o laudo gerado pelo sistema traz a identificação desse responsável técnico.

### 2.6 Trabalhos relacionados

Nesta seção são apresentados estudos e produtos comerciais relacionados ao tema deste trabalho. O levantamento buscou pesquisas que tivessem como objetivo o desenvolvimento, a implementação ou a análise de ferramentas para a gestão de laboratórios e clínicas veterinárias, bem como o emprego de inteligência artificial multimodal em contextos de saúde. Foram utilizados o Google Acadêmico, o Repositório Institucional do Conhecimento do Centro Paula Souza, repositórios de outras instituições de ensino, o PubMed Central e os sites de fornecedores brasileiros de *software* veterinário.

Chaves e Rezende (2022) desenvolveram o Vetservice, um sistema web de gestão para clínicas veterinárias, como trabalho de graduação na Faculdade de Tecnologia de Taubaté, do Centro Paula Souza. Construído com PHP, JavaScript, MySQL e Bootstrap, o sistema parte da constatação de que clínicas de pequeno porte deixam de adotar *softwares* de gestão por causa do preço, das exigências contratuais e da complexidade das soluções existentes, e propõe uma alternativa sem mensalidade. O trabalho é relevante por compartilhar o contexto institucional e o domínio veterinário, embora seu foco seja a clínica, e não o laboratório de patologia.

Marques (2024) desenvolveu o SYSVET, um sistema do tipo *Enterprise Resource Planning* (ERP) para a gestão de clínica veterinária, como trabalho de conclusão de curso na Universidade Federal Fluminense. O sistema, construído em Delphi com banco de dados Firebird, aborda a automação de processos administrativos, o agendamento de consultas, os prontuários eletrônicos, o histórico clínico e a integração com meios de pagamento. O autor compara a proposta aos sistemas SimplesVet, LoopVet, VetBase e Vetus e relata testes realizados com veterinários parceiros. Esse trabalho contrasta com a presente pesquisa pelo escopo amplo de gestão da clínica, enquanto este trabalho se aprofunda no fluxo do laboratório de patologia.

Oliveira (2024) desenvolveu o VETPET, sistema de gerenciamento para clínica veterinária apresentado como trabalho de conclusão do curso técnico em Informática do Instituto Federal Farroupilha. Com método qualitativo, baseado em pesquisa bibliográfica, análise de sistemas semelhantes e dados da Comissão de Animais de Companhia, o trabalho propõe um sistema em formato de *site* com agendamento de consultas, fichas digitais e controle de serviços e medicamentos, e conclui que a solução pode reduzir erros administrativos. Assim como os anteriores, atua na gestão geral da clínica, sem tratar do fluxo laboratorial.

No campo dos sistemas de informação laboratorial aplicados à patologia, Marletta *et al.* (2025) publicaram, no periódico *Journal of Personalized Medicine*, um estudo comparativo de três sistemas utilizados em laboratórios de patologia italianos: Armonia, Pathox Web e WinSAP 3.0. Os sistemas foram avaliados quanto à rastreabilidade das amostras, à integração com sistemas hospitalares, à emissão de laudos digitais, à interface com o usuário e à conformidade com o Regulamento Geral de Proteção de Dados europeu e com a norma ISO 15189. Os autores apontam a integração com inteligência artificial e automação como uma das próximas etapas de evolução desses sistemas. O trabalho oferece a esta pesquisa um referencial de critérios para avaliar um LIS, ainda que no contexto da medicina humana.

Pal e Sankarasubbu (2024), em estudo publicado no repositório arXiv, avaliaram as capacidades do Gemini em tarefas médicas, incluindo raciocínio diagnóstico, detecção de alucinações e respostas a perguntas sobre imagens médicas. Nesta última tarefa, o Gemini obteve 61,45% de acerto, contra 88% do GPT-4V. Os autores concluíram que o modelo é propenso a alucinações e a excesso de confiança, o que torna arriscado o seu uso sem supervisão. O trabalho informa expectativas realistas para o uso do Gemini em saúde e reforça a decisão de empregá-lo apenas na extração de dados, com revisão humana.

O @qua:academicos resume a comparação entre os trabalhos acadêmicos analisados e o presente trabalho.

{{quadro id="academicos" titulo="Comparativo de trabalhos acadêmicos" fonte="Elaborado pelo autor (2026)." larguras="3.2,3.4,2,2.3,2.1,3"}}
| Trabalho | Foco de aplicação | Domínio veterinário | Foco em patologia laboratorial | Uso de IA multimodal | Tecnologia ou plataforma |
| Chaves e Rezende (2022) | Gestão de clínica veterinária | Sim | Não | Não | Web (PHP e MySQL) |
| Marques (2024) | ERP de clínica veterinária | Sim | Não | Não | Delphi e Firebird |
| Oliveira (2024) | Gestão de clínica veterinária | Sim | Não | Não | Web |
| Marletta *et al.* (2025) | Comparativo de LIS em patologia | Não (humana) | Sim | Citada como tendência | Análise de sistemas |
| Pal e Sankarasubbu (2024) | Avaliação do Gemini em saúde | Não (humana) | Não | Sim (Gemini) | Pesquisa experimental |
| Este trabalho (2026) | Fluxo do laboratório de patologia veterinária | Sim | Sim | Sim (Gemini) | Web responsiva (Angular e .NET) |
{{/quadro}}

Quanto aos produtos comerciais, foram mapeadas seis ferramentas brasileiras voltadas a laboratórios e clínicas veterinárias, com destaque para os módulos de exames e laudos. O VetCloud é um *software* em nuvem para laboratórios veterinários que recebe requisições por um formulário no *site* do laboratório, oferece laudos com fórmulas de cálculo automático e envia os resultados por WhatsApp e e-mail (IDEA INFO, 2026). O Workvet, da Criasoft, é exclusivo para laboratórios veterinários e permite identificar espécie, raça e cor dos animais, com valores de referência por espécie e resultados *on-line* (CRIASOFT, 2026). A plataforma Vetus é um sistema de gestão para *pet shops*, clínicas e hospitais que inclui módulo de laboratório e esteira de exames (VETUS, 2026b); seu portal LiveLab permite que o cliente, com usuário e senha, baixe os laudos em PDF (VETUS, 2026a). O AplusVet, da Web Brasília, integra-se a laboratórios de apoio, publica resultados em PDF pela internet e permite personalizar modelos de laudo e cabeçalhos por cliente (WEB BRASÍLIA, 2026). O VetSoft Web, voltado a clínicas, oferece modelos de exame com tabelas de referência por espécie, comparação dos três últimos resultados e compartilhamento por WhatsApp ou por uma área do tutor acessada por convite (VETSOFT, 2026). Por fim, o VetControl oferece formulário *on-line* de pedidos integrado ao *site* do laboratório, etiquetas com código de barras, comunicação bidirecional com equipamentos e envio de resultados por e-mail (VETCONTROL, 2026).

A análise das ferramentas comerciais aponta dois padrões importantes para a justificativa desta pesquisa. Primeiro, todas oferecem recursos consolidados para a gestão de laudos e a disponibilização de resultados, mas, nos materiais públicos consultados em outubro de 2026, nenhuma delas menciona o uso de inteligência artificial para extrair os dados de guias de solicitação enviadas como foto ou áudio: os pedidos dependem de digitação ou de formulários preenchidos pelo solicitante. Segundo, não foi identificada a integração automática entre o estoque de insumos e o consumo de cada tipo de exame realizado. O @qua:comerciais sintetiza essa comparação.

{{quadro id="comerciais" titulo="Comparativo de ferramentas comerciais" fonte="Elaborado pelo autor (2026), com base nos sites dos fornecedores." nota="N.I. = recurso não identificado nos materiais públicos consultados em outubro de 2026." larguras="2.3,2.1,2.5,2.7,1.9,1.9,2.6"}}
| Ferramenta | Foco em laboratório veterinário | Pedido *on-line* pelo solicitante | Resultado *on-line* | Entrada por IA (foto, PDF ou áudio) | Estoque integrado ao exame | Modelos de laudo |
| VetCloud | Sim | Sim (formulário no *site*) | Sim (WhatsApp, e-mail e *download*) | N.I. | N.I. | Sim (com fórmulas) |
| Workvet | Sim | N.I. | Sim | N.I. | N.I. | N.I. |
| Vetus | Módulo do ERP | N.I. | Sim (LiveLab, com *login*) | N.I. | N.I. | N.I. |
| AplusVet | Sim | N.I. | Sim (PDF pela internet) | N.I. | N.I. | Sim (por cliente) |
| VetSoft Web | Não (clínicas) | N.I. | Sim (área do tutor, com *login*) | N.I. | N.I. | Sim |
| VetControl | Sim | Sim (formulário no *site*) | Sim (*on-line* e e-mail) | N.I. | N.I. | N.I. |
| LabPat (este trabalho) | Sim | Sim (veterinário vinculado) | Sim (código público, sem *login*) | Sim (Gemini) | Sim | Sim (versionados) |
{{/quadro}}
