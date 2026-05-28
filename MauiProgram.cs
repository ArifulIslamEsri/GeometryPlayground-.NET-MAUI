using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Maui;

namespace GeometryPlayground;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseArcGISRuntime();

        // ?? Put your API key here
         ArcGISRuntimeEnvironment.ApiKey = "";

        return builder.Build();
    }
}
