// CNPJ e e-mail de login não são editáveis
export interface PerfilLaboratorio {
  emailLogin: string;
  nome: string;
  cnpj: string;
  endereco: string;
  cidade: string;
  uf: string;
  telefone: string;
  email: string;
  responsavelTecnicoNome: string;
  responsavelTecnicoCrmvNumero: string;
  responsavelTecnicoCrmvEstado: string;
  codigoVinculo: string;
}

export type AtualizarPerfilLaboratorioInput = Omit<PerfilLaboratorio, 'emailLogin' | 'cnpj' | 'codigoVinculo'>;

// O CRMV não é editável
export interface PerfilVeterinario {
  email: string;
  nome: string;
  crmvNumero: string;
  crmvEstado: string;
  telefone: string | null;
}

export interface AtualizarPerfilVeterinarioInput {
  nome: string;
  telefone: string | null;
}
