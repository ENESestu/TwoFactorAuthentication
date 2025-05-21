var builder = WebApplication.CreateBuilder(args);

// Servisleri ekle
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // 5 dakika session s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Production ise hata yakalay�c�
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

// Ana dizine gelen istekleri Login/Index'e y�nlendir
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/Login/Index");
    return Task.CompletedTask;
});

// E�er MVC Controller'lar kullan�yorsan route belirt
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

// Razor Pages route'lar�
app.MapRazorPages();

app.Run();
