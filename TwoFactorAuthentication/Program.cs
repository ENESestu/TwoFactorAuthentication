var builder = WebApplication.CreateBuilder(args);

// Servisleri ekle
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // 5 dakika session süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Production ise hata yakalayýcý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Ana dizine gelen istekleri Login/Index'e yönlendir
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/Login/Index");
    return Task.CompletedTask;
});

// Eðer MVC Controller'lar kullanýyorsan route belirt
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

// Razor Pages route'larý
app.MapRazorPages();

app.Run();
