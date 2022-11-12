using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstagramCloneWebApp.Pages
{
    public class HomePageModel : PageModel
    {
        public List<ProfileInfo> foundUsers = new List<ProfileInfo>();
        List<ProfileInfo> allUsers = new List<ProfileInfo>();
        public ProfileInfo currentProfile = new ProfileInfo();
        public string infoMessage = "";
        public int data2;
        public bool isMyProfile = false;
        public bool isAreadyFollowing = false;
        public string searchingString = "";
        public List<string> links = new List<string>();

        //Ovo mi treba
        private List<ProfilePost> allposts = new List<ProfilePost>();
        public List<ProfilePost> postsToShow = new List<ProfilePost>();
        private string followingList = "";
        private List<string> followingAccounts = new List<string>();
        private List<ProfilePost> allPosts = new List<ProfilePost>();
        public string info = "";

        public void OnGet()
        {
            GetFollowingList();
            GetFollowingAccounts();
            GetAllPosts();
            GetPostsToShow();
            info = allposts[0].author;
        }

        private void GetFollowingList()
        {
            GetAllUsers();

            foreach(ProfileInfo p in allUsers)
            {
                if (p.id.ToString() == (String)RouteData.Values["my_id"])
                {
                    followingList = p.followings;
                }
            }
        }

        private void GetFollowingAccounts()
        {
            string s = "";
            for(int i=0;i<followingList.Length;i++)
            {
                if(followingList[i] == ',')
                {
                    followingAccounts.Add(s);
                    s = "";
                }
                else
                {
                    s += followingList[i];
                }
            }
        }

        private void GetAllPosts()
        {
            allposts.Clear();

            string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT * FROM ImagesDetails";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProfilePost post = new ProfilePost();
                            post.id = reader.GetInt32(0).ToString();
                            post.caption = reader.GetString(1);
                            post.author = reader.GetString(2);
                            post.postdata = reader.GetString(4);
                            post.username = reader.GetString(5);
                            post.profilepic = reader.GetString(6);
                            post.likes = reader.GetInt32(7);
                            post.link = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + post.author;

                            allposts.Add(post);
                        }
                    }
                }
            }
        }

        private void GetPostsToShow()
        {
            for(int i=allposts.Count-1;i>=0;i--)
            {
                foreach(string s in followingAccounts)
                {
                    if(allposts[i].author == s)
                    {
                        postsToShow.Add(allposts[i]);
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
        }

        public void OnPostToHome()
        {
            string redirectString;
            redirectString = "https://localhost:44328/HomePage/" + RouteData.Values["my_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToFollowers()
        {
            string redirectString;
            redirectString = "https://localhost:44328/FollowersPage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["profile_id"].ToString();
            Response.Redirect(redirectString);
        }
        public void OnPostToFollowing()
        {
            string redirectString;
            redirectString = "https://localhost:44328/FollowingPage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["profile_id"].ToString();
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
            allUsers.Clear();

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
                            user.imagedata = reader.GetString(10);
                            user.followerss = reader.GetString(11);
                            user.followings = reader.GetString(12);

                            allUsers.Add(user);
                        }
                    }
                }
            }
        }
    }
}
