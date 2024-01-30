using System.Net.Mail;
using System.Net;
using System.Text;

namespace TravelPlan.Services
{
    public class EmailService
    {
        public bool Execute(string UserEmail, string Body, string Subject)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.project.alocms.ir";
                client.EnableSsl = true;
                client.Timeout = 1000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("Mag@Project.alocms.ir", "dmx444#4Z");
                MailMessage message = new MailMessage("Mag@Project.alocms.ir", UserEmail, Subject, Body);
                message.IsBodyHtml = true;
                message.BodyEncoding = UTF8Encoding.UTF8;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                client.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
