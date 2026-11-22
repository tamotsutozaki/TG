"""
Roteiro de testes funcionais do LabPat, automatizado (52 passos em 11 partes).

Cada teste corresponde a um passo do roteiro manual (artifact "Roteiro de testes LabPat")
e deve rodar na ordem em que está escrito: os passos dependem dos dados criados antes.
Perfis em janelas separadas, como no roteiro: admin, laboratório, veterinário e público.
"""
import re
from datetime import datetime, timedelta

import pytest
from playwright.sync_api import expect

from apoio import (aviso, baixar_pdf, botao, botao_icone, campo, confirmar, dialogo, entrar,
                   item, itens_menu, limpar_sessao, linha, menu, preencher, sair, selecionar)
from conftest import ADMIN_EMAIL, ADMIN_SENHA, FRONT_URL, RELATORIO, SENHA

expect.set_options(timeout=15000)

TEMPLATE_HEMOGRAMA = "HEMOGRAMA\nEritrograma: dentro dos valores de referência.\nLeucograma: sem alterações."


def caminho(link: str) -> str:
    return link.replace(FRONT_URL, "")


def hoje() -> str:
    return datetime.now().strftime("%d/%m/%Y")


def abrir_solicitacao(pg, paciente: str):
    menu(pg, "Solicitações")
    linha(pg, paciente).first.click()
    expect(pg).to_have_url(re.compile(r"/solicitacoes/\d+$"))
    expect(pg.locator(".grid-info")).to_contain_text(paciente)


def nova_solicitacao(pg):
    menu(pg, "Solicitações")
    botao(pg, "Nova Solicitação").click()
    expect(pg).to_have_url(re.compile(r"/solicitacoes/nova"))


def secao(pg, titulo: str):
    return pg.locator("mat-card").filter(has=pg.locator("mat-card-title", has_text=re.compile(rf"^\s*{re.escape(titulo)}")))


def preencher_manual(pg, tipo: str, tutor: dict, paciente: dict, vet: dict | None = None):
    selecionar(pg, "Tipo de Exame", tipo)
    if vet:
        uf = vet.pop("UF do CRMV", None)
        preencher(secao(pg, "Médico Veterinário Solicitante"), vet)
        if uf:
            selecionar(secao(pg, "Médico Veterinário Solicitante"), "UF do CRMV", uf)
    preencher(secao(pg, "Tutor"), tutor)
    sexo = paciente.pop("Sexo", None)
    preencher(secao(pg, "Paciente"), paciente)
    if sexo:
        selecionar(secao(pg, "Paciente"), "Sexo", sexo)


def criar_e_ler_codigo(pg) -> str:
    botao(pg, "Criar Solicitação").click()
    expect(pg).to_have_url(re.compile(r"/solicitacoes/\d+$"))
    codigo = pg.locator(".page-header h2 code").inner_text().strip()
    assert codigo
    return codigo


def extrair(pg, metodo: str, arquivo):
    pg.locator("mat-button-toggle").filter(has_text=metodo).click()
    pg.locator("input[type=file]").set_input_files(arquivo)
    botao(pg, "Extrair com IA").click()
    expect(aviso(pg)).to_contain_text("Dados extraídos")
    expect(campo(pg, "Descrição Clínica")).to_have_value(re.compile("Suspeita de anemia"))


def avancar_status(pg, status: str, observacao: str = ""):
    selecionar(pg, "Avançar para", status)
    if observacao:
        campo(pg, "Observação (opcional)").fill(observacao)
    botao(pg, "Atualizar Status").click()
    expect(pg.locator(".page-header .status-chip")).to_have_text(status)


# ═══ Parte 1 — Administrador: convites ═══════════════════════════════════════

def test_1_1_admin_entra(janelas):
    pg = janelas.admin
    pg.goto("/")
    botao(pg, "Entrar").click()
    entrar(pg, ADMIN_EMAIL, ADMIN_SENHA)
    expect(pg).to_have_url(re.compile(r"/admin/convites$"))
    expect(pg.get_by_role("heading", name="Convites de laboratório")).to_be_visible()
    assert itens_menu(pg) == ["Convites", "Laboratórios", "Veterinários", "Meu perfil"]


def gerar_convite(pg, descricao: str) -> str:
    campo(pg, "Para quem é? (opcional)").fill(descricao)
    botao(pg, "Gerar convite").click()
    criado = pg.locator(".convite-criado")
    expect(criado).to_be_visible()
    link = criado.locator("code").inner_text().strip()
    assert "/cadastro-laboratorio?convite=" in link
    return link


def test_1_2_gera_convite_alfa(janelas, estado):
    pg = janelas.admin
    expect(campo(pg, "Validade")).to_contain_text("7 dias")
    estado["convite_alfa"] = gerar_convite(pg, "Laboratório Alfa")
    validade = (datetime.now() + timedelta(days=7)).strftime("%d/%m/%Y")
    expect(pg.locator(".convite-criado .validade")).to_contain_text(f"Válido até {validade}")
    botao(pg.locator(".convite-criado"), "Copiar").click()
    expect(aviso(pg)).to_contain_text("Link copiado")


def test_1_3_gera_convite_beta(janelas, estado):
    pg = janelas.admin
    estado["convite_beta"] = gerar_convite(pg, "Laboratório Beta")
    for nome in ("Laboratório Alfa", "Laboratório Beta"):
        expect(item(pg, nome).locator(".chip")).to_have_text("Ativo")


def test_1_4_revoga_convite(janelas, estado):
    pg = janelas.admin
    estado["convite_revogado"] = gerar_convite(pg, "Teste revogação")
    botao(item(pg, "Teste revogação"), "Revogar").click()
    confirmar(pg, "Revogar")
    expect(item(pg, "Teste revogação").locator(".chip")).to_have_text("Revogado")


def test_1_5_convite_revogado_indisponivel(janelas, estado):
    pg = janelas.publico
    pg.goto(caminho(estado["convite_revogado"]))
    expect(pg.get_by_role("heading", name="Convite indisponível")).to_be_visible()
    expect(pg.get_by_text("Este convite foi revogado.")).to_be_visible()


# ═══ Parte 2 — Cadastro do laboratório pelo convite ══════════════════════════

DADOS_ALFA = {
    "Nome do laboratório": "Laboratório Alfa",
    "Telefone": "(19) 3333-4444",
    "Endereço (rua, número e bairro)": "Rua das Flores, 100, Centro",
    "Cidade": "Indaiatuba",
    "Nome completo": "Dra. Ana Souza",
    "Nº do CRMV": "54321",
    "E-mail": "alfa@lab.com",
}


def test_2_1_abre_convite_alfa(janelas, estado):
    pg = janelas.lab
    pg.goto(caminho(estado["convite_alfa"]))
    expect(pg.locator("mat-card-title")).to_have_text("Cadastro do laboratório")
    expect(pg.locator("mat-card-subtitle")).to_contain_text("Convite: Laboratório Alfa")
    expect(pg.locator("mat-card-subtitle")).to_contain_text("válido até")


def test_2_2_validacoes_do_cadastro(janelas):
    pg = janelas.lab
    preencher(pg, DADOS_ALFA)
    selecionar(pg, "UF", "SP")
    selecionar(pg, "UF do CRMV", "SP")
    # Senha curta e confirmação diferente: erros no próprio formulário
    campo(pg, "Senha").fill("1234567")
    campo(pg, "Confirmar senha").fill("outra-senha")
    campo(pg, "CNPJ").click()
    expect(pg.get_by_text("Mínimo de 8 caracteres")).to_be_visible()
    expect(pg.get_by_text("As senhas não conferem.")).to_be_visible()
    # CNPJ com dígito verificador errado: recusado pela API ao enviar
    campo(pg, "CNPJ").fill("11.222.333/0001-00")
    campo(pg, "Senha").fill(SENHA)
    campo(pg, "Confirmar senha").fill(SENHA)
    botao(pg, "Cadastrar laboratório").click()
    expect(pg.locator(".erro-msg")).to_have_text("CNPJ inválido.")


def test_2_3_cadastra_alfa(janelas):
    pg = janelas.lab
    campo(pg, "CNPJ").fill("11.222.333/0001-81")
    botao(pg, "Cadastrar laboratório").click()
    expect(pg.get_by_role("heading", name="Laboratório cadastrado!")).to_be_visible()


def test_2_4_convite_ja_utilizado(janelas, estado):
    pg = janelas.lab
    pg.goto(caminho(estado["convite_alfa"]))
    expect(pg.get_by_text("Este convite já foi utilizado.")).to_be_visible()


def test_2_5_admin_ve_convite_usado_e_laboratorio(janelas):
    pg = janelas.admin
    pg.reload()
    alfa = item(pg, "Usado por Laboratório Alfa")
    expect(alfa.locator(".chip")).to_have_text("Usado")
    menu(pg, "Laboratórios")
    lab = item(pg, "Laboratório Alfa")
    expect(lab).to_contain_text("CNPJ 11.222.333/0001-81")
    expect(lab.locator(".chip")).to_have_text("Ativo")


# ═══ Parte 3 — Laboratório: configuração ═════════════════════════════════════

def test_3_1_laboratorio_entra(janelas):
    pg = janelas.lab
    pg.goto("/login")
    entrar(pg, "alfa@lab.com", SENHA)
    expect(pg).to_have_url(re.compile(r"/dashboard$"))
    assert itens_menu(pg) == ["Dashboard", "Solicitações", "Tipos de Exame", "Estoque", "Veterinários", "Meu perfil"]
    expect(pg.locator(".sidenav-footer .user-name")).to_have_text("Laboratório Alfa")
    expect(pg.locator(".resumo-card.total .resumo-num")).to_have_text("0")


def test_3_2_perfil_do_laboratorio(janelas, estado):
    pg = janelas.lab
    menu(pg, "Meu perfil")
    leitura = pg.locator(".somente-leitura").first
    expect(leitura).to_contain_text("11.222.333/0001-81")
    expect(leitura).to_contain_text("alfa@lab.com")
    expect(pg.locator(".somente-leitura input")).to_have_count(0)
    campo(pg, "Telefone").fill("(19) 3333-5555")
    botao(pg, "Salvar alterações").click()
    expect(aviso(pg)).to_contain_text("Dados do laboratório atualizados.")
    estado["link_vinculo"] = pg.locator(".link-box code").inner_text().strip()
    assert "/vincular/" in estado["link_vinculo"]
    botao(pg.locator(".link-box"), "Copiar").click()
    expect(aviso(pg)).to_contain_text("Link copiado")


def novo_insumo(pg, nome: str, unidade: str, minimo: int, quantidade: int):
    botao(pg, "Novo Insumo").click()
    preencher(dialogo(pg), {"Nome": nome, "Unidade de Medida": unidade, "Quantidade Mínima (alerta)": minimo})
    botao(dialogo(pg), "Salvar").click()
    expect(linha(pg, nome)).to_be_visible()
    botao_icone(linha(pg, nome), "tune").click()
    campo(dialogo(pg), "Nova Quantidade").fill(str(quantidade))
    botao(dialogo(pg), "Salvar").click()
    expect(linha(pg, nome)).to_contain_text(str(quantidade))


def test_3_3_cadastra_insumos(janelas):
    pg = janelas.lab
    menu(pg, "Estoque")
    novo_insumo(pg, "Tubo EDTA", "un", 5, 10)
    novo_insumo(pg, "Lâmina", "un", 2, 20)
    for nome in ("Tubo EDTA", "Lâmina"):
        expect(linha(pg, nome).locator(".status-chip")).to_have_text("Normal")


def test_3_4_cadastra_tipos_de_exame(janelas):
    pg = janelas.lab
    menu(pg, "Tipos de Exame")
    for nome, prazo in (("Hemograma", 3), ("Citologia", 5)):
        pg.get_by_role("button", name=re.compile(r"Novo Tipo|Cadastrar primeiro tipo")).first.click()
        preencher(dialogo(pg), {"Nome": nome, "Prazo Estimado (dias)": prazo})
        botao(dialogo(pg), "Salvar").click()
        expect(linha(pg, nome)).to_contain_text(str(prazo))
        expect(linha(pg, nome).locator(".status-chip")).to_have_text("Ativo")


def test_3_5_template_do_hemograma(janelas):
    pg = janelas.lab
    botao_icone(linha(pg, "Hemograma"), "description").click()
    campo(dialogo(pg), "Conteúdo").fill(TEMPLATE_HEMOGRAMA)
    botao(dialogo(pg), "Salvar nova versão").click()
    expect(dialogo(pg).locator(".template-box .versao")).to_have_text("Versão 1")
    botao(dialogo(pg), "Fechar").click()
    expect(dialogo(pg)).to_have_count(0)


def test_3_6_vincula_insumo_ao_hemograma(janelas):
    pg = janelas.lab
    botao_icone(linha(pg, "Hemograma"), "inventory_2").click()
    selecionar(dialogo(pg), "Insumo", "Tubo EDTA (un)")
    campo(dialogo(pg), "Qtd. por exame").fill("1")
    botao(dialogo(pg), "Vincular").click()
    vinculo = dialogo(pg).locator(".vinculo")
    expect(vinculo).to_contain_text("Tubo EDTA")
    expect(vinculo).to_contain_text("1 un por exame")
    botao(dialogo(pg), "Fechar").click()


# ═══ Parte 4 — Veterinário: conta e vínculo ══════════════════════════════════

def test_4_1_abre_link_de_vinculo(janelas, estado):
    pg = janelas.vet
    pg.goto(caminho(estado["link_vinculo"]))
    expect(pg.get_by_role("heading", name="Laboratório Alfa")).to_be_visible()
    expect(pg.locator(".local")).to_have_text("Indaiatuba/SP")
    expect(pg.get_by_role("link", name="Entrar")).to_be_visible()
    expect(pg.get_by_role("link", name="Criar conta")).to_be_visible()


def test_4_2_cria_conta_de_veterinario(janelas):
    pg = janelas.vet
    pg.get_by_role("link", name="Criar conta").click()
    expect(pg).to_have_url(re.compile(r"/registro\?returnUrl="))
    preencher(pg, {"Nome completo": "Dr. Bruno Lima", "E-mail": "bruno@vet.com", "Senha": SENHA, "Nº do CRMV": "12345"})
    selecionar(pg, "UF", "SP")
    botao(pg, "Criar conta").click()
    expect(pg.get_by_role("heading", name="Conta criada!")).to_be_visible()


def test_4_3_entra_e_pede_vinculo(janelas, estado):
    pg = janelas.vet
    pg.get_by_role("link", name="Ir para o login").click()
    expect(pg).to_have_url(re.compile(r"/login\?returnUrl=%2Fvincular"))
    entrar(pg, "bruno@vet.com", SENHA)
    expect(pg).to_have_url(re.compile(re.escape(caminho(estado["link_vinculo"])) + "$"))
    botao(pg, "Pedir vínculo").click()
    expect(pg.get_by_text("Pedido enviado. Aguarde a aprovação do laboratório.")).to_be_visible()


def test_4_4_crmv_duplicado(janelas):
    pg = janelas.vet
    pg.goto("/registro")
    preencher(pg, {"Nome completo": "Outro Vet", "E-mail": "outro@vet.com", "Senha": SENHA, "Nº do CRMV": "12345"})
    selecionar(pg, "UF", "SP")
    botao(pg, "Criar conta").click()
    expect(pg.locator(".erro")).to_have_text("Já existe uma conta de veterinário com este CRMV.")


def test_4_5_sem_vinculo_aprovado(janelas):
    pg = janelas.vet
    pg.goto("/solicitacoes")
    botao(pg, "Nova Solicitação").click()
    expect(pg.get_by_text("você precisa de vínculo aprovado")).to_be_visible()
    pg.goto("/laboratorios")
    vinculo = item(pg.locator("mat-card").first, "Laboratório Alfa")
    expect(vinculo.locator(".chip")).to_have_text("Pendente")


def test_4_6_laboratorio_aprova_vinculo(janelas):
    pg = janelas.lab
    menu(pg, "Veterinários")
    botao(item(pg, "Dr. Bruno Lima", ".vet-item.pendente"), "Aprovar").click()
    vinculados = secao(pg, "Veterinários vinculados")
    expect(vinculados).to_contain_text("Dr. Bruno Lima")
    expect(vinculados).to_contain_text("CRMV-SP 12345")


def test_4_7_veterinario_vinculado(janelas):
    pg = janelas.vet
    pg.reload()
    vinculo = item(pg.locator("mat-card").first, "Laboratório Alfa")
    expect(vinculo.locator(".chip")).to_have_text("Vinculado")
    expect(vinculo.get_by_role("link", name="Nova solicitação")).to_be_visible()


# ═══ Parte 5 — Solicitações, laudo e consulta pública ════════════════════════

def test_5_1_solicitacao_manual_do_veterinario(janelas, estado):
    pg = janelas.vet
    nova_solicitacao(pg)
    expect(campo(pg, "Laboratório")).to_contain_text("Laboratório Alfa")
    preencher_manual(pg, "Hemograma",
                     tutor={"Nome": "Carla Dias", "Telefone": "(19) 99999-1111"},
                     paciente={"Nome": "Mel", "Espécie": "Cão", "Anos": 3, "Peso (kg)": 12, "Sexo": "Fêmea"})
    estado["codigo_mel"] = criar_e_ler_codigo(pg)
    expect(pg.locator(".page-header .status-chip")).to_have_text("Solicitado")
    expect(pg.locator(".grid-info")).to_contain_text("Laboratório Alfa")


def test_5_2_solicitacao_por_foto_da_guia(janelas, estado, arquivos):
    pg = janelas.vet
    nova_solicitacao(pg)
    antes = len(janelas.chamadas_gemini)
    extrair(pg, "Foto da guia", arquivos["guia-exemplo.png"])
    assert len(janelas.chamadas_gemini) == antes + 1
    assert janelas.chamadas_gemini[-1].startswith("multipart/form-data")
    expect(campo(pg, "Tipo de Exame")).to_contain_text("Hemograma")
    tutor, paciente = secao(pg, "Tutor"), secao(pg, "Paciente")
    expect(campo(tutor, "Nome")).to_have_value("Mariana Lopes")
    expect(campo(paciente, "Nome")).to_have_value("Thor")
    expect(campo(paciente, "Espécie")).to_have_value("Cão")
    expect(campo(paciente, "Raça")).to_have_value("Labrador")
    expect(campo(paciente, "Sexo")).to_contain_text("Macho")
    expect(campo(paciente, "Anos")).to_have_value("7")
    expect(campo(paciente, "Peso (kg)")).to_have_value("32.5")
    estado["codigo_thor"] = criar_e_ler_codigo(pg)
    expect(pg.locator(".grid-info")).to_contain_text("Foto da guia")


@pytest.mark.parametrize("metodo,arquivo", [("PDF", "guia-exemplo.pdf"), ("Áudio", "pedido-audio.wav")])
def test_5_3_extracao_por_pdf_e_audio(janelas, arquivos, metodo, arquivo):
    pg = janelas.vet
    nova_solicitacao(pg)
    extrair(pg, metodo, arquivos[arquivo])
    expect(campo(secao(pg, "Paciente"), "Nome")).to_have_value("Thor")
    expect(campo(secao(pg, "Tutor"), "Nome")).to_have_value("Mariana Lopes")
    expect(campo(pg, "Tipo de Exame")).to_contain_text("Hemograma")


def test_5_4_listas_e_extracao_pelo_laboratorio(janelas, arquivos):
    vet = janelas.vet
    menu(vet, "Solicitações")
    expect(vet.locator("tr.mat-mdc-row")).to_have_count(2)
    for paciente in ("Mel", "Thor"):
        expect(linha(vet, paciente)).to_contain_text("Laboratório Alfa")
    lab = janelas.lab
    nova_solicitacao(lab)
    extrair(lab, "Foto da guia", arquivos["guia-exemplo.png"])
    vet_guia = secao(lab, "Médico Veterinário Solicitante")
    expect(campo(vet_guia, "Nome")).to_have_value("Dr. Rafael Moura")
    expect(campo(vet_guia, "Nº do CRMV")).to_have_value("23456")
    expect(campo(vet_guia, "UF do CRMV")).to_contain_text("SP")
    criar_e_ler_codigo(lab)


def test_5_5_solicitacoes_pelo_crmv(janelas):
    lab = janelas.lab
    tutor = {"Nome": "Paulo Reis", "Telefone": "(19) 98888-2222"}
    nova_solicitacao(lab)
    preencher_manual(lab, "Citologia", tutor=dict(tutor), paciente={"Nome": "Bidu", "Espécie": "Gato"},
                     vet={"Nome": "Dr. Bruno Lima", "Nº do CRMV": "12345", "UF do CRMV": "SP"})
    criar_e_ler_codigo(lab)
    nova_solicitacao(lab)
    preencher_manual(lab, "Citologia", tutor=dict(tutor), paciente={"Nome": "Nina", "Espécie": "Gato"},
                     vet={"Nome": "Dra. Outra Pessoa", "Nº do CRMV": "99999", "UF do CRMV": "SP"})
    criar_e_ler_codigo(lab)
    vet = janelas.vet
    vet.reload()
    expect(linha(vet, "Bidu")).to_be_visible()
    expect(linha(vet, "Nina")).to_have_count(0)


def test_5_6_avanca_status(janelas):
    pg = janelas.lab
    abrir_solicitacao(pg, "Mel")
    avancar_status(pg, "Aguardando Amostra", "Amostra de sangue solicitada ao veterinário")
    expect(aviso(pg)).to_contain_text("Status atualizado.")
    avancar_status(pg, "Amostra Recebida")
    # Regra de status: só oferece etapas seguintes, e Concluído só é atingido pela emissão do laudo
    campo(pg, "Avançar para").press("Enter")
    expect(pg.locator(".cdk-overlay-pane mat-option")).to_have_text(["Em Análise"])
    pg.keyboard.press("Escape")
    expect(pg.locator(".cdk-overlay-pane mat-option")).to_have_count(0)
    avancar_status(pg, "Em Análise")
    expect(pg.locator(".status-step.ativo")).to_have_count(4)
    historico = pg.locator(".historico-item")
    expect(historico).to_have_count(3)
    expect(historico.first).to_contain_text(hoje())
    expect(historico.first).to_contain_text("Amostra de sangue solicitada")


def test_5_7_emite_laudo_com_template(janelas):
    pg = janelas.lab
    botao(pg, "Emitir Laudo").click()
    texto = campo(pg, "Conteúdo do Laudo")
    expect(texto).to_have_value(TEMPLATE_HEMOGRAMA)
    expect(pg.get_by_text("Modelo de laudo carregado")).to_be_visible()
    texto.fill(TEMPLATE_HEMOGRAMA + "\nConclusão: hemograma sem alterações dignas de nota.")
    botao(pg, "Emitir e Concluir").click()
    expect(pg.locator(".page-header .status-chip")).to_have_text("Concluído")
    expect(pg.locator(".laudo-emitido")).to_contain_text("Dra. Ana Souza")
    expect(pg.locator(".laudo-emitido")).to_contain_text("CRMV-SP 54321")


def test_5_8_pdf_do_laudo(janelas, estado):
    texto = baixar_pdf(janelas.lab, RELATORIO, "laudo-mel.pdf")
    estado["pdf_mel"] = texto
    for esperado in ("Laboratório Alfa", "11.222.333/0001-81", "Rua das Flores", "(19) 3333-5555", "alfa@lab.com",
                     "Mel", "Fêmea", "Dr. Bruno Lima", "sem alterações dignas de nota", "Dra. Ana Souza", "54321",
                     "Brasília"):
        assert esperado in texto, f"'{esperado}' não aparece no PDF do laudo"


def test_5_9_estoque_descontado(janelas):
    pg = janelas.lab
    menu(pg, "Estoque")
    expect(linha(pg, "Tubo EDTA").locator("td").nth(2)).to_have_text("9")


def test_5_10_veterinario_ve_laudo(janelas):
    pg = janelas.vet
    abrir_solicitacao(pg, "Mel")
    expect(pg.locator(".laudo-emitido")).to_contain_text("sem alterações dignas de nota")
    texto = baixar_pdf(pg, RELATORIO, "laudo-mel-vet.pdf")
    assert "Mel" in texto


def test_5_11_consulta_publica(janelas, estado):
    pg = janelas.publico
    pg.goto("/")
    campo(pg, "Código do exame").fill(estado["codigo_mel"])
    botao(pg, "Consultar").click()
    dados = pg.locator(".info-grid")
    expect(dados).to_contain_text("Concluído")
    for esperado in ("Laboratório Alfa", "Hemograma", "Mel", hoje()):
        expect(dados).to_contain_text(esperado)
    expect(pg.locator("body")).not_to_contain_text("Carla Dias")
    expect(pg.locator("body")).not_to_contain_text("99999-1111")


def test_5_12_dashboard(janelas):
    pg = janelas.lab
    menu(pg, "Dashboard")
    expect(pg.locator(".resumo-card.total .resumo-num")).to_have_text("5")
    expect(pg.locator(".resumo-card.andamento .resumo-num")).to_have_text("4")
    expect(pg.locator(".resumo-card.concluido .resumo-num")).to_have_text("1")
    menu(pg, "Estoque")
    botao_icone(linha(pg, "Tubo EDTA"), "tune").click()
    campo(dialogo(pg), "Nova Quantidade").fill("3")
    botao(dialogo(pg), "Salvar").click()
    expect(linha(pg, "Tubo EDTA").locator(".status-chip")).to_have_text("Estoque Baixo")
    menu(pg, "Dashboard")
    expect(secao(pg, "Estoque baixo")).to_contain_text("Tubo EDTA")


# ═══ Parte 6 — Dois laboratórios ═════════════════════════════════════════════

def test_6_1_cadastra_beta_com_cnpj_alfanumerico(janelas, estado):
    pg = janelas.lab
    sair(pg)
    pg.goto(caminho(estado["convite_beta"]))
    preencher(pg, DADOS_ALFA | {"Nome do laboratório": "Laboratório Beta", "Cidade": "Campinas",
                                "Nome completo": "Dr. Marcos Beta", "Nº do CRMV": "11111", "E-mail": "beta@lab.com",
                                "CNPJ": "12.ABC.345/01DE-35", "Senha": SENHA, "Confirmar senha": SENHA})
    selecionar(pg, "UF", "SP")
    selecionar(pg, "UF do CRMV", "SP")
    botao(pg, "Cadastrar laboratório").click()
    expect(pg.get_by_role("heading", name="Laboratório cadastrado!")).to_be_visible()


def test_6_2_beta_isolado_do_alfa(janelas):
    pg = janelas.lab
    pg.goto("/login")
    entrar(pg, "beta@lab.com", SENHA)
    menu(pg, "Solicitações")
    expect(pg.get_by_text("Nenhuma solicitação cadastrada.")).to_be_visible()
    menu(pg, "Tipos de Exame")
    expect(pg.get_by_text("Nenhum tipo de exame cadastrado.")).to_be_visible()
    menu(pg, "Estoque")
    expect(pg.get_by_text("Nenhum insumo cadastrado.")).to_be_visible()
    menu(pg, "Veterinários")
    expect(pg.get_by_text("Nenhum pedido pendente.")).to_be_visible()
    expect(pg.get_by_text("Nenhum veterinário vinculado ainda.")).to_be_visible()


def test_6_3_veterinario_busca_por_cidade(janelas):
    pg = janelas.vet
    menu(pg, "Meus Laboratórios")
    campo(pg, "Nome ou cidade").fill("Campinas")
    botao(pg, "Buscar").click()
    busca = secao(pg, "Encontrar laboratório")
    expect(busca.locator(".item")).to_have_count(1)
    botao(item(busca, "Laboratório Beta"), "Pedir vínculo").click()
    expect(item(busca, "Laboratório Beta").locator(".chip")).to_have_text("Pendente")
    expect(item(secao(pg, "Vínculos"), "Laboratório Beta").locator(".chip")).to_have_text("Pendente")


def test_6_4_beta_recusa_vinculo(janelas):
    lab = janelas.lab
    lab.reload()
    botao(item(lab, "Dr. Bruno Lima", ".vet-item.pendente"), "Recusar").click()
    expect(item(secao(lab, "Recusados e desfeitos"), "Dr. Bruno Lima", ".vet-item").locator(".chip")).to_have_text("Recusado")
    vet = janelas.vet
    vet.reload()
    beta = item(secao(vet, "Vínculos"), "Laboratório Beta")
    expect(beta.locator(".chip")).to_have_text("Recusado")
    expect(botao(beta, "Pedir de novo")).to_be_visible()
    nova_solicitacao(vet)
    campo(vet, "Laboratório").click()
    expect(vet.locator("mat-option")).to_have_count(1)
    expect(vet.locator("mat-option")).to_contain_text("Laboratório Alfa")
    vet.keyboard.press("Escape")


# ═══ Parte 7 — Desfazer vínculo ══════════════════════════════════════════════

def test_7_1_alfa_desfaz_vinculo(janelas):
    pg = janelas.lab
    sair(pg)
    entrar(pg, "alfa@lab.com", SENHA)
    menu(pg, "Veterinários")
    botao(item(secao(pg, "Veterinários vinculados"), "Dr. Bruno Lima", ".vet-item"), "Desfazer").click()
    confirmar(pg, "Desfazer vínculo")
    expect(item(secao(pg, "Recusados e desfeitos"), "Dr. Bruno Lima", ".vet-item").locator(".chip")).to_have_text("Desfeito")


def test_7_2_veterinario_apos_desfazer(janelas):
    pg = janelas.vet
    pg.goto("/laboratorios")
    expect(item(secao(pg, "Vínculos"), "Laboratório Alfa").locator(".chip")).to_have_text("Desfeito")
    menu(pg, "Solicitações")
    expect(linha(pg, "Mel")).to_be_visible()
    expect(linha(pg, "Thor")).to_be_visible()
    expect(linha(pg, "Bidu")).to_have_count(0)
    botao(pg, "Nova Solicitação").click()
    expect(pg.get_by_text("você precisa de vínculo aprovado")).to_be_visible()


# ═══ Parte 8 — Perfil e senha ════════════════════════════════════════════════

def test_8_1_veterinario_edita_perfil(janelas):
    pg = janelas.vet
    menu(pg, "Meu perfil")
    leitura = pg.locator(".somente-leitura")
    expect(leitura).to_contain_text("CRMV-SP 12345")
    expect(leitura).to_contain_text("bruno@vet.com")
    preencher(pg, {"Nome completo": "Dr. Bruno Lima Silva", "Telefone": "(19) 97777-3333"})
    botao(pg, "Salvar alterações").click()
    expect(aviso(pg)).to_contain_text("Seus dados foram atualizados.")
    expect(pg.locator(".sidenav-footer .user-name")).to_have_text("Dr. Bruno Lima Silva")


def trocar_senha(pg, atual: str, nova: str):
    preencher(pg, {"Senha atual": atual, "Nova senha": nova, "Confirmar nova senha": nova})
    botao(pg, "Alterar senha").click()


def test_8_2_veterinario_troca_senha(janelas):
    pg = janelas.vet
    trocar_senha(pg, "senha-errada", "NovaSenha@456")
    expect(aviso(pg)).to_contain_text("A senha atual está incorreta.")
    trocar_senha(pg, SENHA, "NovaSenha@456")
    expect(aviso(pg)).to_contain_text("Senha alterada.")
    sair(pg)
    entrar(pg, "bruno@vet.com", SENHA, sucesso=False)
    expect(pg.locator(".erro-msg")).to_be_visible()
    entrar(pg, "bruno@vet.com", "NovaSenha@456")
    expect(pg).to_have_url(re.compile(r"/solicitacoes$"))


def test_8_3_troca_de_responsavel_tecnico(janelas, estado):
    pg = janelas.lab
    menu(pg, "Meu perfil")
    responsavel = secao(pg, "Responsável técnico")
    preencher(responsavel, {"Nome completo": "Dr. Carlos Mendes", "Nº do CRMV": "67890"})
    selecionar(responsavel, "UF do CRMV", "MG")
    botao(pg, "Salvar alterações").click()
    expect(aviso(pg)).to_contain_text("Dados do laboratório atualizados.")
    abrir_solicitacao(pg, "Thor")
    avancar_status(pg, "Em Análise")
    botao(pg, "Emitir Laudo").click()
    campo(pg, "Conteúdo do Laudo").fill(TEMPLATE_HEMOGRAMA + "\nConclusão: anemia leve.")
    botao(pg, "Emitir e Concluir").click()
    expect(pg.locator(".laudo-emitido")).to_contain_text("Dr. Carlos Mendes")
    texto_thor = baixar_pdf(pg, RELATORIO, "laudo-thor.pdf")
    assert "Dr. Carlos Mendes" in texto_thor and "67890" in texto_thor
    abrir_solicitacao(pg, "Mel")
    texto_mel = baixar_pdf(pg, RELATORIO, "laudo-mel-depois.pdf")
    assert "Dra. Ana Souza" in texto_mel and "Carlos Mendes" not in texto_mel


# ═══ Parte 9 — Administrador: desativar contas ═══════════════════════════════

def alternar_conta(pg, nome: str, acao: str):
    botao(item(pg, nome), acao).click()
    confirmar(pg, acao)
    esperado = "Inativo" if acao == "Desativar" else "Ativo"
    expect(item(pg, nome).locator(".chip")).to_have_text(esperado)


def sessao_encerrada(pg, email: str, senha: str):
    """Ao usar o menu, a sessão cai; o login seguinte avisa que a conta está desativada."""
    menu(pg, "Solicitações")
    expect(pg).to_have_url(re.compile(r"/login"))
    expect(aviso(pg)).to_contain_text("Sua sessão foi encerrada")
    entrar(pg, email, senha, sucesso=False)
    expect(pg.locator(".erro-msg")).to_contain_text("desativada")


def test_9_1_desativa_laboratorio(janelas):
    lab = janelas.lab
    sair(lab)
    entrar(lab, "beta@lab.com", SENHA)
    admin = janelas.admin
    admin.goto("/admin/laboratorios")
    alternar_conta(admin, "Laboratório Beta", "Desativar")
    sessao_encerrada(lab, "beta@lab.com", SENHA)
    vet = janelas.vet
    vet.goto("/laboratorios")
    campo(vet, "Nome ou cidade").fill("Campinas")
    botao(vet, "Buscar").click()
    expect(vet.get_by_text("Nenhum laboratório encontrado.")).to_be_visible()


def test_9_2_reativa_laboratorio(janelas):
    alternar_conta(janelas.admin, "Laboratório Beta", "Reativar")
    lab = janelas.lab
    entrar(lab, "beta@lab.com", SENHA)
    expect(lab).to_have_url(re.compile(r"/dashboard$"))


def test_9_3_desativa_e_reativa_veterinario(janelas):
    admin = janelas.admin
    menu(admin, "Veterinários")
    alternar_conta(admin, "Dr. Bruno Lima Silva", "Desativar")
    vet = janelas.vet
    sessao_encerrada(vet, "bruno@vet.com", "NovaSenha@456")
    alternar_conta(admin, "Dr. Bruno Lima Silva", "Reativar")
    entrar(vet, "bruno@vet.com", "NovaSenha@456")
    expect(vet).to_have_url(re.compile(r"/solicitacoes$"))


def test_9_4_admin_troca_senha(janelas):
    pg = janelas.admin
    menu(pg, "Meu perfil")
    trocar_senha(pg, ADMIN_SENHA, "NovoAdmin@789")
    expect(aviso(pg)).to_contain_text("Senha alterada.")
    sair(pg)
    entrar(pg, ADMIN_EMAIL, "NovoAdmin@789")
    expect(pg).to_have_url(re.compile(r"/admin/convites$"))


# ═══ Parte 10 — Permissões e navegação ═══════════════════════════════════════

def test_10_1_veterinario_nao_abre_area_do_admin_nem_do_laboratorio(janelas):
    pg = janelas.vet
    for rota in ("/admin/convites", "/dashboard"):
        pg.goto(rota)
        expect(pg).to_have_url(re.compile(r"/solicitacoes$"))


def test_10_2_laboratorio_nao_abre_area_do_veterinario(janelas):
    pg = janelas.lab
    pg.goto("/laboratorios")
    expect(pg).to_have_url(re.compile(r"/dashboard$"))


def test_10_3_volta_para_a_pagina_pedida_depois_do_login(janelas):
    pg = janelas.vet
    sair(pg)
    pg.goto("/solicitacoes")
    expect(pg).to_have_url(re.compile(r"/login\?returnUrl=%2Fsolicitacoes"))
    entrar(pg, "bruno@vet.com", "NovaSenha@456")
    expect(pg).to_have_url(re.compile(r"/solicitacoes$"))


# ═══ Parte 11 — Celular ══════════════════════════════════════════════════════

def test_11_1_telas_no_celular(janelas):
    contexto = janelas.browser.new_context(base_url=FRONT_URL, viewport={"width": 390, "height": 844},
                                           is_mobile=True, has_touch=True, device_scale_factor=3, locale="pt-BR")
    pg = contexto.new_page()
    try:
        limpar_sessao(pg)
        pg.goto("/login")
        entrar(pg, "alfa@lab.com", SENHA)
        abrir_menu = pg.get_by_role("button", name="Abrir menu")
        expect(abrir_menu).to_be_visible()
        pasta = RELATORIO / "celular"
        pasta.mkdir(parents=True, exist_ok=True)
        for nome, titulo in (("Dashboard", "Dashboard"), ("Solicitações", "Solicitações"),
                             ("Tipos de Exame", "Tipos de Exame"), ("Estoque", "Estoque de Insumos"),
                             ("Veterinários", "Veterinários"), ("Meu perfil", "Meu perfil")):
            abrir_menu.click()
            menu(pg, nome)
            expect(pg.get_by_role("heading", name=titulo, exact=True)).to_be_visible()
            expect(pg.locator("mat-sidenav")).not_to_be_in_viewport()
            largura = pg.evaluate("[document.documentElement.scrollWidth, window.innerWidth]")
            assert largura[0] <= largura[1], f"{nome}: a página rola na horizontal ({largura[0]} > {largura[1]} px)"
            pg.screenshot(path=str(pasta / f"{nome}.png"))
    finally:
        contexto.close()
