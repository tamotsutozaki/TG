using LabPat.Application.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LabPat.Infrastructure.ExternalServices;

public class QuestPdfGenerator : IPdfGenerator
{
    public byte[] GerarLaudo(LaudoPdfData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, data));
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().Element(c => ComposeFooter(c, data));
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, LaudoPdfData data)
    {
        container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingBottom(8).Row(row =>
        {
            // Identificação do laboratório que emite o laudo
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(data.LaboratorioNome)
                    .Bold().FontSize(13).FontColor(Colors.Grey.Darken3);
                col.Item().Text($"CNPJ {data.LaboratorioCnpj}")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().Text(data.LaboratorioEndereco)
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().Text($"{data.LaboratorioTelefone}  |  {data.LaboratorioEmail}")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(170).AlignRight().AlignMiddle().Text("LAUDO DE EXAME LABORATORIAL")
                .Bold().FontSize(11).FontColor(Colors.Grey.Darken3);
        });
    }

    private static void ComposeContent(IContainer container, LaudoPdfData data)
    {
        container.PaddingTop(12).Column(col =>
        {
            // Código e tipo de exame
            col.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Código: {data.CodigoPublico}").Bold().FontSize(11);
                    c.Item().Text($"Tipo de Exame: {data.TipoExame}").FontSize(10);
                });
                row.ConstantItem(160).Column(c =>
                {
                    c.Item().Text($"Solicitado em: {data.DataSolicitacao:dd/MM/yyyy}");
                    c.Item().Text($"Emitido em: {data.DataEmissao:dd/MM/yyyy}");
                });
            });

            col.Item().PaddingTop(12).Text("DADOS DO PACIENTE").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
            col.Item().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingBottom(6).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                });

                table.Cell().Text($"Nome: {data.PacienteNome}");
                table.Cell().Text($"Espécie: {data.Especie}");
                table.Cell().Text($"Raça: {data.Raca ?? "Não informada"}");
                table.Cell().Text($"Sexo: {data.Sexo}");
                table.Cell().Text($"Idade: {data.Idade ?? "Não informada"}");
                table.Cell().Text($"Peso: {data.Peso ?? "Não informado"}");
                table.Cell().ColumnSpan(3).Text($"Tutor: {data.TutorNome}");
            });

            col.Item().PaddingTop(12).Text("MÉDICO VETERINÁRIO SOLICITANTE").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
            col.Item().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingBottom(6).Row(row =>
            {
                row.RelativeItem().Text($"{data.VetSolicitanteNome}  |  CRMV-{data.CrmvEstado} {data.CrmvNumero}");
            });

            col.Item().PaddingTop(16).Text("RESULTADO DO EXAME").Bold().FontSize(11).FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(6).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                .Padding(10).Text(data.Conteudo).FontSize(10);
        });
    }

    private static void ComposeFooter(IContainer container, LaudoPdfData data)
    {
        container.PaddingTop(8).Column(col =>
        {
            // Assinatura do responsável técnico
            col.Item().AlignCenter().Width(240).BorderTop(0.75f).BorderColor(Colors.Grey.Darken1)
                .PaddingTop(4).AlignCenter().Text(data.ResponsavelTecnicoNome).Bold();
            col.Item().AlignCenter().Text($"Médico(a) Veterinário(a) - Responsável Técnico(a) - {data.ResponsavelTecnicoCrmv}")
                .FontSize(9);
            col.Item().AlignCenter().Text($"Emitido em {data.DataEmissao:dd/MM/yyyy HH:mm} (horário de Brasília)")
                .FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }
}
