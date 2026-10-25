export type PapelUsuario = 'Patologista' | 'Veterinario' | 'Administrador';

export interface LoginInput {
  email: string;
  senha: string;
}

export interface AuthResponse {
  token: string;
  nome: string;
  email: string;
  papel: PapelUsuario;
}

export interface RegistrarVetInput {
  nome: string;
  email: string;
  senha: string;
  crmvNumero: string;
  crmvEstado: string;
  telefone?: string;
}

export interface ConviteInfo {
  descricao: string | null;
  expiraEm: string;
}

export interface RegistrarLaboratorioInput {
  token: string;
  nome: string;
  cnpj: string;
  endereco: string;
  cidade: string;
  uf: string;
  telefone: string;
  email: string;
  senha: string;
  responsavelTecnicoNome: string;
  responsavelTecnicoCrmvNumero: string;
  responsavelTecnicoCrmvEstado: string;
}
