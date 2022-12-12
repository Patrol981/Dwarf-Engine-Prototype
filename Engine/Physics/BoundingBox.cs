using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Cameras;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Dwarf.Engine.Shaders;
using System;
using Dwarf.Engine.Globals;

namespace Dwarf.Engine.Physics;

public class OrientedBoundingBox {
  public readonly Vector3[] Vertices;
  public readonly Vector3 Right;
  public readonly Vector3 Up;
  public readonly Vector3 Forward;

  public OrientedBoundingBox(Vector3 center, Vector3 size, Quaternion rotation) {
    var max = size / 2;
    var min = -max;

    Vertices = new[] {
      center + rotation * min,
      center + rotation * new Vector3(max.X, min.Y, min.Z),
      center + rotation * new Vector3(min.X, max.Y, min.Z),
      center + rotation * new Vector3(max.X, max.Y, min.Z),
      center + rotation * new Vector3(min.X, min.Y, max.Z),
      center + rotation * new Vector3(max.X, min.Y, max.Z),
      center + rotation * new Vector3(min.X, max.Y, max.Z),
      center + rotation * max,
    };

    Right = rotation * new Vector3(1, 0, 0);
    Up = rotation * new Vector3(0, 1, 0);
    Forward = rotation * new Vector3(0, 0, 1);
  }
}

public class BoundingBox : Component {
  private Vector3 _size = Vector3.Zero;
  private Vector3 _center = Vector3.Zero;
  private Vector3 _bottom = Vector3.Zero;
  private Matrix4 _transform = Matrix4.Identity;

  private Matrix4 _model = Matrix4.Identity;
  private Matrix4 _worldModel = Matrix4.Identity;

  private float _minX, _minY, _minZ, _maxX, _maxY, _maxZ;

  private Shader? _shader;

  private bool _render = true;

  int _vbo;
  int _ebo;
  int _vao;

  /*
  float[] _vertices = {
      -0.5f, 0f, -0.5f, 1.0f, // bottom right
       0.5f, 0f, -0.5f, 1.0f, // bottom left
       0.5f, 1f, -0.5f, 1.0f, // upper left
      -0.5f, 1f, -0.5f, 1.0f, // upper right
      -0.5f, 0f, 0.5f, 1.0f, // bottom right
       0.5f, 0f, 0.5f, 1.0f, // bottom left
       0.5f, 1f, 0.5f, 1.0f,
      -0.5f, 1f, 0.5f, 1.0f,
    };
  */

  float[] _vertices = {
      -0.5f, -0.5f, -0.5f, 1.0f,
       0.5f, -0.5f, -0.5f, 1.0f,
       0.5f, 0.5f, -0.5f, 1.0f,
      -0.5f, 0.5f, -0.5f, 1.0f,
      -0.5f, -0.5f, 0.5f, 1.0f,
       0.5f, -0.5f, 0.5f, 1.0f,
       0.5f, 0.5f, 0.5f, 1.0f,
      -0.5f, 0.5f, 0.5f, 1.0f,
    };


  ushort[] _indices = {
      0, 1, 2, 3,
      4, 5, 6, 7,
      0, 4, 1, 5, 2, 6, 3, 7
    };

  public static string vertexCode = @"
      #version 330 core
      layout(location = 0) in vec3 aPosition;
      uniform mat4 uModel;
      uniform mat4 uView;
      uniform mat4 uProjection;
  
		  void main(void)
		  {
			  gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
        // gl_Position = vec4(aPosition, 1.0) * uProjection;
		  }";

  public static string fragmentCode = @" 
      #version 330 core
      out vec4 vFragColor;
      uniform vec3 uDiffuse;

		  void main()
		  {
        vFragColor = vec4(uDiffuse,1);
		  }";

  public static bool Intersect(BoundingBox b1, BoundingBox b2) {
    /*
    var b1Pos = b1.WorldModel.ExtractTranslation();
    var b2Pos = b2.WorldModel.ExtractTranslation();

    bool x = MathF.Abs(b1Pos.X - b2Pos.X) * 2 < (b1.Size.X + b2.Size.X);
    bool y = MathF.Abs(b1Pos.Y - b2Pos.Y) * 2 < (b1.Size.Y + b2.Size.Y);
    bool z = MathF.Abs(b1Pos.Z - b2Pos.Z) * 2 < (b1.Size.Z + b2.Size.Z);

    return x && y && z;
    */

    var vec3 = b2!.Owner!.GetComponent<Transform>().Position - b1!.Owner!.GetComponent<Transform>().Position;
    return false;

  }

  static bool Intersects(OrientedBoundingBox a, OrientedBoundingBox b) {
    if (Separated(a.Vertices, b.Vertices, a.Right))
      return false;
    if (Separated(a.Vertices, b.Vertices, a.Up))
      return false;
    if (Separated(a.Vertices, b.Vertices, a.Forward))
      return false;

    if (Separated(a.Vertices, b.Vertices, b.Right))
      return false;
    if (Separated(a.Vertices, b.Vertices, b.Up))
      return false;
    if (Separated(a.Vertices, b.Vertices, b.Forward))
      return false;

    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Right, b.Right)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Right, b.Up)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Right, b.Forward)))
      return false;

    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Up, b.Right)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Up, b.Up)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Up, b.Forward)))
      return false;

    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Forward, b.Right)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Forward, b.Up)))
      return false;
    if (Separated(a.Vertices, b.Vertices, Vector3.Cross(a.Forward, b.Forward)))
      return false;

    return true;
  }

  public static OrientedBoundingBox ToObb(BoundingBox b) {
    return new OrientedBoundingBox(
      b.WorldModel.ExtractTranslation(),
      b.Size,
      b.WorldModel.ExtractRotation()
    );
  }

  static bool Separated(Vector3[] vA, Vector3[] vB, Vector3 axis) {
    if (axis == Vector3.Zero) {
      return false;
    }

    var aMin = float.MaxValue;
    var aMax = float.MinValue;
    var bMin = float.MaxValue;
    var bMax = float.MinValue;

    for (var i = 0; i < 8; i++) {
      var aDist = Vector3.Dot(vA[i], axis);
      aMin = aDist < aMin ? aDist : aMin;
      aMax = aDist > aMax ? aDist : aMax;
      var bDist = Vector3.Dot(vB[i], axis);
      bMin = bDist < bMin ? bDist : bMin;
      bMax = bDist > bMax ? bDist : bMax;
    }

    var longSpan = MathF.Max(aMax, bMax) - MathF.Min(aMin, bMin);
    var sumSpan = aMax - aMin + bMax - bMin;
    return longSpan >= sumSpan;
  }

  public static List<Entity> Intersect(BoundingBox b, List<Entity> entities) {
    var myPos = b.WorldModel.ExtractTranslation();
    var myObb = ToObb(b);
    var resultList = new List<Entity>();
    for (int i = 0; i < entities.Count; i++) {
      var targetBB = entities[i].GetComponent<BoundingBox>();
      var targetPos = targetBB.WorldModel.ExtractTranslation();
      var targetRot = targetBB.WorldModel.ExtractRotation();

      var targetObb = ToObb(targetBB);

      if (Intersects(myObb, targetObb)) resultList.Add(entities[i]);

      /*
      bool x = MathF.Abs(myPos.X - targetPos.X) * 2 < (b.Size.X + targetBB.Size.X);
      bool y = MathF.Abs(myPos.Y - targetPos.Y) * 2 < (b.Size.Y + targetBB.Size.Y);
      bool z = MathF.Abs(myPos.Z - targetPos.Z) * 2 < (b.Size.Z + targetBB.Size.Z);

      if (x && y && z) resultList.Add(entities[i]);
      */
    }

    return resultList;
  }

  public void CalculateBox(MasterMesh masterMesh) {
    _minX = _maxX = masterMesh.Meshes[0].VertexArray[0].Position.X;
    _minY = _maxY = masterMesh.Meshes[0].VertexArray[0].Position.Y;
    _minZ = _maxZ = masterMesh.Meshes[0].VertexArray[0].Position.Z;

    for (int n = 0; n < masterMesh.Meshes.Count; n++) {
      for (int i = 0; i < masterMesh.Meshes[n].VertexArray.Count; i++) {
        if (masterMesh.Meshes[n].VertexArray[i].Position.X < _minX) _minX = masterMesh.Meshes[n].VertexArray[i].Position.X;
        if (masterMesh.Meshes[n].VertexArray[i].Position.X > _maxX) _maxX = masterMesh.Meshes[n].VertexArray[i].Position.X;
        if (masterMesh.Meshes[n].VertexArray[i].Position.Y < _minY) _minY = masterMesh.Meshes[n].VertexArray[i].Position.Y;
        if (masterMesh.Meshes[n].VertexArray[i].Position.Y > _maxY) _maxY = masterMesh.Meshes[n].VertexArray[i].Position.Y;
        if (masterMesh.Meshes[n].VertexArray[i].Position.Z < _minZ) _minZ = masterMesh.Meshes[n].VertexArray[i].Position.Z;
        if (masterMesh.Meshes[n].VertexArray[i].Position.Z > _maxZ) _maxZ = masterMesh.Meshes[n].VertexArray[i].Position.Z;
      }
    }

    _size = new Vector3(_maxX - _minX, _maxY - _minY, _maxZ - _minZ);
    _center = new Vector3((_minX + _maxX) / 2, (_minY + _maxY) / 2, (_minZ + _maxZ) / 2);
    //_center = new Vector3((_minX + _maxX) / 2, 0, (_minZ + _maxZ) / 2);
    _bottom = new Vector3((_minX), (_minY), (_minZ));

    var _basePos = new Vector3(
      (_minX + _maxX) / 2,
      _minY,
      (_minZ + _maxZ) / 2
    );

    var angleX = (float)MathHelper.DegreesToRadians(Owner!.GetComponent<Transform>().Rotation.X);
    var angleY = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y);
    var angleZ = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z);

    _transform = Matrix4.CreateTranslation(_center) * Matrix4.CreateScale(_size) * Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY) * Matrix4.CreateRotationZ(angleZ);

  }

  public void Setup(MasterMesh masterMesh) {
    // if (masterMesh.VertexArray.Count == 0) return;

    _shader = new Shader(vertexCode, fragmentCode, false);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sizeof(float) * _vertices.Length,
      _vertices,
      BufferUsageHint.StaticDraw
    );
    //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(
      BufferTarget.ElementArrayBuffer,
      sizeof(ushort) * _indices.Length,
      _indices,
      BufferUsageHint.StaticDraw
    );

    CalculateBox(masterMesh);

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.EnableVertexAttribArray(1);
  }

  public void Draw(Camera camera) {
    if (_shader == null) return;

    _shader.Use();
    _shader.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);

    _shader.SetMatrix4("uProjection", camera.GetProjectionMatrix());
    _shader.SetMatrix4("uView", camera.GetViewMatrix());

    var modelPos = Owner!.GetComponent<Transform>().Position;
    var angleX = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.X);
    var angleY = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y);
    var angleZ = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z);
    var scale = Owner.GetComponent<Transform>().Scale;

    // apply rotation matrix to bounding box
    // _transform = ;

    _model = _transform * Matrix4.CreateRotationX(angleX) * Matrix4.CreateRotationY(angleY) * Matrix4.CreateRotationZ(angleZ);
    _worldModel = _model * Matrix4.CreateTranslation(modelPos) * Matrix4.CreateScale(scale);
    _shader.SetMatrix4("uModel", _worldModel);

    _shader.SetVector3("uDiffuse", new Vector3(127, 255, 0));

    //GL.Enable(EnableCap.PolygonOffsetFill);
    //GL.LineWidth(25);
    GL.BindVertexArray(_vao);

    GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedShort, 0);
    GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedShort, sizeof(ushort) * 4);
    GL.DrawElements(PrimitiveType.Lines, 8, DrawElementsType.UnsignedShort, sizeof(ushort) * 8);

    GL.BindVertexArray(0);
    // GL.Disable(EnableCap.PolygonOffsetFill);
    //GL.LineWidth(1);
  }

  public Vector3 Size {
    get { return _size; }
  }

  public Vector3 Center {
    get { return _center; }
  }

  public Matrix4 Tranform {
    get { return _transform; }
  }

  public Matrix4 Model {
    get { return _model; }
  }

  public Matrix4 WorldModel {
    get { return _worldModel; }
  }

  public float MinX {
    get { return _minX; }
  }

  public float MinY {
    get { return _minY; }
  }

  public float MinZ {
    get { return _minZ; }
  }

  public float MaxX {
    get { return _maxX; }
  }

  public float MaxY {
    get { return _maxY; }
  }

  public float MaxZ {
    get { return _maxZ; }
  }

  public bool Render {
    get { return _render; }
    set { _render = value; }
  }
}
