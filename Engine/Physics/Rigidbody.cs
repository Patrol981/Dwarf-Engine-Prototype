using Dwarf.Engine.DataStructures;
using Dwarf.Engine.DataStructures.Enums;
using Dwarf.Engine.DataStructures.Interfaces;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Physics;
public class Rigidbody : Component, IInteractable {
  private bool _isKinematic = false;
  private bool _groundCheck = false;
  private CollisionType _collisionType = CollisionType.Mesh;

  private float _gravity = -50;
  private float _upwardsSpeed = 0;

  // test
  private TerrainMesh _terrainMesh;
  private Entity _terrainPosition;

  public Rigidbody() {

  }

  public Rigidbody(CollisionType collisionType) {
    _collisionType = collisionType;
  }

  [Obsolete]
  public Rigidbody(MasterMesh masterMesh) {
    _terrainMesh = (TerrainMesh)masterMesh.Meshes[0];
    _terrainPosition = masterMesh.Owner!;
  }

  public void Setup(MasterMesh masterMesh) {
    _terrainMesh = (TerrainMesh)masterMesh.Meshes[0];
    _terrainPosition = masterMesh.Owner!;
  }

  public void Update() {
    if (Owner == null) return;
    if (Owner.GetComponent<Transform>() == null) return;

    if (_isKinematic) return;

    _upwardsSpeed += _gravity * (float)WindowGlobalState.GetTime();
    Owner.GetComponent<Transform>().IncreasePosition(
      new Vector3(0, _upwardsSpeed * (float)WindowGlobalState.GetTime(), 0)
    );

    var position = Owner.GetComponent<Transform>().Position;
    float height = _terrainMesh.GetHeightOfTerrain(position.X, position.Z, _terrainPosition);
    if (Owner!.GetComponent<Transform>().Position.Y < height) {
      _upwardsSpeed = 0;
      Owner!.GetComponent<Transform>().Position.Y = height;
      _groundCheck = true;
    }
  }

  public void AddForce(float power) {
    if (!_groundCheck) return;
    _upwardsSpeed = power;
    _groundCheck = false;
  }
}
