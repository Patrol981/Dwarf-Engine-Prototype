using Dwarf.Engine.Windowing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace Dwarf.Engine.Globals;

public static class WindowSettings {
  private static OpenTK.Windowing.Common.Input.Image s_image = GetEngineIcon();

  private static NativeWindowSettings s_nativeWindowGameSettings = new NativeWindowSettings {
    Size = new Vector2i(1200, 700),
    Title = "Dwarf Engine Window",
    // WindowState = WindowState.Fullscreen,
    Flags = ContextFlags.ForwardCompatible,
    Icon = new OpenTK.Windowing.Common.Input.WindowIcon(s_image),
    // APIVersion = new Version(4,6)
  };

  private static OpenTK.Windowing.Common.Input.Image GetEngineIcon() {
    //var image = SixLabors.ImageSharp.Image.Load<Rgba32>("Resources/ico/dwarf_ico.png");
    //var pixels = new List<byte>(4 * image.Width * image.Height);

    using var stream = File.OpenRead("Resources/ico/dwarf_ico.png");
    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

    /*
    for (short y = 0; y < image.Height; y++) {
      var row = image.GetPixelRowSpan(y);

      for (short x = 0; x < image.Width; x++) {
        pixels.Add(row[x].R);
        pixels.Add(row[x].G);
        pixels.Add(row[x].B);
        pixels.Add(row[x].A);
      }
    }
    */
    return new OpenTK.Windowing.Common.Input.Image(64, 64, image.Data);
  }

  public static NativeWindowSettings GetNativeWindowSettings() {
    return s_nativeWindowGameSettings;
  }

  public static void SetNativeWindowSettings(NativeWindowSettings nativeWindowSettings) {
    s_nativeWindowGameSettings = nativeWindowSettings;
  }
}