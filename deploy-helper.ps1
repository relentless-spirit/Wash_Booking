# WashBooking Deployment Helper Script
# This script helps with deployment tasks

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("generate-jwt", "test-docker", "help")]
    [string]$Action = "help"
)

function Show-Help {
    Write-Host @"
╔═══════════════════════════════════════════════════════════╗
║       WashBooking Deployment Helper                      ║
╚═══════════════════════════════════════════════════════════╝

Usage: .\deploy-helper.ps1 -Action <action>

Available Actions:
  generate-jwt   - Generate a secure JWT secret key
  test-docker    - Build and test Docker image locally
  help          - Show this help message

Examples:
  .\deploy-helper.ps1 -Action generate-jwt
  .\deploy-helper.ps1 -Action test-docker

"@
}

function Generate-JWTSecret {
    Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║       Generating Secure JWT Secret Key                   ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
    
    # Generate 32 random bytes and convert to Base64
    $bytes = New-Object byte[] 32
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $rng.GetBytes($bytes)
    $secretKey = [Convert]::ToBase64String($bytes)
    
    Write-Host "✅ Generated JWT Secret Key:" -ForegroundColor Green
    Write-Host ""
    Write-Host "   $secretKey" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "📋 Copy this to your Render environment variable:" -ForegroundColor Cyan
    Write-Host "   Variable Name: JwtSettings__SecretKey" -ForegroundColor White
    Write-Host "   Variable Value: $secretKey" -ForegroundColor White
    Write-Host ""
    
    # Copy to clipboard if available
    try {
        Set-Clipboard -Value $secretKey
        Write-Host "✅ Secret key copied to clipboard!" -ForegroundColor Green
    }
    catch {
        Write-Host "ℹ️  Couldn't copy to clipboard (manual copy required)" -ForegroundColor Yellow
    }
}

function Test-DockerImage {
    Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║       Testing Docker Image Locally                       ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
    
    # Check if Docker is running
    Write-Host "🔍 Checking Docker..." -ForegroundColor Cyan
    try {
        docker version | Out-Null
        Write-Host "✅ Docker is running" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
        return
    }
    
    Write-Host ""
    Write-Host "🏗️  Building Docker image..." -ForegroundColor Cyan
    Write-Host ""
    
    # Build the image
    $imageName = "washbooking-api:test"
    docker build -t $imageName .
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "❌ Docker build failed. Please check the errors above." -ForegroundColor Red
        return
    }
    
    Write-Host ""
    Write-Host "✅ Docker image built successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Image Information:" -ForegroundColor Cyan
    docker images $imageName
    
    Write-Host ""
    Write-Host "🚀 To run the container locally, use:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host @"
docker run -d \
  -p 8080:8080 \
  --name washbooking-test \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnectionStringDB="YOUR_DB_CONNECTION_STRING" \
  -e JwtSettings__SecretKey="YOUR_JWT_SECRET" \
  -e JwtSettings__Issuer="WashBooking.API" \
  -e JwtSettings__Audience="WashBooking.Clients" \
  -e JwtSettings__TokenExpirationInMinutes=30 \
  -e JwtSettings__RefreshTokenExpirationInDays=7 \
  -e EnableSwagger=true \
  $imageName
"@ -ForegroundColor Yellow
    
    Write-Host ""
    Write-Host "📝 After running, access your API at: http://localhost:8080/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "🛑 To stop and remove the container:" -ForegroundColor Cyan
    Write-Host "   docker stop washbooking-test && docker rm washbooking-test" -ForegroundColor Yellow
    Write-Host ""
}

# Main script execution
switch ($Action) {
    "generate-jwt" {
        Generate-JWTSecret
    }
    "test-docker" {
        Test-DockerImage
    }
    "help" {
        Show-Help
    }
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
