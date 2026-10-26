---
description: Mostra o fluxo ideal de uso do Claude Code pro Pedro, com comandos customizados e features built-in
---

Mostre ao Pedro o fluxo ideal de uso do Claude Code, combinando as features nativas e os comandos customizados que ele já tem. Seja denso e acionável — isso é um guia de consulta rápida, não um tutorial.

## Estrutura obrigatória da resposta

### 1. Antes de abrir o Claude Code
- Ter clareza mental do projeto: o que faz, stack, onde roda, orçamento de API.
- Criar a pasta vazia.

### 2. Primeira sessão em projeto novo
- Rodar `/init` (built-in) pra gerar o `CLAUDE.md` do projeto — stack, comandos de build/test/lint, convenções, estrutura.
- Descrever o projeto em alto nível — o CLAUDE.md global já força perguntas antes de tarefas complexas.
- Pedir plano em markdown antes de implementar, revisar, só então autorizar codar.

### 3. Durante o desenvolvimento
- **Plan mode** (`Shift+Tab` duas vezes) antes de mudanças grandes — evita retrabalho.
- **`/duvida <pergunta>`** (custom) pra perguntas rápidas — resposta em 1 parágrafo, sem enrolação.
- Pedir testes **junto** com a feature, não depois.
- Commits pequenos e frequentes (manual, já que `Bash(git *)` tá bloqueado nas permissões).
- Rodar a suíte de testes antes de encerrar a sessão.

### 4. Encerrando a sessão
- **`/save-context`** (custom) — salva resumo denso em `.claude/session.md` do projeto. Sobrescreve a cada uso.
- O arquivo está no `~/.gitignore_global`, então nunca vai pro repo.

### 5. Retomando depois
- Hook `SessionStart` (já configurado em `~/.claude/settings.json`) injeta o `session.md` automaticamente ao abrir a pasta.
- Confirmar com o Pedro se algo mudou desde o último registro antes de agir.

### 6. Quando algo vira dor repetida
- **Só então** considerar criar subagent, hook customizado, MCP server, ou skill. Nunca antes — abstração prematura queima tokens.
- Exemplos de sinal concreto: "toda vez preciso revisar X antes de commitar" → subagent de review; "sempre escrevo o mesmo boilerplate de teste" → subagent de test-writer.

### 7. Permissões atuais (referência)
- `Bash(*)` liberado, `Read(**)` liberado.
- `Bash(git *)` bloqueado — git é manual.
- Pra ajustar: `~/.claude/settings.json` (global) ou `.claude/settings.local.json` (por projeto).

## Regras
- Não invente features que ele não configurou. Fonte da verdade: `~/.claude/settings.json`, `~/.claude/commands/`, `~/.claude/CLAUDE.md`.
- Se ele tiver criado comandos ou agents novos desde a última atualização desse comando, mencione e sugira atualizar este arquivo (`~/.claude/commands/boas-praticas.md`).
- Termine perguntando: "Quer aprofundar algum desses pontos?"
