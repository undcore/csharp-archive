using System.Net;
using System.Net.Mail;
using BlazorWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorWeb.Services;

public sealed class SmtpEmailSender(
    IOptions<SmtpEmailOptions> smtpEmailOptions,
    ILogger<SmtpEmailSender> logger) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser applicationUser, string sEmail, string sConfirmationLink)
    {
        string sBody = $"<p>아래 링크를 눌러 이메일 인증을 완료하세요.</p><p><a href=\"{sConfirmationLink}\">이메일 인증하기</a></p>";

        return SendEmailAsync(sEmail, "Dev Archive 이메일 인증", sBody);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser applicationUser, string sEmail, string sResetLink)
    {
        string sBody = $"<p>아래 링크를 눌러 비밀번호를 재설정하세요.</p><p><a href=\"{sResetLink}\">비밀번호 재설정</a></p>";

        return SendEmailAsync(sEmail, "Dev Archive 비밀번호 재설정", sBody);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser applicationUser, string sEmail, string sResetCode)
    {
        string sBody = $"<p>비밀번호 재설정 코드: <strong>{sResetCode}</strong></p>";

        return SendEmailAsync(sEmail, "Dev Archive 비밀번호 재설정 코드", sBody);
    }

    private async Task SendEmailAsync(string sEmail, string sSubject, string sHtmlBody)
    {
        SmtpEmailOptions emailOptions = smtpEmailOptions.Value;

        if (string.IsNullOrWhiteSpace(emailOptions.Host))
        {
            logger.LogWarning("SMTP is not configured. Email to {Email}: {Subject} {Body}", sEmail, sSubject, sHtmlBody);
            return;
        }

        using MailMessage mailMessage = new()
        {
            From = new MailAddress(emailOptions.FromEmail, emailOptions.FromName),
            Subject = sSubject,
            Body = sHtmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(sEmail);

        using SmtpClient smtpClient = new(emailOptions.Host, emailOptions.Port)
        {
            EnableSsl = emailOptions.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(emailOptions.UserName))
        {
            smtpClient.Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password);
        }

        await smtpClient.SendMailAsync(mailMessage);
    }
}
