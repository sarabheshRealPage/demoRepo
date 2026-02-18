# PowerShell ASCII Color Display Script

function Write-ColorfulBanner {
    Write-Host ""
    Write-Host "  ███████╗██╗  ██╗██╗██╗     ██╗     " -ForegroundColor Red
    Write-Host "  ██╔════╝██║ ██╔╝██║██║     ██║     " -ForegroundColor Yellow
    Write-Host "  ███████╗█████╔╝ ██║██║     ██║     " -ForegroundColor Green
    Write-Host "  ╚════██║██╔═██╗ ██║██║     ██║     " -ForegroundColor Cyan
    Write-Host "  ███████║██║  ██╗██║███████╗███████╗" -ForegroundColor Blue
    Write-Host "  ╚══════╝╚═╝  ╚═╝╚═╝╚══════╝╚══════╝" -ForegroundColor Magenta
    Write-Host ""
}

function Write-SystemInfo {
    Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║     " -NoNewline -ForegroundColor Cyan
    Write-Host "SYSTEM INFORMATION" -NoNewline -ForegroundColor Yellow
    Write-Host "              ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Platform:     " -NoNewline -ForegroundColor Green
    Write-Host "$($PSVersionTable.Platform)" -ForegroundColor White
    
    Write-Host "OS Version:   " -NoNewline -ForegroundColor Green
    Write-Host "$([System.Environment]::OSVersion.VersionString)" -ForegroundColor White
    
    Write-Host "Hostname:     " -NoNewline -ForegroundColor Green
    Write-Host "$env:COMPUTERNAME" -ForegroundColor White
    
    Write-Host "Username:     " -NoNewline -ForegroundColor Green
    Write-Host "$env:USERNAME" -ForegroundColor White
    
    Write-Host "PowerShell:   " -NoNewline -ForegroundColor Green
    Write-Host "$($PSVersionTable.PSVersion)" -ForegroundColor White
    
    $cpu = Get-CimInstance -ClassName Win32_Processor | Select-Object -First 1
    Write-Host "CPU:          " -NoNewline -ForegroundColor Green
    Write-Host "$($cpu.Name)" -ForegroundColor White
    
    $memory = Get-CimInstance -ClassName Win32_ComputerSystem
    $totalMemoryGB = [math]::Round($memory.TotalPhysicalMemory / 1GB, 2)
    Write-Host "Total Memory: " -NoNewline -ForegroundColor Green
    Write-Host "$totalMemoryGB GB" -ForegroundColor White
    Write-Host ""
}

function Write-ColorPalette {
    Write-Host "Color Palette Demo:" -ForegroundColor White
    Write-Host ""
    Write-Host "■ Red     " -NoNewline -ForegroundColor Red
    Write-Host "■ Green   " -NoNewline -ForegroundColor Green
    Write-Host "■ Yellow" -ForegroundColor Yellow
    Write-Host "■ Blue    " -NoNewline -ForegroundColor Blue
    Write-Host "■ Magenta " -NoNewline -ForegroundColor Magenta
    Write-Host "■ Cyan" -ForegroundColor Cyan
    Write-Host ""
}

function Write-SuccessMessage {
    Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "║  " -NoNewline -ForegroundColor Green
    Write-Host "✓" -NoNewline -ForegroundColor Magenta
    Write-Host " Skill Executed Successfully!" -NoNewline -ForegroundColor White
    Write-Host "      ║" -ForegroundColor Green
    Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Green
    Write-Host ""
}

# Execute the display functions
Write-ColorfulBanner
Write-SystemInfo
Write-ColorPalette
Write-SuccessMessage
