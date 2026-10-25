import { StatusVinculo } from './vinculo.model';

// Pedido de vínculo de um veterinário, na visão do laboratório
export interface VinculoVeterinario {
  id: number;
  veterinarioId: number;
  nome: string;
  email: string;
  crmvNumero: string;
  crmvEstado: string;
  telefone: string | null;
  status: StatusVinculo;
  solicitadoEm: string;
  respondidoEm: string | null;
}
