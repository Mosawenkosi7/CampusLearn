var builder = WebApplication.CreateBuilder(args);

//connection string to the database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


//add the Repository here --------------------------------------
builder.Services.AddScoped< CampusLearn.Repositories.TutorRepository>();
builder.Services.AddScoped<CampusLearn.Repositories.BookTutorRepository>();

builder.Services.AddScoped<CampusLearn.Repositories.StudentDashboardRepository>();


//-------------------------------end of adding the repository here



//add services Dependency Injection here----------------------------
builder.Services.AddScoped< CampusLearn.Services.TutorService>();
builder.Services.AddScoped<CampusLearn.Services.BookTutorService>();

builder.Services.AddScoped<CampusLearn.Services.StudentDashboardService>();






//-------------------------------end of adding services Dependency Injection here


//External dependency injection
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});




// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
