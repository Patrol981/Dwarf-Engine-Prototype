using Dwarf.Engine.ECS;
using Dwarf.Engine.Windowing;
using Dwarf.Engine.Globals;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.Physics;

namespace Dwarf.Engine.Controllers;
public class TransformController : Component {
  private float _speed;
  private float _gravity = 0.1f;
  private float _jumpHeight = 2f;

  private bool _groundCheck = false;
  private bool _isJumping = false;

  private float _left, _right, _front, _back;

  public TransformController() { }

  public TransformController(float speed) {
    _speed = speed;
  }

  private bool CollisionCheck() {
    var target = EntityGlobalState.GetEntities();
    var pos = target[3].GetComponent<BoundingBox>().WorldModel.ExtractTranslation();
    var myPos = Owner!.GetComponent<BoundingBox>().WorldModel.ExtractTranslation();

    var b = BoundingBox.Intersect(Owner!.GetComponent<BoundingBox>(), target[3].GetComponent<BoundingBox>());

    Console.WriteLine(b);
    //Console.WriteLine(myPos - pos);
    return b;
  }

  public void HandleMovement() {
    Camera camera = CameraGlobalState.GetCamera();
    // Owner.GetComponent<Transform>().Rotation = camera.GetComponent<ThirdPersonCamera>().Front;

    if(Owner!.GetComponent<Transform>().Position.Y > 0) {
      _groundCheck = false;
      if (_isJumping) return;
      Owner!.GetComponent<Transform>().Position.Y -= _gravity;
    } else {
      Owner!.GetComponent<Transform>().Position.Y = 0;
      _groundCheck = true;
    }

    //float front = 90 - ((camera.Yaw));
    //float back = (camera.Yaw + 90) * -1;
    //float left = (camera.Yaw + 180) * -1;
    //float right = (camera.Yaw + 180);

    //_right = 0;
    //_back = 0;
    //_left = 0;
    //_front = 0;

    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.W)) {
      Owner!.GetComponent<Transform>().Position += (camera.Front) * _speed * (float)WindowGlobalState.GetTime();
      Owner!.GetComponent<Transform>().Rotation.Y = 90 - ((camera.Yaw));
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.S)) {
      Owner!.GetComponent<Transform>().Position -= (camera.Front) * _speed * (float)WindowGlobalState.GetTime();
      //Owner!.GetComponent<Transform>().Rotation.Y = (camera.Yaw + 90) * -1;
      Owner!.GetComponent<Transform>().Rotation.Y = (camera.Yaw + 90) * -1;

      //CollisionCheck();
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.A)) {
      Owner!.GetComponent<Transform>().Position += (-camera.Right) * _speed * (float)WindowGlobalState.GetTime();
      //Owner!.GetComponent<Transform>().Rotation.Y = ;
      _left = (camera.Yaw + 180) * -1;
    }
    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.D)) {
      Owner!.GetComponent<Transform>().Position += (camera.Right) * _speed * (float)WindowGlobalState.GetTime();
      _right = (camera.Yaw + 180);
    }

    if (WindowGlobalState.GetKeyboardState().IsKeyDown(Keys.Space)) {
      if (_isJumping) return;
      if (!_groundCheck) return;
      _isJumping = true;
      while(Owner!.GetComponent<Transform>().Position.Y < _jumpHeight) {
        Owner!.GetComponent<Transform>().Position.Y += _gravity * (float)WindowGlobalState.GetTime();
      }
      _isJumping = false;
    }
  }
}

