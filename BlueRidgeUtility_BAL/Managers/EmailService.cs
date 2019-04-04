using BlueRidgeUtility_BAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlueRidgeUtility_BAL.Managers
{
  public  class EmailService : IEmailService
    {
        string SITE_URL = ConfigurationManager.AppSettings["SITE_URL"];
        string EMAIL_TEMPLATE_PATH = ConfigurationManager.AppSettings["EMAIL_TEMPLATE_PATH"];
        public void sendResetPasswordEmail(ForgotPasswordEmailModel model) {
            SendHtmlFormattedEmail(model.toEmailId, model.subject, PopulateResetPasswordEmailBody(model));
        }

        private string GetWebsiteHtml(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            stream.Dispose();
            reader.Dispose();
            return result;
        }
        private string PopulateResetPasswordEmailBody(ForgotPasswordEmailModel model)
        {
           
            var url = $"{EMAIL_TEMPLATE_PATH}ResetPassword.html";
            string body = GetWebsiteHtml(url);
            body = body.Replace("{{name}}", model.name);
            body = body.Replace("{{action_url}}", model.link);
            body = body.Replace("{{site_url}}", SITE_URL);
          
            return body;
        }

        private void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recepientEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["Host"];
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
                NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                smtp.Send(mailMessage);
            }
        }
    }
}
