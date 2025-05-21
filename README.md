
#  Two-Factor Authentication (2FA) with Custom PRNG — Cryptography Term Project

##  Project Overview
This project implements a secure Two-Factor Authentication (2FA) system using a **custom-designed pseudorandom number generator (PRNG)** based on the **Blum-Blum-Shub** algorithm. The purpose is to enhance authentication security by avoiding insecure built-in random number generators and demonstrating cryptographic PRNG concepts.

##  Features
-  Custom PRNG implementation (Blum-Blum-Shub-based)
-  Email-based OTP (6-digit)
-  3-minute expiration timer for OTP
-  ASP.NET Core MVC structure
-  Session-based OTP verification

##  Technologies
- .NET 8.0
- ASP.NET Core MVC
- C#
- SMTP (MailKit / System.Net.Mail)

##  Project Structure
```
/Controllers
  └── HomeController.cs
  └── LoginController.cs
/Helpers
  └── BlumBlumShub.cs
/Services
  └── MailService.cs
/Views/Login
  └── Home
    ├── Index.cshtml
  └── Login
    ├── Index.cshtml       --> Login Page
    └── TwoFactor.cshtml   --> OTP Input + Timer
/Program.cs
/_Layout.cshtml
```

##  How to Run

### 1. Clone the repository
```bash
git clone https://github.com/ENESestu/TwoFactorAuthentication.git
cd TwoFactorAuthentication
```

### 2. Open with Visual Studio (or run from terminal)
```bash
dotnet restore
dotnet build
dotnet run
```

>  You may need to enable “Less secure apps” or use App Password (e.g. Gmail).

##  How to Test the Project
1. **Start the application**
2. Navigate to: `https://localhost:5001/Login/Index`
3. Enter login credentials (email: `personal email`, username: `admin`, password: `1234`)
4. System will:
   - Generate a secure 6-digit OTP using Blum-Blum-Shub
   - Send the OTP to the configured email
5. Enter the received code in 2FA screen
6. If correct and within 3 minutes → "Giriş başarılı!"

##  About the Custom PRNG
- Based on [Blum, Blum & Shub 1986] algorithm
- Seed derived from `DateTime.Now.Ticks`
- 6-digit OTP generated using `NextNumber(bits)` logic
- Designed to resist brute-force & prediction attacks

##  Author
Enes ÖZTÜRK  
