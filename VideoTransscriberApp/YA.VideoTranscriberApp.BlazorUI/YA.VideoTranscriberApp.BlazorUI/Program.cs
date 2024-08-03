using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Http;
using OpenAI.Extensions;
using YA.VideoTranscriberApp.BlazorUI.Client.Pages;
using YA.VideoTranscriberApp.BlazorUI.Components;
using YA.VideoTranscriberApp.BlazorUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5170/api/") });
builder.Services.AddControllers();
builder.Services.AddScoped<TranscriptionManager>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)));

var openAIApiKey = builder.Configuration.GetSection("OpenAIApiKey").Value;

builder.Services.AddOpenAIService(settings => settings.ApiKey = openAIApiKey);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(YA.VideoTranscriberApp.BlazorUI.Client._Imports).Assembly);

app.UseCors();
app.MapControllers();

app.Run();
