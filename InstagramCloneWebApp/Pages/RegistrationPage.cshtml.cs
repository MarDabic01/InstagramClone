using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class RegistrationPageModel : PageModel
    {
        public List<UserInfo> allUsers = new List<UserInfo>();
        public UserInfo newUser = new UserInfo();
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

            if (AreFieldsFilled() != true || IsEmailUsernameUnique() != true || ArePasswordsMatching() != true)
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

        private bool ArePasswordsMatching()
        {
            if(newUser.password != newUser.repeatpassword)
            {
                errorMessage = "Password and repeated password are not matching";
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
    }
}
