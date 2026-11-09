"""Funções de apoio para interagir com as telas do LabPat (Angular Material)."""
import re
from pathlib import Path

from playwright.sync_api import Locator, Page, expect
from pypdf import PdfReader


def _pagina(escopo) -> Page:
    return escopo if isinstance(escopo, Page) else escopo.page


def _exato(texto: str) -> re.Pattern:
    return re.compile(rf"^\s*{re.escape(texto)}\s*$")


def campo(escopo, rotulo: str) -> Locator:
    """Input, textarea ou mat-select do mat-form-field cujo rótulo é exatamente `rotulo`."""
    pg = _pagina(escopo)
    formulario = escopo.locator("mat-form-field").filter(has=pg.locator("mat-label", has_text=_exato(rotulo)))
    return formulario.locator("input, textarea, mat-select").first


def preencher(escopo, valores: dict):
    for rotulo, valor in valores.items():
        campo(escopo, rotulo).fill(str(valor))


def selecionar(escopo, rotulo: str, opcao: str):
    pg = _pagina(escopo)
    # Pelo teclado: com o select vazio, o rótulo flutuante fica por cima e intercepta o clique
    campo(escopo, rotulo).press("Enter")
    pg.locator("mat-option").filter(has_text=_exato(opcao)).click()
    expect(pg.locator(".cdk-overlay-pane mat-option")).to_have_count(0)


def botao(escopo, nome: str) -> Locator:
    return escopo.get_by_role("button", name=nome, exact=True)


def botao_icone(escopo, icone: str) -> Locator:
    """Botão identificado pelo ícone (os botões de ação das tabelas só têm ícone e tooltip)."""
    pg = _pagina(escopo)
    return escopo.locator("button").filter(has=pg.locator("mat-icon", has_text=_exato(icone)))


def linha(escopo, texto: str) -> Locator:
    return escopo.locator("tr").filter(has_text=texto)


def item(escopo, texto: str, classe: str = ".item") -> Locator:
    return escopo.locator(classe).filter(has_text=texto)


def dialogo(pg: Page) -> Locator:
    return pg.locator("mat-dialog-container")


def confirmar(pg: Page, texto_botao: str):
    """Confirma o diálogo de confirmação (ConfirmarDialogComponent)."""
    botao(dialogo(pg), texto_botao).click()
    expect(dialogo(pg)).to_have_count(0)


def aviso(pg: Page) -> Locator:
    # O aviso anterior pode ainda estar saindo da tela: vale o mais recente
    return pg.locator("mat-snack-bar-container:not([mat-exit])").last


def menu(pg: Page, nome: str):
    pg.locator("mat-sidenav a").filter(has_text=re.compile(rf"{re.escape(nome)}\s*$")).click()


def itens_menu(pg: Page) -> list[str]:
    return [t.strip() for t in pg.locator("mat-sidenav a [matListItemTitle]").all_inner_texts()]


def entrar(pg: Page, email: str, senha: str, sucesso: bool = True):
    if "/login" not in pg.url:
        pg.goto("/login")
    preencher(pg, {"E-mail": email, "Senha": senha})
    botao(pg, "Entrar").click()
    if sucesso:
        expect(pg).not_to_have_url(re.compile(r"/login"))


def sair(pg: Page):
    pg.locator(".sidenav-footer button").click()
    expect(pg).to_have_url(re.compile(r"/login"))


def limpar_sessao(pg: Page):
    pg.goto("/")
    pg.evaluate("localStorage.clear()")


def baixar_pdf(pg: Page, pasta: Path, nome: str) -> str:
    """Clica em "Baixar PDF" e devolve o texto do PDF baixado."""
    with pg.expect_download() as info:
        botao(pg, "Baixar PDF").click()
    destino = pasta / nome
    info.value.save_as(destino)
    leitor = PdfReader(destino)
    return "\n".join(p.extract_text() or "" for p in leitor.pages)
