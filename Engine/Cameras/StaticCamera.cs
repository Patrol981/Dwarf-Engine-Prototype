using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Dwarf.Engine.Globals;
using Dwarf.Engine.Enums;
using Dwarf.Engine.ECS;

namespace Dwarf.Engine.Cameras;
public class StaticCamera : Camera, ICamera {
  public StaticCamera(float aspectRatio): base(aspectRatio) {}

  public StaticCamera() {
    WindowGlobalState.SetCursorVisible(false);
  }

  internal override void UpdateVectors() {
    return;
  }

  public float GetAspectRatio() {
    return AspectRatio;
  }

  public void SetAspectRatio(float aspectRatio) {
    AspectRatio = aspectRatio;
  }

  public void HandleMovement() {
    // throw new NotImplementedException();
  }
}
