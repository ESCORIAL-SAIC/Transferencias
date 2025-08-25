using Microsoft.Extensions.Logging;
using Camera.MAUI;
using Transferencias.Services;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using UraniumUI;

namespace Transferencias
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialIconFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseService>();
            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Placeholder", (h, v) =>
            {
#if ANDROID
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#endif
#if IOS
h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Placeholder", (h, v) =>
            {
#if ANDROID
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#endif
#if IOS
h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
            return builder.Build();
        }
    }
}