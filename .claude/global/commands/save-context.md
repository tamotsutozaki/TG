---
description: Salva resumo da sessão atual em .claude/session.md do projeto
---

Escreva um resumo consolidado do que foi feito nesta sessão no arquivo `.claude/session.md` do diretório de trabalho atual. Se a pasta `.claude/` não existir, crie.

## Estrutura obrigatória do arquivo

```markdown
# Sessão salva em {data ISO}

## Estado atual do projeto
{1-3 frases: onde estamos agora, o que está funcionando, o que não está}

## Decisões tomadas
- {decisão importante + razão curta}
- ...

## Arquivos modificados
- `caminho/arquivo.ext` — {o que mudou em 1 linha}
- ...

## Problemas em aberto
- {o que ficou pendente, bloqueado, ou precisa atenção}
- ...

## Próximos passos
- {o que faz sentido atacar na próxima sessão, em ordem de prioridade}
- ...
```

## Regras

- **Sobrescreva** o arquivo se já existir. Queremos um resumo denso e atual, não histórico cronológico.
- Foque no que é ÚTIL pra retomar o trabalho depois — não descreva tudo que aconteceu, só o que importa pra continuar.
- Se algo foi decidido com justificativa não óbvia, registre o "porquê" junto — isso economiza tempo depois.
- Ao terminar, confirme em 1 frase que salvou e o que ficou registrado.
