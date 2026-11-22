"""
Cria os dados de demonstração no sistema publicado e confere, no ambiente real,
o que a suíte E2E não cobre: a extração com o Gemini de verdade e o PDF gerado no Linux.

Uso (da raiz do repositório):
    tests\\.venv\\Scripts\\python tests\\demo_producao.py

Lê o endereço e as credenciais de .azure-segredos.txt (fora do git) e grava lá as
senhas das contas de demonstração. Rodar uma única vez em um banco recém-criado.
"""
import re
import secrets
import sys
import time
from pathlib import Path

from playwright.sync_api import expect, sync_playwright

sys.path.insert(0, str(Path(__file__).resolve().parent))
from apoio import (aviso, baixar_pdf, botao, botao_icone, campo, dialogo, entrar, item, linha,  # noqa: E402
                   menu, preencher, selecionar)

RAIZ = Path(__file__).resolve().parent.parent
SEGREDOS = RAIZ / ".azure-segredos.txt"
MATERIAL = RAIZ / "material-de-teste"
SAIDA = Path(__file__).resolve().parent / "relatorio" / "producao"

expect.set_options(timeout=60000)  # F1 gratuito e banco que pausa: o primeiro acesso demora


def ler_segredos() -> dict:
    return dict(l.strip().split("=", 1) for l in SEGREDOS.read_text(encoding="utf8").splitlines() if "=" in l)


def nova_senha() -> str:
    return "Demo" + secrets.token_urlsafe(8).replace("-", "x").replace("_", "y") + "5!"


def passo(texto: str):
    print(f"[{time.strftime('%H:%M:%S')}] {texto}", flush=True)


def secao(pg, titulo: str):
    return pg.locator("mat-card").filter(
        has=pg.locator("mat-card-title", has_text=re.compile(rf"^\s*{re.escape(titulo)}")))


def main():
    seg = ler_segredos()
    front = seg["FRONT_URL"]
    # --continuar: laboratório e conta do veterinário já criados; segue a partir do pedido de vínculo
    continuar = "--continuar" in sys.argv
    if "DEMO_LAB_PASS" in seg and not continuar:
        sys.exit("Os dados de demonstração já foram criados (DEMO_LAB_PASS existe em .azure-segredos.txt).")
    lab_email, vet_email = "laboratorio.demo@example.com", "veterinario.demo@example.com"
    if continuar:
        lab_senha, vet_senha = seg["DEMO_LAB_PASS"], seg["DEMO_VET_PASS"]
    else:
        lab_senha, vet_senha = nova_senha(), nova_senha()
        with SEGREDOS.open("a", encoding="utf8") as f:
            f.write(f"DEMO_LAB_EMAIL={lab_email}\nDEMO_LAB_PASS={lab_senha}\n"
                    f"DEMO_VET_EMAIL={vet_email}\nDEMO_VET_PASS={vet_senha}\n")
    SAIDA.mkdir(parents=True, exist_ok=True)

    with sync_playwright() as p:
        b = p.chromium.launch()
        novo = lambda: b.new_context(base_url=front, viewport={"width": 1366, "height": 860},
                                     accept_downloads=True).new_page()
        admin, lab, vet, publico = novo(), novo(), novo(), novo()
        if continuar:
            lab.goto("/login")
            entrar(lab, lab_email, lab_senha)
            menu(lab, "Meu perfil")
            link_vinculo = lab.locator(".link-box code").inner_text().strip()
            vet.goto("/login")
            entrar(vet, vet_email, vet_senha)
        else:
            link_vinculo = criar_laboratorio_e_veterinario(seg, front, admin, lab, vet, lab_email, lab_senha,
                                                           vet_email, vet_senha)
        pedir_vinculo_e_seguir(front, b, lab, vet, publico, link_vinculo)


def criar_laboratorio_e_veterinario(seg, front, admin, lab, vet, lab_email, lab_senha, vet_email, vet_senha):
    if True:
        passo("administrador entra e gera o convite")
        admin.goto("/login")
        entrar(admin, seg["ADMIN_EMAIL"], seg["ADMIN_PASS"])
        campo(admin, "Para quem é? (opcional)").fill("Laboratório Demonstração")
        botao(admin, "Gerar convite").click()
        convite = admin.locator(".convite-criado code").inner_text().strip()

        passo("laboratório se cadastra pelo convite")
        lab.goto(convite.replace(front, ""))
        preencher(lab, {"Nome do laboratório": "Laboratório Demonstração", "Telefone": "(19) 3333-4444",
                        "Endereço (rua, número e bairro)": "Rua das Flores, 100, Centro", "Cidade": "Indaiatuba",
                        "Nome completo": "Dra. Ana Souza", "Nº do CRMV": "54321", "E-mail": lab_email,
                        "CNPJ": "11.222.333/0001-81", "Senha": lab_senha, "Confirmar senha": lab_senha})
        selecionar(lab, "UF", "SP")
        selecionar(lab, "UF do CRMV", "SP")
        botao(lab, "Cadastrar laboratório").click()
        expect(lab.get_by_role("heading", name="Laboratório cadastrado!")).to_be_visible()

        passo("laboratório configura estoque, tipos de exame e modelo de laudo")
        lab.goto("/login")
        entrar(lab, lab_email, lab_senha)
        menu(lab, "Meu perfil")
        link_vinculo = lab.locator(".link-box code").inner_text().strip()
        menu(lab, "Estoque")
        for nome, minimo, qtd in (("Tubo EDTA", 5, 30), ("Lâmina", 10, 50), ("Corante panótico", 2, 3)):
            botao(lab, "Novo Insumo").click()
            preencher(dialogo(lab), {"Nome": nome, "Unidade de Medida": "un", "Quantidade Mínima (alerta)": minimo})
            botao(dialogo(lab), "Salvar").click()
            expect(linha(lab, nome)).to_be_visible()
            botao_icone(linha(lab, nome), "tune").click()
            campo(dialogo(lab), "Nova Quantidade").fill(str(qtd))
            botao(dialogo(lab), "Salvar").click()
            expect(linha(lab, nome)).to_contain_text(str(qtd))
        menu(lab, "Tipos de Exame")
        for nome, prazo in (("Hemograma", 3), ("Citologia", 5), ("Bioquímico", 2)):
            lab.get_by_role("button", name=re.compile(r"Novo Tipo|Cadastrar primeiro tipo")).first.click()
            preencher(dialogo(lab), {"Nome": nome, "Prazo Estimado (dias)": prazo})
            botao(dialogo(lab), "Salvar").click()
            expect(linha(lab, nome)).to_contain_text(str(prazo))
        botao_icone(linha(lab, "Hemograma"), "description").click()
        campo(dialogo(lab), "Conteúdo").fill(
            "HEMOGRAMA\nEritrograma: dentro dos valores de referência.\nLeucograma: sem alterações.\n"
            "Plaquetas: adequadas.")
        botao(dialogo(lab), "Adicionar Template").click()
        expect(dialogo(lab).locator(".template-box .versao")).to_have_text("Versão 1")
        botao(dialogo(lab), "Fechar").click()
        botao_icone(linha(lab, "Hemograma"), "inventory_2").click()
        selecionar(dialogo(lab), "Insumo", "Tubo EDTA (un)")
        campo(dialogo(lab), "Qtd. por exame").fill("1")
        botao(dialogo(lab), "Vincular").click()
        expect(dialogo(lab).locator(".vinculo")).to_contain_text("Tubo EDTA")
        botao(dialogo(lab), "Fechar").click()

        passo("veterinário cria a conta pelo link de vínculo e pede vínculo")
        vet.goto(link_vinculo.replace(front, ""))
        vet.get_by_role("link", name="Criar conta").click()
        preencher(vet, {"Nome completo": "Dr. Bruno Lima", "E-mail": vet_email, "Senha": vet_senha,
                        "Nº do CRMV": "12345"})
        selecionar(vet, "UF", "SP")
        botao(vet, "Criar conta").click()
        expect(vet.get_by_role("heading", name="Conta criada!")).to_be_visible()
        vet.get_by_role("link", name="Ir para o login").click()
        entrar(vet, vet_email, vet_senha)
        return link_vinculo


def pedir_vinculo_e_seguir(front, b, lab, vet, publico, link_vinculo):
    if True:
        vet.goto(link_vinculo.replace(front, ""))
        pedir = botao(vet, "Pedir vínculo")
        try:
            pedir.wait_for(timeout=15000)
        except Exception:
            pedir = None
        if pedir:
            pedir.click()
            expect(vet.get_by_text("Pedido enviado")).to_be_visible()
            passo("laboratório aprova o vínculo")
            menu(lab, "Veterinários")
            botao(item(lab, "Dr. Bruno Lima", ".vet-item.pendente"), "Aprovar").click()
            expect(secao(lab, "Veterinários vinculados")).to_contain_text("Dr. Bruno Lima")
        else:
            passo("vínculo já aprovado")

        passo("veterinário cria uma solicitação manual")
        vet.goto("/solicitacoes")
        botao(vet, "Nova Solicitação").click()
        expect(campo(vet, "Laboratório")).to_contain_text("Laboratório Demonstração")
        vet.wait_for_load_state("networkidle")
        selecionar(vet, "Tipo de Exame", "Hemograma")
        preencher(secao(vet, "Tutor"), {"Nome": "Carla Dias", "Telefone": "(19) 99999-1111"})
        preencher(secao(vet, "Paciente"), {"Nome": "Mel", "Espécie": "Cão", "Anos": 3, "Peso (kg)": "12.5"})
        selecionar(secao(vet, "Paciente"), "Sexo", "Fêmea")
        botao(vet, "Criar Solicitação").click()
        expect(vet).to_have_url(re.compile(r"/solicitacoes/\d+$"))
        codigo_mel = vet.locator(".page-header h2 code").inner_text().strip()

        resultados = {}
        for metodo, arquivo in (("Foto da guia", "guia-exemplo.png"), ("PDF", "guia-exemplo.pdf"),
                                ("Áudio", "pedido-audio.wav")):
            passo(f"extração com o Gemini real: {metodo}")
            vet.goto("/solicitacoes/nova")
            expect(campo(vet, "Laboratório")).to_contain_text("Laboratório Demonstração")
            vet.wait_for_load_state("networkidle")
            vet.locator("mat-button-toggle").filter(has_text=metodo).click()
            vet.locator("input[type=file]").set_input_files(MATERIAL / arquivo)
            t = time.time()
            botao(vet, "Extrair com IA").click()
            expect(aviso(vet)).to_contain_text(re.compile("Dados extraídos|erro|Erro|falha"))
            seg_ = time.time() - t
            msg = aviso(vet).inner_text()
            paciente = campo(secao(vet, "Paciente"), "Nome").input_value()
            tutor = campo(secao(vet, "Tutor"), "Nome").input_value()
            resultados[metodo] = (round(seg_, 1), msg.strip()[:60], paciente, tutor)
            print("   ", resultados[metodo], flush=True)
            if metodo == "Foto da guia" and paciente:
                vet.screenshot(path=str(SAIDA / "extracao-foto.png"), full_page=True)
                botao(vet, "Criar Solicitação").click()
                expect(vet).to_have_url(re.compile(r"/solicitacoes/\d+$"))

        passo("laboratório avança o status da Mel e emite o laudo")
        menu(lab, "Solicitações")
        linha(lab, "Mel").first.click()
        for status in ("Aguardando Amostra", "Amostra Recebida", "Em Análise"):
            selecionar(lab, "Avançar para", status)
            botao(lab, "Atualizar Status").click()
            expect(lab.locator(".page-header .status-chip")).to_have_text(status)
        botao(lab, "Emitir Laudo").click()
        texto = campo(lab, "Conteúdo do Laudo")
        expect(texto).not_to_have_value("")
        texto.fill(texto.input_value() + "\nConclusão: hemograma sem alterações dignas de nota.")
        botao(lab, "Emitir e Concluir").click()
        expect(lab.locator(".page-header .status-chip")).to_have_text("Concluído")

        passo("PDF do laudo gerado no servidor Linux")
        pdf = baixar_pdf(lab, SAIDA, "laudo-mel.pdf")
        faltando = [e for e in ("Laboratório Demonstração", "11.222.333/0001-81", "Mel", "Dr. Bruno Lima",
                                "sem alterações dignas de nota", "Dra. Ana Souza", "54321", "12,5 kg", "Brasília")
                    if e not in pdf]
        print("    PDF ok" if not faltando else f"    faltando no PDF: {faltando}", flush=True)

        passo("consulta pública pelo código")
        publico.goto("/")
        campo(publico, "Código do exame").fill(codigo_mel)
        botao(publico, "Consultar").click()
        expect(publico.locator(".info-grid")).to_contain_text("Concluído")
        assert "Carla Dias" not in publico.locator("body").inner_text()
        publico.screenshot(path=str(SAIDA / "consulta-publica.png"))
        menu(lab, "Dashboard")
        lab.wait_for_timeout(2000)
        lab.screenshot(path=str(SAIDA / "dashboard.png"))

        print("\nCÓDIGO DA MEL:", codigo_mel)
        print("EXTRAÇÃO:", resultados)
        b.close()


if __name__ == "__main__":
    main()
