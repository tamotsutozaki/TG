# Regras pessoais

## Antes de tarefas complexas, faça perguntas

Antes de executar qualquer tarefa considerada complexa (mudanças grandes em projetos de backend/frontend, decisões de design ou UX, layouts de documentos, refatorações não triviais, ou qualquer coisa cujo resultado dependa de contexto que não está no código), faça as perguntas necessárias para cobrir o que está apenas na cabeça do usuário.

- Use bom senso na quantidade — geralmente entre 2 e 7 perguntas.
- Foque no que é CRÍTICO pra fazer a tarefa bem: requisitos implícitos, restrições não ditas, preferências de abordagem, escopo, stakeholders, critérios de aceitação.
- NÃO faça perguntas em tarefas simples (fix pontual, pergunta de dúvida, comando rápido). Use julgamento.
- Se o usuário deixar claro que quer direto ao ponto, respeite e pule as perguntas.

## Contexto de sessão salvo

Se existir `.claude/session.md` no diretório do projeto atual, ele contém o estado em que a sessão anterior parou (decisões, arquivos tocados, pendências, próximos passos). Trate-o como fonte autoritativa do estado atual do projeto — priorize o que está nele sobre suposições próprias, e confirme com o usuário se algo mudou desde o último registro antes de agir.
