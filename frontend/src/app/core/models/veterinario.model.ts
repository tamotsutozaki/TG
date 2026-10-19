export type StatusConta = 'Pendente' | 'Aprovada' | 'Recusada';

export interface VeterinarioConta {
  id: number;
  nome: string;
  email: string;
  crmvNumero: string;
  crmvEstado: string;
  telefone: string | null;
  statusConta: StatusConta;
  criadoEm: string;
}
