export type PapelUsuario = 'Patologista' | 'Veterinario';

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
