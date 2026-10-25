export type StatusConvite = 'Ativo' | 'Usado' | 'Revogado' | 'Expirado';

// O token só vem na resposta de criação
export interface ConviteCriado {
  id: number;
  token: string;
  descricao: string | null;
  expiraEm: string;
}

export interface Convite {
  id: number;
  descricao: string | null;
  status: StatusConvite;
  criadoEm: string;
  expiraEm: string;
  usadoEm: string | null;
  revogadoEm: string | null;
  laboratorioNome: string | null;
}

export interface LaboratorioAdmin {
  id: number;
  nome: string;
  cnpj: string;
  cidade: string;
  uf: string;
  telefone: string;
  email: string;
  responsavelTecnicoNome: string;
  ativo: boolean;
  criadoEm: string;
}

export interface VeterinarioAdmin {
  id: number;
  nome: string;
  email: string;
  crmvNumero: string;
  crmvEstado: string;
  telefone: string | null;
  ativo: boolean;
  criadoEm: string;
}
