export type StatusVinculo = 'Pendente' | 'Aprovado' | 'Recusado' | 'Desfeito';

export const STATUS_VINCULO_LABEL: Record<StatusVinculo, string> = {
  Pendente: 'Pendente',
  Aprovado: 'Vinculado',
  Recusado: 'Recusado',
  Desfeito: 'Desfeito'
};

// Laboratório na busca do veterinário (statusVinculo: vínculo do vet logado, se houver)
export interface LaboratorioBusca {
  id: number;
  nome: string;
  cidade: string;
  uf: string;
  statusVinculo?: StatusVinculo | null;
}

export interface MeuVinculo {
  id: number;
  laboratorioId: number;
  laboratorioNome: string;
  cidade: string;
  uf: string;
  status: StatusVinculo;
  solicitadoEm: string;
  respondidoEm: string | null;
}
