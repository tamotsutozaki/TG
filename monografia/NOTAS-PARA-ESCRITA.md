# Monografia do TG: como está e como gerar (atualizado em 25/10/2026, à noite)

## Como gerar
`python gerar_tg_docx.py` na raiz do projeto. O script:
1. monta o `TG_Pedro_Tozaki.docx` a partir de `monografia/conteudo/*.md` e `monografia/figuras/`;
2. abre o arquivo no Word (via `monografia/atualizar_campos.ps1`) para atualizar o sumário, as listas e as legendas;
3. exporta `TG_Pedro_Tozaki.pdf`.

Use `--sem-word` para pular o passo 2. O Word não pode estar com o .docx aberto. **Não edite o .docx à mão**: edite os .md e gere de novo. A sintaxe da marcação está no cabeçalho de `gerar_tg_docx.py`.

## Estado
- Texto completo escrito, com 68 páginas: Introdução, Capítulos I a III, Conclusão, Referências e Apêndices A (questionário) e B (roteiro).
  - 23 figuras: 7 diagramas e 16 telas.
  - 9 quadros e 5 tabelas.
- Paginação: a capa não conta, a folha de rosto é a página 1 e a numeração aparece a partir da Introdução (página 9).
- Orientador: "Marlon", com titulação e sobrenome ainda como [PREENCHER] (constante `ORIENTACAO` no script). É o único [PREENCHER] restante.
- **ATENÇÃO, DADOS ILUSTRATIVOS** (versão enviada ao orientador para revisão, em 25/10/2026): a pedido do Pedro, as partes de avaliação foram preenchidas com números e observações de exemplo, marcados em amarelo com "[DADOS ILUSTRATIVOS – a serem substituídos...]" e com "(dados ilustrativos)" no título das Tabelas 4 e 5. São eles:
  - a frase da avaliação no resumo;
  - 4.2.5 (validação com a colaboradora);
  - 4.2.6 (questionário, n=15, e Tabelas 4 e 5);
  - o 3º parágrafo de 4.2.7;
  - o parágrafo do 4º objetivo na Conclusão;
  - o resultado do roteiro no Apêndice B.

  **Antes da entrega oficial, trocar tudo pelos dados reais** da validação, do questionário e do roteiro. Nunca tirar as marcas sem os dados reais.
- Deploy: o Pedro vai publicar no Azure **depois da revisão do orientador**. Até lá, 4.1.12 diz que a publicação está prevista; quando publicar, incluir o endereço.

## Dados que sustentam o texto
- Referências conferidas na web em 25/10/2026 (todas as datas de acesso são "25 out. 2026"). O PTG tinha autores errados em três obras, já corrigidos: Marletta et al. (2025), Oliveira (2024), Chaves e Rezende (2022). O sistema do Marques é em Delphi.
- Testes na versão final (commit 0b3d0ec, bancos isolados):
  - API: multilaboratório 89/89, laudo/perfil 25/25, regras de status 10/10;
  - interface: jornada 27/27, insumos 7/7;
  - total: **158/158**.

  Scripts em scratchpad (`reteste/`).
- Extração com Gemini, 5 vezes cada formato: 240/240 campos; média de 6,9 s (foto 6,8; PDF 6,8; áudio 7,0). Script em scratchpad (`docs/bench_gemini.py`).
- Capturas de tela: banco de demonstração `LabPat_Docs` (dados fictícios). Para refazer, suba a API com essa connection string na porta 5190, rode `docs/seed.py` só se o banco for recriado e depois `docs/capturas.js` (desvia a API do front para a 5190).

## Correções feitas a partir da escrita
- Commit 0b3d0ec:
  - só a emissão do laudo conclui a solicitação;
  - o status só avança;
  - o laudo só sai depois do recebimento da amostra.
- Ainda aberto, só visual: o detalhe da solicitação mostra o peso "32.5 kg", com ponto.
