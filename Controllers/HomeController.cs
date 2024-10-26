using E_indeks.Models;
using E_Indeks.Data;
using E_Indeks.Models;
using E_Indeks.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Localization;
using PagedList;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Rotativa;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SelectPdf;
using Microsoft.Net.Http.Headers;
using IronPdf;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Http;
using IronPdf.Extensions.Mvc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IronPdf.Rendering;

namespace E_Indeks.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<HomeController> _logger;//objekt koj go koristime za logiranje
    private readonly IStringLocalizer<HomeController> _localizer;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IRazorViewRenderer _viewRenderService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(ILogger<HomeController> logger,ApplicationDbContext db, IStringLocalizer<HomeController> localizer, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IRazorViewRenderer viewRenderService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _localizer = localizer;
        _db = db;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _viewRenderService = viewRenderService;
        _httpContextAccessor = httpContextAccessor;
    }

    //Gi zemame site Studetns od database i gi prikazhuvame na view
    /*public IActionResult Index()
    {
        //so _db.Students pristapuvame do applicationDbContext pa potoa od tamu do tabelata vo sql database
        //SO FromSqlRaw(" EXEC SelectStudents") direkno izvrshuvame vo bazata na poadatoci,vo ovoj sluchaj izvrshuvame procedurata SelectStudents
        //so toList(); go konvertirame rezultatot od procedurata vo lista od objecti od Student
        //listata ni e smestena vo GetAllStudents
        var GetAllStudents = _db.Students.FromSqlRaw(" EXEC SelectStudents").ToList();
        return View(GetAllStudents);//so ova ja isprakjame listata do soodvetniot view vo ovoj sluchaj Index
    }*/

    //SORT ORDER KAKO PARAMETAR SE PRIMA OD URL,SE PRAKJA OD VIEW SO KLIKANJE NA HEADEROT
    //SE INICIJALIZIRAAT ZA DA MOZHE DA SE KORISTAT VO VIEW
    //KREIRAME PROMENLIVA KADE GI SMESTUVAME SITE STUDENTI ,KOJA JA KORISTIME ZA DA GI PRATIME DO VIEW
    //VO ZAVISNOST KOJA VREDNOST SE PRATILA OD URL VO SORTORDER TOJ CASE SE IZVRSHUVA
    public async Task<IActionResult> Index(int? id,string sortOrder, string searchIndeks,int? pageNumber, string sortField)
    {
        //ova go pravime za da mozheme da ispratime do view
        // Initialize sorting parameters
        ViewData["IndeksSortParam"] = String.IsNullOrEmpty(sortOrder) ? "indeks_desc" : "";
        ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
        ViewData["ProsekSortParam"] = sortOrder == "prosek_asc" ? "prosek_desc" : "prosek_asc";
        ViewData["GradSortParam"] = sortOrder == "grad_asc" ? "grad_desc" : "grad_asc";

        ViewData["CurrentSearch"] = searchIndeks;

        // Fetch data from database
        //vo student gi stavame rezultatite od izvrshuvanjeto na procedurata kako lista
        var students = _db.Students.FromSqlRaw(" EXEC SelectStudents").ToList();

       
       
        //AKO VLEZE VO OVOJ IF USLOV GO MENUVA students SO TIE STUDENTI SHO GO ISPOLNUVAAT USLOVOT
        //AKO NE VLEZE VO OVOJ USLOV SI OSTANUVA students SO TOA POGORE
        if (!String.IsNullOrEmpty(searchIndeks))//ako sakame neshto da preabarame(ako vneseme neshto vo input delot za search) go dobivame preku kontrolerot
        {
            //INFORMACIJATA SHTO SME JA DOBILE OD VIEW JA PRAVIME DA BIDE SO SITE MALI BUKVI
            searchIndeks = searchIndeks.ToLower();

            // FILTRIRANJE NA STUDENTITE BAZIRAN NA INDEKSOT
            //TOLOWER GO POVIKUVAME ZA DA NEMAME CHUSTVITELNOST NA BUKVI
            // VO DATABAZATA VO TABELATA STUDENT ODI DO INDEKS PRETVORI GO VO STRING NAPRAVI GI SITE MALI BUKVI
            //SITE MALI BUKVI GI PRAVIME ZA DA IZBEGNEME CHUSTVITELNOST NA BUKVI
            //SO Contains(searchIndeks) PROVERUVAME DALI ONA SHTO SME GO DOBILE KAKO searchIndeks SE SODRZHI VO TABELATA
            students = students.Where(s => s.Indeks.ToString().ToLower().Contains(searchIndeks)).ToList();

        }
        //so klik na header od tabelata na nekoj od kolonite se prakja informacijata preku url
        //vo view toa ni e kako <a> element 
        // Apply sorting based on sortOrder

        //PROMENITE SHTO KE SE NAPRAVAT SE SMESTUVAAT VO students I POTOA TOA SE PRAKJA DO VIEW
        //NA TOJ NACHIN SE PRAKJA SORTIRANJETO
        switch (sortOrder)
        {
            //ako sortOrder ni e indeks_desc podatocite gi podreduvame po opagjachki redosled  spored indeks
            //toa go pravime so pomosh na OrderByDescending
            case "indeks_desc":
                students = students.OrderByDescending(s => s.Indeks).ToList();
                break;
                //ako za sortOrder imame name_asc se podreduvaat podatocite po rastechki redosled spored imeto 
                //so pomosh na OrderBy se pravi toa
            case "name_asc":
                students = students.OrderBy(s => s.FullName).ToList();
                break;
                //ako vo sortOrder imame name_desc se podreduvaat podatocite po opagjachki redosled 
            case "name_desc":
                students = students.OrderByDescending(s => s.FullName).ToList();
                break;
            case "prosek_asc":
                students = students.OrderBy(s => s.Prosek).ToList();
                break;
            case "prosek_desc":
                students = students.OrderByDescending(s => s.Prosek).ToList();
                break;
            case "grad_asc":
                students = students.OrderBy(s => s.Grad).ToList();
                break;
            case "grad_desc":
                students = students.OrderByDescending(s => s.Grad).ToList();
                break;
                //po default,ako ne bide kliknata nikoja kolona od headerot,podatocite ke bidat podredeni po rastechki redosled
            default:
                students = students.OrderBy(s => s.Indeks).ToList();
                break;
        }


        int pageSize = 10;//po kolku studenti da imam na edna strana
        var studentsPaginated =  PaginatedList<Student>.Create(students.ToList(), pageNumber ?? 1, pageSize);
        // Check if the index already exists
        //SqlConnection ja smestuvame vo promenlivata con
        List<Predmeti> predmetiList = new List<Predmeti>();

        string connectionString = "Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Predmeti";

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Predmeti predmet = new Predmeti
                    {
                        Indeks = reader.GetInt32(reader.GetOrdinal("Indeks")),
                        SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                        Grade = reader.GetInt32(reader.GetOrdinal("Grade"))
                    };

                    predmetiList.Add(predmet);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
        var subjectAverageGrades = predmetiList
        .GroupBy(predmet => predmet.SubjectName)
        .ToDictionary(
            group => group.Key,
            group => group.Average(predmet => predmet.Grade)
        );

        //var subjectAverageGrades = predmetiList
        //.GroupBy(predmet => predmet.SubjectName)//со ова ги фрупираме предметите според името на предметот(сите предмети со исто име ке бидат групирани заедно)
        //.ToDictionary(//групираните податоци ги групираме во речник
        //group => group.Key,//како клуч го земаме името на предметот
        ////group.key знаеме дека се однесува за името на предметот бидејќи според тој критериум го правиме групиранјето
        //group => group.Average(predmet => predmet.Grade)//како вредност се зема просечната вредност на оценките на сите предмети
        //);
        //int maxFrequency = predmetiList
        //    .GroupBy(predmet => predmet.Grade)
        //    .Max(group => group.Count());

        //// Find all grades with the maximum frequency
        //var mostFrequentGrades = predmetiList
        //    .GroupBy(predmet => predmet.Grade)
        //    .Where(group => group.Count() == maxFrequency)
        //    .Select(group => group.Key);

        int maxFrequency = 0;//PROMENLIVA ZA DA JA ZEMEME MAKSIMALNATA FREKVENCIJA
        foreach (var predmeti in predmetiList)//FOR CIKLUS ZA DA IZVRTAM ZA EDEN PREDMET
        {
            int frequency = 0;//POMOSHNA PROMENLIVA KADE KE JA SMESTUVAME FREKVENCIJATA ZA SEKOJ PREDMET
            foreach (var otherPredmeti in predmetiList)//FOR CIKLUS ZA DA IZVRTIME I DRUG PREDMET ZA DA MOZHEME SPOREDBA DA PRAVIME
            {
                if (predmeti.Grade == otherPredmeti.Grade)//AKO OCENKITE SE ISTI DA SE ZGOLEMI FREKVENCIJATA ZA TAA OCENKA
                {
                    frequency++;
                }
            }
            if (frequency > maxFrequency)//AKO FREKVENCIJATA E POGOLEMA OD MAKSIMALNATA FREKVENCIJA,TAA FREKVENCIJA DA BIDE MAKSIMALNA
            {
                maxFrequency = frequency;
            }
        }
        List<int> mostFrequentGrades = new List<int>();//LISTA ZA VO SLUCHAJ AKO IMAME POVEKJE PREDMETI SO ISTA NAJGOLEMA FREKVENCIJA
        foreach (var predmeti in predmetiList)//IZMINUVAME FOR CIKLUS ZA DA ZEMEME EDEN PREDMET
        {
            var frequency = 0;//FREKVENCIJATA NA TOJ PREDMET
            foreach (var otherPredmeti in predmetiList)//USHTE EDEN FOR CIKLUS ZA DA MOZHEME DA NAPRAVIME SPOREDBI
            {
                if (predmeti.Grade == otherPredmeti.Grade)//PROVERUVAME DALI SE JAVUVA TAA OCENKA
                {
                    frequency++;
                }
            }
            //AKO FREKVENCIJATA SHTO SME JA  NASHLE KAKO MAKSIMUM E EDNAKVA NA FREKVENCIJATA NA ZA TAA OCENKA I
            //AK POVEKJE LISTATA NE SODRZHI DRUGI OCENKI
            if (frequency == maxFrequency && !mostFrequentGrades.Contains(predmeti.Grade))
            {
                mostFrequentGrades.Add(predmeti.Grade);//DODADI JA OCENKATA VO LISTATA ZA NAJCHESTI OCENKI
            }
        }

        var sortedList = predmetiList.OrderBy(predmet => predmet.Grade).ToList();
        string centerGrades = "";
        int middleIndex = sortedList.Count / 2;//големината на листата ја делиме со 2 со цел да го добиеме средниот индекс
        if (sortedList.Count % 2 != 0)//ако големината на листата е непарен број
        {
            var centerGrade = sortedList[middleIndex].Grade;//во centerGrade го сместуваме само едниот елемент кои ние на средина
            centerGrades = centerGrade.ToString();//во овој дел го сместуваме бројот што сме го нашле за да го пратиме до view

        }
        else//ако бројот во низата парен(ако имаме парен број на предмети)
        {
            //ги земаме двата елементи кои се во средина
            var centerGrade1 = sortedList[middleIndex - 1].Grade;
            var centerGrade2 = sortedList[middleIndex].Grade;
            var sredna = (centerGrade1 + centerGrade2) / 2;
            centerGrades = sredna.ToString();
        }


        ViewData["NameSubjectSortParam"] = sortOrder == "sub_asc" ? "sub_desc" : "sub_asc";
        ViewData["AverageSortParam"] = sortOrder == "average_asc" ? "average_desc" : "average_asc";
        // Sort subject average grades based on the sortField parameter
        if (sortField == "SubjectName")
        {
            switch (sortOrder)
            {
                case "sub_asc":
                    //ако sortOrder="sub_asc" во subjectAverageGrades да се смести речникот каде ќе биде подреден по растечки редослед по клучот(името на предметот)
                    subjectAverageGrades = subjectAverageGrades.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
                    break;
                case "sub_desc":
                    //во речникот стести го резултатот од подредувањето по опаѓачки редослед на клучевите(имината на предметите) во речнкиот од клуцх и вредност
                    subjectAverageGrades = subjectAverageGrades.OrderByDescending(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
                    break;
            }
        }
        else if (sortField == "AverageGrade")
        {
            switch (sortOrder)
            {
                case "sub_asc":
                    subjectAverageGrades = subjectAverageGrades.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                    break;
                case "sub_desc":
                    subjectAverageGrades = subjectAverageGrades.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                    break;
            }

        }

        MyViewModels myViewModels = new MyViewModels()
        {
            myStudents = studentsPaginated,
            myPredmeti = predmetiList,
            SubjectAverageGrades = subjectAverageGrades,
            MostFrequentGrades = mostFrequentGrades.ToList(),
            CenterGrades = centerGrades // Assign center grades to the property
        };


        return View(myViewModels);

    }


    //za izvrshuvanje na ovaa funkcija id go zemame od URL
    public IActionResult Details(int? id)
    {
        var localizedTitle = _localizer["Title"];
        //FromSqlInterpolated znachi deka kje mozheme da vmetnuvame vrednosti vo promenlivite
        //pristapuvame do bazata na podatoci odnosno do tabelata student 
        //AsEnumerable ni ovzmozhuva da se chitaat elementite eden po eden element od sekvencata(od rezultatot)
        //FirstOrDefault se zema prviot element ili ako e prazno se zema null
        var GetSingleData = _db.Students.FromSqlInterpolated($"EXEC SelectStudentByIndex {id}").AsEnumerable().FirstOrDefault();
        var gradovi = _db.Gradovi.ToList(); // Assuming Gradovi is a DbSet in your ApplicationDbContext

        // Pass gradovi to ViewBag
        //ova se upotrebuva vaka za da mozheme ponatamu vo view da go koristime
        ViewBag.Gradovi = gradovi;

        // Retrieve the student data and return the view
        var GetSingleData2 = _db.Students.FromSqlInterpolated($"EXEC SelectStudentByIndex {id}").AsEnumerable().FirstOrDefault();

        return View(GetSingleData2);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDetails(int Indeks, string FullName, double Prosek, string Grad, string Kvota)
    {
        try
        {
            //ExecuteSqlInterpolatedAsync ni ovozmozhuva da izvrshime nekoja procedura od sql, postavuvame mesta za parametri i potoa vrednostite se vmetnuvaat vo tie mesta za parametri
            //ExecuteSqlInterpolatedAsync se koristi za izvrshuvanje na procedura koja ne vrakja rezultat,kako INSERT UPDATE ILI DELETE
            //kako rezultat ni vrakja broj na zasegnati redovi

            var StudentUpdateResult = await _db.Database.ExecuteSqlInterpolatedAsync($"Exec UpdateStudent {Indeks}, {FullName}, {Prosek}, {Grad},{Kvota}");
            if (StudentUpdateResult == 1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateKvota(int Indeks,string Kvota)
    {
        try
        {
            //ExecuteSqlInterpolatedAsync ni ovozmozhuva da izvrshime nekoja procedura od sql, postavuvame mesta za parametri i potoa vrednostite se vmetnuvaat vo tie mesta za parametri
            //ExecuteSqlInterpolatedAsync se koristi za izvrshuvanje na procedura koja ne vrakja rezultat,kako INSERT UPDATE ILI DELETE
            //kako rezultat ni vrakja broj na zasegnati redovi

            var StudentUpdateKvota = await _db.Database.ExecuteSqlInterpolatedAsync($"Exec UpdateKvota {Indeks},{Kvota}");
            if (StudentUpdateKvota == 1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            throw;
        }
    }

    //ZA DA SE PRIKAZHAT INFORMACITE PRED DA SE IZBRISHE
    //PRISTAPUVAME DO DATA BAZATA,PA DO STUDENTS I POTOA JA POVIKUVAME FROMSQLRAW FUNKCIJATA,
    public async Task<IActionResult> IndexDelete(int id)
    {
        //so _db.Students pristapuvame do tabelata na studenti vo data bazata
        //go koristime FromSqlRaw za da izvrshime neshto od databazata
        //vo ovoj sluchaj ja izvrshuvame procedurata SelectStudentByIndex koj zema parametar id
        //rezultatot koj ke go dobieme da ni bide kako nabrojuvachka vrednost
        //FirstOrDefault se koristi za da se zeme prviot element od kolekcijata ili null ako kolekcijata e prazna
        var GetSingleData = _db.Students.FromSqlRaw($"EXEC SelectStudentByIndex {id}").AsEnumerable().FirstOrDefault();
        return View(GetSingleData);//go isprakjame geSingleData preku view za da se prikazhat podatocite na ekran
    }
    //GO BRISHEME STUDENTOT OVDE
    //пост метод за делете
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            //pristapuvame do bazata na podatoci za da ja izvrshime funkcijata za brishenje.
            //vaka koristime koga sakame da pristapime do cela databaza a ne samo do tabela
            //so executeSqlInterpolatedAsync izvrshuvame  toa shto sme go stavile vo zagradata
            //vo ovoj sluchaj ovoj metod ni ovozmozhuva bezbedno vmetnuvanje na parametri
            await _db.Database.ExecuteSqlInterpolatedAsync($"EXEC DeleteStudent {id}");

            // Redirect to a success page or reload the index page
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
            return RedirectToAction("Details");
        }
    }
    
    //Get metod za Create
    public ActionResult Create()
    {
        //FromSqlRaw upotrebuvame zatoa shto treba da ni bide vratena nekoj rezultat
        var student = _db.Students.FromSqlRaw(" EXEC SelectStudents").ToList();
        //pomoshna promenliva kade gi zemame site indeksi na studenti gi pravime kako lista i gi smestuvame vo promenlivata existingIndices kako lista od stringovi
        //so select se zema samo odredena kolona,vo ovoj sluchaj samo kolonata so indeksi 
        var existingIndices = student.Select(s => s.Indeks).ToList();
        //so Join ja konkatenirame listata od stingovi i ja odeluvame so zapirka,ja smestuvame vo ExistingIndices pa ja prakjame do view
        //ovde gi smestuvame promenlivite za da gi pratime na view
        //go pravime ova za da gi oddelime so zapirka za da mozheme po lesno da proverime dali indeksot postoi
        ViewBag.ExistingIndices = string.Join(",", existingIndices);
        //gi zemame site gradovi i gi smestuvame vo promenlivata gradovi
        var gradovi = _db.Gradovi.FromSqlRaw("EXEC SelectGradovi").ToList();

        //so pomosh na ViewBag prakjame do View
        // Pass gradovi to ViewBag
        ViewBag.Gradovi = gradovi;
        return View();
    }

    //post metod za kreiranje na student
    [HttpPost]
    public ActionResult CreateNewStudent(Student student)
    {
        //служи за валидација,дали влезните параметри што се пратени од корисникот во овој случај оние во student 
        //ги исполнуваат условите за валидација
        if (!ModelState.IsValid)
        {
            return View(student);
        }
        
        // Check if the index already exists
        //SqlConnection ja smestuvame vo promenlivata con
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
        //so ovoj del ja kreirame pomoshnata promenliva za komandata
        SqlCommand cmd = con.CreateCommand();
        //koja procedura treba da ni se izvrshi
        cmd.CommandText = "Execute InsertStudent @Indeks,@FullName,@Prosek,@Grad,@Kvota";
        //gi dodavame parametrite vo procedurata na toa mesto sho ni e obelezhano za promenlivi
        cmd.Parameters.Add("@Indeks", SqlDbType.Int).Value = student.Indeks;
        cmd.Parameters.Add("@FullName", SqlDbType.VarChar,100).Value = student.FullName;
        cmd.Parameters.Add("@Prosek", SqlDbType.Float).Value = student.Prosek;
        cmd.Parameters.Add("@Grad", SqlDbType.VarChar,50).Value = student.Grad;
        cmd.Parameters.Add("@Kvota", SqlDbType.VarChar, 50).Value = student.Kvota;
        //ja otvarame konekcijata za da mozheme da ja izvrshime procedurata
        con.Open();
        //vo ovoj del ja izvrshuvame procedurata(ili query koe go imame napishano)
        cmd.ExecuteNonQuery();
        //po izvrshuvanje na procedurata ja prekinuvame konekcijata
        con.Close();
        //kako rezultat se vrakjame na tabelata so indeksi
        return RedirectToAction("Index");
    }

    public IActionResult DownloadFile(string fileName)
    {
        var memory = DownloadSingleFile(fileName, "wwwroot\\MyFile");

        if (memory != null)
        {
            string contentType;
            switch (Path.GetExtension(fileName).ToLowerInvariant())
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                default:
                    contentType = "text/plain";
                    break;
            }
            return File(memory.ToArray(), contentType, fileName);
        }
        else
        {
            // Handle the case when the file is not found or cannot be downloaded
            return NotFound(); // Or any other appropriate action
        }

    }

    private MemoryStream DownloadSingleFile(string fileName,string uploadPath)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), uploadPath, fileName);
        var memory = new MemoryStream();
        if (System.IO.File.Exists(path))
        {
            var net = new System.Net.WebClient();
            var data = net.DownloadData(path);
            var content = new System.IO.MemoryStream(data);
            memory = content;
        }
        memory.Position = 0;
        return memory;
    }


    /* public IActionResult Admin()
     {
         // Fetch users from the database
         var users = _db.Korisnici.FromSqlRaw("EXEC SelectUsers").ToList();

         // Pass the users to the view
         return View(users);
     }*/

    //ARGUMENTITE SHTO SE VO ADMIN SE DOBIVAAT PREKU URL, ODNOSNO PREKU VIEW DELOT
    //NA POCHETOKOT PRAVIME INICIJALIZACIJA(ViewData["IdSortParam"]) SO CEL DA MOZHEME DA GI KORISTIME VO VIEW DELOT
    public IActionResult Admin(string sortOrder, string searchUserName, int? pageNumber)
    {
        //go zemame userot koj e momentalno najaven
        string loggedInUserName = HttpContext.Session.GetString("UserName");
        // Initialize sorting parameter
        //OVDE SE PRAVI INICIJALIZACIJA NA SORTIRACHKITE PARAMETRI
        //VO ZAVISNOST OD TOA SHTO IMAME VO SORTORDER TOJ CASE SE ZIMA PODOLE
        ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
        ViewData["UserNameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
        ViewData["PasswordSortParam"] = sortOrder == "pass_asc" ? "pass_desc" : "pass_asc";
        ViewData["CurrentSearch"] = searchUserName;

        // Fetch data from database
        //GI ZEMAME SITE KORISNICI OD SQL SO POMOSH NA PROCEDURA I GI SMESTUVAME VO USERS
        var users = _db.Korisnici.FromSqlRaw("EXEC SelectUsers").ToList();
        if (!String.IsNullOrEmpty(searchUserName))
        {
            //INFORMACIJATA SHTO SME JA DOBILE OD VIEW JA PRAVIME DA BIDE SO SITE MALI BUKVI
            searchUserName = searchUserName.ToLower();

            //OVDE BARAME DALI ONA SHTO SME GO VNELE SE SODRZHI VO NEKOJ OD USERNAME VO TABELATA I AKO DA KAKO LISTA GO SMESTUVAME VO USERS
            //users=users(site korisnici).kade(pristaouvame do UserName na korisnikot,so ToString go pretvarame vo string ako ne e,ToLower() go pravime site mali bukvi, Contains(podatokot shto sme go vneme vo search) proveruvame dali se sodrzhi vo listata)
            users = users.Where(s => s.UserName.ToString().ToLower().Contains(searchUserName)).ToList();

        }

        //VO ZAVISNOST SHTO IMAME VO sortOrder TOJ CASE SE ZEMA
        // Apply sorting based on sortOrder
        switch (sortOrder)
        {
            case "id_desc":
                //AKO IMAME id_desc SE PODREDUVA OD NAJMALATA VREDNOST SPORED Id
                users = users.OrderByDescending(s => s.Id).ToList();
                break;
            case "name_asc":
                //AKO IMAMFE name_asc SE PODREDUVAAT PO RASTECHKI REDOSLED SPORED UserName
                users = users.OrderBy(s => s.UserName).ToList();
                break;
            case "name_desc":
                //AKO IMAME name_desc DA SE NAPRAVI PODREDUVANJE PO OPAGJACHKI REDOSLED SPORED Username
                users = users.OrderByDescending(s => s.UserName).ToList();
                break;
            case "pass_asc":
                users = users.OrderBy(s => s.Password).ToList();
                break;
            case "pass_desc":
                users = users.OrderByDescending(s => s.Password).ToList();
                break;
            default:
                users = users.OrderBy(s => s.Id).ToList();
                break;
        }

        int pageSize = 10;//po kolku korisnici da imam na edna strana
        
        //pravime ViewData za da ispratime do view userot koj e momentalno najaven
        ViewData["LoggedInUserName"] = loggedInUserName;

        //Go povikuvam metodot Create od PaginatedList i mu davam parametri
        //pageNumber go dobiva od URL(АКО Е NULL КЕ СЕ ЗЕМЕ 1)
        //vo ovoj del pravime i paginated(stranichenje)
        return View(PaginatedList<Korisnik>.Create(users.ToList(), pageNumber ?? 1, pageSize));
    }

    //pravime get metod za editiranje na korisnikot
    public IActionResult EditKorisnik(int? id)

    {

        //pristapuvame do bazata na podatoci odnosno do tabelata student 
        //AsEnumerable ni ovozmozhuva da se chitaat elementite eden po eden od listata
        //FirstOrDefault go zemame prviot element od listata ili ako nema elementi null
        var GetSingleData = _db.Korisnici.FromSqlInterpolated($"EXEC SelectKorisnikById {id}").AsEnumerable().FirstOrDefault();
        //go vrakjame korisnikot so toj indeks
        return View(GetSingleData);
    }

    //Post metod za editiranje na korisnik
    [HttpPost]
    public async Task<IActionResult> UpdateKorisnik(int Id, string userName, string Password,int IsActive)
    {
        try
        {
            //ExecuteSqlInterpolatedAsync ni ovozmozhuva da izvrshime nekoja procedura od sql, postavuvame mesta za parametri i potoa vrednostite se vmetnuvaat vo tie mesta za parametri
            //ovaa funkcija ja koristime bidejki ne ochekuvame da ni vrati rezultat
            var KorisnikUpdateResult = await _db.Database.ExecuteSqlInterpolatedAsync($"Exec UpdateKorisnik {Id}, {userName}, {Password},{IsActive}");
            if (KorisnikUpdateResult == 1)
            {
                //ako imame podatok vo korisnikUpdateResult ni se vrakja kaj Admin View
                return RedirectToAction("Admin");
            }
            else
            {
                return View();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            throw;
        }
    }
    //get metod za da ni se prikazhat informaciite pred da go izbrisheme korisnikot
    //kako parametar na funkcijata go isprakjame id preku url od view
    public async Task<IActionResult> IndexDeleteKorisnik(int id)
    {
        //so _db.Korisnici pristapuvame do tabelata na korisnici vo data bazata
        //go koristime FromSqlRaw za da izvrshime neshto od databazata
        //vo ovoj sluchaj ja izvrshuvame procedurata SelectKorisnikByIndex koj zema parametar id
        //rezultatot koj ke go dobieme da ni bide kako nabrojuvachka vrednost i elementite mozhe da se chitaat eden po eden
        //FirstOrDefault se koristi za da se zeme prviot element od kolekcijata ili null ako kolekcijata e prazna
        var GetSingleData = _db.Korisnici.FromSqlRaw($"EXEC SelectKorisnikById {id}").AsEnumerable().FirstOrDefault();
        //vaka return pravime najchesto vo get metodite
        return View(GetSingleData);//go isprakjame geSingleData preku view za da se prikazhat podatocite na ekran
    }
    //post metod za brishenje na korisnik
    public async Task<IActionResult> DeleteKorisnik(int? id)
    {
        try
        {
            //pristapuvame do bazata na podatoci za da ja izvrshime funkcijata za brishenje.
            //vaka koristime koga sakame da pristapime do cela databaza a ne samo do tabela
            //so executeSqlInterpolatedAsync izvrshuvame  toa shto sme go stavile vo zagradata
            //vo ovoj sluchaj ovoj metod ni ovozmozhuva bezbedno vmetnuvanje na parametri
            //reshenieto od funkcijava ne go smestuvame vo ni edna promenliva bidejki treba samo da se izbrishe
            await _db.Database.ExecuteSqlInterpolatedAsync($"EXEC DeleteKorisnik {id}");

            //ako uspeshno se izvrshila funkcijata vrati me kaj Admin View
            // Redirect to a success page or reload the index page
            return RedirectToAction("Admin");
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
            return RedirectToAction("Details");
        }
    }
    public IActionResult ViewSubjects(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        //Koristime databazata,se povrzuvame
        using (var con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
        {
            //ja otvarame konekcijata za da mozheme da pravime promeni
            con.Open();
            //pravime promenliva kade ke mozheme da ja smestime komandata shto sakame da ja izvrshime
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SelectUspeh";//imeto na komandata
                cmd.CommandType = CommandType.StoredProcedure;//kakov tip
                cmd.Parameters.AddWithValue("@Indeks", id);//dodavame parametar bidejki takva e procedurata
                //ExecuteReader() се користи кога командата враќа резултати
                using (var reader = cmd.ExecuteReader())//vo reader go smestuvame rezultatot od izvrshenata komanda
                {
                    var subjects = new List<Predmeti>();//promenliva kade ke smestime lista od predmeti,za da mozheme da ja pratime kaj View
                    while (reader.Read())//chitame red po red od reader,koga ke snema redovi prekinuva while
                    {
                        var subject = new Predmeti//ovde gi smestuvame site rezultati zaedno za predmetite
                        {
                            //ВО ОВОЈ ДЕЛ СЕ ЗЕМААТ ЕЛЕМЕНТИТЕ ОД КОЛОНИТЕ ВО РЕДИЦАТА(КОЈА НИ Е СМЕСТЕНА ВО reader) И СЕ СМЕСТУВААТ СООДВЕТНО
                            //GetInt32 e metod koi se koristi za da se prochita int vrednost od kolonata,kako argument prima pozicija
                            //so reader.GetOrdinal("Indeks") ja zemame pozicijata na Indeks vo redot koi go imame vo reader
                            Indeks = reader.GetInt32(reader.GetOrdinal("Indeks")),
                            //so GetString se chita string vrednost od kolonata
                            SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade"))
                        };
                        subjects.Add(subject);//ovie podatoci gi dodavame vo listata od predmeti
                    }

                    //овде ја земаме информацијата за IsAdmin  од корисникот кои е моментално најавен
                    int loggedInUserName = HttpContext.Session.GetInt32("IsAdmin")??0;
                    // Pass isAdmin status to the view
                    //ова го користиме за да можеме да испратиме информација за IsAdmin до View
                    ViewData["IsAdmin"] = loggedInUserName;
                    return View(subjects);//listata od predmeti ja prakjame do view
                }
            }
        }
    }
    public async Task<IActionResult> UpdateGrade(int indeks, int[] newGrade, string[] subjectName)
    {
        try
        {
            string connectionString = "Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UpdateGrade"; // Name of the stored procedure
                    command.CommandType = CommandType.StoredProcedure;

                    // formirame for ciklus za da se update site ocenki a ne samo prvata
                    //for ke vrti kolku shto imame ocenki
                    for (int i = 0; i < newGrade.Length; i++)
                    {
                        //gi brisheme starite parametri pred da postavime novi parametri
                        command.Parameters.Clear(); // Clear parameters before setting new ones

                        //Za sekoj predmet dodavame nova ocenka i imeto na predmetot
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Indeks", indeks);
                        command.Parameters.AddWithValue("@Grade", newGrade[i]);
                        command.Parameters.AddWithValue("@SubjectName", subjectName[i]);

                        //ja izvrshuvame komandata od koga ke gi dodademe parametrite
                        // Execute the command
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            ////OVA E KODOT ZA UPDATE NA PROSEKOT VO INDEX DELOT ZA STUDENTI
            var students = _db.Students.FromSqlRaw(" EXEC SelectStudents").ToList();
            //go izminuvame sekoj student od listata na studenti vo students
            foreach (var student in students)
            {
                // Call the stored procedure to update the Prosek for each student
                //ja zemame bazata
                using (var connection = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
                {
                    connection.Open();//se povrzuvame so bazata na podatovi
                    using (var command = new SqlCommand("UpdateProsek", connection))//kreirame nova komanda,kade ja smestuvame procedurata koja ke ni treba
                    {
                        command.CommandType = CommandType.StoredProcedure;//tipot na comandata
                        command.Parameters.AddWithValue("@Indeks", student.Indeks);//dodavame parametar(Indeksot)bidejki ni treba vo procedurata)
                        command.ExecuteNonQuery();//ja izvrshuvame comandata koja sme ja napravile
                    }
                }

                // Fetch the updated Prosek value from the database and update the student object
                //    со ова проверуваме дали индексот на студентото е ист со тој индекс кои моментално го имаме
                //го наоѓаме студентот со конкретен индекс и го сместуваме во updatedStudent
                var updatedStudent = _db.Students.FirstOrDefault(s => s.Indeks == student.Indeks);//go zemame studentot so odreden indeks
                //vo s.Indeks se zema studentot od _db.Students, a vo student.Indeks se zema indeksot na studentot koi momentalno se razgleduva
                if (updatedStudent != null)
                {
                    student.Prosek = updatedStudent.Prosek;//prosekot koj sme go updejtirale go smestuvame na mestoto na stariot prosek
                                                           //vo student ni se naogjaa eden od studentite koi sme go zemale so for-ot pogore
                }
            }


            // Redirect to the same view after the update
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Handle the exception
            return RedirectToAction("Index");
        }
    }

    /*        public async Task<IActionResult> UpdateGrade(int id,int Grade)
            {
                try
                {
                    var GradeUpdateResult = await _db.Database.ExecuteSqlInterpolatedAsync($"Exec UpdateGrade {id}, {Grade}");
                    if (GradeUpdateResult == 1)
                    {
                        return RedirectToAction("ViewSubjects");
                    }
                    else
                    {
                        return View();
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception:"+ex.Message);
                    throw;
                }
            }
    */
    public IActionResult Report(int? id)
    {

        if (id == null)
        {
            return NotFound();
        }

        //Koristime databazata,se povrzuvame
        using (var con = new SqlConnection("Data Source=DESKTOP-1DEPT4C\\SQLEXPRESS;database=Student;Integrated Security=True;Connect Timeout=30;Encrypt=False;"))
        {
            //ja otvarame konekcijata za da mozheme da pravime promeni
            con.Open();
            //pravime promenliva kade ke mozheme da ja smestime komandata shto sakame da ja izvrshime
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SelectUspeh";//imeto na komandata
                cmd.CommandType = CommandType.StoredProcedure;//kakov tip
                cmd.Parameters.AddWithValue("@Indeks", id);//dodavame parametar bidejki takva e procedurata
                                                           //ExecuteReader() се користи кога командата враќа резултати
                using (var reader = cmd.ExecuteReader())//vo reader go smestuvame rezultatot od izvrshenata komanda
                {
                    var subjects = new List<Predmeti>();//promenliva kade ke smestime lista od predmeti,za da mozheme da ja pratime kaj View
                    while (reader.Read())//chitame red po red od reader,koga ke snema redovi prekinuva while
                    {
                        var subject = new Predmeti//ovde gi smestuvame site rezultati zaedno za predmetite
                        {
                            //ВО ОВОЈ ДЕЛ СЕ ЗЕМААТ ЕЛЕМЕНТИТЕ ОД КОЛОНИТЕ ВО РЕДИЦАТА(КОЈА НИ Е СМЕСТЕНА ВО reader) И СЕ СМЕСТУВААТ СООДВЕТНО
                            //GetInt32 e metod koi se koristi za da se prochita int vrednost od kolonata,kako argument prima pozicija
                            //so reader.GetOrdinal("Indeks") ja zemame pozicijata na Indeks vo redot koi go imame vo reader
                            Indeks = reader.GetInt32(reader.GetOrdinal("Indeks")),
                            //so GetString se chita string vrednost od kolonata
                            SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                            Grade = reader.GetInt32(reader.GetOrdinal("Grade"))
                        };
                        subjects.Add(subject);//ovie podatoci gi dodavame vo listata od predmeti
                    }

                    //овде ја земаме информацијата за IsAdmin  од корисникот кои е моментално најавен
                    int loggedInUserName = HttpContext.Session.GetInt32("IsAdmin") ?? 0;
                    // Pass isAdmin status to the view
                    //ова го користиме за да можеме да испратиме информација за IsAdmin до View
                    ViewData["IsAdmin"] = loggedInUserName;
                    var GetSingleData2 = _db.Students.FromSqlInterpolated($"EXEC SelectStudentByIndex {id}").AsEnumerable().FirstOrDefault();
                    ReportModels myViewModels = new ReportModels()
                    {
                        myPredmeti = subjects,
                        // Assign center grades to the property
                        myStudent = GetSingleData2,
                        CurrentDate = DateTime.Today
                    };
                    ChromePdfRenderer renderer = new ChromePdfRenderer();
                    renderer.RenderingOptions.CssMediaType = PdfCssMediaType.Print;
                    IronPdf.PdfDocument pdf = renderer.RenderRazorViewToPdf(_viewRenderService, "Views/Home/Report.cshtml", myViewModels);

                    // Return the PDF document
                    Response.Headers.Add("Content-Disposition", "inline");
                    return File(pdf.BinaryData, "application/pdf", "viewToPdfMVCCore.pdf");

                }
            }
        }
        /* var GetSingleData = _db.Students.FromSqlRaw($"EXEC SelectStudentByIndex {id}").AsEnumerable().FirstOrDefault();*/
/*            return View(GetSingleData);*/
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
   




}