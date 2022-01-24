using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Globals;
using Voxelized.Enums;

namespace Voxelized.Cameras;

public class FreeCamera : Camera {
  public FreeCamera(Vector3 postion, float aspectRatio) : base(postion, aspectRatio) {}

  public void HandleMovement() {
    var camera = CameraGlobalState.GetCamera();
    if(CameraGlobalState.GetFirstMove()) {
      CameraGlobalState.SetFirstMove(false);
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));
    } else {
      var deltaX = WindowGlobalState.GetMouseState().Position.X - CameraGlobalState.GetLastPosition().X;
      var deltaY = WindowGlobalState.GetMouseState().Position.Y - CameraGlobalState.GetLastPosition().Y;
      CameraGlobalState.SetLastPosition(new Vector2(WindowGlobalState.GetMouseState().Position.X, WindowGlobalState.GetMouseState().Position.Y));
      camera.Yaw += deltaX * CameraGlobalState.GetSensitivity();
      camera.Pitch -= deltaY * CameraGlobalState.GetSensitivity();
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.W)) {
      camera.Position += camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.S)) {
      camera.Position -= camera.Front * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.A)) {
      camera.Position -= camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.D)) {
      camera.Position += camera.Right * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }

    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.LeftShift)) {
      camera.Position += camera._up * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
    }
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Space)) {
      camera.Position -= camera._up * CameraGlobalState.GetCameraSpeed() * (float)WindowGlobalState.GetTime();
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
  }

  internal override void UpdateVectors() {
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
}