using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class DiscoverPageModel : PageModel
    {
        public List<DiscoverUserInfo> foundUsers = new List<DiscoverUserInfo>();
        public List<DiscoverUserInfo> allUsers = new List<DiscoverUserInfo>();
        public int data2;
        public string infoMessage = "";
        public string searchingString = "";
        public List<string> links = new List<string>();

        private string myFollowingString = "";
        public List<DiscoverUserInfo> myFollowingAccounts = new List<DiscoverUserInfo>();

        private string friendsFollowingString = "";
        public List<DiscoverUserInfo> friendsFollowingAccounts = new List<DiscoverUserInfo>();

        public List<DiscoverUserInfo> suggestedAccounts = new List<DiscoverUserInfo>();

        public void OnGet()
        {
            //My following list
            GetMyFollowingString();
            GetMyFollowingAccounts();

            //My following accounts following list
            GetFriendsFollowingString();
            GetFriendsFollowingAccounts();

            GetSuggestion();
            infoMessage = myFollowingString;
        }

        private void GetMyFollowingString()
        {
            GetAllUsers();

            foreach(DiscoverUserInfo d in allUsers)
            {
                if(d.id == (String)RouteData.Values["my_id"])
                {
                    myFollowingString = d.following;
                }
            }
        }

        private void GetMyFollowingAccounts()
        {
            string s = "";
            for(int i=0;i<myFollowingString.Length;i++)
            {
                if(myFollowingString[i] == ',')
                {
                    foreach(DiscoverUserInfo u in allUsers)
                    {
                        if (u.id == s)
                            myFollowingAccounts.Add(u);
                    }
                    s = "";
                }
                else
                {
                    s += myFollowingString[i];
                }
            }
        }

        private void GetFriendsFollowingString()
        {
            foreach(DiscoverUserInfo d in allUsers)
            {
                if(myFollowingAccounts.Contains(d))
                {
                    friendsFollowingString += d.following;
                }
            }
        }

        private void GetFriendsFollowingAccounts()
        {
            string s = "";
            for (int i = 0; i < friendsFollowingString.Length; i++)
            {
                if (friendsFollowingString[i] == ',')
                {
                    foreach (DiscoverUserInfo u in allUsers)
                    {
                        if (u.id == s)
                            friendsFollowingAccounts.Add(u);
                    }
                    s = "";
                }
                else
                {
                    s += friendsFollowingString[i];
                }
            }
        }

        private void GetSuggestion()
        {
            foreach(DiscoverUserInfo s in myFollowingAccounts)
            {
                foreach(DiscoverUserInfo a in friendsFollowingAccounts)
                {
                    if(a.id != (String)RouteData.Values["my_id"] && myFollowingAccounts.Contains(a) != true && suggestedAccounts.Contains(a) != true)
                    {
                        suggestedAccounts.Add(a);
                    }
                }
            }
        }

        public void OnPostSearch()
        {
            searchingString = Request.Form["searchbar"];
            if (searchingString.Length > 0)
            {
                GetAllUsers();
                foreach (DiscoverUserInfo u in allUsers)
                {
                    if (u.username.Contains(searchingString))
                    {
                        string linkString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"] + "/" + u.id;

                        links.Add(linkString);
                        foundUsers.Add(u);
                    }
                }
            }
        }

        public void OnPostToHome()
        {
            string redirectString;
            redirectString = "https://localhost:44328/HomePage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToDiscover()
        {
            string redirectString;
            redirectString = "https://localhost:44328/DiscoverPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToAccount()
        {
            string redirectString;
            redirectString = "https://localhost:44328/AccountPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }

        private void GetAllUsers()
        {
            string data = RouteData.Values["my_id"].ToString();
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
                            DiscoverUserInfo user = new DiscoverUserInfo();
                            user.id = reader.GetInt32(0).ToString();
                            user.username = reader.GetString(2);
                            user.imagedata = reader.GetString(10);
                            user.following = reader.GetString(12);
                            user.link = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + user.id;

                            allUsers.Add(user);
                        }
                    }
                }
            }
        }
    }
    public class DiscoverUserInfo
    {
        public string id;
        public string username;
        public string imagedata;
        public string following;
        public string link;
    }
}
