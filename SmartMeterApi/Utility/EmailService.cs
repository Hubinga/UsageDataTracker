namespace SmartMeterApi.Utility
{
    using MailKit.Security;
    using MimeKit;
    using System;

    public class EmailService
    {
        private readonly string _smtpServer = "smtp-mail.outlook.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        /*Sicherheitsprinzip:
          - Vermeidung der festen Codierung von Schlüsseln und Anmeldedaten
         */
        public EmailService()
        {
            // Get SMTP settings from environment variables
            _smtpUsername = Environment.GetEnvironmentVariable("SMG_EMAIL_NAME") ?? throw new InvalidOperationException("SMG_EMAIL_NAME environment variable is not set.");
            _smtpPassword = Environment.GetEnvironmentVariable("SMG_EMAIL_PASSWORD") ?? throw new InvalidOperationException("SMG_EMAIL_PASSWORD environment variable is not set.");
        }

        /// <summary>
        /// Method to send OTP as an email
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userEmail"></param>
        public void SendOtpAsEmail(string otp, string userEmail)
        {
            try
            {
                var message = new MimeMessage();
                // Set the sender's name and address
                message.From.Add(new MailboxAddress("Smart Meter Gateway API", _smtpUsername));
                // Set the recipient's email address
                message.To.Add(new MailboxAddress("", EncryptionHelper.Decrypt(userEmail)));
                // Set the email subject
                message.Subject = "Your OTP Code for Login";

                // Set the email body with the OTP code
                message.Body = new TextPart("plain")
                {
                    Text = $"Your OTP code is: {otp}"
                };

                // Create a new SMTP client
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    //Only for testing environment, not for production! Bypassing certificate validation -> // remove in production
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Connect to the Outlook.com SMTP server and port with StartTLS
                    client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    // Authenticate using the Outlook.com username and password
                    client.Authenticate(_smtpUsername, _smtpPassword);
                    // Send the email
                    client.Send(message);
                    // Disconnect the client
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }
}
