export type StatusSolicitacao =
  | 'Solicitado'
  | 'AguardandoAmostra'
  | 'AmostraRecebida'
  | 'EmAnalise'
  | 'Concluido';

export type MetodoEntrada = 'Manual' | 'Imagem' | 'PDF' | 'Audio';

export interface SolicitacaoDto {
  id: number;
  codigoPublico: string;
  status: StatusSolicitacao;
  tipoExameNome: string;
  pacienteNome: string;
  vetSolicitanteNome: string;
  dataCriacao: string;
  dataEstimadaConclusao: string | null;
}

export interface SolicitacaoDetalhadaDto {
  id: number;
  codigoPublico: string;
  status: StatusSolicitacao;
  metodoEntrada: MetodoEntrada;
  descricaoClinica: string | null;
  observacoesInternas: string | null;
  arquivoEntradaUrl: string | null;
  dataCriacao: string;
  dataEstimadaConclusao: string | null;
  dataConclusao: string | null;
  tipoExame: { id: number; nome: string; prazoEstimadoDias: number };
  paciente: {
    id: number; nome: string; especie: string; raca: string | null;
    sexo: string; idadeAnos: number | null; idadeMeses: number | null; pesoKg: number | null;
  };
  tutor: { id: number; nome: string; telefone: string; email: string | null };
  vetSolicitante: {
    id: number; nome: string; crmvNumero: string; crmvEstado: string;
    email: string | null; telefone: string | null;
  };
  historico: HistoricoStatusDto[];
  laudo?: LaudoResumoDto;
}

export interface HistoricoStatusDto {
  statusAnterior: StatusSolicitacao;
  statusNovo: StatusSolicitacao;
  alteradoEm: string;
  observacao: string | null;
}

export interface LaudoResumoDto {
  id: number;
  emitidoEm: string;
}

export interface ConsultaPublicaDto {
  codigoPublico: string;
  status: string;
  tipoExame: string;
  pacienteNome: string;
  dataCriacao: string;
  dataEstimadaConclusao: string | null;
}

export interface UpdateStatusInput {
  novoStatus: StatusSolicitacao;
  observacao?: string;
}
