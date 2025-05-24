using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using TwoFactorAuthentication.Helpers;
using TwoFactorAuthentication.Services;
namespace TwoFactorAuthentication.Controllers;

[Route("Login")]
public class LoginController : Controller
{


    [HttpGet(nameof(Index))]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost(nameof(Index))]
    public IActionResult Index(string email, string username, string password)
    {
        if (username == "admin" && password == "1234")
        {
            string secretCode = string.Empty;
            var stopwatch = Stopwatch.StartNew(); // Zamanlayıcıyı başlat
            secretCode = GenerateTwoFactorCode();
         
            HttpContext.Session.SetString("TwoFactorCode", secretCode);
            HttpContext.Session.SetString("UserName", username);

            SendMail(email,secretCode);
            stopwatch.Stop(); // Zamanlayıcıyı durdur
            Console.WriteLine($"Fonksiyon süresi: {stopwatch.ElapsedMilliseconds} ms");
            return RedirectToAction("TwoFactor");
        }

        ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
        return View();
    }

    [HttpGet(nameof(TwoFactor))]
    public IActionResult TwoFactor()
    {
        return View();
    }

    [HttpPost(nameof(VerifyCode))]
    public IActionResult VerifyCode(string code)
    {
        var expectedCode = HttpContext.Session.GetString("TwoFactorCode");
        var username = HttpContext.Session.GetString("UserName");
        if (!TwoFALimiter.IsAllowed(username))
        {
            ViewBag.Error = "Çok fazla deneme yaptınız. Lütfen 5 dakika sonra tekrar deneyin.";
            return View("Index");
        }

        if (code == expectedCode)
        {
            TwoFALimiter.Reset(username);
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
        return View("TwoFactor");
    }

    private void SendMail(string email,string verificationCode)
    {
        var emailSender = new MailService();
        emailSender.SendEmail(
            fromEmail: "wnss433@gmail.com",
            toEmail: email,
            subject: "Verification Code",
            body: "<b>"+ verificationCode + "</b>",
            smtpServer: "smtp.gmail.com",
            port: 587,
            username: "wnss433@gmail.com",
            password: "xtcw tbwz wyou nxmu"
        );
    }

    private string GenerateTwoFactorCode() { 
    
        // p ve q büyük ve mod 4 ≡ 3 olan asal sayılar
        BigInteger p = BigInteger.Parse("9767004122294309419553464204387980056645056427386287437353631667657057416229974543300062960928736378357480457424650995694616552982640435669718802225678359");
        BigInteger q = BigInteger.Parse("12709274962289423451030864605822882507896641222656008607779725686353632015882771008595817358672513223741575650345932554609008120464634073855185437191205127");

        // Seed bileşenleri
        BigInteger timeComponent = new BigInteger(DateTime.UtcNow.Ticks);
        BigInteger guidComponent = new BigInteger(Guid.NewGuid().ToByteArray());
        BigInteger randomComponent = new BigInteger(RandomNumberGenerator.GetBytes(64));

        // XOR işlemiyle karıştır, negatiflik kontrolü
        BigInteger seed = BigInteger.Abs(timeComponent ^ guidComponent ^ randomComponent);
        if (seed == 0) seed = 1;

        var bbs = new BlumBlumShub(seed, p, q);

        // 6 haneli 2FA kodu üret
        string code = "";
        for (int i = 0; i < 6; i++)
        {
            code += (bbs.NextNumber(4) % 10);
        }

        return code;
    }

    public static class TwoFALimiter
    {
        private static readonly Dictionary<string, (int attempts, DateTime lastAttempt)> _attempts = new();

        private static readonly object _lock = new();

        public static bool IsAllowed(string key)
        {
            lock (_lock)
            {
                if (_attempts.TryGetValue(key, out var entry))
                {
                    if (entry.attempts >= 3 && DateTime.UtcNow - entry.lastAttempt < TimeSpan.FromMinutes(5))
                        return false;

                    if (DateTime.UtcNow - entry.lastAttempt > TimeSpan.FromMinutes(5))
                        _attempts[key] = (1, DateTime.UtcNow);
                    else
                        _attempts[key] = (entry.attempts + 1, DateTime.UtcNow);
                }
                else
                {
                    _attempts[key] = (1, DateTime.UtcNow);
                }

                return true;
            }
        }

        public static void Reset(string key)
        {
            lock (_lock)
            {
                _attempts.Remove(key);
            }
        }
    }

}
