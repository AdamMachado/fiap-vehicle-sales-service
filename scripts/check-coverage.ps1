param(
    [Parameter(Mandatory = $false)]
    [string] $ResultsPath = "TestResults",

    [Parameter(Mandatory = $false)]
    [double] $Minimum = 80
)

$ErrorActionPreference = "Stop"
$reports = Get-ChildItem -LiteralPath $ResultsPath -Recurse -Filter "coverage.cobertura.xml"

if ($reports.Count -eq 0) {
    throw "Nenhum relatório Cobertura foi encontrado em '$ResultsPath'."
}

$lines = @{}

foreach ($report in $reports) {
    [xml] $document = Get-Content -LiteralPath $report.FullName

    foreach ($class in $document.coverage.packages.package.classes.class) {
        $file = ([string] $class.filename).Replace("\", "/")

        if ($file -match "/obj/|/Migrations/|/Program\.cs$|/DependencyInjection/") {
            continue
        }

        foreach ($line in $class.lines.line) {
            $key = "$file`:$($line.number)"
            $hits = [int] $line.hits

            if (-not $lines.ContainsKey($key) -or $hits -gt $lines[$key]) {
                $lines[$key] = $hits
            }
        }
    }
}

if ($lines.Count -eq 0) {
    throw "Os relatórios não contêm linhas de código elegíveis."
}

$covered = @($lines.Values | Where-Object { $_ -gt 0 }).Count
$percentage = [Math]::Round(($covered / $lines.Count) * 100, 2)
$summary = "Cobertura consolidada: $percentage% ($covered/$($lines.Count) linhas). Mínimo: $Minimum%."

Write-Output $summary

if ($env:GITHUB_STEP_SUMMARY) {
    Add-Content -LiteralPath $env:GITHUB_STEP_SUMMARY -Value "## Cobertura de testes`n`n$summary"
}

if ($percentage -lt $Minimum) {
    throw "Cobertura abaixo do mínimo obrigatório de $Minimum%."
}
