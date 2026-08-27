export interface LoginInput {
  email: string;
  senha: string;
}

export interface AuthResponse {
  token: string;
  nome: string;
  email: string;
}
