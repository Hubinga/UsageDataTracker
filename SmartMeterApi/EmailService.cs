namespace SmartMeterApi
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using System;
    using System.Net.Mail;

    public class EmailService
    {
        private readonly string _smtpServer = "smtp-mail.outlook.com"; 
        private readonly int _smtpPort = 587; 
        private readonly string _smtpUsername = "";
        private readonly string _smtpPassword = "";

        public void SendOtpAsEmail(string otp, string userEmail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Smart Meter Gateway API", _smtpUsername)); // Absender Name und Adresse
                message.To.Add(new MailboxAddress("", userEmail)); // Empfänger E-Mail Adresse
                message.Subject = "Ihr OTP-Code für die Anmeldung";

                message.Body = new TextPart("plain")
                {
                    Text = $"Ihr OTP-Code lautet: {otp}"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    ///TODO: nur für Testumgebung, nciht für produktiven Einsatz! Umgehung der Zertifikatsvalidierung
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.StartTls); // Outlook.com SMTP-Server und Port
                    client.Authenticate(_smtpUsername, _smtpPassword); // Outlook.com Benutzername und Passwort
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log, throw, etc.)
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

    }

}
