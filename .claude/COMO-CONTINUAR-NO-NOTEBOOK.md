# Como continuar o TG em outra máquina

## 1. Clonar
```
git clone https://github.com/tamotsutozaki/TG.git
cd TG
```

## 2. Segredos (NÃO estão no GitHub — o repo é público)
Copie `backend/LabPat.Api/appsettings.Development.example.json` para
`backend/LabPat.Api/appsettings.Development.json` e preencha:
- `Gemini:ApiKey` — chave em https://aistudio.google.com/apikey (pode ser a mesma do PC)
- `Jwt:Secret` — qualquer texto aleatório com 32+ caracteres

## 3. Contexto do Claude Code
- `.claude/session.md` — já é lido automaticamente pelo hook de SessionStart (depois de copiar o settings abaixo).
- Configs globais (copie para `%USERPROFILE%\.claude\`):
  - `.claude/global/CLAUDE.md` → `~/.claude/CLAUDE.md`
  - `.claude/global/settings.json` → `~/.claude/settings.json` (a statusLine aponta pro plugin caveman; se não instalar o plugin, remova esse trecho)
  - `.claude/global/commands/*.md` → `~/.claude/commands/` (`/duvida`, `/save-context`, `/boas-praticas`)
- Memórias: copie `.claude/memory/*.md` para
  `~/.claude/projects/<pasta-do-projeto-com-traços>/memory/`
  (ex.: projeto em `C:\Users\X\TG` → `C--Users-X-TG`). Ou simplesmente peça pro Claude: "leia .claude/memory e instale as memórias".

## 4. Ambiente
- .NET SDK + `dotnet tool install --global dotnet-ef`
- SQL Server LocalDB, depois em `backend/LabPat.Api`: `dotnet ef database update`
- Node + `npm install` em `frontend`
- Backend: `dotnet run --launch-profile https` em `backend/LabPat.Api` → https://localhost:7135
- Frontend: `npm start` em `frontend` → http://localhost:4200
- Login seed: admin@labpat.com / Admin@123
- Monografia: `python gerar_tg_docx.py` gera o .docx a partir de `monografia/` (nunca editar o .docx à mão)
