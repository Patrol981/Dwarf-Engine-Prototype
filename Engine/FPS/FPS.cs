using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics.GL;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Voxelized.Globals;

namespace Voxelized.Info;

public class FPS {
  double lastTime = GLFW.GetTime();
  int frames = 0;
  public FPS() {}

  public void Update() {
    double currentTime = GLFW.GetTime();
    frames++;
    if(currentTime - lastTime >=1.0) {
      FPSState.SetFrames(1000.0/(double)frames);
      frames = 0;
      lastTime += 1f;
    }
  }
}