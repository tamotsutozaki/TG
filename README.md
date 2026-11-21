# LabPat: sistema web para laboratórios de patologia veterinária

Trabalho de Graduação — FATEC Indaiatuba, Análise e Desenvolvimento de Sistemas.
Autor: Pedro Henrique Tamotsu Tozaki · Orientador: Prof. Marlon Palata Fanger Rodrigues.

Plataforma web para gestão do fluxo de trabalho em laboratórios de patologia veterinária,
da solicitação do exame à emissão do laudo. Atende vários laboratórios com dados isolados,
médicos veterinários vinculados e consulta pública do andamento do exame por código.
A solicitação pode ser criada a partir de foto, PDF ou áudio da guia, com extração de dados
pelo Google Gemini.

## Stack

| Parte | Tecnologia |
|---|---|
| Frontend | Angular 18 (Angular Material) |
| Backend | API REST em .NET 10 (C#), arquitetura limpa |
| Banco | SQL Server (LocalDB em desenvolvimento) via Entity Framework Core |
| IA | Google Gemini API (multimodal) |
| Testes | Playwright + pytest (52 passos do roteiro funcional) |

## Estrutura

```
backend/            API .NET (Api, Application, Domain, Infrastructure)
frontend/           aplicação Angular
tests/              testes E2E automatizados (ver tests/LEIA-ME.md)
material-de-teste/  guia fictícia em PNG, PDF e áudio para testar a extração com IA
monografia/         texto da monografia em Markdown (gera o TG_Pedro_Tozaki.docx)
```

## Como rodar localmente (Windows)

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 20 ou superior](https://nodejs.org/)
- SQL Server LocalDB (vem com o Visual Studio ou pelo instalador do SQL Server Express)
- Uma chave da API do Gemini, gratuita em https://aistudio.google.com/apikey
  (só é necessária para criar solicitações por foto, PDF ou áudio)

### 1. Configurar o backend

Copie o arquivo de exemplo e preencha a chave do Gemini e um segredo JWT qualquer
com 32 caracteres ou mais:

```powershell
cd backend\LabPat.Api
copy appsettings.Development.example.json appsettings.Development.json
```

O `appsettings.Development.json` não é versionado, porque guarda segredos.

### 2. Subir a API

```powershell
cd backend\LabPat.Api
dotnet run --launch-profile https
```

A API sobe em `https://localhost:7135`. Na primeira execução, o banco `LabPat` é criado
automaticamente (as migrations rodam no startup) e o usuário administrador é cadastrado.

### 3. Subir o frontend

Em outro terminal:

```powershell
cd frontend
npm install
npm start
```

Acesse http://localhost:4200.

## Como testar o fluxo

1. Entre como administrador: `admin@labpat.com` / `Admin@123`.
2. Em **Convites**, gere um convite de laboratório e abra o link para cadastrar o laboratório.
3. Pela página inicial, cadastre uma conta de médico veterinário e peça vínculo com o laboratório.
4. Como laboratório, aprove o vínculo, cadastre tipos de exame (com modelo de laudo e insumos) e itens de estoque.
5. Como veterinário, crie uma solicitação manualmente ou enviando um arquivo da pasta
   `material-de-teste/` para o preenchimento automático pela IA.
6. Como laboratório, receba a amostra, acompanhe o andamento e emita o laudo em PDF.
7. Na página inicial, sem login, consulte o andamento pelo código da solicitação.

O roteiro completo, com 52 passos, está no Apêndice B da monografia.

## Testes automatizados

```powershell
cd tests
.\rodar.ps1
```

O script sobe um ambiente isolado (banco `LabPat_E2E`, API na porta 5180, front na 4300),
executa os 52 passos do roteiro no navegador e gera o relatório em `tests/relatorio/`.
A extração com IA é simulada, então os testes não precisam da chave do Gemini.
Detalhes em [tests/LEIA-ME.md](tests/LEIA-ME.md).
