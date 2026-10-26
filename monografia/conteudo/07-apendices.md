# APÊNDICE A – Questionário de avaliação

O questionário a seguir é aplicado por meio do Google Forms a patologistas e médicos veterinários, após o uso do sistema. A primeira página do formulário apresenta o objetivo da pesquisa e pede o consentimento do participante: a participação é voluntária e anônima, nenhum dado que identifique o respondente é coletado, e as respostas são utilizadas exclusivamente para fins acadêmicos.

**Seção 1 – Perfil do participante**

- Qual é a sua função? (patologista veterinário; médico veterinário clínico ou solicitante; outra)
- Há quanto tempo atua na área? (menos de 2 anos; de 2 a 5 anos; de 6 a 10 anos; mais de 10 anos)
- Como as solicitações de exame são enviadas ou recebidas hoje? (aplicativo de mensagens; e-mail; guia em papel; sistema próprio; outra forma)
- Quantos exames, em média, envia ou recebe por semana? (até 10; de 11 a 30; de 31 a 100; mais de 100)

**Seção 2 – Afirmações (1 = discordo totalmente; 2 = discordo; 3 = indiferente; 4 = concordo; 5 = concordo totalmente)**

Utilidade percebida:

- U1. O sistema reduziria o tempo gasto para abrir e organizar solicitações de exame.
- U2. As informações das solicitações ficam mais completas e padronizadas do que no processo atual.
- U3. O controle de estoque integrado aos exames seria útil na rotina do laboratório.
- U4. O laudo em PDF, com os dados do laboratório e do responsável técnico, atende às necessidades da rotina.

Facilidade de uso percebida:

- F1. Foi fácil aprender a usar o sistema.
- F2. As telas são claras e as ações estão onde eu esperava encontrá-las.
- F3. Consigo usar o sistema no celular sem dificuldade.

Entrada por inteligência artificial:

- I1. O preenchimento automático a partir da foto ou do PDF da guia reduziria a digitação.
- I2. Os dados preenchidos pela IA estavam corretos ou exigiram poucas correções.
- I3. Eu usaria a opção de enviar a solicitação por áudio.

Transparência e privacidade:

- T1. A consulta do andamento pelo código torna o acompanhamento do exame mais transparente.
- T2. A consulta pública mostra apenas as informações necessárias, sem expor dados pessoais.

Intenção de uso:

- N1. Eu usaria o sistema no meu dia a dia.
- N2. Eu recomendaria o sistema a outros profissionais.

**Seção 3 – Perguntas abertas**

- O que mais ajudou no uso do sistema?
- O que dificultou ou poderia ser melhorado?
- Há alguma funcionalidade que você sentiu falta?

# APÊNDICE B – Roteiro de testes funcionais

Além dos testes automatizados, o autor elaborou um roteiro de 52 passos, organizados em 11 partes, para a verificação manual do sistema no navegador, com uma janela para cada perfil (administrador, laboratório e veterinário). O @qua:roteiro resume as partes do roteiro e o que cada uma verifica.

{{quadro id="roteiro" titulo="Partes do roteiro de testes funcionais" fonte="Elaborado pelo autor (2026)." larguras="5,11"}}
| Parte | O que verifica |
| 1. Administrador e convites | Geração, cópia, revogação e expiração de convites |
| 2. Cadastro do laboratório | Cadastro pelo link, validação do CNPJ e uso único do convite |
| 3. Configuração do laboratório | Tipos de exame, modelos de laudo, insumos e estoque |
| 4. Conta e vínculo do veterinário | Criação da conta, busca de laboratórios, pedido e aprovação do vínculo |
| 5. Solicitações, laudo e consulta | Solicitação manual e por IA, status, laudo, PDF, desconto de estoque e consulta pública |
| 6. Dois laboratórios | Isolamento dos dados entre laboratórios |
| 7. Desfazer vínculo | Perda de acesso e leitura do que o veterinário enviou |
| 8. Perfil e senha | Edição dos dados, troca do responsável técnico e troca de senha |
| 9. Desativação de contas | Efeito imediato da desativação e reativação de contas |
| 10. Permissões | Bloqueio das telas e das rotas conforme o perfil |
| 11. Celular | Uso das telas em um celular |
{{/quadro}}

[[DADOS ILUSTRATIVOS – a serem substituídos pelos resultados reais da coleta]] O roteiro foi executado integralmente pelo autor: dos 52 passos, 50 foram aprovados na primeira execução, e os 2 restantes apresentaram falhas que foram corrigidas e verificadas novamente.

# APÊNDICE C – Material utilizado na avaliação da extração

A @fig:guia-teste reproduz a guia de solicitação fictícia utilizada na avaliação da extração multimodal (seção 4.2.3), enviada nos formatos PNG e PDF. Todos os nomes, registros e contatos são fictícios.

{{figura id="guia-teste" arquivo="guia-teste.png" largura="10" titulo="Guia de solicitação fictícia utilizada nos testes de extração" fonte="Elaborado pelo autor (2026)."}}

O mesmo pedido foi gravado em áudio, no formato WAV, com voz sintética, contendo a seguinte fala:

> Olá, aqui é o doutor Rafael Moura, CRMV São Paulo, número dois três quatro cinco seis. Gostaria de solicitar um hemograma completo para o Thor, um cão da raça labrador, macho, de sete anos, pesando trinta e dois quilos e meio. A tutora é a Mariana Lopes, telefone dezenove, nove nove um dois três, quatro cinco seis sete. Ele está apático há três dias, com as mucosas hipocoradas e sem apetite. Obrigado.

O @qua:valores-esperados apresenta os valores esperados para os 16 campos solicitados ao modelo. Nos campos marcados como vazios, a informação não consta no material, e o acerto consistia em o modelo não preenchê-los.

{{quadro id="valores-esperados" titulo="Valores esperados na extração de dados" fonte="Elaborado pelo autor (2026)." larguras="4.4,6.2,5.4"}}
| Campo | Foto e PDF | Áudio |
| Nome do veterinário | Dr. Rafael Moura | Rafael Moura |
| Número do CRMV | 23456 | 23456 |
| UF do CRMV | SP | SP |
| E-mail do veterinário | rafael.moura@bemestar.vet | vazio |
| Telefone do veterinário | (19) 98877-6655 | vazio |
| Nome do tutor | Mariana Lopes | Mariana Lopes |
| Telefone do tutor | (19) 99123-4567 | (19) 99123-4567 |
| E-mail do tutor | vazio | vazio |
| Nome do paciente | Thor | Thor |
| Espécie | Cão | Cão |
| Raça | Labrador | Labrador |
| Sexo | Macho | Macho |
| Idade | 7 anos | 7 anos |
| Peso | 32,5 kg | 32,5 kg |
| Tipo de exame | Hemograma completo | Hemograma completo |
| Descrição clínica | Apatia há 3 dias, mucosas hipocoradas e inapetência | Apatia há três dias, mucosas hipocoradas e falta de apetite |
{{/quadro}}
