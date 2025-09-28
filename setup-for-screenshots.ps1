# GoodsEnterprise Screenshot Setup Script
# This script prepares the application for screenshot capture

param(
    [string]$ProjectPath = "e:\GoodsEnterprise\New",
    [switch]$StopProcesses = $false,
    [switch]$BuildOnly = $false,
    [switch]$RunApp = $false
)

Write-Host "=== GoodsEnterprise Screenshot Setup ===" -ForegroundColor Green
Write-Host "Project Path: $ProjectPath" -ForegroundColor Yellow

# Function to stop running processes
function Stop-RunningProcesses {
    Write-Host "`n1. Stopping running processes..." -ForegroundColor Cyan
    
    # Get all dotnet processes
    $dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    
    if ($dotnetProcesses) {
        Write-Host "Found $($dotnetProcesses.Count) dotnet processes. Stopping them..." -ForegroundColor Yellow
        foreach ($process in $dotnetProcesses) {
            try {
                $process.Kill()
                Write-Host "Stopped process ID: $($process.Id)" -ForegroundColor Green
            }
            catch {
                Write-Host "Could not stop process ID: $($process.Id)" -ForegroundColor Red
            }
        }
        Start-Sleep -Seconds 3
    }
    else {
        Write-Host "No dotnet processes found running." -ForegroundColor Green
    }
    
    # Check for Visual Studio processes
    $vsProcesses = Get-Process -Name "devenv" -ErrorAction SilentlyContinue
    if ($vsProcesses) {
        Write-Host "Warning: Visual Studio is running. Please close it manually to avoid file locking issues." -ForegroundColor Yellow
    }
}

# Function to build the application
function Build-Application {
    Write-Host "`n2. Building the application..." -ForegroundColor Cyan
    
    Set-Location $ProjectPath
    
    # Clean the solution first
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    $cleanResult = dotnet clean
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Clean failed, but continuing..." -ForegroundColor Yellow
    }
    
    # Build the solution
    Write-Host "Building solution..." -ForegroundColor Yellow
    $buildResult = dotnet build --configuration Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build successful!" -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
        return $false
    }
}

# Function to run the application
function Start-Application {
    Write-Host "`n3. Starting the application..." -ForegroundColor Cyan
    
    $webProjectPath = Join-Path $ProjectPath "GoodsEnterprise.Web"
    
    if (Test-Path $webProjectPath) {
        Set-Location $webProjectPath
        
        Write-Host "Starting application from: $webProjectPath" -ForegroundColor Yellow
        Write-Host "The application will start in a new window..." -ForegroundColor Yellow
        Write-Host "Press Ctrl+C in that window to stop the application when done." -ForegroundColor Yellow
        
        # Start the application in a new PowerShell window
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$webProjectPath'; dotnet run"
        
        Start-Sleep -Seconds 5
        
        Write-Host "`nApplication should be starting..." -ForegroundColor Green
        Write-Host "Check these URLs:" -ForegroundColor Yellow
        Write-Host "  HTTP:  http://localhost:5000" -ForegroundColor Cyan
        Write-Host "  HTTPS: https://localhost:5001" -ForegroundColor Cyan
        
        return $true
    }
    else {
        Write-Host "Web project not found at: $webProjectPath" -ForegroundColor Red
        return $false
    }
}

# Function to create screenshot directories
function Create-ScreenshotDirectories {
    Write-Host "`n4. Creating screenshot directories..." -ForegroundColor Cyan
    
    $screenshotBase = Join-Path $ProjectPath "Screenshots"
    
    $directories = @(
        "01-Authentication",
        "02-Product-Management", 
        "03-Supplier-Management",
        "04-Customer-Management",
        "05-Cost-Management",
        "06-Category-Brand",
        "07-Admin-Features",
        "08-Error-Handling",
        "09-Responsive-Design",
        "10-Advanced-Features"
    )
    
    foreach ($dir in $directories) {
        $fullPath = Join-Path $screenshotBase $dir
        if (!(Test-Path $fullPath)) {
            New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
            Write-Host "Created: $dir" -ForegroundColor Green
        }
        else {
            Write-Host "Exists: $dir" -ForegroundColor Yellow
        }
    }
    
    Write-Host "Screenshot directories ready at: $screenshotBase" -ForegroundColor Green
}

# Function to check prerequisites
function Check-Prerequisites {
    Write-Host "`n0. Checking prerequisites..." -ForegroundColor Cyan
    
    # Check if .NET is installed
    try {
        $dotnetVersion = dotnet --version
        Write-Host ".NET Version: $dotnetVersion" -ForegroundColor Green
    }
    catch {
        Write-Host ".NET is not installed or not in PATH!" -ForegroundColor Red
        return $false
    }
    
    # Check if project exists
    if (!(Test-Path $ProjectPath)) {
        Write-Host "Project path does not exist: $ProjectPath" -ForegroundColor Red
        return $false
    }
    
    # Check if solution file exists
    $solutionFile = Get-ChildItem -Path $ProjectPath -Filter "*.sln" | Select-Object -First 1
    if (!$solutionFile) {
        Write-Host "No solution file found in: $ProjectPath" -ForegroundColor Red
        return $false
    }
    
    Write-Host "Solution file: $($solutionFile.Name)" -ForegroundColor Green
    return $true
}

# Function to show next steps
function Show-NextSteps {
    Write-Host "`n=== NEXT STEPS FOR SCREENSHOT CAPTURE ===" -ForegroundColor Green
    Write-Host ""
    Write-Host "1. VERIFY APPLICATION IS RUNNING:" -ForegroundColor Yellow
    Write-Host "   - Open browser and go to: https://localhost:5001" -ForegroundColor Cyan
    Write-Host "   - Login with admin credentials" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. PREPARE SAMPLE DATA:" -ForegroundColor Yellow
    Write-Host "   - Add sample products, suppliers, customers" -ForegroundColor Cyan
    Write-Host "   - Create test scenarios for error states" -ForegroundColor Cyan
    Write-Host "   - Prepare Excel files for import testing" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "3. CAPTURE SCREENSHOTS:" -ForegroundColor Yellow
    Write-Host "   - Follow the SCREENSHOT_CAPTURE_GUIDE.md" -ForegroundColor Cyan
    Write-Host "   - Use consistent browser settings (Chrome/Edge, 100% zoom)" -ForegroundColor Cyan
    Write-Host "   - Capture in order: Login -> Products -> Suppliers -> etc." -ForegroundColor Cyan
    Write-Host ""
    Write-Host "4. SCREENSHOT LOCATIONS:" -ForegroundColor Yellow
    Write-Host "   - Save to: $ProjectPath\Screenshots\" -ForegroundColor Cyan
    Write-Host "   - Use organized folder structure created above" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "5. WHEN FINISHED:" -ForegroundColor Yellow
    Write-Host "   - Stop the application (Ctrl+C in the application window)" -ForegroundColor Cyan
    Write-Host "   - Review and organize all screenshots" -ForegroundColor Cyan
    Write-Host "   - Add screenshots to the comprehensive documentation" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "=== DOCUMENTATION FILES CREATED ===" -ForegroundColor Green
    Write-Host "- COMPREHENSIVE_APPLICATION_DOCUMENTATION.md (Main documentation)" -ForegroundColor Cyan
    Write-Host "- SCREENSHOT_CAPTURE_GUIDE.md (Screenshot guidelines)" -ForegroundColor Cyan
    Write-Host "- setup-for-screenshots.ps1 (This setup script)" -ForegroundColor Cyan
}

# Main execution
try {
    # Check prerequisites
    if (!(Check-Prerequisites)) {
        Write-Host "Prerequisites check failed. Please fix the issues above." -ForegroundColor Red
        exit 1
    }
    
    # Stop processes if requested or if we're going to build/run
    if ($StopProcesses -or !$BuildOnly) {
        Stop-RunningProcesses
    }
    
    # Build the application
    $buildSuccess = Build-Application
    
    if (!$buildSuccess) {
        Write-Host "`nBuild failed. Cannot proceed with running the application." -ForegroundColor Red
        Write-Host "Please fix build errors and try again." -ForegroundColor Yellow
        exit 1
    }
    
    # Create screenshot directories
    Create-ScreenshotDirectories
    
    # Run the application if requested
    if ($RunApp -and !$BuildOnly) {
        $runSuccess = Start-Application
        
        if ($runSuccess) {
            Write-Host "`nApplication setup complete!" -ForegroundColor Green
        }
        else {
            Write-Host "`nApplication failed to start." -ForegroundColor Red
        }
    }
    
    # Show next steps
    Show-NextSteps
    
    Write-Host "`n=== SETUP COMPLETE ===" -ForegroundColor Green
    
}
catch {
    Write-Host "An error occurred: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Usage examples at the end
Write-Host "`n=== SCRIPT USAGE EXAMPLES ===" -ForegroundColor Magenta
Write-Host ".\setup-for-screenshots.ps1                    # Full setup (build + create directories)" -ForegroundColor Gray
Write-Host ".\setup-for-screenshots.ps1 -StopProcesses     # Stop processes and build" -ForegroundColor Gray
Write-Host ".\setup-for-screenshots.ps1 -BuildOnly         # Only build, don't run" -ForegroundColor Gray
Write-Host ".\setup-for-screenshots.ps1 -RunApp            # Build and run application" -ForegroundColor Gray
