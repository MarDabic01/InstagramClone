using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InstagramCloneWebApp.DataLayer;
using InstagramCloneWebApp.Entities;
using System.IO;
using System.Data.SqlClient;

namespace InstagramCloneWebApp.Pages
{
    public class UploadPhotoPageModel : PageModel
    {
        //Variables for storing searcing results
        public List<UserInfo> foundUsers = new List<UserInfo>();
        List<UserInfo> allUsers = new List<UserInfo>();
        public List<string> links = new List<string>();//Storing links for all found users
        public string searchingString = "";

        //Variables for uploading picture
        public string infoMessage = "";
        private readonly EmpDBContext _context;
        public int data2;
        private string aut = "", autname = "", autpic = "";

        [BindProperty]
        public ImageEntity _empData { get; set; }

        public UploadPhotoPageModel(EmpDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        //Method called when user is uploading photo
        public void OnPost()
        {
            //Storing image into byte array 
            byte[] bytes = null;
            if(_empData.ImageFile != null)
            {
                using (Stream fs = _empData.ImageFile.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }
                GetAllUsers();
                FinAuthor();//Finding picture author's id
                _empData.ImageAuthor = aut;
                _empData.ImageDecsription = _empData.ImageDecsription;
                _empData.ImageDate = DateTime.Now;
                _empData.ImageData = Convert.ToBase64String(bytes, 0, bytes.Length);//Converting byte array to string and storing it to database
                _empData.authorname = autname;
                _empData.authorpic = autpic;
                infoMessage = "Image successfully uploaded";
            }
            _context.ImagesDetails.Add(_empData);
            _context.SaveChanges();
            IncrementPostsValue();
        }

        //Incrementing posts value of the user, which will be displayed on user profile
        private void IncrementPostsValue()
        {
            foreach(UserInfo u in allUsers)
            {
                if(u.id == (String)RouteData.Values["passed_id"])
                {
                    int new_posts_num = u.postsnum + 1;//Adding 1 to current posts value
                    try
                    {
                        string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ReachMeDB;Integrated Security=True";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sqlQuery = "UPDATE users " +
                                              "SET posts = " + new_posts_num +
                                              "WHERE id ='" + u.id +"'";
                            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                            {
                                command.Parameters.AddWithValue("@posts", new_posts_num);

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        //Method for finding image author id
        private void FinAuthor()
        {
            foreach(UserInfo u in allUsers)
            {
                if(u.id == (String)RouteData.Values["passed_id"])
                {
                    aut = u.id;
                    autname = u.username;
                    autpic = u.picdata;
                }
            }
        }

        //Logging out the user and redirecting to main page
        public void OnPostLogOut()
        {
            Response.Redirect("https://localhost:44328/");
        }

        //Taking user to the upload photo page 
        public void OnpostPicture()
        {
            string redirectString;
            redirectString = "https://localhost:44328/UploadPhotoPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }

        //Called when user is searching for other accounts
        public void OnPostSearch()
        {
            searchingString = Request.Form["searchbar"];//Getting string value from the searchbar
            if (searchingString.Length > 0)
            {
                GetAllUsers();
                foreach (UserInfo u in allUsers)
                {
                    if (u.username.Contains(searchingString))
                    {
                        string linkString = "https://localhost:44328/ProfilePage/" + RouteData.Values["passed_id"] + "/" + u.id;

                        links.Add(linkString);
                        foundUsers.Add(u);//Adding found user to foundUsers list
                    }
                }
            }
        }

        //Redirecting user to home page
        public void OnPostToHome()
        {
            string redirectString;
            redirectString = "https://localhost:44328/HomePage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to profile page
        public void OnPostToProfile()
        {
            string redirectString;
            redirectString = "https://localhost:44328/ProfilePage/" + RouteData.Values["passed_id"].ToString() + "/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to discover page
        public void OnPostToDiscover()
        {
            string redirectString;
            redirectString = "https://localhost:44328/DiscoverPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Redirecting user to account page
        public void OnPostToAccount()
        {
            string redirectString;
            redirectString = "https://localhost:44328/AccountPage/" + RouteData.Values["passed_id"].ToString();
            Response.Redirect(redirectString);
        }
        //Getting all the users from table 
        private void GetAllUsers()
        {
            string data = RouteData.Values["passed_id"].ToString();
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
                            UserInfo user = new UserInfo();
                            user.id = reader.GetInt32(0).ToString();
                            user.email = reader.GetString(1);
                            user.username = reader.GetString(2);
                            user.password = reader.GetString(3);
                            user.repeatpassword = reader.GetString(4);
                            user.postsnum = reader.GetInt32(7);
                            user.picdata = reader.GetString(10);

                            allUsers.Add(user);
                        }
                    }
                }
            }
        }
    }
    public class UserInfo
    {
        public string id;
        public string email;
        public string username;
        public string password;
        public string repeatpassword;
        public int postsnum;
        public string picdata;
    }
}
