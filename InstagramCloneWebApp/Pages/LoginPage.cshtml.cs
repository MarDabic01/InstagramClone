using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class LoginPageModel : PageModel
    {
        public List<ExistingUserInfo> allUsers = new List<ExistingUserInfo>();
        public ExistingUserInfo existingUser = new ExistingUserInfo();
        public string errorMessage = "";

        //Code running on post call
        public void OnPost()
        {
            existingUser.username = Request.Form["username"];
            existingUser.password = Request.Form["password"];
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
                                ExistingUserInfo user = new ExistingUserInfo();
                                user.username = reader.GetString(2);
                                user.password = reader.GetString(3);

                                allUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                Console.WriteLine("Error: " + e.Message);
            }

            if(IsLoginValid())
                errorMessage = "Successfully logged in"; //Redirect to profile page
            else
                errorMessage = "Invalid log in";
        }

        private bool IsLoginValid()
        {
            foreach (ExistingUserInfo u in allUsers)
                if (u.username == existingUser.username && u.password == existingUser.password)
                    return true;
            return false;
        }

        //Class used to store user information
        public class ExistingUserInfo
        {
            public string username;
            public string password;
        }
    }
}
