using Microsoft.AspNetCore.Mvc;
using System.Numerics;
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

            secretCode = GenerateTwoFactorCode(); 
            HttpContext.Session.SetString("TwoFactorCode", secretCode);
            HttpContext.Session.SetString("UserName", username);

            SendMail(email,secretCode);
            
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

    private string GenerateTwoFactorCode()
    {
        BigInteger p = 499;  // 3 mod 4 ≡ 3
        BigInteger q = 547;  // 3 mod 4 ≡ 3
        BigInteger seed = DateTime.Now.Ticks % (p * q);

        var bbs = new BlumBlumShub(seed, p, q);

        // 6 haneli kod üretelim:
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
