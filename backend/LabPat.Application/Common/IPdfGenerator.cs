namespace LabPat.Application.Common;

public interface IPdfGenerator
{
    byte[] GerarLaudo(LaudoPdfData data);
}
