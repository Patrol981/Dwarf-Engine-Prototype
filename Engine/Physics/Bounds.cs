using OpenTK.Mathematics;

namespace Dwarf.Engine.Physics;

public enum BoundTypes {
  AABB,
  Sphere,
}

public class BoundingRegion {
  private BoundTypes _boundType;

  private Vector3 _center = Vector3.Zero;
  private Vector3 _min = Vector3.Zero;
  private Vector3 _max = Vector3.Zero;

  float _radius = 0.0f;

  // init with type
  public BoundingRegion(BoundTypes boundType) {
    _boundType = boundType;
  }

  // init as sphere
  public BoundingRegion(Vector3 center, float radius) {
    _boundType = BoundTypes.Sphere;
    _center = center;
    _radius = radius;
  }

  // init as AABB
  public BoundingRegion(Vector3 min, Vector3 max) {
    _boundType = BoundTypes.AABB;
    _min = min;
    _max = max;
  }

  Vector3 CalculateCenter() {
    return (_boundType == BoundTypes.AABB) ? (_min + _max) / 2.0f : _center;
  }

  Vector3 CalculateDimentions() {
    return (_boundType == BoundTypes.AABB) ? (_max - _min) : new Vector3(2.0f * _radius);
  }

  bool ContainsPoint(Vector3 point) {
    if(_boundType == BoundTypes.AABB) {
      return (point.X > _min.X) && (point.X <= _max.X) &&
        (point.Y >= _min.Y) && (point.Y <= _max.Y) &&
        (point.Z >= _min.Z) && (point.Z <= _max.Z);
    } else {
      float distSquared = 0.0f;
      for (int i = 0; i < 3; i++) {
        distSquared += (_center[i] - point[i]) * (_center[i] - point[i]);
      }
      return distSquared <= (_radius * _radius);
    }
  }

  bool ContainsRegion(BoundingRegion br) {
    return true;
  }

  bool IntersectsWith(BoundingRegion br) {
    return true;
  }

  public BoundTypes BoundType {
    get { return _boundType; }
    set { _boundType = value; }
  }
}
