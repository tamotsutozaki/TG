---
name: roteiro-de-testes-artifact
description: Onde ficam o roteiro de testes do LabPat (artifact com checklist) e os resultados que o Pedro marca nele
metadata:
  node_type: memory
  type: reference
  originSessionId: ff36ac48-9a08-40e4-a11d-485eaec058a7
  modified: 2026-10-25T22:10:15.580Z
---

Roteiro de testes do LabPat (52 passos, 11 partes) publicado em 2026-10-25: https://claude.ai/artifact/VpRdchdxtvhcSkcEg6soSc

As marcações e anotações do Pedro ficam no db do artifact, coleção `resultados`, um documento por passo (id "parte-passo", ex.: "5-7") com `status` ("ok" | "falha" | "pulado" | null), `nota` e `atualizadoEm`. Para ver o resultado do teste, ler com ArtifactData `list` em `resultados` e focar nos de status "falha" e nas notas.

Material para testar a IA (guia fictícia em PNG/PDF e pedido em áudio WAV) fica em `material-de-teste/` na raiz do projeto, fora do git. Contas de teste sugeridas: alfa@lab.com, beta@lab.com, bruno@vet.com, senha Senha@123; admin de desenvolvimento admin@labpat.com / Admin@123. Ver [[fluxo-tg-dev-antes-da-escrita]].
