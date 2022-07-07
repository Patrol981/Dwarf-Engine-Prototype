namespace Voxelized.Engine.Globals;

public static class FPSState {
  private static double _frames = 0;

  public static void SetFrames(double frames) {
    _frames = frames;
  }

  public static double GetFrames() {
    return _frames;
  }
}