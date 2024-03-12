
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using HelloDocMVC.Entity.DataContext;

namespace HelloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View( );
        }
        public IActionResult Forgetpass()
        {
            return View("../Login/Forgetpass");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Validate(string Email, string PasswordHash)
        //{
        //    NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=HelloDoc;User Id=postgres;Password=Krisha@1357;Include Error Detail=True");
        //    string Query = "SELECT * FROM  \"AspNetUsers\" where \"Email\"=@Email and \"PasswordHash\"=@PasswordHash";
        //    connection.Open();
        //    NpgsqlCommand command = new NpgsqlCommand(Query, connection);
        //    command.Parameters.AddWithValue("@Email", Email);
        //    command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
        //    NpgsqlDataReader reader = command.ExecuteReader();
        //    DataTable dataTable = new DataTable();
        //    dataTable.Load(reader);
        //    int numRows = dataTable.Rows.Count;
        //    if (numRows > 0)
        //    {
        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            HttpContext.Session.SetString("UserName", row["username"].ToString());
        //            HttpContext.Session.SetString("UserID", row["Id"].ToString());
        //        }
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    else
        //    {
        //        ViewData["error"] = "Invalid Id Pass";
        //        return View("../Login/Index");
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> Index(string Email, string PasswordHash)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == PasswordHash);

            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../Login/Index");
            }
            else
            {
                int id = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id).UserId;
                string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                _httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                _httpContextAccessor.HttpContext.Session.SetString("Name", userName);

                return RedirectToAction("Index", "PatientDashboard");
            }
        }
            public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        //[HttpGet]
        //public IActionResult Resetpass(string email, string datetime)
        //{
        //    //Encyptdecypt en = new Encyptdecypt();
        //    TempData["email"] = email;
        //    TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
        //    if (time.TotalHours > 24)
        //    {
        //        return View("LinkExpired");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public IActionResult SavePassword(string email, string Password)
        //{
        //    //var hasher = new PasswordHasher<string>();
        //    //string hashedPassword = hasher.HashPassword(null, Password);
        //    var aspnetuser = _context.AspNetUsers.FirstOrDefault(m => m.Email == email);
        //    if (aspnetuser != null)
        //    {
        //        aspnetuser.PasswordHash = Password;
        //        _context.AspNetUsers.Update(aspnetuser);
        //        _context.SaveChangesAsync();
        //        TempData["emailmessage"] = "Your password is changed!!";
        //        return RedirectToAction("Index", "Login");
        //    }
        //    else
        //    {
        //        TempData["emailmessage"] = "Email is not registered!!";
        //        return View("Resetpass");
        //    }
        //}
        //public async Task<IActionResult> resetEmail(string Email)
        //{
        //    if (await CheckregisterdAsync(Email))
        //    {
        //        var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Email);
        //        aspnetuser.PasswordHash = generatepass();
        //        aspnetuser.ModifiedDate = DateTime.Now;
        //        try
        //        {
        //            _context.Update(aspnetuser);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AspnetuserExists(aspnetuser.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        //        string email = Email;
        //        DateTime dateTime = DateTime.Now;
        //        string datetime = dateTime.ToString();
        //        string resetLink = $"https://localhost:44398/Login/Resetpass?email={email}&datetime={datetime}";
        //        //send mail
        //        MailMessage MM = new();
        //        MM.From = new MailAddress(_emailConfig.From);
        //        MM.Subject = "Change PassWord";
        //        MM.To.Add(new MailAddress(Email));
        //        MM.Body = $@"
        //        <html>
        //        <body>
        //            <p>We received a request to reset your password.</p>
        //            <p>To reset your password, click the following link:</p>
        //            <p><a href=""{resetLink}"">Reset Password</a></p>
        //            <p>If you didn't request a password reset, you can ignore this email.</p>
        //        </body>
        //        </html>";
        //        MM.IsBodyHtml = true;
        //        using (var smtpClient = new SmtpClient(_emailConfig.SmtpServer))
        //        {
        //            smtpClient.Port = _emailConfig.Port;
        //            smtpClient.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
        //            smtpClient.EnableSsl = true;
        //            smtpClient.Send(MM);
        //        }
        //        ViewData["EmailCheck"] = "Your ID Pass Send In Your Mail";
        //    }
        //    else
        //    {
        //        ViewData["EmailCheck"] = "Your Email Is not registered";
        //        return View("Resetpass");
        //    }
        //    return RedirectToAction("Index", "Login");
        //}
        //private bool AspnetuserExists(string id)
        //{
        //    return (_context.AspNetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
        //public async Task<bool> CheckregisterdAsync(string email)
        //{
        //    string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        //    if (!string.IsNullOrEmpty(email) && Regex.IsMatch(email, pattern))
        //    {
        //        var U = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
        //        if (U != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private static Random random = new Random();
        //public static string generatepass()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    return new string(Enumerable.Repeat(chars, 8)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //}
    }
}
