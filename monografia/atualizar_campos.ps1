param(
    [Parameter(Mandatory = $true)][string]$Docx,
    [string]$Pdf
)
# Abre o .docx no Word, atualiza sumário, listas de ilustrações e campos, salva e exporta o PDF.
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open($Docx, $false, $false)
    # Sumário: primeiro nível em negrito; demais níveis e listas em texto normal (Times 12)
    foreach ($id in -20, -21, -22, -36) {
        try {
            $st = $doc.Styles.Item($id)
            $st.Font.Name = "Times New Roman"
            $st.Font.Size = 12
            $st.Font.Bold = [int]($id -eq -20)
            $st.ParagraphFormat.SpaceBefore = 0
            $st.ParagraphFormat.SpaceAfter = 0
        } catch { }
    }
    $doc.Fields.Update() | Out-Null
    foreach ($t in $doc.TablesOfContents) { $t.Update() | Out-Null }
    foreach ($t in $doc.TablesOfFigures) { $t.Update() | Out-Null }
    $doc.Repaginate()
    foreach ($t in $doc.TablesOfContents) { $t.UpdatePageNumbers() | Out-Null }
    foreach ($t in $doc.TablesOfFigures) { $t.UpdatePageNumbers() | Out-Null }
    $doc.Save()
    if ($Pdf) { $doc.ExportAsFixedFormat($Pdf, 17) }
    $paginas = $doc.ComputeStatistics(2)
    $doc.Close([ref]0)
    "Word: campos atualizados; $paginas páginas" + $(if ($Pdf) { "; PDF exportado" } else { "" })
}
finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
