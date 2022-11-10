using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace InstagramCloneWebApp.Pages
{
    public class RegistrationPageModel : PageModel
    {
        public List<UserInfo> allUsers = new List<UserInfo>();
        public UserInfo newUser = new UserInfo();
        public Email email = new Email();
        public string errorMessage = "";
        public string successMessage = "";

        //Method running on GET call
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM users";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                UserInfo user = new UserInfo();
                                user.id = "" + reader.GetInt32(0);
                                user.email = reader.GetString(1);
                                user.username = reader.GetString(2);
                                user.password = reader.GetString(3);
                                user.repeatpassword = reader.GetString(4);

                                allUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        //Method running on POST call
        public void OnPost()
        {
            newUser.email = Request.Form["email"];
            newUser.username = Request.Form["username"];
            newUser.password = Request.Form["password"];
            newUser.repeatpassword = Request.Form["repeatpassword"];
            email.To = Request.Form["email"];

            if (AreFieldsFilled() != true || IsEmailUsernameUnique() != true || ArePasswordsMatching() != true
                || IsUsernameValid() != true || IsPasswordValid() != true || PasswordLength() != true)
                return;

            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "INSERT INTO users " +
                                      "(email, username, password, repeatpassword) VALUES " +
                                      "(@email, @username, @password, @repeatpassword);";
                    using(SqlCommand command = new SqlCommand(sqlQuery,connection))
                    {
                        command.Parameters.AddWithValue("@email", newUser.email);
                        command.Parameters.AddWithValue("@username", newUser.username);
                        command.Parameters.AddWithValue("@password", newUser.password);
                        command.Parameters.AddWithValue("@repeatpassword", newUser.repeatpassword);

                        command.ExecuteNonQuery();
                    }
                }
                successMessage = "User successfully created";
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
            }

            MailAddress to = new MailAddress(newUser.email);
            MailAddress from = new MailAddress("markodabic00@gmail.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = "REACH ME - Verification message";
            message.Body = "" +
                "<!DOCTYPE>" +
                "<html>" +
                "<body><h1>Welcome to ReachMe</h1><p>Please verify your e-mail <a href='https://localhost:44328/VerificationPage/" + newUser.email +"'>here</a></p></body>" +
                "</html>";
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("reachme286@gmail.com", "qdivulnnzawbdqei"),
                EnableSsl = true
            };
            // code in brackets above needed if authentication required
            client.Send(message);
        }
        
        //Check if all fields have been filled
        private bool AreFieldsFilled()
        {
            if (newUser.email.Length == 0 || newUser.username.Length == 0 || newUser.password.Length == 0 || newUser.repeatpassword.Length == 0)
            {
                errorMessage = "All fields are required";
                return false;
            }
            return true;
        }

        //Check if email and username are unique
        private bool IsEmailUsernameUnique()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM users";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo user = new UserInfo();
                                user.id = "" + reader.GetInt32(0);
                                user.email = reader.GetString(1);
                                user.username = reader.GetString(2);
                                user.password = reader.GetString(3);
                                user.repeatpassword = reader.GetString(4);

                                allUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
            }

            foreach(UserInfo u in allUsers)
            {
                if(u.email == newUser.email)
                {
                    errorMessage = "Email is already in use";
                    return false;
                }
                if(u.username == newUser.username)
                {
                    errorMessage = "Username already exist";
                    return false;
                }
            }

            return true;
        }

        //Checks if password and repeated password are matched
        private bool ArePasswordsMatching()
        {
            if(newUser.password != newUser.repeatpassword)
            {
                errorMessage = "Password and repeated password are not matching";
                return false;
            }
            return true;
        }

        //Checks if username is valid
        private bool IsUsernameValid()
        {
            if(newUser.username.Contains(' '))
            {
                errorMessage = "Username cannot contain whitespace";
                return false;
            }
            return true;
        }

        //Checks if password is valid
        private bool IsPasswordValid()
        {
            string s = newUser.password;
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            char[] lower = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] upper = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] special = {' ','!','"','#','$','%','&','(',')','*','=', '+', ',', '-', '.', '/', ':', ';', '?', '@', '_'};

            if(NumberOfCharacters(numbers,s) != true || NumberOfCharacters(lower,s) != true 
            || NumberOfCharacters(upper,s) != true || NumberOfCharacters(special,s) != true)
            {
                errorMessage = "Password needs upper and lower case, number and special character";
                return false;
            }
            return true;
        }

        //Checks does string contain at least one member of the array
        private bool NumberOfCharacters(char[] array, string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (s[i] == array[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool PasswordLength()
        {
            if(newUser.password.Length < 8)
            {
                errorMessage = "Password needs to be at least 8 character long";
                return false;
            }
            return true;
        }

        //Class that defines user
        public class UserInfo
        {
            public string id;
            public string email;
            public string username;
            public string password;
            public string repeatpassword;
        }

        public class Email
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
