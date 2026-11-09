"""
Infraestrutura dos testes E2E do LabPat.

Sobe um ambiente isolado do de desenvolvimento:
  - banco LabPat_E2E no LocalDB (apagado e recriado a cada execução);
  - API em http://localhost:5180 (migrations e administrador criados no startup);
  - frontend em http://localhost:4300 (configuração "e2e" do Angular, que aponta para a API acima).

A extração com o Gemini é simulada no navegador: a chamada a /api/gemini/extrair
devolve os dados da guia fictícia, para o teste não depender da rede nem da chave.

Variável opcional: E2E_REUSAR=1 usa API e front que já estejam no ar (não recria o banco).
"""
import json
import os
import shutil
import subprocess
import time
from pathlib import Path

import pytest
import requests

RAIZ = Path(__file__).resolve().parent.parent
API_URL = "http://localhost:5180"
FRONT_URL = "http://localhost:4300"
BANCO = "LabPat_E2E"
INSTANCIA = r"(localdb)\mssqllocaldb"
ADMIN_EMAIL = "admin@labpat.com"
ADMIN_SENHA = "Admin@123"
SENHA = "Senha@123"

RELATORIO = Path(__file__).resolve().parent / "relatorio"

# Dados da guia fictícia (material-de-teste/guia-exemplo.*), como o Gemini os devolve
GUIA_EXTRAIDA = {
    "vetNome": "Dr. Rafael Moura",
    "vetCrmvNumero": "23456",
    "vetCrmvEstado": "SP",
    "vetEmail": "rafael.moura@vet.com",
    "vetTelefone": "(19) 99876-5432",
    "tutorNome": "Mariana Lopes",
    "tutorTelefone": "(19) 98765-4321",
    "tutorEmail": "mariana@email.com",
    "pacienteNome": "Thor",
    "especie": "Cão",
    "raca": "Labrador",
    "sexo": "Macho",
    "idade": "7 anos",
    "peso": "32,5",
    "tipoExame": "Hemograma completo",
    "descricaoClinica": "Apatia e perda de apetite há 5 dias. Suspeita de anemia.",
}


# ── Servidores ──────────────────────────────────────────────────────────────

def _no_ar(url: str) -> bool:
    try:
        requests.get(url, timeout=2)
        return True
    except requests.RequestException:
        return False


def _esperar(url: str, processo: subprocess.Popen, nome: str, limite: int = 240):
    inicio = time.time()
    while time.time() - inicio < limite:
        if processo.poll() is not None:
            raise RuntimeError(f"{nome} encerrou durante a inicialização (código {processo.returncode}). "
                               f"Veja {RELATORIO / (nome + '.log')}")
        if _no_ar(url):
            return
        time.sleep(1)
    raise TimeoutError(f"{nome} não respondeu em {limite} s ({url})")


def _sqlcmd() -> str:
    achado = shutil.which("sqlcmd")
    if achado:
        return achado
    for versao in ("170", "180", "130"):
        caminho = Path(rf"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\{versao}\Tools\Binn\SQLCMD.EXE")
        if caminho.exists():
            return str(caminho)
    raise RuntimeError("sqlcmd não encontrado: necessário para recriar o banco de testes.")


def _apagar_banco():
    sql = (f"IF DB_ID('{BANCO}') IS NOT NULL BEGIN "
           f"ALTER DATABASE [{BANCO}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; "
           f"DROP DATABASE [{BANCO}]; END")
    subprocess.run([_sqlcmd(), "-S", INSTANCIA, "-b", "-Q", sql], check=True, capture_output=True)


def _encerrar(processo: subprocess.Popen):
    if processo.poll() is None:
        # taskkill /T derruba também os processos filhos (dotnet e node criam vários)
        subprocess.run(["taskkill", "/PID", str(processo.pid), "/T", "/F"], capture_output=True)


@pytest.fixture(scope="session")
def servidores():
    RELATORIO.mkdir(exist_ok=True)
    if os.environ.get("E2E_REUSAR") == "1" and _no_ar(API_URL) and _no_ar(FRONT_URL):
        yield
        return

    for url in (API_URL, FRONT_URL):
        if _no_ar(url):
            raise RuntimeError(f"Já existe algo rodando em {url}. Encerre antes de rodar os testes.")

    _apagar_banco()

    env_api = os.environ | {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": API_URL,
        "ConnectionStrings__DefaultConnection":
            f"Server={INSTANCIA};Database={BANCO};Trusted_Connection=True;TrustServerCertificate=True;",
        "Cors__AllowedOrigin": FRONT_URL,
        "Admin__Nome": "Administrador",
        "Admin__Email": ADMIN_EMAIL,
        "Admin__Senha": ADMIN_SENHA,
        "Jwt__Secret": "segredo-dos-testes-e2e-com-mais-de-32-caracteres",
        "Gemini__ApiKey": "nao-usada-nos-testes",
    }
    log_api = open(RELATORIO / "API.log", "w", encoding="utf-8")
    api = subprocess.Popen(
        ["dotnet", "run", "--project", str(RAIZ / "backend" / "LabPat.Api"), "--no-launch-profile"],
        env=env_api, stdout=log_api, stderr=subprocess.STDOUT)

    log_front = open(RELATORIO / "Frontend.log", "w", encoding="utf-8")
    front = subprocess.Popen(
        "npx ng serve --configuration e2e --port 4300",
        cwd=RAIZ / "frontend", shell=True, stdout=log_front, stderr=subprocess.STDOUT)

    try:
        _esperar(f"{API_URL}/api/solicitacoes/consulta/INEXISTENTE", api, "API")
        _esperar(FRONT_URL, front, "Frontend")
        yield
    finally:
        _encerrar(front)
        _encerrar(api)
        log_api.close()
        log_front.close()


# ── Navegador: uma "janela" por perfil, como no roteiro manual ──────────────

def _simular_gemini(contexto, chamadas: list):
    def responder(route):
        chamadas.append(route.request.headers.get("content-type", ""))
        route.fulfill(status=200, content_type="application/json", body=json.dumps(GUIA_EXTRAIDA))
    contexto.route("**/api/gemini/extrair", responder)


class Janelas:
    """Contextos isolados do navegador (cada um com seu próprio login)."""

    def __init__(self, browser):
        self.browser = browser
        self.chamadas_gemini: list[str] = []
        opcoes = dict(base_url=FRONT_URL, locale="pt-BR", timezone_id="America/Sao_Paulo",
                      viewport={"width": 1366, "height": 860}, accept_downloads=True)
        self._contextos = {}
        self.paginas = {}
        for perfil in ("admin", "lab", "vet", "publico"):
            contexto = browser.new_context(**opcoes)
            contexto.set_default_timeout(15000)
            _simular_gemini(contexto, self.chamadas_gemini)
            self._contextos[perfil] = contexto
            self.paginas[perfil] = contexto.new_page()

    @property
    def admin(self):
        return self.paginas["admin"]

    @property
    def lab(self):
        return self.paginas["lab"]

    @property
    def vet(self):
        return self.paginas["vet"]

    @property
    def publico(self):
        return self.paginas["publico"]

    def fechar(self):
        for contexto in self._contextos.values():
            contexto.close()


@pytest.fixture(scope="session")
def janelas(servidores, browser):
    j = Janelas(browser)
    yield j
    j.fechar()


@pytest.fixture(scope="session")
def estado():
    """Dados que passam de um passo para o outro (links, códigos)."""
    return {}


@pytest.fixture(scope="session")
def arquivos(tmp_path_factory):
    """Arquivos enviados à extração. O conteúdo não importa: a resposta do Gemini é simulada."""
    pasta = tmp_path_factory.mktemp("guia")
    material = RAIZ / "material-de-teste"
    saida = {}
    for nome, padrao in (("guia-exemplo.png", b"\x89PNG\r\n\x1a\n"), ("guia-exemplo.pdf", b"%PDF-1.4\n%%EOF\n"),
                         ("pedido-audio.wav", b"RIFF\x24\x00\x00\x00WAVEfmt ")):
        origem = material / nome
        destino = pasta / nome
        destino.write_bytes(origem.read_bytes() if origem.exists() else padrao)
        saida[nome] = destino
    return saida


# ── Capturas de tela das janelas quando um passo falha ──────────────────────

@pytest.hookimpl(hookwrapper=True)
def pytest_runtest_makereport(item, call):
    resultado = yield
    relatorio = resultado.get_result()
    if relatorio.when == "call" and relatorio.failed and "janelas" in item.funcargs:
        pasta = RELATORIO / "falhas"
        pasta.mkdir(parents=True, exist_ok=True)
        for perfil, pagina in item.funcargs["janelas"].paginas.items():
            try:
                pagina.screenshot(path=str(pasta / f"{item.name}-{perfil}.png"), full_page=True)
            except Exception:
                pass
