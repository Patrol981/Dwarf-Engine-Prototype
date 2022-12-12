using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using StbImageSharp;

namespace Dwarf.Engine.Globals;

public static class WindowSettings {
  private static string s_globalPath = "";
  private static string s_localEnginePath = GetEngineLocation();
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
    using var stream = File.OpenRead($"Resources/ico/dwarf_ico.png");
    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

    return new OpenTK.Windowing.Common.Input.Image(64, 64, image.Data);
  }

  private static string GetEngineLocation() {
    var path = Directory.GetCurrentDirectory();
    return path;
  }

  public static NativeWindowSettings GetNativeWindowSettings() {
    return s_nativeWindowGameSettings;
  }

  public static string GetGlobalPath() {
    return s_globalPath;
  }

  public static string GetEnginePath() {
    return s_localEnginePath;
  }

  public static void SetNativeWindowSettings(NativeWindowSettings nativeWindowSettings) {
    s_nativeWindowGameSettings = nativeWindowSettings;
  }

  public static void SetGlobalPath(string path) {
    s_globalPath = path;
  }
}