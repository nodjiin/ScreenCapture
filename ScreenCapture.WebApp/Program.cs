using Core.Configurations;
using Core.Services.Implementers;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScreenCapture.WebApp.Areas.Identity;
using ScreenCapture.WebApp.Configurations;
using ScreenCapture.WebApp.Data;
using ScreenCapture.WebApp.Services.Implementers;
using ScreenCapture.WebApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<IRemoteAgentsMonitor, RemoteAgentsMonitor>();
builder.Services.AddSingleton<IRemoteAgentCommunicationManager, RemoteAgentCommunicationManager>();
builder.Services.AddSingleton<IMetadataFileManager, XmlMetadataFileManager>();
builder.Services.AddScoped<IDtoFactory, LocalStorageDtoFactory>();
builder.Services.AddScoped<IMediaExplorer, LocalDiskMediaExplorer>();
builder.Services.AddHttpClient();
builder.Services.AddOptions<List<RemoteAgentConfiguration>>().Bind(builder.Configuration.GetSection("RemoteAgentsConfigurations")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<Dictionary<string, SettingsGroupConfiguration>>().Bind(builder.Configuration.GetSection("SettingsGroupsConfiguration")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
