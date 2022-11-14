using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class ForgotPasswordPageModel : PageModel
    {
        public List<UserInfo> allUsers = new List<UserInfo>();
        public UserInfo newUser = new UserInfo();

        //Method sending e-mail for recovering password
        public void OnPost()
        {
            newUser.email = Request.Form["email"];

            MailAddress to = new MailAddress(newUser.email);
            MailAddress from = new MailAddress("markodabic00@gmail.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = "REACH ME - Reset password";
            message.Body = "" +
                "<!DOCTYPE>" +
                "<html>" +
                "<body><h1>Forgot yout password?</h1><p>Don't worry reset your passrowd <a href='https://localhost:44328/ResetPasswordPage/" + newUser.email + "'>here</a></p></body>" +
                "</html>";
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("reachme286@gmail.com", "qdivulnnzawbdqei"),
                EnableSsl = true
            };
            client.Send(message);
        }

        public class UserInfo
        {
            public string email;
        }
    }
}
