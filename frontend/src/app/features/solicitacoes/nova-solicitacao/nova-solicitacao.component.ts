import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SolicitacaoService } from '../../../core/services/solicitacao.service';
import { TipoExameService } from '../../../core/services/tipo-exame.service';
import { GeminiService } from '../../../core/services/gemini.service';
import { AuthService } from '../../../core/services/auth.service';
import { VinculoService } from '../../../core/services/vinculo.service';
import { TipoExameDto } from '../../../core/models/tipo-exame.model';
import { MeuVinculo } from '../../../core/models/vinculo.model';

@Component({
  selector: 'app-nova-solicitacao',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, FormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatButtonToggleModule, MatDividerModule, MatSnackBarModule
  ],
  templateUrl: './nova-solicitacao.component.html',
  styleUrl: './nova-solicitacao.component.scss'
})
export class NovaSolicitacaoComponent implements OnInit {
  form: FormGroup;
  tiposExame: TipoExameDto[] = [];
  // Veterinário: laboratórios com vínculo aprovado (destino da solicitação)
  laboratorios: MeuVinculo[] = [];
  carregandoLaboratorios = false;
  metodoEntrada: 'Manual' | 'Imagem' | 'PDF' | 'Audio' = 'Manual';
  arquivoSelecionado: File | null = null;
  extraindo = false;
  salvando = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private solicitacaoService: SolicitacaoService,
    private tipoExameService: TipoExameService,
    private geminiService: GeminiService,
    private auth: AuthService,
    private vinculoService: VinculoService,
    private route: ActivatedRoute,
    private snack: MatSnackBar
  ) {
    this.form = this.fb.group({
      laboratorioId: [null as number | null],
      tipoExameId: [null, Validators.required],
      descricaoClinica: [''],
      vetSolicitante: this.fb.group({
        nome: ['', Validators.required],
        crmvNumero: ['', Validators.required],
        crmvEstado: ['', [Validators.required, Validators.maxLength(2)]],
        email: [''],
        telefone: ['']
      }),
      tutor: this.fb.group({
        nome: ['', Validators.required],
        telefone: ['', Validators.required],
        email: ['']
      }),
      paciente: this.fb.group({
        nome: ['', Validators.required],
        especie: ['', Validators.required],
        raca: [''],
        sexo: ['NaoInformado'],
        idadeAnos: [null],
        idadeMeses: [null],
        pesoKg: [null]
      })
    });
  }

  get isVet() {
    return this.auth.isVeterinario();
  }

  ngOnInit() {
    if (!this.isVet) {
      this.tipoExameService.getAll().subscribe(tipos => this.tiposExame = tipos);
      return;
    }

    // Vet logado usa o próprio perfil: a seção do vet some e não é exigida
    this.form.get('vetSolicitante')?.disable();
    this.form.get('laboratorioId')?.addValidators(Validators.required);
    this.form.get('laboratorioId')?.valueChanges.subscribe(id => this.carregarTiposDoLaboratorio(id));

    this.carregandoLaboratorios = true;
    this.vinculoService.meusVinculos().subscribe({
      next: vinculos => {
        this.laboratorios = vinculos.filter(v => v.status === 'Aprovado');
        this.carregandoLaboratorios = false;

        // Vem pré-selecionado pelo link (?laboratorio=) ou quando só há um laboratório
        const sugerido = Number(this.route.snapshot.queryParamMap.get('laboratorio'));
        const inicial = this.laboratorios.find(l => l.laboratorioId === sugerido)
          ?? (this.laboratorios.length === 1 ? this.laboratorios[0] : undefined);
        if (inicial) this.form.get('laboratorioId')?.setValue(inicial.laboratorioId);
      },
      error: () => { this.carregandoLaboratorios = false; }
    });
  }

  private carregarTiposDoLaboratorio(laboratorioId: number | null) {
    this.tiposExame = [];
    this.form.get('tipoExameId')?.reset();
    if (!laboratorioId) return;
    this.tipoExameService.getAll(laboratorioId).subscribe(tipos => this.tiposExame = tipos);
  }

  onArquivoChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.arquivoSelecionado = input.files?.[0] ?? null;
  }

  extrairComGemini() {
    if (!this.arquivoSelecionado) return;
    this.extraindo = true;
    this.geminiService.extrairDados(this.arquivoSelecionado).subscribe({
      next: dados => {
        this.extraindo = false;
        this.preencherFormulario(dados);
        this.snack.open('Dados extraídos. Revise e confirme.', 'OK', { duration: 4000 });
      },
      error: () => {
        this.extraindo = false;
        this.snack.open('Erro ao extrair. Preencha manualmente.', 'OK', { duration: 4000 });
      }
    });
  }

  private preencherFormulario(dados: any) {
    const set = (path: string, val: any) => { if (val) this.form.get(path)?.setValue(val); };
    set('vetSolicitante.nome', dados.vetNome);
    set('vetSolicitante.crmvNumero', dados.vetCrmvNumero);
    set('vetSolicitante.crmvEstado', dados.vetCrmvEstado);
    set('vetSolicitante.email', dados.vetEmail);
    set('vetSolicitante.telefone', dados.vetTelefone);
    set('tutor.nome', dados.tutorNome);
    set('tutor.telefone', dados.tutorTelefone);
    set('tutor.email', dados.tutorEmail);
    set('paciente.nome', dados.pacienteNome);
    set('paciente.especie', dados.especie);
    set('paciente.raca', dados.raca);
    set('descricaoClinica', dados.descricaoClinica);

    if (dados.sexo) {
      const m: Record<string, string> = { macho: 'Macho', 'fêmea': 'Femea', femea: 'Femea' };
      set('paciente.sexo', m[dados.sexo.toLowerCase()] ?? 'NaoInformado');
    }
    if (dados.peso) {
      const p = parseFloat(dados.peso);
      if (!isNaN(p)) this.form.get('paciente.pesoKg')?.setValue(p);
    }
    if (dados.tipoExame) {
      const tipo = this.tiposExame.find(t =>
        t.nome.toLowerCase().includes(dados.tipoExame!.toLowerCase())
      );
      if (tipo) this.form.get('tipoExameId')?.setValue(tipo.id);
    }
  }

  salvar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.salvando = true;
    this.solicitacaoService.create({ ...this.form.value, metodoEntrada: this.metodoEntrada }).subscribe({
      next: s => {
        this.snack.open('Solicitação criada!', 'OK', { duration: 3000 });
        this.router.navigate(['/solicitacoes', s.id]);
      },
      error: err => {
        this.salvando = false;
        this.snack.open(err?.error?.message ?? 'Não foi possível criar a solicitação.', 'OK', { duration: 4000 });
      }
    });
  }

  voltar() { this.router.navigate(['/solicitacoes']); }

  get aceitaArquivo(): string {
    if (this.metodoEntrada === 'PDF') return '.pdf';
    if (this.metodoEntrada === 'Audio') return 'audio/*';
    return 'image/*';
  }
}
