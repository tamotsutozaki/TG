import { Directive, HostListener, inject } from '@angular/core';
import { AbstractControl, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { normalizarCnpj } from './formatadores';

/**
 * Regras de validação dos campos, iguais em todas as telas e alinhadas às do backend.
 * Cada campo combina os validadores daqui com Validators.required quando for obrigatório,
 * e o template mostra o erro com erroCampo().
 */

export const LIMITE = {
  nome: 120,          // pessoas (veterinário, tutor, responsável técnico)
  nomeLaboratorio: 150,
  endereco: 200,
  cidade: 80,
  email: 150,
  senha: 100,
  especie: 60,
  raca: 60,
  nomeItem: 100,      // tipo de exame, insumo
  unidade: 20,
  descricaoCurta: 300,
  observacao: 500,
  descricaoClinica: 2000,
  laudo: 10000,
  busca: 100,
  codigo: 12
} as const;

const LETRAS = /^[A-Za-zÀ-ÖØ-öø-ÿ'´`^~.\- ]+$/;

/** Campo obrigatório que não aceita só espaços. */
export const obrigatorio: ValidatorFn = c =>
  typeof c.value === 'string' && c.value.trim() === '' ? { required: true } : Validators.required(c);

/** Só letras, espaços, apóstrofo, ponto e hífen (nomes de pessoas, cidades, espécie, raça). */
export const soLetras: ValidatorFn = c =>
  !c.value || LETRAS.test(String(c.value).trim()) ? null : { soLetras: true };

/** Telefone brasileiro: 10 ou 11 dígitos (com DDD). */
export const telefone: ValidatorFn = c => {
  if (!c.value) return null;
  const n = String(c.value).replace(/\D/g, '').length;
  return n === 10 || n === 11 ? null : { telefone: true };
};

/** E-mail com domínio (exige o ponto: nome@dominio.com). */
export const email: ValidatorFn = c =>
  !c.value || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(c.value).trim()) ? null : { email: true };

/** Número do CRMV: 1 a 6 dígitos. */
export const crmv: ValidatorFn = c =>
  !c.value || /^\d{1,6}$/.test(String(c.value).trim()) ? null : { crmv: true };

/** CNPJ numérico ou alfanumérico, com os dígitos verificadores (mesma regra do backend). */
export const cnpj: ValidatorFn = c => {
  if (!c.value) return null;
  const v = normalizarCnpj(c.value);
  if (!/^[A-Z0-9]{12}[0-9]{2}$/.test(v) || new Set(v).size === 1) return { cnpj: true };
  const digito = (base: string, pesos: number[]) => {
    const soma = pesos.reduce((s, p, i) => s + (base.charCodeAt(i) - 48) * p, 0);
    const resto = soma % 11;
    return resto < 2 ? 0 : 11 - resto;
  };
  const d1 = digito(v.slice(0, 12), [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
  const d2 = digito(v.slice(0, 13), [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
  return +v[12] === d1 && +v[13] === d2 ? null : { cnpj: true };
};

/** Número inteiro (sem casas decimais). */
export const inteiro: ValidatorFn = c =>
  c.value === null || c.value === '' || Number.isInteger(Number(c.value)) ? null : { inteiro: true };

/** No máximo `casas` casas decimais. */
export const casasDecimais = (casas: number): ValidatorFn => c => {
  if (c.value === null || c.value === '') return null;
  const partes = String(c.value).split('.');
  return !partes[1] || partes[1].length <= casas ? null : { casasDecimais: { casas } };
};

/** Mensagem do primeiro erro do campo, para o <mat-error>. */
export function erroCampo(c: AbstractControl | null): string {
  const e: ValidationErrors | null | undefined = c?.errors;
  if (!e) return '';
  if (e['required']) return 'Obrigatório';
  if (e['maxlength']) return `Máximo de ${e['maxlength'].requiredLength} caracteres`;
  if (e['minlength']) return `Mínimo de ${e['minlength'].requiredLength} caracteres`;
  if (e['soLetras']) return 'Use apenas letras';
  if (e['telefone']) return 'Telefone com DDD: 10 ou 11 dígitos';
  if (e['email']) return 'E-mail inválido';
  if (e['crmv']) return 'Só números (até 6 dígitos)';
  if (e['cnpj']) return 'CNPJ inválido';
  if (e['inteiro']) return 'Use um número inteiro';
  if (e['casasDecimais']) return `No máximo ${e['casasDecimais'].casas} casas decimais`;
  if (e['min']) return `Mínimo: ${String(e['min'].min).replace('.', ',')}`;
  if (e['max']) return `Máximo: ${String(e['max'].max).replace('.', ',')}`;
  if (e['senhasDiferentes']) return 'As senhas não conferem.';
  return 'Valor inválido';
}

// ── Máscaras: formatam o valor enquanto o usuário digita ────────────────────

@Directive()
abstract class Mascara {
  private control = inject(NgControl, { optional: true, self: true });
  protected abstract formatar(valor: string): string;

  @HostListener('input', ['$event'])
  aoDigitar(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const formatado = this.formatar(input.value ?? '');
    if (formatado === input.value) return;
    input.value = formatado;
    this.control?.control?.setValue(formatado, { emitEvent: true });
    this.control?.viewToModelUpdate(formatado); // campos com [(ngModel)]
  }
}

/** (19) 3333-4444 ou (19) 99999-1111 */
export function formatarTelefone(valor: string | null | undefined): string {
  const d = (valor ?? '').replace(/\D/g, '').slice(0, 11);
  if (d.length <= 2) return d.length ? `(${d}` : '';
  const ddd = d.slice(0, 2), resto = d.slice(2);
  const corte = d.length === 11 ? 5 : 4;
  return resto.length <= corte ? `(${ddd}) ${resto}` : `(${ddd}) ${resto.slice(0, corte)}-${resto.slice(corte)}`;
}

@Directive({ selector: 'input[appTelefone]', standalone: true })
export class TelefoneMascaraDirective extends Mascara {
  protected formatar(valor: string): string {
    return formatarTelefone(valor);
  }
}

/** XX.XXX.XXX/XXXX-XX, em maiúsculas (aceita o CNPJ alfanumérico). */
@Directive({ selector: 'input[appCnpj]', standalone: true })
export class CnpjMascaraDirective extends Mascara {
  protected formatar(valor: string): string {
    const v = normalizarCnpj(valor).slice(0, 14);
    const blocos: [number, string][] = [[2, '.'], [5, '.'], [8, '/'], [12, '-']];
    let saida = '';
    for (let i = 0; i < v.length; i++) {
      const sep = blocos.find(([pos]) => pos === i);
      saida += (sep ? sep[1] : '') + v[i];
    }
    return saida;
  }
}

/** Só dígitos, até `maxlength` (CRMV). */
@Directive({ selector: 'input[appSoDigitos]', standalone: true })
export class SoDigitosDirective extends Mascara {
  protected formatar(valor: string): string {
    return valor.replace(/\D/g, '');
  }
}

/** Letras e números em maiúsculas (código público do exame). */
@Directive({ selector: 'input[appCodigo]', standalone: true })
export class CodigoMascaraDirective extends Mascara {
  protected formatar(valor: string): string {
    return valor.replace(/[^0-9a-zA-Z]/g, '').toUpperCase().slice(0, LIMITE.codigo);
  }
}

export const MASCARAS = [TelefoneMascaraDirective, CnpjMascaraDirective, SoDigitosDirective, CodigoMascaraDirective];
