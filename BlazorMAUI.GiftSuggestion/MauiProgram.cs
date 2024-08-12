using Blazored.Modal;
using Microsoft.Extensions.Logging;
using OpenAI.Extensions;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            //var openAIApiKey = builder
            //                        .Configuration
            //                        .GetSection("OpenAIApiKey")
            //                        .Value ?? "";

            var openAIApiKey = "sk-xxx";

            builder.Services.AddOpenAIService(settings => settings.ApiKey = openAIApiKey);

            builder.Services.AddBlazoredModal();

            return builder.Build();
        }
    }
}
