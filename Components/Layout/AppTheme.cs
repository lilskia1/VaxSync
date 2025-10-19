using MudBlazor;

namespace VaxSync.Web.Components.Layout;

public static class AppTheme
{
    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1BA1E2",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#0B3D91",
            SecondaryContrastText = "#0B3D91",
            Info = "#0B3D91",
            Success = "#16A34A",
            Warning = "#F59E0B",
            Error = "#DC2626",
            Background = "#F9FAFB",
            Surface = "#FFFFFF",
            TextPrimary = "#0B3D91",
            TextSecondary = "#4B5563",
            DrawerBackground = "#0B3D91",
            DrawerText = "#FFFFFF",
            DrawerIcon = "#FFFFFF",
            AppbarBackground = "#0B3D91",
            AppbarText = "#FFFFFF"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Inter", "Roboto", "Segoe UI", "Arial", "sans-serif" }
            },
            H5 = new DefaultTypography { FontWeight = "600" },
            Body1 = new DefaultTypography { LineHeight = "1.5" }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "4px",
            DrawerWidthLeft = "250px",
            AppbarHeight = "64px"
        }
    };
}
