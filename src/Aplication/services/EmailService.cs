using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
        {
            Port = int.TryParse(_configuration["Email:Port"], out var port) ? port : 25,
            Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:Username"] ?? throw new ArgumentNullException("Email:Username configuration is missing")),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }
    }
}