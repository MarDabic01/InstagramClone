using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class AccountPageModel : PageModel
    {
        List<UserInfo> allUsers = new List<UserInfo>();
        UserInfo currentUser = new UserInfo();
        public int data2;
        public string infoMessage = "";

        public void OnPostChangeEmail()
        {
            GetAllUsers();
            currentUser.email = Request.Form["email"];

            if(currentUser.email == "")
            {
                infoMessage = "Invalid e-mail input";
            }
            else
            {
                string sqlQuery = "UPDATE users " +
                                  "SET email = '" + currentUser.email + "'" +
                                  "WHERE id = " + data2 + ";";
                MakeChange(sqlQuery, "@email", currentUser.email, "Email successfully changed");
            }
        }

        public void OnPostChangeUsername()
        {
            GetAllUsers();
            currentUser.username = Request.Form["username"];

            if(currentUser.username.Contains(" ") == true || currentUser.username == "")
            {
                infoMessage = "Username cannot contain whitespace";
            }
            else
            {
                string sqlQuery = "UPDATE users " +
                                  "SET username = '" + currentUser.username + "'" +
                                  "WHERE id = " + data2 + ";";
                MakeChange(sqlQuery, "@username", currentUser.username, "Username successfully changed");
            }
        }

        public void OnPostChangePassword()
        {
            GetAllUsers();
            currentUser.password = Request.Form["password"];

            if(IsPasswordValid() && PasswordLength())
            {
                foreach (UserInfo u in allUsers)
                {
                    if (u.id == (String)RouteData.Values["passed_id"])
                    {
                        string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sqlQuery = "UPDATE users " +
                                              "SET password = '" + currentUser.password + "', repeatpassword ='" + currentUser.password + "'" +
                                              "WHERE id = " + data2 + ";";
                            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                            {
                                command.Parameters.AddWithValue("@password", currentUser.password);
                                command.Parameters.AddWithValue("@repeatpassword", currentUser.password);
                                infoMessage = "Password changed successfully";

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public void OnPostDeleteAccount()
        {
            GetAllUsers();
            currentUser.password = Request.Form["password"];

            foreach (UserInfo u in allUsers)
            {
                if (u.id == (String)RouteData.Values["passed_id"])
                {
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "DELETE FROM users " +
                                          "WHERE id = " + data2 + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            infoMessage = "Account deleted";

                            command.ExecuteNonQuery();
                            Response.Redirect("https://localhost:44328/");
                        }
                    }
                }
            }
        }

        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }

        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }

        private void GetAllUsers()
        {
            string data = RouteData.Values["passed_id"].ToString();
            data2 = int.Parse(data);

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
                            user.id = reader.GetInt32(0).ToString();
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

        private void MakeChange(string query, string parameter, string value, string info)
        {
            foreach (UserInfo u in allUsers)
            {
                if (u.id == (String)RouteData.Values["passed_id"])
                {
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue(parameter, value);
                            infoMessage = info;

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        //Checks if password is valid
        private bool IsPasswordValid()
        {
            string s = currentUser.password;
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] lower = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] upper = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] special = { ' ', '!', '"', '#', '$', '%', '&', '(', ')', '*', '=', '+', ',', '-', '.', '/', ':', ';', '?', '@', '_' };

            if (NumberOfCharacters(numbers, s) != true || NumberOfCharacters(lower, s) != true
            || NumberOfCharacters(upper, s) != true || NumberOfCharacters(special, s) != true)
            {
                infoMessage = "Password needs upper and lower case, number and special character";
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
            if (currentUser.password.Length < 8)
            {
                infoMessage = "Password needs to be at least 8 character long";
                return false;
            }
            return true;
        }

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
