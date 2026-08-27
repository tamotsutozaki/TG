export interface TipoExameDto {
  id: number;
  nome: string;
  descricao: string | null;
  prazoEstimadoDias: number;
  ativo: boolean;
}

export interface TipoExameDetalhadoDto extends TipoExameDto {
  templates: TemplateLaudoDto[];
  insumos: ExameInsumoDto[];
}

export interface TemplateLaudoDto {
  id: number;
  conteudo: string;
  versao: number;
  criadoEm: string;
}

export interface ExameInsumoDto {
  insumoId: number;
  insumoNome: string;
  unidadeMedida: string;
  quantidadeConsumida: number;
}

export interface CreateTipoExameInput {
  nome: string;
  descricao?: string;
  prazoEstimadoDias: number;
}
