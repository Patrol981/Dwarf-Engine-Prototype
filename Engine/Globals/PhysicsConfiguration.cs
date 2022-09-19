using BulletSharp;
using BulletSharp.SoftBody;

namespace Dwarf.Engine.Globals;
public static class PhysicsConfiguration {
  private static CollisionConfiguration _collisionConfiguration = new DefaultCollisionConfiguration();

  public static CollisionConfiguration CollisionConfiguration {
    get { return _collisionConfiguration; }
    set { _collisionConfiguration = value; }
  }
}
