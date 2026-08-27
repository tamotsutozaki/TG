export interface InsumoDto {
  id: number;
  nome: string;
  unidadeMedida: string;
  quantidadeAtual: number;
  quantidadeMinima: number;
  ativo: boolean;
  emEstoqueBaixo: boolean;
}

export interface CreateInsumoInput {
  nome: string;
  unidadeMedida: string;
  quantidadeMinima: number;
}
