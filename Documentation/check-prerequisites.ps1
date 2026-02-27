# Family Tree Project - Prerequisites Check Script (Windows PowerShell)
# Run this script to verify you have all required software installed

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Family Tree Project - Prerequisites Check" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

$allGood = $true

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion) {
        Write-Host "  ✓ .NET SDK installed: $dotnetVersion" -ForegroundColor Green
        
        # Check if it's .NET 10 or later
        $majorVersion = [int]($dotnetVersion.Split('.')[0])
        if ($majorVersion -lt 10) {
            Write-Host "  ⚠ Warning: .NET $majorVersion detected. This project requires .NET 10 or later." -ForegroundColor Yellow
            Write-Host "    Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
            $allGood = $false
        }
    }
} catch {
    Write-Host "  ✗ .NET SDK not found" -ForegroundColor Red
    Write-Host "    Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    $allGood = $false
}

Write-Host ""

# Check Node.js
Write-Host "Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version 2>$null
    if ($nodeVersion) {
        Write-Host "  ✓ Node.js installed: $nodeVersion" -ForegroundColor Green
        
        # Check if it's v18 or later
        $majorVersion = [int]($nodeVersion.TrimStart('v').Split('.')[0])
        if ($majorVersion -lt 18) {
            Write-Host "  ⚠ Warning: Node.js $nodeVersion detected. Recommended: v18 or later." -ForegroundColor Yellow
            Write-Host "    Download from: https://nodejs.org/" -ForegroundColor Yellow
        }
    }
} catch {
    Write-Host "  ✗ Node.js not found" -ForegroundColor Red
    Write-Host "    Download from: https://nodejs.org/" -ForegroundColor Yellow
    $allGood = $false
}

Write-Host ""

# Check npm
Write-Host "Checking npm..." -ForegroundColor Yellow
try {
    $npmVersion = npm --version 2>$null
    if ($npmVersion) {
        Write-Host "  ✓ npm installed: $npmVersion" -ForegroundColor Green
    }
} catch {
    Write-Host "  ✗ npm not found (should come with Node.js)" -ForegroundColor Red
    $allGood = $false
}

Write-Host ""

# Check PostgreSQL
Write-Host "Checking PostgreSQL..." -ForegroundColor Yellow
try {
    $pgVersion = psql --version 2>$null
    if ($pgVersion) {
        Write-Host "  ✓ PostgreSQL installed: $pgVersion" -ForegroundColor Green
    }
} catch {
    Write-Host "  ✗ PostgreSQL not found" -ForegroundColor Red
    Write-Host "    Download from: https://www.postgresql.org/download/" -ForegroundColor Yellow
    Write-Host "    OR use Docker: docker run --name familytree-postgres -e POSTGRES_PASSWORD=yourpassword -p 5432:5432 -d postgres" -ForegroundColor Yellow
    $allGood = $false
}

Write-Host ""

# Check Git
Write-Host "Checking Git..." -ForegroundColor Yellow
try {
    $gitVersion = git --version 2>$null
    if ($gitVersion) {
        Write-Host "  ✓ Git installed: $gitVersion" -ForegroundColor Green
    }
} catch {
    Write-Host "  ✗ Git not found" -ForegroundColor Red
    Write-Host "    Download from: https://git-scm.com/download/win" -ForegroundColor Yellow
    $allGood = $false
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan

# Check Entity Framework Core tools
Write-Host "Checking EF Core Tools (optional but recommended)..." -ForegroundColor Yellow
try {
    $efVersion = dotnet ef --version 2>$null
    if ($efVersion) {
        Write-Host "  ✓ EF Core Tools installed: $efVersion" -ForegroundColor Green
    }
} catch {
    Write-Host "  ℹ EF Core Tools not found (optional)" -ForegroundColor Yellow
    Write-Host "    Install with: dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

if ($allGood) {
    Write-Host "✓ All required prerequisites are installed!" -ForegroundColor Green
    Write-Host "You're ready to start building the project!" -ForegroundColor Green
} else {
    Write-Host "⚠ Some prerequisites are missing. Please install them before continuing." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Optional: Install EF Core Tools if not already installed:" -ForegroundColor Cyan
Write-Host "  dotnet tool install --global dotnet-ef" -ForegroundColor White
Write-Host ""

# Pause so user can read the output
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
