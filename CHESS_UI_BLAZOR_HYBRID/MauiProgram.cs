﻿using System;
using Microsoft.Extensions.Logging;
using Frontend.Controller;
using Frontend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace CHESS_UI_BLAZOR_HYBRID;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        builder.Services.AddLogging(logging =>
        logging.AddFilter("Microsoft.AspNetCore.Components.WebView", LogLevel.Trace)
            );
#endif
        builder.Services.AddHttpClient<ApiManager>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5056");
        });
        builder.Services.AddSingleton<PuzzleManager>();
        builder.Services.AddTransient<GameManager>();
        builder.Services.AddTransient<IFileStorageService, FileStorageService>();
        return builder.Build();
    }
}