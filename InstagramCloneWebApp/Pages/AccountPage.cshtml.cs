using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class AccountPageModel : PageModel
    {
        public List<UserInfo> foundUsers = new List<UserInfo>();
        public List<UserInfo> allUsers = new List<UserInfo>();
        UserInfo currentUser = new UserInfo();
        public int data2;
        public string infoMessage = "";
        public string searchingString = "";
        public List<string> links = new List<string>();

        [BindProperty]
        public ProfilePicture profile_pic { get; set; }

        [BindProperty]
        public IFormFile formFile { get; set; }

        //Method called when user wants to change e-mail
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

        //Method called when user is changing username
        public void OnPostChangeUsername()
        {
            GetAllUsers();
            currentUser.username = Request.Form["username"];

            //Checking if username is valid
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
        //Method called when user is changing password
        public void OnPostChangePassword()
        {
            GetAllUsers();
            currentUser.password = Request.Form["password"];

            //If password is valid then apply changes
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
        //Adding description method
        public void OnPostAddDescription()
        {
            GetAllUsers();
            currentUser.description = Request.Form["description"];

            foreach (UserInfo u in allUsers)
            {
                if (u.id == (String)RouteData.Values["passed_id"])
                {
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET profilebio = '" + currentUser.description + "'" +
                                          "WHERE id = " + data2 + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@description", currentUser.description);
                            infoMessage = "Profile description added";

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //Deleting account
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
        //Logging out
        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }
        //Redirecting user to upload picture page
        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Setting up user profile picture
        public void OnPostProfilePic()
        {
            byte[] bytes = null;
            profile_pic.imageFile = formFile;
            if(profile_pic.imageFile == null)
            {
                infoMessage = "Nije selektovana slika";
            }
            else
            {
                using (Stream fs = profile_pic.imageFile.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                    profile_pic.picturedata = Convert.ToBase64String(bytes, 0, bytes.Length);
                }
                GetAllUsers();
                foreach (UserInfo u in allUsers)
                {
                    if (u.id == (String)RouteData.Values["passed_id"])
                    {
                        string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sqlQuery = "UPDATE users " +
                                              "SET profilepic = '" + profile_pic.picturedata + "'" +
                                              "WHERE id = " + data2 + ";";
                            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                            {
                                command.Parameters.AddWithValue("@profilepic", profile_pic.picturedata);
                                infoMessage = "Profile picture added";

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
        //Method called when searching
        public void OnPostSearch()
        {
            searchingString = Request.Form["searchbar"];
            if(searchingString.Length > 0)
            {
                GetAllUsers();
                foreach (UserInfo u in allUsers)
                {
                    if (u.username.Contains(searchingString))
                    {
                        string linkString = "https://localhost:44328/ProfilePage/" + RouteData.Values["passed_id"] + "/" + u.id;

                        links.Add(linkString);
                        foundUsers.Add(u);
                    }
                }
            }
        }
        //Redirecting user to the home page
        public void OnPostToHome()
        {
            string redirectString;
            redirectString = "https://localhost:44328/HomePage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to profile page
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["passed_id"].ToString() + "/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to discover page
        public void OnPostToDiscover()
        {
            string redirectString;
            redirectString = "https://localhost:44328/DiscoverPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to account page
        public void OnPostToAccount()
        {
            string redirectString;
            redirectString = "https://localhost:44328/AccountPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Getting all the users
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
                            user.imagedata = reader.GetString(10);

                            allUsers.Add(user);
                        }
                    }
                }
            }
        }
        //Method for setting value into databse
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
        //Check password length
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
            public string description;
            public string imagedata;
        }

        public class ProfilePicture
        {
            public string picturedata;
            public IFormFile imageFile;
        }
    }
}
