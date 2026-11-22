r"""
Captura todas as telas do sistema publicado (computador e celular) para revisão de layout.
Uso: tests\.venv\Scripts\python tests\capturar_telas.py <pasta-de-saida>
"""
import sys
from pathlib import Path

from playwright.sync_api import sync_playwright

sys.path.insert(0, str(Path(__file__).resolve().parent))
from apoio import entrar  # noqa: E402
from demo_producao import ler_segredos  # noqa: E402

PUBLICAS = [("home", "/"), ("login", "/login"), ("registro", "/registro"),
            ("consulta", "/consulta/4AYK85T3"), ("consulta-invalida", "/consulta/ZZZ99999"),
            ("convite-invalido", "/cadastro-laboratorio?convite=xxx")]
PERFIS = {
    "lab": ("DEMO_LAB_EMAIL", "DEMO_LAB_PASS",
            [("dashboard", "/dashboard"), ("solicitacoes", "/solicitacoes"), ("detalhe-concluido", "/solicitacoes/1"),
             ("detalhe-andamento", "/solicitacoes/4"), ("nova", "/solicitacoes/nova"), ("tipos", "/tipos-exame"),
             ("insumos", "/insumos"), ("veterinarios", "/veterinarios"), ("perfil", "/perfil")]),
    "vet": ("DEMO_VET_EMAIL", "DEMO_VET_PASS",
            [("solicitacoes", "/solicitacoes"), ("laboratorios", "/laboratorios"), ("nova", "/solicitacoes/nova"),
             ("detalhe", "/solicitacoes/3"), ("perfil", "/perfil")]),
    "admin": ("ADMIN_EMAIL", "ADMIN_PASS",
              [("convites", "/admin/convites"), ("laboratorios", "/admin/laboratorios"),
               ("veterinarios", "/admin/veterinarios"), ("perfil", "/perfil")]),
}


def main(saida: Path):
    saida.mkdir(parents=True, exist_ok=True)
    seg = ler_segredos()
    front = seg["FRONT_URL"]
    with sync_playwright() as p:
        b = p.chromium.launch()
        for disp, vp in (("pc", {"width": 1366, "height": 800}), ("cel", {"width": 390, "height": 844})):
            pg = b.new_context(base_url=front, viewport=vp, device_scale_factor=1).new_page()
            for nome, url in PUBLICAS:
                pg.goto(url); pg.wait_for_load_state("networkidle"); pg.wait_for_timeout(800)
                pg.screenshot(path=str(saida / f"{disp}-publico-{nome}.png"), full_page=True)
            for perfil, (ke, ks, telas) in PERFIS.items():
                ctx = b.new_context(base_url=front, viewport=vp, device_scale_factor=1)
                pg = ctx.new_page()
                pg.goto("/login"); entrar(pg, seg[ke], seg[ks])
                pg.wait_for_load_state("networkidle")
                for nome, url in telas:
                    pg.goto(url); pg.wait_for_load_state("networkidle"); pg.wait_for_timeout(1000)
                    pg.screenshot(path=str(saida / f"{disp}-{perfil}-{nome}.png"), full_page=True)
                ctx.close()
        b.close()
    print(len(list(saida.glob("*.png"))), "capturas em", saida)


if __name__ == "__main__":
    main(Path(sys.argv[1]))
