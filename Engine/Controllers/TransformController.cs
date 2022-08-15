using Dwarf.Engine.ECS;
using Dwarf.Engine.Windowing;
using Dwarf.Engine.Globals;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Controllers;
public class TransformController : Component {
  private float _speed;

  public TransformController() { }

  public TransformController(float speed) {
    _speed = speed;
  }

  public void HandleMovement() {
    if(WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Up)) {
      Owner!.GetComponent<Transform>().Position += (Vector3.UnitX) * _speed * (float)WindowGlobalState.GetTime();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Down)) {
      Owner!.GetComponent<Transform>().Position += (-Vector3.UnitX) * _speed * (float)WindowGlobalState.GetTime();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Left)) {
      Owner!.GetComponent<Transform>().Position += (-Vector3.UnitZ) * _speed * (float)WindowGlobalState.GetTime();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Right)) {
      Owner!.GetComponent<Transform>().Position += (Vector3.UnitZ) * _speed * (float)WindowGlobalState.GetTime();
    }
  }
}

