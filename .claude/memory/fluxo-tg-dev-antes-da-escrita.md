---
name: fluxo-tg-dev-antes-da-escrita
description: Estado da monografia ABNT do TG — texto gerado por script a partir de .md; o que falta preencher e como regenerar
metadata:
  node_type: memory
  type: project
  originSessionId: ff36ac48-9a08-40e4-a11d-485eaec058a7
  modified: 2026-10-26T02:47:06.280Z
---

Ordem combinada em 2026-10-25: desenvolvimento → testes → escrita → refinamento → envio ao professor. Na noite de 2026-10-25 o "grosso" foi escrito: `TG_Pedro_Tozaki.docx` (68 p.) é gerado por `gerar_tg_docx.py` a partir de `monografia/conteudo/*.md` + `monografia/figuras/`, e o Word (`monografia/atualizar_campos.ps1`) atualiza sumário/listas e exporta o PDF. Detalhes e dados em `monografia/NOTAS-PARA-ESCRITA.md`.

**Why:** o Pedro quer o texto pronto para revisar e mandar ao professor; ele não preenche nada (ver [[pedro-orquestra-claude-executa]]).

**How to apply:** nunca editar o .docx à mão — editar os .md e regenerar. Orientador: "Marlon" (titulação e sobrenome ainda pendentes). Pendências em `[PREENCHER]`: URL do deploy, validação com a patologista colaboradora (que ajudou nos requisitos e ainda vai validar), resultados do questionário Likert (vai ser aplicado), conclusão do 4º objetivo e resultado do roteiro manual. Decisões: sistema chamado "LabPat" no texto; sem folha de aprovação e sem abstract. Deploy: Pedro vai publicar no Azure depois da revisão do orientador (eu faço a configuração; ele cria a conta Azure for Students e faz login). Bug "Concluído sem laudo" corrigido no commit 0b3d0ec; testes 158/158. IMPORTANTE: em 2026-10-25 o Pedro pediu para "inventar como se já tivesse feito" a avaliação para mandar ao orientador; recusei apresentar como real e, com o aval dele, preenchi com DADOS ILUSTRATIVOS marcados em amarelo (resumo, 4.2.5, 4.2.6, Tabelas 4 e 5, 4.2.7, Conclusão, Apêndice B). Esses trechos DEVEM ser trocados pelos dados reais antes da entrega oficial, e as marcas nunca devem ser removidas sem os dados reais. Ver [[roteiro-de-testes-artifact]].
