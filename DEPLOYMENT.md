# WashBooking - Render Deployment Guide

This guide will help you deploy your WashBooking application to Render.com.

## 📋 Prerequisites

1. **Render Account**: Sign up at https://render.com
2. **GitHub/GitLab Account**: Your code must be in a Git repository
3. **Database**: PostgreSQL database on Render (can be created during setup)

---

## 🚀 Deployment Steps

### Option 1: Using Render Blueprint (Recommended)

1. **Push Your Code to GitHub/GitLab**
   ```bash
   git add .
   git commit -m "Prepare for Render deployment"
   git push origin main
   ```

2. **Deploy Using Blueprint**
   - Go to https://render.com/dashboard
   - Click **"New"** → **"Blueprint"**
   - Connect your GitHub/GitLab repository
   - Render will automatically detect `render.yaml` and create:
     - Web Service (API)
     - PostgreSQL Database
   - Review the configuration and click **"Apply"**

3. **Configure Environment Variables** (if not using render.yaml)
   - Go to your Web Service dashboard
   - Navigate to **"Environment"** tab
   - Add the following environment variables:

### Option 2: Manual Deployment

#### Step 1: Create PostgreSQL Database

1. In Render Dashboard, click **"New"** → **"PostgreSQL"**
2. Configure:
   - **Name**: `washbooking-db`
   - **Database**: `wasing_booking`
   - **User**: Auto-generated
   - **Region**: Choose closest to your users (e.g., Singapore)
   - **Plan**: Free (Starter) or paid
3. Click **"Create Database"**
4. Copy the **Internal Database URL** for later use

#### Step 2: Create Web Service

1. Click **"New"** → **"Web Service"**
2. Connect your Git repository
3. Configure:
   - **Name**: `washbooking-api`
   - **Region**: Same as database
   - **Branch**: `main` (or your default branch)
   - **Runtime**: **Docker**
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: Starter ($7/month) or higher

#### Step 3: Configure Environment Variables

Add these in the **"Environment"** section:

| Variable Name | Value | Notes |
|--------------|-------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | |
| `ASPNETCORE_URLS` | `http://+:8080` | Port must be 8080 |
| `ConnectionStrings__DefaultConnectionStringDB` | (Your DB Internal URL) | From Step 1 |
| `JwtSettings__SecretKey` | (Generate secure key) | Use `openssl rand -base64 32` |
| `JwtSettings__Issuer` | `WashBooking.API` | |
| `JwtSettings__Audience` | `WashBooking.Clients` | |
| `JwtSettings__TokenExpirationInMinutes` | `30` | |
| `JwtSettings__RefreshTokenExpirationInDays` | `7` | |
| `EnableSwagger` | `true` | Set `false` for production |
| `BookingSettings__MaxConcurrentBookings` | `5` | |
| `BookingSettings__TransitionBufferInMinutes` | `15` | |

> **Note**: Use double underscores `__` to represent nested JSON configuration in .NET

#### Step 4: Deploy

1. Click **"Create Web Service"**
2. Render will:
   - Build your Docker image
   - Deploy the container
   - Provide a public URL (e.g., `https://washbooking-api.onrender.com`)

---

## 🔒 Security Recommendations

### 1. Generate Secure JWT Secret Key

```bash
# On Linux/Mac
openssl rand -base64 32

# On Windows (PowerShell)
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```

### 2. Disable Swagger in Production

Set `EnableSwagger` to `false` in environment variables.

### 3. Use Environment Variables for Secrets

**Never commit** sensitive data like:
- Database connection strings
- JWT secret keys
- API keys

Always use Render's environment variables feature.

### 4. Configure CORS (if needed)

Add CORS policy in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...

app.UseCors("ProductionPolicy");
```

---

## 🔍 Monitoring & Debugging

### Check Deployment Logs

1. Go to your Web Service dashboard
2. Click **"Logs"** tab
3. Monitor real-time logs during deployment

### Health Check

Render will automatically check your service health using:
- URL: `https://your-service.onrender.com/swagger/index.html`
- Interval: Every 30 seconds

### Database Migrations

If you need to run EF Core migrations:

```bash
# Locally, targeting production database
dotnet ef database update --connection "YOUR_PRODUCTION_CONNECTION_STRING"
```

> ⚠️ **Warning**: Be careful when running migrations on production!

---

## 📊 Performance Optimization

### 1. Use Alpine Linux

The Dockerfile already uses `aspnet:8.0-alpine` (smaller image size).

### 2. Docker Layer Caching

The Dockerfile is optimized for layer caching:
- Dependencies are restored before copying source code
- Only changed layers are rebuilt

### 3. Database Connection Pooling

Already enabled by default in EF Core.

### 4. Upgrade Render Plan

For production workloads:
- **Standard Plan**: More memory and CPU
- **Pro Plan**: Auto-scaling and zero-downtime deploys

---

## 🆘 Troubleshooting

### Issue: Container Fails to Start

**Check**:
1. Logs for error messages
2. Port configuration (must be 8080)
3. Database connection string format

### Issue: Database Connection Failed

**Check**:
1. Connection string format (PostgreSQL style)
2. Database is in the same region
3. Use **Internal Database URL** (faster, free traffic)

### Issue: 502 Bad Gateway

**Possible Causes**:
- Application crashed during startup
- Health check endpoint unreachable
- Port mismatch

**Solution**:
- Check logs for startup errors
- Verify `ASPNETCORE_URLS=http://+:8080`
- Ensure health check path exists

### Issue: Slow First Request (Cold Start)

Render's free tier spins down after 15 minutes of inactivity.

**Solution**:
- Upgrade to paid plan (never spins down)
- Use external uptime monitor (e.g., UptimeRobot)

---

## 📝 Post-Deployment Checklist

- [ ] Test all API endpoints
- [ ] Verify database connectivity
- [ ] Test authentication (login/register)
- [ ] Check Swagger UI (if enabled)
- [ ] Monitor logs for errors
- [ ] Set up custom domain (optional)
- [ ] Configure SSL/TLS (automatic on Render)
- [ ] Set up monitoring/alerts
- [ ] Create database backups schedule

---

## 🌐 Custom Domain (Optional)

1. Go to Web Service → **"Settings"** → **"Custom Domains"**
2. Click **"Add Custom Domain"**
3. Enter your domain (e.g., `api.washbooking.com`)
4. Update DNS records as instructed by Render
5. Render will automatically provision SSL certificate

---

## 🔄 Continuous Deployment

Once set up, Render will automatically:
1. Detect commits to your Git branch
2. Build new Docker image
3. Deploy with zero downtime (on paid plans)
4. Rollback if health checks fail

---

## 📞 Support

- **Render Docs**: https://render.com/docs
- **Render Community**: https://community.render.com
- **ASP.NET Core Docs**: https://learn.microsoft.com/aspnet/core

---

## 🎉 Success!

Your WashBooking API should now be live at:
`https://your-service-name.onrender.com`

Test your API:
```bash
curl https://your-service-name.onrender.com/swagger/index.html
```
