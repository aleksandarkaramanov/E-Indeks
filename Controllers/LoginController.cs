using E_indeks.Models;
using E_Indeks.Data;
using E_Indeks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;

namespace E_Indeks.Controllers
{
    public class LoginController : Controller
    {
        //pravime connectionString za povrzuvanje so bazata
        private readonly string connectionString = "Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
        private readonly ApplicationDbContext _db;
        public LoginController( ApplicationDbContext db)
        {
            _db = db;
        }
        //Get metod za prikazhuvanje na login delot
        public IActionResult Index()
        {
            return View();
        }

        //Post metod za Login delot
        public async Task<IActionResult> Login(string userName,string password)
        {
            Korisnik korisnik = null;//kreirame object od tipot(model) Korisnik koi ke se vika korisnik(Ovoj object go koristime za da gi chuvame podatocite na korisnicite)
            using (SqlConnection connection = new SqlConnection(connectionString))//so ova kreirame nova vrska za da mozheme da pristapime do bazata na podatoci
            {
                using (SqlCommand command=new SqlCommand("SelectKorisnik", connection))//kreirame sql komanda za povikuvanje na procedura i ja koristime prehodno sozadenata vrska
                {
                    command.CommandType = CommandType.StoredProcedure;//tipot na naredbata ja postavuvame kako stored procedura
                    //bidejki vo procedurata koja ni e napishana vo sql ni trebaat parametri gi dodavame parametrive
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();//ja otvarame konekcijata so bazata na podatoci

                    using(SqlDataReader reader = command.ExecuteReader())//vo reader smestuvame rezultati po izvrshuvanje na procedurata
                    {
                        if (reader.Read())//Proveruvame dali ima rezultati od izvrshuvanjeto na procedurata,ako ima znachi e najden takov korisnik
                        {
                            //ako e najden takov korisnik
                            korisnik = new Korisnik()//go popolnuvame objektot(modelot) od tip korisnik
                            {
                                //rezultatot od procedurata go smestuvame vo modelot za potoa da proveruvame 
                                UserName = reader["UserName"].ToString(),//UserName(OD MODELOT KORISNIK).reader["UserName].ToString()(TOA [TO SME GO DOBILE KAKO REZULTAT OD PROCEDURATA)
                                Password = reader["Password"].ToString(),
                                IsActive = Convert.ToInt32(reader["IsActive"]),
                                IsAdmin = Convert.ToInt32(reader["IsAdmin"])
                            };
                        }
                    }

                }
            }
            if (korisnik != null && korisnik.IsActive == 1)//ako sme go popolnile korisnik so info shto sme gi dobile od procedurata i ako korisnikot e aktiven
            {
                //СО HttpContext пристапуваме до тековната http средина
                //со Session пристапуваме до сесијата на корисникот,со сесијата ги зачувуваме податоците 
                //со SetString зачувуваме string вредности во сесијата кои ке ги користиме понатаму
                //SetString прима две вредности и тоа клуч кои ни служи за да можеме UserName да го користиме во другите делови
                //а korisnik.UserName ни е вресноста што ќе ја зачуваме во UserName

                //HttpContext пристапуваме до http средината,во таа средина со помош на session пристапуваме до сесијата на корисникот
                //пристапените информации ги поставуваме во променливи кои потоа ке можеме да ги користиме, со помош на SetString
                HttpContext.Session.SetString("UserName", korisnik.UserName);
                HttpContext.Session.SetString("Password", korisnik.Password);
                HttpContext.Session.SetInt32("IsAdmin", korisnik.IsAdmin);
                return  RedirectToAction("Index","Home");//da se pridvizhime na home delot
            }
            //ako postoi takov korisnik i ako ne e aktiven
            else if (korisnik != null && korisnik.IsActive == 0) // Check if user is not active
            {
                TempData["ErrorInactive"] = "Your account is inactive. Please contact the administrator.";
                return RedirectToAction("Index", "Login");
            }
            //ako ne postoi takov korisnik
            else
            {
                TempData["Error"] = "Wrong credentials. Please, try again.";
                return RedirectToAction("Index","Login");
            }

        }

        //metod za odjavuvanje,samo get ni treba
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Login");
        }
        //Get metod za promena na pw
        public IActionResult ChangePassword()
        {

            return View();
        }
        //Post  metod za promena na pw
        //oldPassword i newPassword gi dobivame preku view delot i go prakjame do kontrolerot
        public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword)
        {
            //vo userName go smestuvame korisnikot koj e momentalno najaven
            string userName = HttpContext.Session.GetString("UserName");// Access HttpContext directly

            //vo oldPasswordT go smestuvame passwordot na korisnikot koj e momentalno najaven
            string oldPasswordT = HttpContext.Session.GetString("Password");

            Debug.WriteLine("Username:  ", userName);
            Debug.WriteLine("Old Password: ",oldPasswordT);
            try
            {
                //ko koristime connecting string za povrzuvanje so bazata,koi sme go napishale pogore
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //ovde se povikuva procedurata za update na password
                    using (SqlCommand command = new SqlCommand("UpdatePassword", connection))
                    {
                        //pravime proverka dali stariot pw e toj koi sme go vnele
                        if (oldPasswordT != oldPassword)
                        {
                            TempData["ErrorMessage"] = "Password is not correct.";
                            return RedirectToAction("ChangePassword");
                        }
                        //ako sme vneme taman pw se povikuva procedurata
                        //za parametri gi dodavame vrednostite 
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NewPassword", newPassword);
                        command.Parameters.AddWithValue("@UserName",userName);
                        //ja otvarame konekcijata za izvrshuvanje na procedurata
                        connection.Open();
                        //vo rowsAffected go smestuvame rezultatot od izvrshuvanje na komandata
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Password updated successfully.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Failed to update password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the password: " + ex.Message;
            }

            return RedirectToAction("ChangePassword", "Login");
        }
        //Get metod za registracija
        public ActionResult Registration()
        {

            return View();
        }
        /*[HttpPost]
        public ActionResult RegistrationUser(Korisnik user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Check if the index already exists
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "Execute InsertKorisnik @UserName,@Password";
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar,20).Value = user.UserName;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar,20).Value = user.Password;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index","Login");
        }*/


        //Post metod za registracija na korisnik
        [HttpPost]
        //parametarot vo metodot go dobivame od view 
        public ActionResult RegistrationUser(Korisnik user)
        {
            //proveruvame dali user koi go imame prateno kako argument gi ispolnuva uslovite za validacija
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            bool usernameExists;//pomoshna promenliva  za dali korisnikot postoi

            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SelectUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Execute the procedure to get all users
                SqlDataReader reader = cmd.ExecuteReader();//rezultatot od procedurata go smestuvame vo reader

                // Check if the username already exists
                usernameExists = false;//na pochetok stavame deka username ne ni postoi
                while (reader.Read())//ja chitame toa shto ni e smesteno vo reader,chitame red po red
                {
                    //vo reader ni e smesten rezultatot po izvrzhuvanje na procedurata
                    //vo existingUsername go smestuva UserName od redot koj e zemen,vo string forma
                    string existingUsername = reader["UserName"].ToString();
                    if (existingUsername.Equals(user.UserName))//proveruvame dali korisnichkoto ime shto sme go vnele vo input delot vo view
                    //e ednakov so toa shto sme go zele vo redot(vo ovoj while chitame red po red od reader)
                    {
                        usernameExists = true;//ako username vekje postoi
                        break;
                    }
                }
                //go zatvoram reader delot
                reader.Close();
            }

            if (usernameExists)//dokolku username vekje postoi se ispishuva poraka deka postoi username
            {
                TempData["Error"] = "Username already exists.";
                //ako postoi korisnikot prefrli se na registration
                return RedirectToAction("Registration");
            }
            else
            {
                //dokolku username ne postoi uspeshno se najavuvame
                TempData["Success"] = "User successfully registered.";
            }

            //so ovoj del go smestuvame noviot registriran korisnik vo databazata ako prethodnite uslovi se kako shto treba
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
            {
                con.Open();//ja otvarame databazata
                SqlCommand cmd = con.CreateCommand();//kreirame promenlica kade ke smestime komandata shto sakame da ja izvrshime
                cmd.CommandText = "Execute InsertKorisnik @UserName,@Password,@IsActive,@IsAdmin";
                //dodavame parametri na procedurata bidejki ni e potrebno toa od samata procedura
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 20).Value = user.UserName;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 20).Value = user.Password;
                cmd.Parameters.Add("@IsActive", SqlDbType.Int).Value = 1; //е активен
                cmd.Parameters.Add("@IsAdmin", SqlDbType.Int).Value = 0;  //не е админ
                cmd.ExecuteNonQuery();//so ovoj del ja izvrshuvame komandata shto e smestena vo cmd
            }

            return RedirectToAction("Registration", "Login");
        }
    }
}
