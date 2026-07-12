[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Position = 0)]
    [string]$ProjectPath
)

$ErrorActionPreference = 'Stop'
$Version = '1.2.0'

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot 'CloudflarePolicyUpdater\CloudflarePolicyUpdater.csproj'
}

$resolvedProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
$versionParts = $Version.Split('.')

foreach ($part in $versionParts) {
    [UInt16]$value = 0
    if (-not [UInt16]::TryParse($part, [ref]$value)) {
        throw "Each version component must be between 0 and 65535. Invalid version: '$Version'."
    }
}

$content = [System.IO.File]::ReadAllText($resolvedProjectPath)
$properties = @('FileVersion', 'AssemblyVersion')

foreach ($property in $properties) {
    $pattern = "(<$property>)[^<]*(</$property>)"
    $matches = [regex]::Matches($content, $pattern)

    if ($matches.Count -ne 1) {
        throw "Expected exactly one <$property> element in '$resolvedProjectPath', but found $($matches.Count)."
    }

    $content = [regex]::Replace(
        $content,
        $pattern,
        { param($match) "$($match.Groups[1].Value)$Version$($match.Groups[2].Value)" }
    )
}

if ($PSCmdlet.ShouldProcess($resolvedProjectPath, "Set FileVersion and AssemblyVersion to $Version")) {
    [System.IO.File]::WriteAllText($resolvedProjectPath, $content)
    Write-Host "Updated '$resolvedProjectPath' to version $Version."
}
