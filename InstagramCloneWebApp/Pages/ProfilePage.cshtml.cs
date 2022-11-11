using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using InstagramCloneWebApp.DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class ProfilePageModel : PageModel
    {
        public List<ProfileInfo> foundUsers = new List<ProfileInfo>();
        List<ProfileInfo> allUsers = new List<ProfileInfo>();
        public ProfileInfo currentProfile = new ProfileInfo();
        public string infoMessage = "";
        public int data2;
        public bool isMyProfile = false;
        public string searchingString = "";
        public List<string> links = new List<string>();

        public void OnGet()
        {
            GetAllUsers();
            foreach(ProfileInfo pi in allUsers)
            {
                if(pi.id == data2)
                {
                    currentProfile.id = pi.id;
                    currentProfile.username = pi.username;
                    currentProfile.description = pi.description;
                    currentProfile.posts = pi.posts;
                    currentProfile.followers = pi.followers;
                    currentProfile.following = pi.following;
                }
            }
        }

        public void OnPostSearch()
        {
            searchingString = Request.Form["searchbar"];
            if (searchingString.Length > 0)
            {
                GetAllUsers();
                foreach (ProfileInfo u in allUsers)
                {
                    if (u.username.Contains(searchingString))
                    {
                        string linkString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"] + "/" + u.id;

                        links.Add(linkString);
                        foundUsers.Add(u);
                    }
                }
            }
            foreach (ProfileInfo pi in allUsers)
            {
                if (pi.id == data2)
                {
                    currentProfile.id = pi.id;
                    currentProfile.username = pi.username;
                    currentProfile.description = pi.description;
                    currentProfile.posts = pi.posts;
                    currentProfile.followers = pi.followers;
                    currentProfile.following = pi.following;
                }
            }
        }

        public void OnPostToHome()
        {

        }
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToDiscover()
        {

        }
        public void OnPostToAccount()
        {
            string redirectString;
            redirectString = "https://localhost:44328/AccountPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }

        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        private void GetAllUsers()
        {
            string data = RouteData.Values["profile_id"].ToString();
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
                            ProfileInfo user = new ProfileInfo();
                            user.id = reader.GetInt32(0);
                            user.username = reader.GetString(2);
                            user.description = reader.GetString(6);
                            user.posts = reader.GetInt32(7);
                            user.followers = reader.GetInt32(8);
                            user.following = reader.GetInt32(9);

                            allUsers.Add(user);
                        }
                    }
                }
            }
        }

        public bool CheckIfProfileIsMine()
        {
            if ((String)RouteData.Values["my_id"] == (String)RouteData.Values["profile_id"])
            {
                return true;
            }
            return false;
        }
    }

    public class ProfileInfo
    {
        public int id;
        public string username;
        public string description;
        public int posts;
        public int followers;
        public int following;
    }
}
