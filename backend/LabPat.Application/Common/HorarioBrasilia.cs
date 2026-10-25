namespace LabPat.Application.Common;

/// <summary>
/// As datas são gravadas em UTC; documentos gerados no servidor (como o PDF do laudo)
/// precisam exibi-las no horário de Brasília.
/// </summary>
public static class HorarioBrasilia
{
    private static readonly TimeZoneInfo Fuso = ObterFuso();

    public static DateTime DeUtc(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Fuso);

    private static TimeZoneInfo ObterFuso()
    {
        // Id IANA (Linux e Windows com ICU) e, por garantia, o Id do Windows
        foreach (var id in new[] { "America/Sao_Paulo", "E. South America Standard Time" })
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var fuso))
                return fuso;
        }

        // Sem horário de verão desde 2019
        return TimeZoneInfo.CreateCustomTimeZone("BRT", TimeSpan.FromHours(-3), "Brasília", "Brasília");
    }
}
