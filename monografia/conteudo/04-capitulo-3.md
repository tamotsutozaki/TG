# CAPÍTULO III

## 4 APRESENTAÇÃO, DOCUMENTAÇÃO E ANÁLISE DE DADOS

### 4.1 Desenvolvimento do sistema

Esta seção apresenta o sistema desenvolvido, denominado LabPat, organizado pelos perfis de acesso e pelos módulos que compõem o fluxo do laboratório. As telas foram capturadas com dados fictícios, cadastrados apenas para a demonstração.

#### 4.1.1 Perfis de acesso

O sistema atende os quatro perfis apresentados no diagrama de casos de uso, resumidos no @qua:perfis. Cada perfil autenticado vê um menu próprio, e as rotas da interface são protegidas por guardas que conferem o perfil do usuário. Essa proteção, no entanto, serve apenas à navegação: a API confere o perfil novamente em cada requisição, de modo que uma chamada feita diretamente a ela, sem passar pela interface, também é barrada.

{{quadro id="perfis" titulo="Perfis de acesso do sistema" fonte="Elaborado pelo autor (2026)." larguras="3.2,4,8.8"}}
| Perfil | Como obtém acesso | O que pode fazer |
| Administrador | Conta criada a partir da configuração do sistema | Gerar e revogar convites; consultar, ativar e desativar laboratórios e veterinários. Não opera os laboratórios. |
| Laboratório (patologista) | Cadastro pelo link de um convite válido | Configurar tipos de exame, modelos de laudo e estoque; aprovar vínculos; registrar e acompanhar solicitações; emitir laudos; consultar indicadores. |
| Médico(a) veterinário(a) | Cadastro livre, com nome, e-mail e CRMV | Pedir vínculo a laboratórios; enviar solicitações aos laboratórios que aprovaram o vínculo; acompanhar as próprias solicitações e baixar os laudos. |
| Tutor | Sem conta | Consultar o andamento do exame pelo código público. |
{{/quadro}}

Na configuração adotada, cada laboratório possui uma única conta de acesso, utilizada pelo patologista responsável. Essa decisão, tomada com base no perfil dos laboratórios de pequeno e médio porte que motivaram o trabalho, simplificou o controle de acesso; a criação de várias contas por laboratório, com papéis internos, é apontada como trabalho futuro.

#### 4.1.2 Organização do *backend*

Seguindo a divisão em projetos apresentada na @fig:camadas, os controladores da API foram mantidos enxutos: recebem a requisição, chamam o serviço correspondente e devolvem a resposta. As regras de negócio ficam nos serviços da camada de aplicação, organizados por funcionalidade (solicitações, laudos, tipos de exame, insumos, vínculos, administração e perfil), e o acesso aos dados é feito por repositórios, com a gravação concentrada em uma unidade de trabalho, que confirma todas as alterações de uma operação de uma só vez.

As violações de regras de negócio são sinalizadas por exceções específicas, convertidas em respostas HTTP padronizadas por um tratador global: regra de negócio violada gera o código 400, acesso negado gera o 403 e conflito, como um e-mail ou CNPJ já cadastrado, gera o 409. Conflitos de gravação simultânea detectados pelo banco também são devolvidos como 409, com uma mensagem compreensível ao usuário. O @qua:endpoints resume os recursos expostos pela API.

{{quadro id="endpoints" titulo="Recursos da API REST" fonte="Elaborado pelo autor (2026)." larguras="3.6,7.6,4.8"}}
| Recurso | Principais operações | Perfis com acesso |
| /api/auth | *Login*, cadastro de veterinário, validação de convite e cadastro de laboratório | Público |
| /api/admin | Convites (gerar, listar e revogar); laboratórios e veterinários (listar, ativar e desativar) | Administrador |
| /api/tipos-exame | Cadastro de tipos de exame, modelos de laudo e insumos consumidos | Laboratório; leitura também para veterinário vinculado |
| /api/insumos | Cadastro de insumos e ajuste de quantidade | Laboratório |
| /api/solicitacoes | Listar, detalhar, criar e atualizar o status; consulta pública por código | Laboratório e veterinário; consulta: público |
| /api/laudos | Emitir, consultar e baixar o PDF | Emissão: laboratório; consulta: laboratório e veterinário |
| /api/gemini/extrair | Extração de dados de foto, PDF ou áudio | Laboratório e veterinário |
| /api/veterinarios | Pedidos de vínculo: listar, aprovar, recusar e desfazer | Laboratório |
| /api/laboratorios e /api/vinculos | Buscar laboratórios, abrir o link de vínculo, pedir e listar vínculos | Veterinário; link: público |
| /api/perfil | Dados do laboratório, dados do veterinário e troca de senha | Conforme o perfil |
{{/quadro}}

#### 4.1.3 Persistência e isolamento entre laboratórios

O banco de dados foi criado e evoluído por meio das migrações do Entity Framework Core, quatro ao final do desenvolvimento, aplicadas automaticamente na inicialização da API em ambiente de desenvolvimento. Para isolar os laboratórios, adotou-se a abordagem de banco compartilhado com coluna de identificação (MICROSOFT, 2022): as entidades que pertencem a um laboratório, como tipos de exame, insumos, tutores, pacientes e solicitações, possuem a coluna LaboratorioId e recebem um filtro global de consulta, que restringe automaticamente toda leitura ao laboratório do usuário autenticado. Quando não há laboratório definido para a requisição, o filtro não devolve nenhum registro, ou seja, o comportamento padrão é negar o acesso. Na gravação, o identificador do laboratório é preenchido automaticamente nos novos registros, sem depender de cada serviço.

O veterinário é o único caso em que o laboratório não vem do *login*, já que um mesmo veterinário pode atender vários laboratórios. Quando ele age em nome de um laboratório, por exemplo ao listar seus tipos de exame ou ao criar uma solicitação, o sistema verifica antes se há vínculo aprovado e, só então, define o laboratório da requisição. As consultas que reúnem as solicitações do veterinário em mais de um laboratório desligam o filtro global, mas aplicam uma condição explícita: ele vê as solicitações que ele próprio registrou e aquelas que os laboratórios com vínculo aprovado cadastraram com o seu CRMV.

Índices únicos no banco reforçam as regras de negócio: o CNPJ e o código do link de vínculo de cada laboratório, o *hash* do convite, o código público da solicitação, o e-mail de cada conta, o CRMV entre as contas ativas, o par veterinário e laboratório nos vínculos e o laudo de cada solicitação. O convite possui, ainda, um controle de concorrência que impede que duas requisições simultâneas o utilizem. As datas são gravadas em UTC e convertidas para o horário local na interface.

#### 4.1.4 Autenticação e segurança

O *login* devolve um JWT válido por 8 horas, com o identificador, o nome, o perfil e, no caso das contas de laboratório, o laboratório do usuário. A cada requisição autenticada, a API confere se a conta e o laboratório continuam ativos; assim, quando o administrador desativa uma conta, o acesso é interrompido imediatamente, mesmo que o *token* ainda esteja dentro da validade, e novas tentativas de *login* são recusadas. No *frontend*, um interceptador anexa o *token* às requisições e, ao receber uma resposta de sessão inválida, encerra a sessão e avisa o usuário.

As senhas são armazenadas com *hash* bcrypt e devem ter ao menos oito caracteres. O convite de laboratório é um valor aleatório de 256 bits, enviado no link e guardado no banco apenas na forma de *hash* SHA-256, de modo que nem quem tem acesso ao banco consegue reconstruir o link. Cada convite pode ser usado uma única vez, vale pelo prazo escolhido pelo administrador, de até 30 dias, e pode ser revogado enquanto não for utilizado. O código público das solicitações tem oito caracteres, sorteados por um gerador criptograficamente seguro a partir de um alfabeto sem caracteres ambíguos, como as letras I e O e os algarismos 0 e 1.

No cadastro do laboratório, o CNPJ é validado pelos dígitos verificadores, tanto no formato numérico quanto no alfanumérico, adotado pela Receita Federal a partir de julho de 2026 (BRASIL, 2024). O CRMV dos veterinários e dos responsáveis técnicos é validado quanto ao número e à Unidade Federativa (UF), e não podem existir duas contas ativas com o mesmo CRMV. As chaves de assinatura dos *tokens* e de acesso ao Gemini, bem como as credenciais do administrador, ficam na configuração do ambiente e não são versionadas com o código-fonte.

#### 4.1.5 Administração e adesão de laboratórios

O administrador gera os convites na tela mostrada na @fig:admin-convites, informando opcionalmente para quem é o convite e escolhendo a validade. O link é exibido uma única vez, com um botão para copiá-lo, já que o sistema guarda apenas o seu *hash*. A lista de convites indica o estado de cada um (ativo, usado, revogado ou expirado) e, quando usado, qual laboratório se cadastrou com ele. Em telas próprias, o administrador consulta os laboratórios e os veterinários cadastrados e pode desativá-los ou reativá-los, sempre com uma confirmação.

{{figura id="admin-convites" arquivo="tela-admin-convites.png" largura="15.5" titulo="Gestão de convites pelo administrador" fonte="Elaborado pelo autor (2026)."}}

Ao abrir o link, o laboratório encontra o formulário da @fig:cadastro-laboratorio, com os dados que aparecerão no cabeçalho dos laudos: nome, CNPJ, telefone, endereço, cidade, UF e o responsável técnico, com nome e CRMV, além do e-mail e da senha de acesso. Após o cadastro, o convite passa a constar como usado e não pode ser reaproveitado.

{{figura id="cadastro-laboratorio" arquivo="tela-cadastro-laboratorio.png" largura="15.5" titulo="Cadastro do laboratório a partir do convite" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.6 Configuração do laboratório

Antes de receber solicitações, o laboratório cadastra os tipos de exame que oferece, com nome, descrição e prazo estimado em dias, usado para calcular a previsão de conclusão de cada solicitação. Para cada tipo de exame, é possível cadastrar modelos de laudo, mostrados na @fig:templates. Os modelos são versionados: cada novo modelo recebe o número de versão seguinte, e os laudos já emitidos guardam o próprio conteúdo, de modo que alterações futuras no modelo não os afetam. Tipos de exame e insumos são desativados logicamente, preservando o histórico das solicitações já registradas.

{{figura id="templates" arquivo="tela-templates.png" largura="15.5" titulo="Modelos de laudo de um tipo de exame" fonte="Elaborado pelo autor (2026)."}}

O controle de estoque é feito na tela de insumos, em que se cadastram o nome, a unidade de medida e a quantidade mínima de cada item e se ajusta a quantidade atual, por exemplo após uma compra ou um inventário. Os insumos com quantidade abaixo do mínimo recebem destaque visual, como mostra a @fig:estoque.

{{figura id="estoque" arquivo="tela-estoque.png" largura="15.5" titulo="Controle de estoque com destaque para os itens abaixo do mínimo" fonte="Elaborado pelo autor (2026)."}}

O vínculo entre o estoque e os exames é configurado no diálogo de insumos consumidos de cada tipo de exame (@fig:insumos-consumidos), em que o laboratório informa quais insumos e em que quantidade cada exame consome. Se um insumo já vinculado for informado novamente, apenas a quantidade é atualizada. Essa configuração é a base do desconto automático de estoque, feito na emissão do laudo.

{{figura id="insumos-consumidos" arquivo="tela-insumos-consumidos.png" largura="15.5" titulo="Insumos consumidos por tipo de exame" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.7 Veterinários e vínculos

Na tela de veterinários (@fig:veterinarios), o laboratório encontra o link de vínculo, que pode ser enviado aos veterinários com quem trabalha, os pedidos pendentes, que pode aprovar ou recusar, e a lista de veterinários vinculados, com a opção de desfazer o vínculo.

{{figura id="veterinarios" arquivo="tela-veterinarios.png" largura="15.5" titulo="Gestão de vínculos com veterinários pelo laboratório" fonte="Elaborado pelo autor (2026)."}}

O veterinário, por sua vez, cria a conta informando nome, e-mail, senha e CRMV, sem necessidade de aprovação prévia, e pode pedir vínculo de duas formas: pelo link recebido do laboratório, que funciona mesmo antes do *login* e o leva de volta ao pedido após entrar, ou pela busca de laboratórios por nome ou cidade, na tela da @fig:meus-laboratorios, que também mostra a situação de cada vínculo (pendente, aprovado, recusado ou desfeito).

{{figura id="meus-laboratorios" arquivo="tela-meus-laboratorios.png" largura="15.5" titulo="Laboratórios do veterinário e busca de laboratórios" fonte="Elaborado pelo autor (2026)."}}

Sem vínculo aprovado, o veterinário não consegue criar solicitações para o laboratório. Após uma recusa, ele pode pedir o vínculo novamente. Se o laboratório desfizer um vínculo aprovado, o veterinário deixa de enviar novas solicitações e de ver as que o laboratório cadastrou em seu nome, mas mantém o acesso, apenas para leitura, às solicitações que ele mesmo enviou e aos respectivos laudos.

#### 4.1.8 Solicitações e entrada multimodal com IA

O formulário de nova solicitação é organizado em seções: o laboratório de destino, no caso do veterinário, escolhido entre os que aprovaram o vínculo; o método de entrada; o exame, com o tipo e a descrição clínica; os dados do veterinário solicitante, preenchidos apenas quando a solicitação é registrada pelo laboratório, já que a do veterinário usa os dados da própria conta; os dados do tutor; e os dados do paciente. Ao salvar, o sistema reaproveita, dentro do laboratório, os cadastros já existentes: o tutor é localizado pelo telefone, o paciente pelo nome entre os animais do tutor e o veterinário solicitante pelo CRMV. Em seguida, gera o código público e calcula a data estimada de conclusão a partir do prazo do tipo de exame.

Os métodos de entrada são quatro: manual, foto da guia, PDF e áudio. Nos três últimos, o usuário envia o arquivo e aciona a extração com IA, que segue a sequência da @fig:sequencia. A API aceita arquivos de imagem, PDF e áudio de até 10 MB e envia ao Gemini o arquivo e uma instrução que descreve os 16 campos esperados: nome, CRMV, UF, e-mail e telefone do veterinário; nome, telefone e e-mail do tutor; nome, espécie, raça, sexo, idade e peso do paciente; tipo de exame; e descrição clínica. A instrução pede que os campos não encontrados no documento sejam devolvidos como nulos, e a geração usa temperatura 0,1, que torna as respostas mais literais, e resposta no formato JSON.

Durante os testes, verificou-se que o modelo às vezes devolve números em campos pedidos como texto, como o peso. Por isso, a leitura da resposta foi implementada de forma tolerante, aceitando números e valores lógicos onde se esperava texto e ignorando estruturas inesperadas; se, ainda assim, a resposta não puder ser interpretada, a API devolve uma mensagem de erro amigável e o usuário pode preencher o formulário manualmente. No *frontend*, os dados recebidos passam por ajustes antes de preencher o formulário: o peso é convertido para o formato com vírgula, a idade em texto, como "7 anos", é separada em anos e meses e o tipo de exame é associado aos tipos cadastrados pelo laboratório, ignorando acentos e permitindo que "Hemograma completo" corresponda a "Hemograma". A @fig:nova-solicitacao-ia mostra o formulário preenchido a partir da foto de uma guia fictícia.

{{figura id="nova-solicitacao-ia" arquivo="tela-nova-solicitacao-ia.png" largura="14.5" titulo="Formulário de solicitação preenchido pela IA a partir da foto da guia" fonte="Elaborado pelo autor (2026)."}}

O método de entrada fica registrado na solicitação. A lista de solicitações do veterinário, mostrada na @fig:solicitacoes-veterinario, reúne os pedidos enviados a todos os laboratórios a que ele está vinculado, com uma coluna que identifica cada laboratório.

{{figura id="solicitacoes-veterinario" arquivo="tela-solicitacoes-veterinario.png" largura="15.5" titulo="Solicitações de um veterinário vinculado a dois laboratórios" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.9 Acompanhamento e emissão do laudo

A página de detalhe da solicitação apresenta o progresso do exame, os dados do paciente e do tutor, os dados do exame e do solicitante e o histórico das mudanças de status. O laboratório avança o status escolhendo a próxima etapa, com uma observação opcional; a etapa Concluído não aparece entre as opções, pois só é atingida com a emissão do laudo. A partir dos estados Amostra recebida ou Em análise, o botão de emitir laudo abre o editor já preenchido com a versão mais recente do modelo do tipo de exame (@fig:editor-laudo), que o patologista completa com os achados do caso.

{{figura id="editor-laudo" arquivo="tela-editor-laudo.png" largura="14.5" titulo="Editor de laudo com o modelo do tipo de exame carregado" fonte="Elaborado pelo autor (2026)."}}

Ao confirmar a emissão, o sistema executa, em uma única transação, as seguintes operações: verifica se a amostra já foi recebida e se a solicitação ainda não possui laudo; registra o laudo com o conteúdo, a data de emissão e o responsável técnico vigente no laboratório; conclui a solicitação, gerando o registro de histórico; e desconta do estoque os insumos configurados para o tipo de exame. O responsável técnico é copiado para o laudo no momento da emissão: se o laboratório o trocar depois, em sua tela de perfil, os novos laudos sairão com o novo responsável, e os já emitidos manterão a assinatura original. A @fig:detalhe-concluido mostra uma solicitação concluída, com o laudo e o histórico completo.

{{figura id="detalhe-concluido" arquivo="tela-detalhe-concluido.png" largura="14" titulo="Solicitação concluída com o laudo e o histórico de status" fonte="Elaborado pelo autor (2026)."}}

O laudo em PDF, gerado com a biblioteca QuestPDF no momento do *download*, traz no cabeçalho o nome, o CNPJ, o endereço e os contatos do laboratório; em seguida, a identificação da solicitação, os dados do paciente e do tutor, o veterinário solicitante com seu CRMV e o resultado do exame; no rodapé, a assinatura do responsável técnico, com o CRMV, e a data e a hora de emissão no horário de Brasília (@fig:laudo-pdf). O veterinário solicitante acessa o mesmo laudo e o mesmo PDF pela sua conta.

{{figura id="laudo-pdf" arquivo="laudo-pdf.png" largura="12" titulo="Laudo em PDF gerado pelo sistema" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.10 Painel de indicadores

Ao entrar no sistema, o laboratório vê o painel da @fig:dashboard, com o total de solicitações, as que estão em andamento, as concluídas e as que estão com o prazo vencido, a distribuição por status, a lista de solicitações não concluídas com previsão já ultrapassada e a lista de insumos abaixo da quantidade mínima, com acesso direto ao estoque.

{{figura id="dashboard" arquivo="tela-dashboard.png" largura="15.5" titulo="Painel de indicadores do laboratório" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.11 Página inicial e consulta pública

A página inicial do sistema é pública e tem como principal função a consulta do exame pelo código (@fig:home). Ela também orienta os veterinários a entrar ou criar uma conta e informa que os laboratórios entram no sistema por convite.

{{figura id="home" arquivo="tela-home.png" largura="9" titulo="Página inicial pública" fonte="Elaborado pelo autor (2026)."}}

Ao informar o código, o tutor ou o solicitante vê o andamento do exame, com o código, o status, o laboratório, o tipo de exame, o nome do animal, a data da solicitação e a previsão de conclusão (@fig:consulta-publica). Em atenção ao princípio da necessidade da LGPD, a consulta não exibe dados pessoais do tutor nem do veterinário, nem a descrição clínica ou o resultado do exame.

{{figura id="consulta-publica" arquivo="tela-consulta-publica.png" largura="9" titulo="Consulta pública do exame pelo código" fonte="Elaborado pelo autor (2026)."}}

#### 4.1.12 Responsividade e implantação

A interface foi adaptada a telas estreitas: no celular, o menu lateral passa a abrir como gaveta, e as tabelas e os diálogos se ajustam à largura da tela, como mostra a @fig:celular.

{{figura id="celular" arquivo="tela-celular.png" largura="14" titulo="Telas do sistema em um celular: consulta pública, solicitações e menu" fonte="Elaborado pelo autor (2026)."}}

Durante o desenvolvimento, o sistema foi executado localmente, com o banco de dados no SQL Server LocalDB. Para a avaliação com usuários, a publicação foi planejada no Microsoft Azure, por meio do programa Azure for Students: o *frontend* no Azure Static Web Apps, a API no Azure App Service e o banco de dados no Azure SQL Database. Como as chaves, a conexão com o banco, o endereço autorizado do *frontend* e as credenciais do administrador são lidos da configuração, a publicação não exige alterações no código. A publicação será feita antes da avaliação com usuários, para que a patologista colaboradora e os participantes do questionário acessem o sistema pelo navegador, sem depender do computador do autor.

### 4.2 Implementação, avaliação e análise dos testes realizados

#### 4.2.1 Testes automatizados da API

Os testes da API foram escritos em Python e executados contra uma instância do sistema ligada a um banco de dados exclusivo para testes, recriado a cada execução. Cada verificação envia requisições HTTP e confere o código de resposta e o conteúdo devolvido; algumas consultam também o banco, por exemplo para confirmar que apenas o *hash* do convite é armazenado e que nenhuma solicitação mistura dados de laboratórios diferentes. Foram organizadas três suítes: a primeira cobre a administração, os convites, o isolamento entre laboratórios, os vínculos, os perfis e a desativação de contas; a segunda cobre o laudo, o PDF, as datas em UTC, o perfil e a troca de senha; e a terceira cobre as regras de status, como a proibição de retroceder etapas, de concluir a solicitação sem laudo e de emitir o laudo antes do recebimento da amostra. A @tab:testes-api apresenta o resultado da execução na versão final do sistema, em 25 de outubro de 2026.

{{tabela id="testes-api" titulo="Resultados dos testes automatizados da API" fonte="Elaborado pelo autor (2026)." larguras="10,3,3"}}
| Grupo de verificações | Verificações | Aprovadas |
| **Suíte 1: multilaboratório** | | |
| Administrador | 5 | 5 |
| Convites e cadastro de laboratórios | 14 | 14 |
| Isolamento entre laboratórios | 20 | 20 |
| Veterinários e vínculos | 37 | 37 |
| Perfis de acesso | 3 | 3 |
| Desativação de contas | 8 | 8 |
| Integridade no banco de dados | 2 | 2 |
| **Suíte 2: laudo, perfil e datas** | | |
| Preparação dos dados | 2 | 2 |
| Datas em UTC | 2 | 2 |
| Laudo e PDF | 3 | 3 |
| Perfil do laboratório | 10 | 10 |
| Perfil do veterinário | 3 | 3 |
| Troca de senha | 4 | 4 |
| Perfis de acesso | 1 | 1 |
| **Suíte 3: regras de status** | | |
| Avanço de etapas, conclusão pelo laudo e emissão após a amostra | 10 | 10 |
| **Total** | **124** | **124** |
{{/tabela}}

Entre as verificações de isolamento, destacam-se as que tentam, com a conta de um laboratório, ler, alterar ou excluir dados de outro laboratório, emitir laudo em solicitação alheia e usar o tipo de exame de outro laboratório em uma solicitação; em todos os casos, a API respondeu como se o registro não existisse. Nas verificações de vínculo, confirmou-se, por exemplo, que o veterinário sem vínculo aprovado não vê nenhuma solicitação, que, após o desfazimento do vínculo, mantém apenas a leitura do que enviou e que o administrador não acessa as rotas de operação dos laboratórios.

#### 4.2.2 Testes de interface

Os testes de interface foram automatizados com a biblioteca Playwright, que controla o navegador Google Chrome e interage com as telas reais do sistema, como faria um usuário. Foram escritos dois roteiros: uma jornada completa, que percorre o fluxo desde a geração do convite pelo administrador até o *download* do laudo pelo veterinário e a desativação de sua conta, e um roteiro específico para o vínculo de insumos e o desconto de estoque. A @tab:testes-interface apresenta os resultados.

{{tabela id="testes-interface" titulo="Resultados dos testes automatizados de interface" fonte="Elaborado pelo autor (2026)." larguras="10,3,3"}}
| Etapa verificada | Verificações | Aprovadas |
| **Jornada completa** | | |
| Administrador gera o convite | 3 | 3 |
| Laboratório se cadastra pelo convite | 3 | 3 |
| Laboratório confere o perfil e se prepara | 4 | 4 |
| Veterinário chega pelo link de vínculo | 5 | 5 |
| Laboratório aprova o vínculo | 2 | 2 |
| Veterinário envia a solicitação | 4 | 4 |
| Laboratório avança o status e emite o laudo | 2 | 2 |
| Veterinário baixa o laudo em PDF | 1 | 1 |
| Administrador desativa a conta do veterinário | 3 | 3 |
| **Insumos e estoque** | | |
| Vínculo de insumos ao tipo de exame | 5 | 5 |
| Desconto de estoque na emissão do laudo | 2 | 2 |
| **Total** | **34** | **34** |
{{/tabela}}

Somadas as duas abordagens, as 158 verificações automatizadas foram aprovadas na versão final. As duas respostas de erro registradas durante a jornada eram esperadas e fazem parte do que se queria verificar: a tentativa de reutilizar um convite já usado e o acesso de um veterinário logo após a desativação de sua conta.

#### 4.2.3 Avaliação da extração multimodal

Para avaliar a extração, foi elaborada uma guia de solicitação fictícia, com os dados de um veterinário, de um tutor, de um paciente, do exame e do histórico clínico, apresentada em dois formatos, imagem PNG e PDF. O mesmo pedido foi gravado em áudio, no formato WAV, com voz sintética, ditando os dados como faria um veterinário em uma mensagem. Cada arquivo foi enviado cinco vezes à API do sistema, com o modelo gemini-3.8-flash. Em cada execução, os 16 campos devolvidos foram comparados aos valores esperados; nos campos ausentes do documento, como o e-mail do tutor, e, no áudio, também o e-mail e o telefone do veterinário, o acerto consistia em devolver o campo vazio, sem inventar valores. Na comparação, foram desconsideradas diferenças de maiúsculas, acentos e formatação; os telefones foram comparados pelos dígitos e a descrição clínica, pela presença dos termos essenciais. O conteúdo da guia, a transcrição do áudio e os valores esperados encontram-se no Apêndice C. Os tempos foram medidos a partir do computador de desenvolvimento e incluem o envio do arquivo pela internet e o processamento pelo Gemini. A @tab:extracao resume os resultados.

{{tabela id="extracao" titulo="Resultados da extração de dados com o Gemini" fonte="Elaborado pelo autor (2026)." larguras="4,2.4,3.2,3,3.4"}}
| Tipo de entrada | Execuções | Campos corretos | Tempo médio (s) | Tempo mín.–máx. (s) |
| Foto (PNG) | 5 | 80 de 80 | 6,8 | 4,6–9,5 |
| PDF | 5 | 80 de 80 | 6,8 | 6,1–8,5 |
| Áudio (WAV) | 5 | 80 de 80 | 7,0 | 6,4–7,8 |
| **Total** | **15** | **240 de 240** | **6,9** | **4,6–9,5** |
{{/tabela}}

Todos os campos foram extraídos corretamente nas 15 execuções, inclusive os vazios, e o tempo de resposta ficou entre 4,6 e 9,5 segundos, compatível com o uso durante a abertura de uma solicitação. Esses resultados indicam a viabilidade da abordagem, mas devem ser lidos com cautela: a guia utilizada é digitada e bem enquadrada, e o áudio foi gerado por voz sintética, sem ruído. Guias manuscritas, fotos com pouca luz ou inclinadas e áudios gravados em ambientes ruidosos tendem a reduzir a taxa de acerto, e o risco de alucinação apontado por Pal e Sankarasubbu (2024) não desaparece. Por isso, o sistema mantém a revisão obrigatória dos dados pelo usuário antes da criação da solicitação, e a avaliação com guias reais é indicada como trabalho futuro.

#### 4.2.4 Problemas encontrados e correções

Os testes revelaram problemas que não eram aparentes durante o desenvolvimento das funcionalidades isoladas. O @qua:correcoes reúne os principais, com a causa identificada e a correção aplicada.

{{quadro id="correcoes" titulo="Problemas encontrados nos testes e correções aplicadas" fonte="Elaborado pelo autor (2026)." larguras="5,5,6"}}
| Problema identificado | Causa | Correção aplicada |
| Erro ao criar solicitação com tutor, paciente ou veterinário ainda não cadastrados | As chaves estrangeiras eram preenchidas antes de os registros relacionados receberem identificador | Uso das propriedades de navegação do Entity Framework Core, que define a ordem correta de gravação |
| Segundo laudo para a mesma solicitação não era barrado | A verificação dependia de um relacionamento que não era carregado na consulta | Consulta explícita ao laudo da solicitação antes da emissão |
| Datas exibidas com três horas de diferença | As datas em UTC eram devolvidas sem indicação de fuso | Conversão que marca as datas como UTC na leitura do banco; PDF no horário de Brasília |
| Extração por IA indisponível | O modelo do Gemini inicialmente configurado foi descontinuado pelo fornecedor | Adoção de um modelo atual, definido por configuração |
| Falha ao interpretar a resposta da IA | O modelo devolvia números em campos esperados como texto, como o peso | Leitura tolerante da resposta e mensagem de erro amigável |
| Estoque nunca era descontado | Não havia tela para vincular os insumos aos tipos de exame | Diálogo de insumos consumidos na tela de tipos de exame |
| Solicitação podia ser concluída sem laudo, sem baixa de estoque | A atualização de status aceitava qualquer estado, inclusive Concluído | Concluído passou a ser atingido somente pela emissão do laudo; o status só avança, na interface e na API |
{{/quadro}}

O problema do estoque ilustra a importância dos testes de ponta a ponta: o desconto automático estava implementado e funcionava na API, mas, sem uma tela para configurar o consumo de cada exame, ele nunca seria acionado por um usuário real. A falha só foi percebida ao percorrer o fluxo completo pela interface. Já a possibilidade de concluir uma solicitação sem laudo foi identificada durante a preparação das telas para este documento; após a correção, foi criada a suíte de regras de status, para que o problema não volte a ocorrer.

#### 4.2.5 Validação com a patologista colaboradora

[[DADOS ILUSTRATIVOS – a serem substituídos pelos resultados reais da coleta]]

A validação foi realizada em uma sessão de cerca de uma hora com a médica veterinária patologista colaboradora, que utilizou o sistema publicado a partir de um roteiro de tarefas representativas da rotina do laboratório: configurar um tipo de exame com modelo de laudo e insumos consumidos, aprovar o vínculo de um veterinário, registrar uma solicitação a partir da foto de uma guia, acompanhar o status e emitir o laudo. Durante a sessão, a profissional comentou em voz alta suas impressões e, ao final, respondeu a uma entrevista sobre a adequação dos campos ao seu fluxo real, a clareza da interface, a utilidade da extração por IA e a percepção de redução de esforço em relação ao processo atual.

A profissional considerou os campos da solicitação adequados ao fluxo do laboratório e destacou a padronização das informações como o principal ganho em relação ao recebimento de pedidos por aplicativo de mensagens. Avaliou positivamente o preenchimento a partir da foto da guia, ressaltando que a revisão dos dados antes do envio é indispensável. Entre as sugestões, apontou a inclusão de um campo para o tipo de material enviado, como sangue, tecido ou material de punção, a possibilidade de anexar a imagem da guia à solicitação e a criação de contas separadas para a equipe do laboratório, registradas como trabalhos futuros na seção 4.3.

#### 4.2.6 Questionário aplicado aos usuários

[[DADOS ILUSTRATIVOS – a serem substituídos pelos resultados reais da coleta]]

O questionário do Apêndice A foi aplicado por meio do Google Forms durante duas semanas, após a publicação do sistema, e divulgado a patologistas e médicos veterinários da rede de contatos da colaboradora e do autor. Antes de responder, cada participante recebeu o endereço do sistema e um roteiro curto de tarefas. Foram obtidas 15 respostas válidas, cujo perfil é apresentado na @tab:perfil-respondentes.

{{tabela id="perfil-respondentes" titulo="Perfil dos respondentes do questionário (dados ilustrativos)" fonte="Dados ilustrativos (2026), a serem substituídos pelos dados da pesquisa." larguras="8,4,4"}}
| Função | Respondentes | Percentual |
| Patologista veterinário(a) | 4 | 26,7% |
| Médico(a) veterinário(a) clínico(a) ou solicitante | 9 | 60,0% |
| Outra função | 2 | 13,3% |
| **Total** | **15** | **100%** |
{{/tabela}}

A @tab:respostas resume a concordância com as afirmações, agrupadas por construto. A concordância corresponde à proporção de respostas com notas 4 ("concordo") ou 5 ("concordo totalmente") no conjunto das afirmações de cada construto.

{{tabela id="respostas" titulo="Concordância com as afirmações do questionário, por construto (dados ilustrativos)" fonte="Dados ilustrativos (2026), a serem substituídos pelos dados da pesquisa." larguras="5.5,3,4,3.5"}}
| Construto | Afirmações | Concordância (notas 4 e 5) | Mediana |
| Utilidade percebida | U1 a U4 | 85,0% | 5 |
| Facilidade de uso percebida | F1 a F3 | 80,0% | 4 |
| Entrada por IA | I1 a I3 | 77,8% | 4 |
| Transparência e privacidade | T1 e T2 | 93,3% | 5 |
| Intenção de uso | N1 e N2 | 83,3% | 4 |
{{/tabela}}

A transparência e a privacidade da consulta pública obtiveram a maior concordância (93,3%), seguidas da utilidade percebida (85,0%) e da intenção de uso (83,3%). O construto com menor concordância foi o da entrada por IA (77,8%), influenciado pela afirmação sobre o envio por áudio, que dividiu os participantes: parte deles relatou preferir a foto ou o PDF da guia. Nas perguntas abertas, os comentários mais frequentes destacaram a rapidez para abrir solicitações e a consulta por código, e as sugestões concentraram-se em avisos automáticos de mudança de status e em uma versão do sistema para instalar no celular.

#### 4.2.7 Análise dos resultados

Em relação à padronização das informações, o sistema passou a exigir, em toda solicitação, os mesmos dados mínimos (tipo de exame, tutor, paciente e veterinário solicitante com CRMV válido), registrados de forma estruturada e reaproveitados nas solicitações seguintes do mesmo laboratório, o que reduz a dependência de mensagens livres para a abertura do pedido. O acompanhamento do exame deixou de depender de contatos individuais: cada mudança de status fica registrada com data e responsável, e o andamento pode ser consultado pelo código público.

Quanto à qualidade da extração pelo Gemini, a medição realizada mostrou acerto em todos os campos da guia de teste nos três formatos, em poucos segundos, o que responde positivamente, dentro das limitações discutidas, à segunda questão de pesquisa. Os testes automatizados confirmaram ainda o isolamento entre laboratórios e o controle de acesso por perfil, requisitos essenciais para a operação de vários laboratórios na mesma plataforma, e a minimização dos dados expostos na consulta pública, relacionada à terceira questão de pesquisa.

[[DADOS ILUSTRATIVOS – a serem substituídos pelos resultados reais da coleta]] A validação com a patologista colaboradora e o questionário indicam percepção favorável quanto à redução do esforço na abertura e no acompanhamento das solicitações e quanto à transparência do processo, o que sustenta, dentro dos limites de uma amostra pequena e não probabilística, a hipótese formulada e responde à primeira e à terceira questões de pesquisa. A menor concordância em relação ao envio por áudio sugere que esse canal deve ser oferecido como alternativa, e não como forma principal de solicitação.

### 4.3 Trabalhos futuros

A partir dos resultados obtidos e das limitações identificadas, propõem-se os seguintes trabalhos futuros:

a) avaliar a extração com guias reais, manuscritas ou fotografadas em condições variadas, e com áudios gravados por veterinários, medindo a taxa de acerto por campo;
b) medir, em um laboratório em operação, o tempo de abertura e de acompanhamento das solicitações antes e depois da adoção do sistema, complementando a avaliação baseada na percepção dos usuários;
c) armazenar o arquivo original da guia junto à solicitação, para consulta posterior;
d) registrar o tipo de material enviado (sangue, tecido ou material de punção, por exemplo) e as condições de recebimento da amostra;
e) enviar avisos automáticos ao veterinário e ao tutor a cada mudança de status e na liberação do laudo, por e-mail ou aplicativo de mensagens;
f) permitir várias contas por laboratório, com papéis internos, como recepção, técnico e patologista, e trilha de auditoria por usuário;
g) implementar a recuperação de senha por e-mail;
h) permitir a edição e o cancelamento de solicitações e a retificação de laudos, com controle de versões;
i) adotar a assinatura digital dos laudos com certificado da Infraestrutura de Chaves Públicas Brasileira;
j) integrar o sistema aos equipamentos do laboratório e emitir etiquetas com código de barras para as amostras;
k) oferecer relatórios gerenciais de produtividade, de cumprimento de prazos e de consumo de insumos.
