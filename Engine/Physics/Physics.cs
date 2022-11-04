using BulletSharp;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;

namespace Dwarf.Engine.Physics;
public class Physics {

  public DiscreteDynamicsWorld World { get; }

  private const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;
  private System.Numerics.Vector3 _startPosition = new System.Numerics.Vector3(-5, 1, -3) - new System.Numerics.Vector3(ArraySizeX / 2, 0, ArraySizeZ / 2);

  private CollisionDispatcher _dispatcher;
  private DbvtBroadphase _broadphase;
  private List<CollisionShape> _collisionShapes = new List<CollisionShape>();
  private CollisionConfiguration _collisionConfig;

  public Physics() {
    _collisionConfig = new DefaultCollisionConfiguration();
    _dispatcher = new CollisionDispatcher(_collisionConfig);

    _broadphase = new DbvtBroadphase();
    World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, null, _collisionConfig);

    var groundShape = new BoxShape(50, 50, 50);
    _collisionShapes.Add(groundShape);
    CollisionObject ground = CreateStaticBody(Matrix4x4.CreateTranslation(0, -50, 0), groundShape);
    ground.UserObject = "Ground";

    var colShape = new BoxShape(1);
    _collisionShapes.Add(colShape);

    float mass = 1.0f;
    System.Numerics.Vector3 localInertia = colShape.CalculateLocalInertia(mass);

    var rbInfo = new RigidBodyConstructionInfo(mass, null, colShape, localInertia);
    for (int y = 0; y < ArraySizeY; y++) {
      for (int x = 0; x < ArraySizeX; x++) {
        for (int z = 0; z < ArraySizeZ; z++) {
          var startTransform = System.Numerics.Matrix4x4.CreateTranslation(
              _startPosition + 2 * new System.Numerics.Vector3(x, y, z));

          // using motionstate is recommended, it provides interpolation capabilities
          // and only synchronizes 'active' objects
          rbInfo.MotionState = new DefaultMotionState(startTransform);
          var body = new RigidBody(rbInfo);

          // make it drop from a height
          body.Translate(new System.Numerics.Vector3(0, 15, 0));

          World.AddRigidBody(body);
        }
      }
    }

    rbInfo.Dispose();
  }

  public static Matrix4 Convert(ref System.Numerics.Matrix4x4 m) {
    return new Matrix4(
        m.M11, m.M12, m.M13, m.M14,
        m.M21, m.M22, m.M23, m.M24,
        m.M31, m.M32, m.M33, m.M34,
        m.M41, m.M42, m.M43, m.M44);
  }

  public static System.Numerics.Matrix4x4 Convert(ref Matrix4 m) {
    System.Numerics.Matrix4x4 r = new System.Numerics.Matrix4x4();
    r.M11 = m.M11; r.M12 = m.M12; r.M13 = m.M13; r.M14 = m.M14;
    r.M21 = m.M21; r.M22 = m.M22; r.M23 = m.M23; r.M24 = m.M24;
    r.M31 = m.M31; r.M32 = m.M32; r.M33 = m.M33; r.M34 = m.M34;
    r.M41 = m.M41; r.M42 = m.M42; r.M43 = m.M43; r.M44 = m.M44;
    return r;
  }

  public static Vector3 Convert(System.Numerics.Vector3 v) {
    return new Vector3(v.X, v.Y, v.Z);
  }

  private RigidBody CreateStaticBody(System.Numerics.Matrix4x4 startTransform, CollisionShape shape) {
    System.Numerics.Vector3 localInertia = System.Numerics.Vector3.Zero;
    return CreateBody(0, startTransform, shape, localInertia);
  }

  private RigidBody CreateDynamicBody(float mass, System.Numerics.Matrix4x4 startTransform, CollisionShape shape) {
    System.Numerics.Vector3 localInertia = shape.CalculateLocalInertia(mass);
    return CreateBody(mass, startTransform, shape, localInertia);
  }

  private RigidBody CreateBody(float mass, System.Numerics.Matrix4x4 startTransform, CollisionShape shape, System.Numerics.Vector3 localInertia) {
    var motionState = new DefaultMotionState(startTransform);
    using (var rbInfo = new RigidBodyConstructionInfo(mass, motionState, shape, localInertia)) {
      var body = new RigidBody(rbInfo);
      World.AddRigidBody(body);
      return body;
    }
  }
}

