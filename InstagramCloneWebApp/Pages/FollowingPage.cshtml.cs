using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class FollowingPageModel : PageModel
    {
        List<Follower> allFollowers = new List<Follower>();
        List<string> myFollowersIds = new List<string>();
        public List<Follower> myFollowers = new List<Follower>();
        private string followersString = "";
        public void OnGet()
        {
            GetAllFollowers();
            GetFollowersString();
            GetMyFollowes();
        }
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }

        private void GetAllFollowers()
        {
            allFollowers.Clear();
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
                            Follower follower = new Follower();
                            follower.id = reader.GetInt32(0).ToString();
                            follower.username = reader.GetString(2);
                            follower.profilePictureData = reader.GetString(10);
                            follower.followers = reader.GetString(12);
                            follower.link = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"] + "/" + reader.GetInt32(0).ToString();

                            allFollowers.Add(follower);
                        }
                    }
                }
            }
        }

        private void GetMyFollowes()
        {
            string s = "";
            for (int i = 0; i < followersString.Length; i++)
            {
                if (followersString[i] == ',')
                {
                    myFollowersIds.Add(s);
                    s = "";
                }
                else
                {
                    s += followersString[i];
                }
            }
            foreach (Follower f in allFollowers)
            {
                foreach (string x in myFollowersIds)
                {
                    if (f.id == x)
                    {
                        myFollowers.Add(f);
                    }
                }
            }
        }

        private void GetFollowersString()
        {
            foreach (Follower f in allFollowers)
            {
                if (f.id == (String)RouteData.Values["profile_id"])
                {
                    followersString = f.followers;
                }
            }
        }
    }
}
