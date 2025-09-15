# Gmail Email Service Setup Instructions

## Overview
The application has been successfully converted from SendGrid to Gmail SMTP for sending password reset emails. This document explains how to configure and use the new Gmail email service.

## Changes Made

### 1. New Service Created
- **File**: `Services/GmailEmailService.cs`
- **Purpose**: Replaces SendGridEmailService with Gmail SMTP functionality
- **Features**: 
  - Uses standard SMTP protocol
  - Supports HTML email templates
  - Includes proper error handling and logging
  - Maintains the same interface as the previous SendGrid service

### 2. Dependency Injection Updated
- **File**: `Startup.cs` (line 54)
- **Change**: `services.AddTransient<IEmailService, GmailEmailService>();`
- **Impact**: Application now uses Gmail service instead of SendGrid

### 3. Configuration Updated
- **Files**: `appsettings.json` and `appsettings.Development.json`
- **Section**: Replaced `SendGrid` configuration with `Gmail` configuration

## Gmail Configuration Setup

### Step 1: Enable 2-Factor Authentication
1. Go to your Google Account settings
2. Navigate to Security > 2-Step Verification
3. Enable 2-Step Verification if not already enabled

### Step 2: Generate App Password
1. In Google Account Security settings
2. Go to "App passwords" section
3. Generate a new app password for "Mail"
4. Copy the 16-character app password (this will be used in configuration)

### Step 3: Update Configuration Files

#### appsettings.json
```json
{
  "Gmail": {
    "FromEmail": "your-actual-gmail@gmail.com",
    "FromName": "Redneval Support",
    "Password": "your-16-character-app-password",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587"
  }
}
```

#### appsettings.Development.json
```json
{
  "Gmail": {
    "FromEmail": "your-dev-gmail@gmail.com",
    "FromName": "Redneval Support (Dev)",
    "Password": "your-16-character-app-password",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587"
  }
}
```

## Security Considerations

### 1. App Passwords
- **Never use your regular Gmail password**
- **Always use App Passwords** generated specifically for this application
- App passwords are 16 characters long and look like: `abcd efgh ijkl mnop`

### 2. Configuration Security
- **Never commit real passwords to source control**
- Consider using Azure Key Vault or similar for production
- Use different Gmail accounts for development and production

### 3. Environment Variables (Recommended for Production)
Instead of storing passwords in appsettings.json, use environment variables:

```json
{
  "Gmail": {
    "FromEmail": "your-gmail@gmail.com",
    "FromName": "Redneval Support",
    "Password": "",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587"
  }
}
```

Then set environment variable: `Gmail__Password=your-app-password`

## Testing the Email Service

### 1. Build and Run
```bash
dotnet build
dotnet run
```

### 2. Test Password Reset
1. Navigate to the password reset page
2. Enter a valid email address
3. Check the configured Gmail account's sent items
4. Verify the email was sent successfully

### 3. Check Logs
Monitor the application logs for any SMTP-related errors:
- Success: "Password reset email sent successfully to {email}"
- Error: "SMTP error sending password reset email to {email}"

## Troubleshooting

### Common Issues

#### 1. Authentication Failed
- **Cause**: Incorrect app password or 2FA not enabled
- **Solution**: Regenerate app password, ensure 2FA is enabled

#### 2. SMTP Connection Issues
- **Cause**: Firewall blocking port 587
- **Solution**: Ensure port 587 is open for outbound connections

#### 3. "Less Secure Apps" Error
- **Cause**: Using regular password instead of app password
- **Solution**: Use app password, not regular Gmail password

#### 4. Rate Limiting
- **Cause**: Sending too many emails too quickly
- **Solution**: Gmail has sending limits; implement rate limiting if needed

### Error Messages and Solutions

| Error | Cause | Solution |
|-------|--------|----------|
| "Gmail password is not configured" | Missing password in config | Add app password to configuration |
| "SMTP authentication failed" | Wrong credentials | Verify email and app password |
| "Connection timeout" | Network/firewall issue | Check network connectivity |
| "Mailbox unavailable" | Invalid recipient email | Verify recipient email address |

## Email Template

The service uses the same HTML email template as the previous SendGrid implementation:
- Professional Redneval branding
- Responsive design
- Clear call-to-action button
- Security messaging about link expiration

## Migration Notes

### What Stayed the Same
- `IEmailService` interface unchanged
- Email template design unchanged
- Password reset functionality unchanged
- Error handling patterns maintained

### What Changed
- SMTP instead of API-based sending
- Configuration structure (Gmail vs SendGrid)
- Authentication method (app password vs API key)
- Dependency injection registration

## Production Deployment Checklist

- [ ] Create dedicated Gmail account for production
- [ ] Enable 2-Factor Authentication
- [ ] Generate app password
- [ ] Update production configuration
- [ ] Test email sending in production environment
- [ ] Monitor logs for any issues
- [ ] Set up email monitoring/alerting

## Support

For issues with the Gmail email service:
1. Check application logs first
2. Verify Gmail account configuration
3. Test SMTP connectivity manually if needed
4. Review this documentation for common solutions
