using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Primitives;
public class Line3D : Component {
  float[] _vertices = {
    -0.0f, -0.0f, 0.0f,
     -0.0f, 0.0f, 15.0f,
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
		  }";

  public static string fragmentCode = @" 
      #version 330 core
      out vec4 vFragColor;
      uniform vec3 uDiffuse;

		  void main()
		  {
        vFragColor = vec4(uDiffuse,1);
		  }";

  public Line3D() {

  }

  public void SetLine(Vector3 start, Vector3 end) {
    _vertices = new float[] {
      start.X, start.Y, start.Z,
      end.X, end.Y, end.Z
    };

    Update();
  }

  public float[] GetFloats() {
    return _vertices;
  }

  public MasterMesh Load() {
    var array = new List<Vertex>();
    for(int i = 0; i < _vertices.Length; i+=3) {
      var vertex = new Vertex();
      vertex.Position = new Vector3(_vertices[i], _vertices[i+1], _vertices[i+2]);
      array.Add(vertex);
    }
    Mesh mesh = new(array);
    MasterMesh masterMesh = new(new List<Mesh> { mesh }, Enums.MeshRenderType.Line);
    return masterMesh;
  }

  private void Update() {
    if (Owner == null) return;
    var array = new List<Vertex>();
    for (int i = 0; i < _vertices.Length; i += 3) {
      var vertex = new Vertex();
      vertex.Position = new Vector3(_vertices[i], _vertices[i + 1], _vertices[i + 2]);
      array.Add(vertex);
    }
    Mesh mesh = new(array);
    Owner.GetComponent<MasterMesh>().Meshes.RemoveAt(0);
    Owner.GetComponent<MasterMesh>().Meshes.Add(mesh);
    Owner.GetComponent<MeshRenderer>().Update();
  }
}

