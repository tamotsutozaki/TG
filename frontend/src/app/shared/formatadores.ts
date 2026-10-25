export const UFS = [
  'AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA',
  'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN',
  'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'
];

export const SEXO_LABEL: Record<string, string> = {
  Macho: 'Macho',
  Femea: 'Fêmea',
  NaoInformado: 'Não informado'
};

export const METODO_ENTRADA_LABEL: Record<string, string> = {
  Manual: 'Manual',
  Imagem: 'Foto da guia',
  PDF: 'PDF',
  Audio: 'Áudio'
};

/** Remove pontuação do CNPJ (numérico ou alfanumérico). */
export function normalizarCnpj(cnpj: string | null | undefined): string {
  return (cnpj ?? '').replace(/[^0-9a-zA-Z]/g, '').toUpperCase();
}

/** XX.XXX.XXX/XXXX-XX (vale também para o CNPJ alfanumérico). */
export function formatarCnpj(cnpj: string | null | undefined): string {
  const v = normalizarCnpj(cnpj);
  return v.length === 14
    ? `${v.slice(0, 2)}.${v.slice(2, 5)}.${v.slice(5, 8)}/${v.slice(8, 12)}-${v.slice(12)}`
    : (cnpj ?? '');
}

export function formatarCrmv(estado: string, numero: string): string {
  return `CRMV-${estado} ${numero}`;
}

/** Link completo para compartilhar (convite de laboratório, link de vínculo). */
export function linkAbsoluto(caminho: string): string {
  return `${window.location.origin}${caminho}`;
}
