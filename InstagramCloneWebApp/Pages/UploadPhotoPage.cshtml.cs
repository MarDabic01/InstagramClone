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
        List<UserInfo> allUsers = new List<UserInfo>();
        public string infoMessage = "";
        private readonly EmpDBContext _context;
        public int data2;
        private string aut = "";

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

        public void OnPost()
        {
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
                FinAuthor();
                _empData.ImageAuthor = aut;
                _empData.ImageDecsription = _empData.ImageDecsription;
                _empData.ImageDate = DateTime.Now;
                _empData.ImageData = Convert.ToBase64String(bytes, 0, bytes.Length);
                infoMessage = "Image successfully uploaded";
            }
            _context.ImagesDetails.Add(_empData);
            _context.SaveChanges();
            
            //return Redirect("https://localhost:44328/UploadPhotoPage/" + RouteData.Values["passed_id"]);
        }

        private void FinAuthor()
        {
            foreach(UserInfo u in allUsers)
            {
                if(u.id == (String)RouteData.Values["passed_id"])
                {
                    aut = u.username;
                }
            }
        }

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
    }
}