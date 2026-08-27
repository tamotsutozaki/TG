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

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().Element(c => ComposeFooter(c, data));
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingBottom(8).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("LAUDO DE EXAME LABORATORIAL")
                    .Bold().FontSize(14).FontColor(Colors.Grey.Darken3);
                col.Item().Text("Laboratório de Patologia Veterinária")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
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
                row.RelativeItem().Text($"{data.VetSolicitanteNome}  |  CRMV {data.CrmvEstado}-{data.CrmvNumero}");
            });

            col.Item().PaddingTop(16).Text("RESULTADO DO EXAME").Bold().FontSize(11).FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(6).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                .Padding(10).Text(data.Conteudo).FontSize(10);
        });
    }

    private static void ComposeFooter(IContainer container, LaudoPdfData data)
    {
        container.BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(8).Column(col =>
        {
            col.Item().AlignCenter().Text(data.PatologistaNome).Bold();
            col.Item().AlignCenter().Text("Médico Veterinário Patologista").FontSize(9);
            col.Item().AlignCenter().Text($"Emitido em {data.DataEmissao:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }
}
