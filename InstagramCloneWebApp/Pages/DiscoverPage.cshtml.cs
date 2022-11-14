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
        //Variables for storing searched data
        public List<DiscoverUserInfo> foundUsers = new List<DiscoverUserInfo>();
        public List<DiscoverUserInfo> allUsers = new List<DiscoverUserInfo>();
        public string searchingString = "";
        public List<string> links = new List<string>();

        private string myFollowingString = "";
        public List<DiscoverUserInfo> myFollowingAccounts = new List<DiscoverUserInfo>();

        private string friendsFollowingString = "";
        public List<DiscoverUserInfo> friendsFollowingAccounts = new List<DiscoverUserInfo>();

        public List<DiscoverUserInfo> suggestedAccounts = new List<DiscoverUserInfo>();

        //Automatically called when page loads
        public void OnGet()
        {
            //Getting the list of users I follow
            GetMyFollowingString();
            GetMyFollowingAccounts();

            //Getting the list of users my following accounts follow
            GetFriendsFollowingString();
            GetFriendsFollowingAccounts();

            //Calculating suggestions
            GetSuggestion();
        }

        //Getting "followingString" from database, which includes all the ids of people I follow
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

        //Separating "followingString" to get a list of ids
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

        //Getting my friends "followingString" from database
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

        //Separating friends following string and storing into the list
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

        //Checking if my friends follow someone that I don't follow
        //If yes then that profile is suggested 
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
        //Called when user is searching for other accounts
        public void OnPostSearch()
        {
            searchingString = Request.Form["searchbar"];//Getting string value from the searchbar
            if (searchingString.Length > 0)
            {
                GetAllUsers();
                foreach (DiscoverUserInfo u in allUsers)
                {
                    if (u.username.Contains(searchingString))
                    {
                        string linkString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"] + "/" + u.id;

                        links.Add(linkString);
                        foundUsers.Add(u);//Adding found user to foundUsers list
                    }
                }
            }
        }
        //Redirecting to home page
        public void OnPostToHome()
        {
            string redirectString;
            redirectString = "https://localhost:44328/HomePage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting to profile page
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting to discover page
        public void OnPostToDiscover()
        {
            string redirectString;
            redirectString = "https://localhost:44328/DiscoverPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting to account page
        public void OnPostToAccount()
        {
            string redirectString;
            redirectString = "https://localhost:44328/AccountPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting to upload picture page
        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Logging user out
        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }
        //Getting all the users from users table
        private void GetAllUsers()
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
