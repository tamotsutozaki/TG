# Contexto do projeto — TG

## Visão geral

Sistema web responsivo para gestão do fluxo completo de trabalho de laboratórios de patologia veterinária: da solicitação do exame até a emissão e disponibilização do laudo. Inclui entrada multimodal de dados (foto, PDF, áudio) via Google Gemini API.

**Aluno:** Pedro Henrique Tamotsu Tozaki  
**Curso:** Tecnologia em Análise e Desenvolvimento de Sistemas — FATEC Indaiatuba  

---

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Frontend | Angular |
| Backend | .NET / C# — API REST |
| Banco de dados | SQL Server (Azure SQL Database) |
| IA multimodal | Google Gemini API |
| Hospedagem | Azure (Azure for Students) |

---

## Atores

- **Patologista** — usuário interno autenticado. Gerencia o sistema inteiro: exames, amostras, laudos, estoque.
- **Médico veterinário solicitante** — cria solicitações (manual, foto, PDF ou áudio) e consulta status.
- **Tutor do animal** — consulta status do exame via código público.

---

## Fluxo de status do exame

```
Solicitado → Aguardando Amostra → Amostra Recebida → Em Análise → Concluído
```

---

## Fases de desenvolvimento (TG — M7 a M12)

### Fase 1 — Levantamento de requisitos (M7)
- Reunião detalhada com a colaboradora patologista veterinária
- Mapeamento dos campos obrigatórios por tipo de exame
- Validação do fluxo de status proposto
- Definição dos templates de laudo por tipo de exame

### Fase 2 — Modelagem e prototipação (M7–M8)
- Modelagem do banco de dados (diagrama ER)
- Prototipação das telas principais (Figma ou similar)
- Definição dos endpoints da API REST
- Setup dos projetos Angular e .NET, configuração do Azure SQL

### Fase 3 — Módulo de cadastros e estoque (M8–M9)
- CRUD de tipos de exame
- CRUD de templates de laudo por tipo de exame
- Módulo de controle de estoque (insumos vinculados a tipos de exame)
- Autenticação do patologista (login)

### Fase 4 — Fluxo de solicitação e consulta pública (M9–M10)
- Formulário de solicitação manual (pelo solicitante ou pelo patologista)
- Gestão de solicitações pelo patologista (atualização de status, recebimento de amostra)
- Página pública de consulta por código único (sem login, dados mínimos por LGPD)
- Cadastro de paciente (animal), tutor e médico vet solicitante

### Fase 5 — Integração Gemini multimodal (M10–M11)
- Upload de imagem (foto de guia), PDF e áudio pelo solicitante
- Chamada à API Gemini no backend com o arquivo recebido
- Extração dos dados estruturados e preenchimento automático do formulário de solicitação
- Tratamento de erros e fallback para preenchimento manual

### Fase 6 — Editor de laudos e exportação PDF (M10–M11)
- Editor de laudo com template carregado por tipo de exame
- Preenchimento pelo patologista e assinatura digital (ou equivalente)
- Exportação do laudo em PDF
- Disponibilização do PDF ao solicitante (download ou link)

### Fase 7 — Testes, avaliação e entrega (M11–M12)
- Testes funcionais end-to-end
- Validação final com a colaboradora patologista
- Aplicação do formulário Google Forms a patologistas e médicos vet solicitantes (escala Likert)
- Análise dos resultados
- Redação da conclusão do TG
- Entrega e apresentação

---

## Decisões técnicas relevantes

- A integração com o Gemini é feita **exclusivamente no backend** — o frontend envia o arquivo, a API .NET chama o Gemini e retorna os dados extraídos.
- A consulta pública por código expõe apenas dados mínimos (status, tipo de exame, data estimada) — sem dados sensíveis de paciente ou tutor, em conformidade com a LGPD.
- SQL Server foi escolhido por coesão com o stack .NET e hospedado no Azure SQL para viabilizar hospedagem gratuita via Azure for Students.
- Angular foi escolhido no lugar de Next.js por maior coesão com o ecossistema .NET/Microsoft.

---

## Critérios de avaliação (do PTG)

1. Redução percebida do esforço operacional do patologista
2. Padronização das informações coletadas nas solicitações
3. Transparência percebida pelos solicitantes e tutores
4. Qualidade da extração de dados pelo Gemini (foto, PDF, áudio)
