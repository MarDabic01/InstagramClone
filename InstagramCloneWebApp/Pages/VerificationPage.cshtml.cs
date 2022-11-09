using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class VerificationPageModel : PageModel
    {
        List<UserInfo> allUsers = new List<UserInfo>();
        UserInfo currentUser = new UserInfo();
        public string data2;

        public void OnGet()
        {
            string data = (string)RouteData.Values["passedemail"];
            data2 = data;

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
                                user.isVerified = reader.GetBoolean(5);

                                allUsers.Add(user);
                            }
                        }
                    }
                }
            


            foreach(UserInfo u in allUsers)
            {
                if(u.email == (String)RouteData.Values["passedemail"])
                {
                        connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sqlQuery = "UPDATE users " +
                                              "SET isVerified = 'TRUE' " +
                                              "WHERE email = 'reachme286@gmail.com';";
                            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                            {
                                command.Parameters.AddWithValue("@isVerified", true);

                                command.ExecuteNonQuery();
                            }
                        }
                }
            }
            Console.WriteLine(data2);
        }

        public class UserInfo
        {
            public string id;
            public string email;
            public bool isVerified;
        }
    }
}
