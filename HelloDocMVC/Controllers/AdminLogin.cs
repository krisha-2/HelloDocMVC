using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace HelloDocMVC.Controllers
{
    public class AdminLogin : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminLogin/Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(string Email, string Passwordhash)
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HelloDoc;User Id=postgres;Password=Krisha@1357;Include Error Detail=True");
            string Query = "select * from \"AspNetUsers\" au inner join \"AspNetUserRoles\" aur on au.\"Id\" = aur.\"UserId\" inner join \"AspNetRoles\" roles on aur.\"RoleId\" = roles.\"Id\"\r\n where \"Email\"=@Email and \"PasswordHash\"=@Passwordhash";
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@Passwordhash", Passwordhash);
            NpgsqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            int numRows = dataTable.Rows.Count;
            if (numRows > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    HttpContext.Session.SetString("UserName", row["username"].ToString());
                    HttpContext.Session.SetString("UserID", row["Id"].ToString());
                    HttpContext.Session.SetString("RoleId", row["roleid"].ToString());
                }
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../AdminLogin/Index");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "AdminLogin");
        }
        public IActionResult AuthError()
        {
            return View("../AdminLogin/AuthError");
        }
    }
}