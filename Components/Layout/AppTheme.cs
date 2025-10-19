using MudBlazor;

namespace VaxSync.Web.Components.Layout;

public static partial class AppTheme
{
    public static MudTheme Theme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0B3D91",
            Secondary = "#1BA1E2",
            AppbarBackground = "#0B3D91",
            DrawerBackground = "linear-gradient(135deg, #0B3D91, #1BA1E2)",
            DrawerText = "white",
            DrawerIcon = "white",
            TextPrimary = "white"
        },
        Typography = new Typography
        {
            Default = new BaseTypography
            {
                FontFamily = ["Inter", "Roboto", "Segoe UI", "Arial", "sans-serif"]
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "4px",
            DrawerWidthLeft = "250px",
            AppbarHeight = "64px"
        }
    };
}