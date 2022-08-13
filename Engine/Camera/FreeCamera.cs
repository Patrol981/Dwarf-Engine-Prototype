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

public class FreeCamera : Camera, ICamera {
  public FreeCamera(Vector3 postion, float aspectRatio) : base(postion, aspectRatio) {}

  public FreeCamera() {}

  public void HandleMovement() {
    var camera = CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>();
    var cameraEntity = CameraGlobalState.GetCameraEntity();
    if(CameraGlobalState.GetFirstMove()) {
      CameraGlobalState.SetFirstMove(false);
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));
    } else {
      var deltaX = WindowGlobalState.GetMouseState().Position.X - CameraGlobalState.GetLastPosition().X;
      var deltaY = WindowGlobalState.GetMouseState().Position.Y - CameraGlobalState.GetLastPosition().Y;
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));
      
      if(!WindowGlobalState.GetCursorVisible()) {
        camera.Yaw += deltaX * CameraGlobalState.GetSensitivity();
        camera.Pitch -= deltaY * CameraGlobalState.GetSensitivity();
      }
      
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.W)) {
      cameraEntity.GetComponent<Transform>().Position += camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.S)) {
      cameraEntity.GetComponent<Transform>().Position -= camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.A)) {
      cameraEntity.GetComponent<Transform>().Position -= camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.D)) {
      cameraEntity.GetComponent<Transform>().Position += camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.LeftShift)) {
      cameraEntity.GetComponent<Transform>().Position += camera.WorldUp * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Space)) {
      cameraEntity.GetComponent<Transform>().Position -= camera.WorldUp * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyPressed(Keys.Tab)) {
      var currentRenderMode = CameraGlobalState.GetWindowRenderMode();
      switch(currentRenderMode) {
        case WindowRenderMode.Normal:
          GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
          CameraGlobalState.SetWindowRenderMode(WindowRenderMode.Wireframe);
          break;
        case WindowRenderMode.Wireframe:
          GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
          CameraGlobalState.SetWindowRenderMode(WindowRenderMode.Normal);
          break;
      }
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyPressed(Keys.F)) {
      var currentCursorState = WindowGlobalState.GetCursorVisible();
      switch(currentCursorState) {
        case true:
          WindowGlobalState.SetCursorVisible(false);
          break;
        case false:
          WindowGlobalState.SetCursorVisible(true);
          break;
      }
    }
  }

  internal override void UpdateVectors() {
    if(WindowGlobalState.GetCursorVisible()) {
      return;
    }
    // First, the front matrix is calculated using some basic trigonometry.
    _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
    _front.Y = MathF.Sin(_pitch);
    _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

    // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
    _front = Vector3.Normalize(_front);

    // Calculate both the right and the up vector using cross product.
    // Note that we are calculating the right from the global up; this behaviour might
    // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
    _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
    _up = Vector3.Normalize(Vector3.Cross(_right, _front));
  }

  public float GetAspectRatio() {
    return AspectRatio;
  }

  public void SetAspectRatio(float aspectRatio) {
    AspectRatio = aspectRatio;
  }
}