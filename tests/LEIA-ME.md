# Testes E2E do LabPat

Automatizam, com Playwright, os 52 passos do roteiro de testes funcionais
(admin, laboratório, veterinário e consulta pública, em janelas separadas).

## Como rodar

```powershell
cd tests
.\rodar.ps1
```

Na primeira vez o script cria o ambiente Python e baixa o Chromium.
Requisitos: Python 3.12, .NET SDK, Node (com `npm install` feito em `frontend`) e SQL Server LocalDB com `sqlcmd`.

O próprio teste sobe um ambiente isolado, sem tocar no banco de desenvolvimento:

| Peça | Endereço |
|---|---|
| Banco | `LabPat_E2E` no LocalDB (apagado e recriado a cada execução) |
| API | http://localhost:5180 |
| Frontend | http://localhost:4300 (configuração `e2e` do Angular) |

A extração com o Gemini é simulada no navegador (resposta fixa com os dados da guia fictícia),
para o teste não depender da rede nem da chave da API.

## Resultado

- `relatorio/relatorio.html`: relatório de cada passo;
- `relatorio/resultado.xml`: resultado em JUnit XML;
- `relatorio/*.pdf`: laudos baixados durante o teste;
- `relatorio/celular/`: capturas das telas no celular;
- `relatorio/falhas/`: capturas de todas as janelas quando um passo falha;
- `relatorio/API.log` e `relatorio/Frontend.log`: saída dos servidores.
