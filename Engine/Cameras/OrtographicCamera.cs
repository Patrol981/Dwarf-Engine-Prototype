using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Dwarf.Engine.Cameras;
public class OrtographicCamera : Camera, ICamera {

  public OrtographicCamera(float aspectRatio) : base(aspectRatio) {
    WindowGlobalState.SetCursorVisible(false);
  }

  public OrtographicCamera() {
    WindowGlobalState.SetCursorVisible(false);
  }

  public override Matrix4 GetProjectionMatrix() {
    float scale = 3f;
    return Matrix4.CreateOrthographicOffCenter(
      -AspectRatio * scale,
      AspectRatio * scale,
      -scale,
      scale,
      0.01f,
      100f
    );
  }

  public override Matrix4 GetViewMatrix() {
    Vector3 position = Owner!.GetComponent<Transform>().Position;
    return Matrix4.LookAt(position, position + _front, _up);
  }

  public virtual void HandleMovement() {
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.A)) {
      Owner!.GetComponent<Transform>().Position -= Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.D)) {
      Owner!.GetComponent<Transform>().Position += Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }

    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.W)) {
      Owner!.GetComponent<Transform>().Position += WorldUp * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.S)) {
      Owner!.GetComponent<Transform>().Position -= WorldUp * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }

    if (WindowGlobalState.GetKeyboardState().IsKeyPressed(Keys.F)) {
      var currentCursorState = WindowGlobalState.GetCursorVisible();
      switch (currentCursorState) {
        case true:
          WindowGlobalState.SetCursorVisible(false);
          break;
        case false:
          WindowGlobalState.SetCursorVisible(true);
          break;
      }
    }
  }
}
