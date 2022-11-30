using Dwarf.Engine.Cameras;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Physics;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Dwarf.Engine.Controllers;
public class TransformController : Component {
  private float _speed;
  private float _gravity = -50;
  private float _jumpHeight = 2f;
  private float _jumpPower = 20;
  private float _upwardsSpeed = 0;

  private bool _groundCheck = false;
  private bool _isJumping = false;

  private float _left, _right, _front, _back;
  private TerrainMesh _terrain;
  private Entity _terrainPosition;

  public TransformController() { }

  public TransformController(float speed, MasterMesh masterMesh = null!) {
    _speed = speed;
  }

  private bool CollisionCheck() {
    var target = EntityGlobalState.GetEntities();
    var pos = target[3].GetComponent<BoundingBox>().WorldModel.ExtractTranslation();
    var myPos = Owner!.GetComponent<BoundingBox>().WorldModel.ExtractTranslation();

    var b = BoundingBox.Intersect(Owner!.GetComponent<BoundingBox>(), target[3].GetComponent<BoundingBox>());
    return b;
  }

  public void HandleMovement() {
    Camera camera = CameraGlobalState.GetCamera();

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
      Jump();
    }
  }

  private void Jump() {
    Owner!.GetComponent<Rigidbody>().AddForce(_jumpPower);
    /*
    if (!_groundCheck) return;
    _upwardsSpeed = _jumpPower;
    _groundCheck = false;
    */
  }
}

