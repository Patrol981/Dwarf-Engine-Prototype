using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace Voxelized.Engine.Globals;

public static class WindowSettings {
  private static NativeWindowSettings s_nativeWindowGameSettings = new NativeWindowSettings {
    Size = new Vector2i(1200,700),
      Title = "Voxel 3D Game",
      // WindowState = WindowState.Fullscreen,
      Flags = ContextFlags.ForwardCompatible
      // APIVersion = new Version(4,6)
  };

  public static NativeWindowSettings GetNativeWindowSettings() {
    return s_nativeWindowGameSettings;
  }

  public static void SetNativeWindowSettings(NativeWindowSettings nativeWindowSettings) {
    s_nativeWindowGameSettings = nativeWindowSettings;
  }
}