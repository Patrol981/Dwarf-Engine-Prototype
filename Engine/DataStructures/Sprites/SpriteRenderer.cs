using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Dwarf.Engine.Cameras;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Shaders;
using Dwarf.Engine.Physics;

namespace Dwarf.Engine.DataStructures;
public class SpriteRenderer : Component {
  private Shader? _shader;
  private int _vbo;
  private int _vao;
  private int _ebo;

  public SpriteRenderer() { }

  public SpriteRenderer Init(string vs, string fs, bool isFile = true) {
    var sprite = Owner!.GetComponent<Sprite>();
    if (sprite == null) return null!;

    _shader = new Shader(vs, fs, isFile);

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(
      BufferTarget.ArrayBuffer,
      sprite.Vertices.Length * sizeof(float),
      sprite.Vertices,
      BufferUsageHint.StaticDraw
    );

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        sprite.Indices.Length * sizeof(uint),
        sprite.Indices,
        BufferUsageHint.StaticDraw
      );

    sprite.Texture.Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));

    // position
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

    // texture
    GL.EnableVertexAttribArray(1);
    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.BindVertexArray(0);

    _shader?.SetInt("texture0", 0);

    // strech sprite to correct size
    var scaleX = sprite.Size.X / sprite.Size.Y;
    var scaleY = sprite.Size.Y / sprite.Size.X;

    Owner.GetComponent<Transform>().Scale = new Vector3(scaleX, scaleY, 1);

    return this;
  }

  public void Render(Camera camera) {
    var sprite = Owner!.GetComponent<Sprite>();
    if (sprite == null) return;

    _shader!.Use();
    var model = Matrix4.Identity;
    var pos = Owner!.GetComponent<Transform>().Position;
    var angleX = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.X);
    var angleY = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Y);
    var angleZ = (float)MathHelper.DegreesToRadians(Owner.GetComponent<Transform>().Rotation.Z);
    var scale = Owner.GetComponent<Transform>().Scale;

    model =
      Matrix4.CreateRotationX(angleX) *
      Matrix4.CreateRotationY(angleY) *
      Matrix4.CreateRotationZ(angleZ);

    var worldModel = model * Matrix4.CreateTranslation(pos) * Matrix4.CreateScale(scale);

    _shader!.SetVector3("aPosition", camera.Owner!.GetComponent<Transform>().Position);
    Material material = Owner!.GetComponent<Material>();
    _shader?.SetVector3("spriteColor", material?.GetColor() ?? new Vector3(1, 1f, 1f));

    Matrix4 projection = camera.GetProjectionMatrix();
    Matrix4 view = camera.GetViewMatrix();
    _shader!.SetMatrix4("uProjection", projection);
    _shader!.SetMatrix4("uView", view);
    _shader!.SetMatrix4("uModel", worldModel);

    sprite.Texture.Use((TextureUnit)Enum.Parse(typeof(TextureUnit), $"Texture0"));
    // _shader!.Use();

    GL.BindVertexArray(_vao);
    // GL.DrawArrays(PrimitiveType.Triangles, 0, sprite.Vertices.Length);
    GL.DrawElements(PrimitiveType.Triangles, sprite.Indices.Length, DrawElementsType.UnsignedInt, 0);

    GL.BindVertexArray(0);
    GL.ActiveTexture(TextureUnit.Texture0);

    var boundingBox = Owner!.GetComponent<BoundingBox2D>();
    if (boundingBox == null) return;

    boundingBox.Draw(camera);
  }
}
