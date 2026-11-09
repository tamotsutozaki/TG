# Sessão salva em 2026-11-08

## Estado atual do projeto
Desenvolvimento essencialmente completo (último commit de código: `0b3d0ec`). Sistema multi-laboratório: administrador (convites de laboratório), laboratório/patologista (tipos de exame com template de laudo e insumos vinculados, estoque, solicitações, laudo em PDF assinado pelo responsável técnico, dashboard, aprovação de vínculos), veterinário (conta própria, vínculo com laboratórios, cria solicitações manual ou via Gemini por foto/PDF/áudio) e público (home com consulta por código, sem dados do tutor).

Monografia escrita (68 p.): `monografia/conteudo/*.md` → `python gerar_tg_docx.py` → `TG_Pedro_Tozaki.docx`. Nunca editar o .docx à mão. Trechos com DADOS ILUSTRATIVOS (marcados em amarelo) devem ser trocados pelos dados reais antes da entrega oficial — ver `monografia/NOTAS-PARA-ESCRITA.md`.

## Ambiente
- Backend: `dotnet run --launch-profile https` em `backend/LabPat.Api` → `https://localhost:7135` (migrations rodam sozinhas no startup em Development)
- Frontend: `npm start` em `frontend` → `http://localhost:4200`
- Login seed: `admin@labpat.com` / `Admin@123`. Contas de teste: alfa@lab.com, beta@lab.com, bruno@vet.com (senha `Senha@123`)
- Segredos (`appsettings.Development.json`) fora do git; ver `.claude/COMO-CONTINUAR-NO-NOTEBOOK.md`

## Itens antigos já resolvidos (não refazer)
Template no editor de laudo (`23311a3`), dashboard (`1764cc2`), migrate no startup (`21f24c2`), home pública (`ceba2f2`), solicitação pelo vet (resolvida pelo multi-laboratório: vet loga e tem vínculo).

## Testes E2E (feito em 2026-11-08)
- Suíte em `tests/` (Playwright + Python), automatiza os 52 passos do roteiro (artifact https://claude.ai/artifact/VpRdchdxtvhcSkcEg6soSc) em 53 casos: **53/53** em três execuções seguidas. Rodar: `tests\rodar.ps1` (ver `tests/LEIA-ME.md`). Ambiente isolado: banco `LabPat_E2E`, API 5180, front 4300 (config `e2e` em `angular.json` + `environment.e2e.ts`). Gemini simulado.
- Os scripts antigos (158/158, API + interface) se perderam num scratchpad; o Pedro decidiu **tirar os testes de API do TG**.
- Monografia atualizada e regenerada (72 p.): nova 4.2.1, seções renumeradas (4.2.2 extração … 4.2.6 análise), resumo/cap. 2/conclusão sem "158", Apêndice B com resultado real (sem marca de ilustrativo).
- Nada disso foi commitado ainda.

## Pendências
1. Commitar a suíte E2E e as mudanças da monografia (aguarda o Pedro).
2. Sugestões do Pedro no roteiro manual: validar o CNPJ no próprio campo (antes de enviar, como a senha) e validar tamanho/formato dos campos do cadastro do laboratório.
3. Roteiro manual: Pedro executou 12 de 52 passos (todos OK).
4. Fase 7: deploy no Azure (depois da revisão do orientador), validação com a patologista colaboradora, questionário Likert, trocar os DADOS ILUSTRATIVOS pelos reais, URL do deploy, titulação/sobrenome do orientador Marlon.
