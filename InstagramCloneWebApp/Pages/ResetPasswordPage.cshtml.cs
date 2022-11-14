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

        //Method called when reset password button is clicked
        public void OnPost()
        {
            //Getting user email because program needs to know which password to update
            string data = (string)RouteData.Values["passedemail"];
            data2 = data;

            //Getting input from form
            currentUser.password = Request.Form["password"];
            currentUser.repeatpassword = Request.Form["repeatpassword"];

            //Checking if new password is valid
            if (AreFieldsFilled() != true || ArePasswordsMatching() != true)
                return;

            //Getting all users from users table
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

            //Check which user match the passed e-mail
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

        //Returns true if password fields are filled
        private bool AreFieldsFilled()
        {
            if(currentUser.password == "" || currentUser.repeatpassword == "")
            {
                infoMessage = "All fields are required";
                return false;
            }
            return true;
        }

        //Returns true if password and repeated password are matching
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
