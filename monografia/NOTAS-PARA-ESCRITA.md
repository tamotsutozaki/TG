# Monografia do TG: como está e como gerar (atualizado em 25/10/2026, à noite)

## Como gerar
`python gerar_tg_docx.py` na raiz do projeto. O script:
1. monta o `TG_Pedro_Tozaki.docx` a partir de `monografia/conteudo/*.md` e `monografia/figuras/`;
2. abre o arquivo no Word (via `monografia/atualizar_campos.ps1`) para atualizar o sumário, as listas e as legendas;
3. exporta `TG_Pedro_Tozaki.pdf`.

Use `--sem-word` para pular o passo 2. O Word não pode estar com o .docx aberto. **Não edite o .docx à mão**: edite os .md e gere de novo. A sintaxe da marcação está no cabeçalho de `gerar_tg_docx.py`.

## Estado (revisão de 20/11/2026, antes do envio ao orientador)
- Estrutura igual à do modelo do professor (pasta `modelo prof marlon/`, fora do git; é o mesmo `modelo.pdf`):
  Introdução com seções sem número (Contextualização, Problema, Hipótese, Objetivo, Natureza, Organização),
  Capítulo I = 1, Capítulo II = 2, Capítulo III = 3, com 3.3 Trabalhos futuros e 3.4 Considerações finais
  (o arquivo `05-conclusao.md` agora contém a seção 3.4; não há mais CONCLUSÃO separada).
- Objetivo: três objetivos, sem objetivos específicos (o modelo pede no máximo três e proíbe a lista de específicos).
- Capa e folha de rosto: fonte 14 sem negrito, título 16 negrito, texto da folha de rosto com recuo de 7 cm (como no modelo).
- Orientador: Prof. Marlon Palata Fanger Rodrigues (constante `ORIENTACAO`; titulação não informada). Não há [PREENCHER].
- Folha de aprovação (banca) só na versão final, depois da defesa, conforme o Manual.
- **Não há mais dados ilustrativos.** A pedido do Pedro, a avaliação com usuários (validação com a patologista e
  questionário) foi reescrita como **etapa em andamento**: resumo, Abstract, 3.2.4 (procedimento previsto),
  3.2.5 (3º parágrafo) e 3.4 (3º e 4º parágrafos). As antigas Tabelas 3 e 4 foram removidas.
  **Quando a coleta acontecer**, atualizar esses trechos com os dados reais (e recriar as tabelas de perfil e de concordância).
- Deploy: ainda não decidido (o professor não pediu). 3.1.12 diz que a publicação está prevista no Azure;
  se mudar a hospedagem ou publicar, ajustar 3.1.12, o Quadro 3 e o RNF07.

## Dados que sustentam o texto
- Referências conferidas na web em 25/10/2026 (todas as datas de acesso são "25 out. 2026"). O PTG tinha autores errados em três obras, já corrigidos: Marletta et al. (2025), Oliveira (2024), Chaves e Rezende (2022). O sistema do Marques é em Delphi.
- Testes funcionais automatizados (08/11/2026; repetidos em 20/11/2026 na versão final): suíte Playwright/Python em `tests/` (no repositório), automatiza os 52 passos do roteiro em 53 casos; **53/53** em três execuções seguidas (~70 s cada). Ambiente isolado: banco `LabPat_E2E`, API na 5180, front na 4300 (config `e2e`); Gemini simulado. Rodar: `tests
odar.ps1`. Seção 3.2.1 e Tabela 1.
  - Histórico: a versão de 25/10 citava 158/158 (API 124 + interface 34), mas aqueles scripts ficaram num scratchpad temporário e se perderam. Por decisão do Pedro (08/11/2026), os testes de API saíram do texto.
- Extração com Gemini, 5 vezes cada formato: 240/240 campos; média de 6,9 s (foto 6,8; PDF 6,8; áudio 7,0). Script em scratchpad (`docs/bench_gemini.py`).
- Capturas de tela: banco de demonstração `LabPat_Docs` (dados fictícios). Para refazer, suba a API com essa connection string na porta 5190, rode `docs/seed.py` só se o banco for recriado e depois `docs/capturas.js` (desvia a API do front para a 5190).

## Correções feitas a partir da escrita
- Commit 0b3d0ec:
  - só a emissão do laudo conclui a solicitação;
  - o status só avança;
  - o laudo só sai depois do recebimento da amostra.
- Ainda aberto, só visual: o detalhe da solicitação mostra o peso "32.5 kg", com ponto.
