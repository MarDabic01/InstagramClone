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
        public bool isAreadyFollowing = false;
        public string searchingString = "";
        public List<string> links = new List<string>();

        //Variables for my posts
        List<ProfilePost> allposts = new List<ProfilePost>();
        public List<ProfilePost> myposts = new List<ProfilePost>();

        //Calls when page loads
        //Filling up the profile information and showing user's posts
        public void OnGet()
        {
            GetAllUsers();
            GetMyPosts();
            GetAllComments();
            ShowMyPosts();
            
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
                    currentProfile.imagedata = pi.imagedata;
                }
            }
        }
        //Searching for specified users
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
            //Setting profile data again after page finish searching
            UpdateProfileData(RouteData.Values["profile_id"].ToString());
            GetMyPosts();
            GetAllComments();
            ShowMyPosts();
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
        //Method for following user
        public void OnPostFollow()
        {
            GetAllUsers();

            //Getting my user id and user that is being followed id
            string user1="", user2="";
            foreach(ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["my_id"]) 
                    user1 = u.id.ToString();
                if (u.id.ToString() == (String)RouteData.Values["profile_id"]) 
                    user2 = u.id.ToString();
            }

            //Incrementing following value of my account
            //Adding followed user id to my following list in database
            foreach(ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["my_id"])
                {
                    int following_num = u.following + 1;
                    string following_s = u.followings + user2 + ",";
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET following = " + following_num + ", followinglist = '" + following_s + "'" +
                                          "WHERE id = " + u.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@following", following_num);
                            command.Parameters.AddWithValue("@followinglist", following_s);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            //Incrementing followers value of followed account
            //Adding my user id to followed user's followed list in database
            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["profile_id"])
                {
                    int followers_num = u.followers + 1;
                    string followers_s = u.followerss + user1 + ",";
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET followers = " + followers_num + ", followerslist = '" + followers_s + "'" +
                                          "WHERE id = " + u.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@followers", followers_num);
                            command.Parameters.AddWithValue("@followerslist", followers_s);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            UpdateProfileData(RouteData.Values["profile_id"].ToString());//Updating following/followers data after method is done
        }
        //Calling when user is being unfollowed
        public void OnPostUnfollow()
        {
            GetAllUsers();
            UpdateFollowersList();
            UpdateFollowingList();
            UpdateProfileData(RouteData.Values["profile_id"].ToString());
        }
        //Checking if user is already being followed by me
        public bool CheckIfFollowingUser()
        {
            string user = "";

            List<string> allfollowings = new List<string>();
            string followinguser = "";
            string followingstring = "";

            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["my_id"])
                    followingstring = u.followings;
                if (u.id.ToString() == (String)RouteData.Values["profile_id"])
                    user = u.id.ToString();
            }

            for(int i=0;i<followingstring.Length;i++)
            {
                if(followingstring[i] == ',')
                {
                    allfollowings.Add(followinguser);
                    followinguser = "";
                }
                else
                {
                    followinguser += followingstring[i];
                }
            }

            foreach(string s in allfollowings)
            {
                if (s == user)
                    return true; 
            }
            return false;
        }
        //Removing my account id from unfollowed user followers list
        private void UpdateFollowersList()
        {
            List<string> allfollowers = new List<string>();
            List<string> allfollowersafter = new List<string>();
            string followeruser = "";
            string followerstring = "";
            string finished = "";
            string user = "";

            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["profile_id"])
                    followerstring = u.followerss;
                if (u.id.ToString() == (String)RouteData.Values["my_id"])
                    user = u.id.ToString();
            }
            for (int i = 0; i < followerstring.Length; i++)
            {
                if (followerstring[i] == ',')
                {
                    allfollowers.Add(followeruser);
                    followeruser = "";
                }
                else
                {
                    followeruser += followerstring[i];
                }
            }
            foreach (string s in allfollowers)
            {
                if (s != user)
                {
                    allfollowersafter.Add(s);
                    allfollowersafter.Add(",");
                }
            }
            foreach (string s in allfollowersafter)
            {
                finished += s;
            }
            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["profile_id"])
                {
                    int followers_num = u.followers - 1;
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET followerslist = '" + finished + "', followers = " + followers_num +
                                          "WHERE id = " + u.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@followerslist", finished);
                            command.Parameters.AddWithValue("@followers", followers_num);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //Removing unfollowed user id from my following list
        private void UpdateFollowingList()
        {
            List<string> allfollowings = new List<string>();
            List<string> allfollowingsafter = new List<string>();
            string followinguser = "";
            string followingstring = "";
            string finished = "";
            string user = "";

            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["my_id"])
                    followingstring = u.followings;
                if (u.id.ToString() == (String)RouteData.Values["profile_id"])
                    user = u.id.ToString();
            }
            for (int i = 0; i < followingstring.Length; i++)
            {
                if (followingstring[i] == ',')
                {
                    allfollowings.Add(followinguser);
                    followinguser = "";
                }
                else
                {
                    followinguser += followingstring[i];
                }
            }
            foreach (string s in allfollowings)
            {
                if (s != user)
                {
                    allfollowingsafter.Add(s);
                    allfollowingsafter.Add(",");
                }
            }
            foreach (string s in allfollowingsafter)
            {
                finished += s;
            }
            foreach (ProfileInfo u in allUsers)
            {
                if (u.id.ToString() == (String)RouteData.Values["my_id"])
                {
                    int following_num = u.following - 1;
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE users " +
                                          "SET followinglist = '" + finished + "', following = " + following_num +
                                          "WHERE id = " + u.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@followinglist", finished);
                            command.Parameters.AddWithValue("@following", following_num);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //Updating all the information about the profile
        private void UpdateProfileData(string str)
        {
            GetAllUsers();
            foreach (ProfileInfo pi in allUsers)
            {
                if (pi.id.ToString() == str)
                {
                    currentProfile.id = pi.id;
                    currentProfile.username = pi.username;
                    currentProfile.description = pi.description;
                    currentProfile.posts = pi.posts;
                    currentProfile.followers = pi.followers;
                    currentProfile.following = pi.following;
                    currentProfile.imagedata = pi.imagedata;
                }
            }
        }
        //Getting all users from database
        private void GetAllUsers()
        {
            string data = RouteData.Values["profile_id"].ToString();
            data2 = int.Parse(data);
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
        //Checking if profile displaying is mine
        //If yes, follow/unfollow button should be disabled
        public bool CheckIfProfileIsMine()
        {
            if ((String)RouteData.Values["my_id"] == (String)RouteData.Values["profile_id"])
            {
                return true;
            }
            return false;
        }
        //Getting all the post from table ImagesDetails
        private void GetMyPosts()
        {
            myposts.Clear();

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
                            post.link = "https://localhost:44328/ProfilePage/" + RouteData.Values["my_id"].ToString() + "/" + RouteData.Values["profile_id"].ToString();
                            post.likedby = reader.GetString(8);

                            allposts.Add(post);
                        }
                    }
                }
            }
        }
        //Filtering all posts and adding mine to myposts list
        private void ShowMyPosts()
        {
            foreach(ProfilePost p in allposts)
            {
                if(p.author == (String)RouteData.Values["profile_id"])
                {
                    myposts.Add(p);
                }
            }
        }
        //Checking if picture is already liked
        public bool IsPictureLiked(string s)
        {
            if (s.Contains((String)RouteData.Values["my_id"]))
                return true;
            else
                return false;
        }
        //Method handling liking user post
        public void OnPostOnLike(string imgId)
        {
            GetMyPosts();
            UpdateProfileData(RouteData.Values["profile_id"].ToString());
            foreach (ProfilePost p in allposts)
            {
                if (p.id == imgId)
                {
                    int likes_num = p.likes + 1;
                    string likedbyString = p.likedby + (String)RouteData.Values["my_id"] + ",";
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE ImagesDetails " +
                                          "SET likes = " + likes_num + ", likedby = '" + likedbyString + "'" +
                                          "WHERE id = " + p.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@likes", likes_num);
                            command.Parameters.AddWithValue("@likedby", likedbyString);

                            command.ExecuteNonQuery();
                            infoMessage = "successful";
                        }
                    }
                }
            }
            //Reseting values of the profile and posts after like is done
            RefreshPage();
        }
        //Handling post unlike
        public void OnPostOnUnlike(string imgId)
        {
            GetAllPosts();
            foreach (ProfilePost p in allposts)
            {
                if (p.id == imgId)
                {
                    int likes_num = p.likes - 1;
                    string likedbyString = NewLikedbyString(p.likedby);
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE ImagesDetails " +
                                          "SET likes = " + likes_num + ", likedby = '" + likedbyString + "'" +
                                          "WHERE id = " + p.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@likes", likes_num);
                            command.Parameters.AddWithValue("@likedby", likedbyString);

                            command.ExecuteNonQuery();
                            infoMessage = "successful";
                        }
                    }
                }
            }
            //Reseting values of the profile and posts after unlike is done
            RefreshPage();
        }
        //Getting all the posts
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
                            post.likedby = reader.GetString(8);
                            post.commentsString = reader.GetString(9);

                            allposts.Add(post);
                        }
                    }
                }
            }
        }
        //Creating new likedy string 
        //Likedby string is string that contains all user's ids that already liked specified post
        private string NewLikedbyString(string s)
        {
            string newString = "", workingString = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                {
                    if (workingString != (String)RouteData.Values["my_id"])
                    {
                        newString += workingString + ",";
                    }
                    workingString = "";
                }
                else
                {
                    workingString += s[i];
                }
            }
            return newString;
        }
        //Getting all the post's comments
        private void GetAllComments()
        {
            GetAllPosts();
            int index = 0;
            foreach (ProfilePost p in allposts)
            {
                string workingString = "";
                for (int i = 0; i < p.commentsString.Length; i++)
                {
                    if (p.commentsString[i] == '=')
                    {
                        p.comments.Add(new Comment());
                        p.comments[index].username = workingString;
                        workingString = "";
                    }
                    else if (p.commentsString[i] == '-')
                    {
                        p.comments[index].body = workingString;
                        index++;
                        workingString = "";
                    }
                    else
                    {
                        workingString += p.commentsString[i];
                    }
                }
                index = 0;
            }
        }
        //Method called when comment button is clicked
        public void OnPostOnComment(string imgId)
        {
            GetAllPosts();
            GetAllUsers();

            foreach (ProfilePost p in allposts)
            {
                if (p.id == imgId)
                {
                    string s = p.commentsString;
                    foreach (ProfileInfo u in allUsers)
                    {
                        if (u.id.ToString() == (String)RouteData.Values["my_id"])
                        {
                            s += u.username + "=" + Request.Form["comment"] + "-";
                        }
                    }
                    string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sqlQuery = "UPDATE ImagesDetails " +
                                          "SET comments = '" + s + "'" +
                                          "WHERE id = " + p.id + ";";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@comments", s);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            //Refreshing page and post info after posting a comment
            RefreshPage();
        }
        //Refreshing the page and setting values again after making changes on page
        private void RefreshPage()
        {
            allposts.Clear();
            myposts.Clear();
            GetMyPosts();
            GetAllComments();
            ShowMyPosts();
            UpdateProfileData(RouteData.Values["profile_id"].ToString());
        }
    }
    //Class defines profile information
    public class ProfileInfo
    {
        public int id;
        public string username;
        public string description;
        public int posts;
        public int followers;
        public int following;
        public string imagedata;
        public string followerss;
        public string followings;
    }
    //Class defines all the post information
    public class ProfilePost
    {
        public string id;
        public string author;
        public string profilepic;
        public string postdata;
        public string caption;
        public string username;
        public int likes;
        public string link;
        public string likedby;
        public string commentsString;
        public List<Comment> comments = new List<Comment>();
    }
}
