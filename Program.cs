using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Services;
using MudBlazorWebApp1.Components;
using MudBlazorWebApp1.Services;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices(config => {
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 2000;
});

// Add services to the container.
builder.Services.AddHttpClient<IExerciseService, ExerciseService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra los servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<PesoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RutinaService>();
builder.Services.AddScoped<AdministracionService>();
// Configura la conexión MySQL de manera correcta
builder.Services.AddScoped<MySqlConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnection(connectionString);
});

// Añade soporte para MudBlazor
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
