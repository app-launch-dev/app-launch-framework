using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AppLaunch.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AppLaunch.Services;

public class AwsSesIdentityEmailService(ISettingsService settingsService, IEmailSender emailSender) : IEmailSender<ApplicationUser>
{
    
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        await emailSender.SendEmailAsync(email,"Confirm your account.", $"Welcome to AppLaunch, {user.FirstName}.<br /><br />Click this link to confirm your account: <a href='{confirmationLink}'>{confirmationLink}</a>.");
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        await emailSender.SendEmailAsync(email,"Password reset request.", $"Hi, {user.FirstName}.<br /><br />Click this link to reset your account: <a href='{resetLink}'>{resetLink}</a>.");
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        await emailSender.SendEmailAsync(email,"Password reset code request.", $"Hi, {user.FirstName}.<br /><br />Here is your reset code: {resetCode}.");
    }
}