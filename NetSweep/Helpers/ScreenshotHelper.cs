using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetSweep.Helpers;

public static class ScreenshotHelper
{
    public static void Capture(Window window, string path)
    {
        var bitmap = new RenderTargetBitmap(
            (int)window.ActualWidth, (int)window.ActualHeight,
            96, 96, PixelFormats.Pbgra32);
        bitmap.Render(window);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using var stream = File.Create(path);
        encoder.Save(stream);
    }
}
