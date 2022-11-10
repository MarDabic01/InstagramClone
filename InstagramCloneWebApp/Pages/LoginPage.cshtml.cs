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
        public int index;

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
                                user.id = reader.GetInt32(0);
                                user.email = reader.GetString(1);
                                user.username = reader.GetString(2);
                                user.password = reader.GetString(3);
                                user.isVerified = reader.GetBoolean(5);

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
            {
                errorMessage = "Successfully logged in"; //Redirect to profile page
                string redirectString = "accountPage/" + existingUser.id + "";
                //string redirectString = "accountPage/1";
                Response.Redirect(redirectString);
            }
        }

        private bool IsLoginValid()
        {
            foreach (ExistingUserInfo u in allUsers)
            {
                if (u.username == existingUser.username && u.password == existingUser.password)
                    if (u.isVerified != true)
                    {
                        errorMessage = "Account is not verified";
                        return false;
                    }
                    else
                    {
                        existingUser.email = u.email;
                        existingUser.id = u.id;
                        return true;
                    }
            }
            errorMessage = "Invalid log in";
            return false;
        }
        
        private bool IsAccountVerified()
        {
            foreach (ExistingUserInfo u in allUsers)
                if (u.username == existingUser.username && u.password == existingUser.password)
                    if (existingUser.isVerified == true)
                        return true;
            return true;
        }

        //Class used to store user information
        public class ExistingUserInfo
        {
            public int id;
            public string email;
            public string username;
            public string password;
            public bool isVerified;
        }
    }
}
