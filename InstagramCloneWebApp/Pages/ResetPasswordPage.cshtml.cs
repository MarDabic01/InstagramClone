using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class ResetPasswordPageModel : PageModel
    {
        List<UserInfo> allUsers = new List<UserInfo>();
        UserInfo currentUser = new UserInfo();
        public string data2 = "";
        public string infoMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            string data = (string)RouteData.Values["passedemail"];
            data2 = data;

            currentUser.password = Request.Form["password"];
            currentUser.repeatpassword = Request.Form["repeatpassword"];

            if (AreFieldsFilled() != true || ArePasswordsMatching() != true)
                return;

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
                            user.email = reader.GetString(1);
                            user.password = reader.GetString(3);
                            user.repeatpassword = reader.GetString(4);

                            allUsers.Add(user);
                        }
                    }
                }
            }

            foreach (UserInfo u in allUsers)
            {
                if (u.email == (String)RouteData.Values["passedemail"])
                {
                    connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET password = '" + currentUser.password + "', repeatpassword = '" + currentUser.repeatpassword + "'" +
                                          "WHERE email = '" + data2 +"';";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@password", currentUser.password);
                            command.Parameters.AddWithValue("@repeatpassword", currentUser.repeatpassword);
                            infoMessage = "Password successfully changed";

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool AreFieldsFilled()
        {
            if(currentUser.password == "" || currentUser.repeatpassword == "")
            {
                infoMessage = "All fields are required";
                return false;
            }
            return true;
        }

        private bool ArePasswordsMatching()
        {
            if(currentUser.password != currentUser.repeatpassword)
            {
                infoMessage = "Password and repeated password are not matching";
                return false;
            }
            return true;
        }

        public class UserInfo
        {
            public string email;
            public string password;
            public string repeatpassword;
        }
    }
}
