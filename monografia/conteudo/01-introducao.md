# INTRODUÇÃO

O mercado brasileiro de produtos e serviços para animais de estimação mantém uma trajetória de crescimento. Segundo dados da Associação Brasileira da Indústria de Produtos para Animais de Estimação (ABINPET), o setor faturou R$ 75,4 bilhões em 2024, alta de 9,6% em relação ao ano anterior, o que mantém o Brasil como o terceiro maior mercado *pet* do mundo (PEREIRA, 2025). Estima-se ainda que o país tenha cerca de 160,9 milhões de animais de estimação, entre eles 62,2 milhões de cães e 30,8 milhões de gatos (RAVAGNANI, 2024). Esse movimento amplia a demanda por serviços veterinários especializados, entre os quais se destacam os exames realizados por laboratórios de patologia veterinária, como citologia, histopatologia, hemograma e análises bioquímicas, essenciais para o diagnóstico e o tratamento adequados dos animais.

Apesar do aumento da demanda, muitos laboratórios de patologia veterinária, sobretudo os de pequeno e médio porte, ainda operam com processos predominantemente manuais ou apoiados em ferramentas genéricas, como aplicativos de mensagens, e-mail, planilhas e pastas de arquivos. Nesse cenário, as solicitações de exame chegam de forma desestruturada e, com frequência, incompletas; o patologista perde tempo recolhendo as informações que faltam; o acompanhamento de cada exame depende da memória da equipe; e o repasse do resultado ao médico veterinário solicitante e ao tutor do animal ocorre de forma artesanal.

Este trabalho apresenta o desenvolvimento do LabPat, um sistema web responsivo para a gestão do fluxo de trabalho de laboratórios de patologia veterinária, desde a solicitação do exame, passando pelo recebimento da amostra e pela análise, até a emissão e a disponibilização do laudo. Além de organizar o fluxo, o sistema incorpora recursos de inteligência artificial (IA) generativa multimodal, por meio da *Application Programming Interface* (API) do Google Gemini, para extrair automaticamente os dados de guias de solicitação enviadas como foto, documento em *Portable Document Format* (PDF) ou áudio, o que reduz a digitação na abertura de cada pedido. Ao longo do desenvolvimento, o escopo inicial, voltado a um único laboratório, foi ampliado para uma plataforma capaz de atender vários laboratórios de forma isolada, com a entrada de cada laboratório controlada por convite.

## 1 CONTEXTUALIZAÇÃO

O cenário típico de um laboratório de patologia veterinária envolve múltiplas partes interessadas: o patologista responsável pelo laboratório; os médicos veterinários solicitantes, que atendem o animal e demandam o exame; os tutores, que aguardam o resultado; e, em muitos casos, clínicas e hospitais veterinários que terceirizam seus exames. A comunicação entre essas partes ocorre, em grande parte, por canais informais, como aplicativos de mensagens, telefonemas e e-mails, o que dificulta a rastreabilidade do processo, a padronização das informações e a conferência das etapas executadas.

Um patologista veterinário, em sua rotina, executa funções que vão além do exame em si. Ele recebe solicitações em diversos formatos (guias manuscritas, mensagens de texto e áudios), interpreta essas solicitações, organiza prioridades, executa ou supervisiona os procedimentos técnicos, redige laudos, envia os resultados e responde a pedidos de informação sobre o andamento dos exames. Cada uma dessas atividades, quando não é apoiada por um sistema dedicado, consome um tempo que poderia ser direcionado à atividade-fim, o diagnóstico.

A literatura de medicina laboratorial reforça a importância de organizar as etapas que antecedem e sucedem a análise. Plebani (2006) mostra que a maior parte dos erros laboratoriais ocorre fora da etapa analítica: os estudos revisados pelo autor atribuem de 46% a 68,2% dos erros à fase pré-analítica, que inclui a solicitação, a identificação do paciente e a coleta, e de 18,5% a 47% à fase pós-analítica, em que o resultado é registrado, interpretado e comunicado. Embora esses dados se refiram à medicina humana, o fluxo de um laboratório veterinário tem as mesmas fases e está sujeito ao mesmo tipo de falha, sobretudo quando a solicitação chega incompleta ou o resultado é repassado manualmente. A informatização desse fluxo por meio de um Sistema de Informação Laboratorial (*Laboratory Information System* – LIS) específico para a patologia veterinária representa, portanto, uma oportunidade de ganho de eficiência e de qualidade.

Adicionalmente, o avanço recente dos modelos de linguagem multimodais de grande escala, capazes de interpretar texto, imagem e áudio, abre uma nova frente de automação ainda pouco explorada no contexto dos laboratórios veterinários brasileiros. A possibilidade de transformar a foto de uma guia ou um áudio enviado por aplicativo de mensagens diretamente em uma solicitação estruturada representa um ganho de produtividade relevante tanto para o médico veterinário solicitante quanto para o laboratório que recebe o pedido.

### 1.1 Problema de pesquisa

Diante do contexto apresentado, este trabalho parte de três questões norteadoras, que constituem o problema de pesquisa:

a) de que forma a centralização do fluxo de solicitação, acompanhamento e laudo em uma única plataforma web pode reduzir o esforço operacional do patologista veterinário?
b) em que medida modelos multimodais de inteligência artificial generativa são capazes de extrair dados estruturados de guias de solicitação enviadas em diferentes formatos (imagem, PDF e áudio), com qualidade suficiente para alimentar o formulário de solicitação do sistema?
c) como uma página de consulta pública por código único pode aumentar a transparência percebida por solicitantes e tutores, preservando, ao mesmo tempo, a privacidade dos dados pessoais, em conformidade com a Lei Geral de Proteção de Dados Pessoais (LGPD)?

### 1.2 Hipótese

A hipótese deste trabalho é a de que a centralização do fluxo de solicitação, acompanhamento e emissão de laudos em uma plataforma web responsiva, aliada à entrada multimodal de dados assistida por inteligência artificial generativa, é capaz de reduzir o tempo gasto pelo patologista em atividades acessórias, padronizar as informações coletadas nas solicitações de exame e aumentar a transparência do processo para solicitantes e tutores, preservando a privacidade dos dados pessoais por meio da minimização das informações exibidas na consulta pública.

### 1.3 Objetivo

Este trabalho teve como objetivo geral desenvolver um sistema web responsivo para a gestão do fluxo de trabalho de laboratórios de patologia veterinária, da solicitação do exame à emissão do laudo. Para isso, foram definidos os seguintes objetivos específicos:

a) desenvolver uma plataforma web para a gestão integrada do fluxo de trabalho do laboratório, contemplando a solicitação, o acompanhamento do andamento, a emissão e a disponibilização de laudos e o controle de estoque dos insumos consumidos nos exames;
b) integrar à plataforma um módulo de extração de dados baseado em inteligência artificial generativa multimodal, por meio da API do Google Gemini, capaz de transformar guias enviadas como foto, PDF ou áudio em solicitações estruturadas;
c) permitir o uso da plataforma por vários laboratórios, com dados isolados entre si, cadastro controlado por convite e vínculo dos médicos veterinários a cada laboratório;
d) avaliar a solução por meio de testes funcionais, da validação com uma profissional patologista veterinária e da aplicação de um questionário a potenciais usuários.

### 1.4 Natureza da pesquisa

Em Análise e Desenvolvimento de Sistemas, é comum optar pela pesquisa de natureza experimental. Segundo Gil (2002), a pesquisa experimental consiste em determinar um objeto de estudo, selecionar as variáveis capazes de influenciá-lo e definir as formas de controle e de observação dos efeitos que essas variáveis produzem no objeto. Neste trabalho, o objeto de estudo é o fluxo operacional do laboratório de patologia veterinária; as variáveis manipuladas são a digitalização do processo e a introdução da extração multimodal de dados por IA; e os efeitos foram observados por meio de testes funcionais e de medições da extração, complementados pela validação com uma profissional da área e por um questionário aplicado a potenciais usuários.

### 1.5 Organização dos capítulos

O Capítulo I apresenta a fundamentação teórica do trabalho, com os conceitos de sistemas de informação laboratorial, aplicações web, inteligência artificial generativa multimodal e patologia clínica veterinária, além da análise dos trabalhos acadêmicos e das ferramentas comerciais relacionados, organizada em quadros comparativos.

O Capítulo II descreve a metodologia adotada: as tecnologias empregadas, as etapas do desenvolvimento, os requisitos levantados, os diagramas que modelam o sistema e os critérios definidos para avaliar os resultados.

O Capítulo III apresenta o sistema desenvolvido, com suas telas e decisões de implementação, os testes realizados e a análise dos resultados obtidos, além das propostas de trabalhos futuros. Por fim, a Conclusão retoma o problema de pesquisa e avalia o cumprimento dos objetivos. Os apêndices reúnem o questionário de avaliação, o roteiro de testes funcionais e o material utilizado na avaliação da extração de dados.
