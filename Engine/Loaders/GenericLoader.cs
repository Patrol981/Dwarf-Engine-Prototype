using Assimp;
using OpenTK.Mathematics;
using Dwarf.Engine.DataStructures;

namespace Dwarf.Engine.Loaders;
public class GenericLoader : MeshLoader {
  private readonly AssimpContext _assimpContext;
  private readonly ConsoleLogStream _logger;

  List<DataStructures.Mesh> _meshes = new();
  List<Textures.TextureStruct> _textures = new();

  public GenericLoader() {
    _assimpContext = new AssimpContext();

    Assimp.LogStream.IsVerboseLoggingEnabled = true;
    _logger = new Assimp.ConsoleLogStream();
    _logger.Attach();
  }

  public override MasterMesh Load(string path) {
    var scene = _assimpContext.ImportFile($"{path}", PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);

    var node = scene.RootNode;

    for(int i=0; i<node.ChildCount; i++) {
      Console.WriteLine($"Processing Node {node.Children[i].Name}");
      ProcessNode(node.Children[i], scene);
    }
    

    return new MasterMesh(_meshes, Enums.MeshRenderType.FbxModel);
  }

  private void ProcessNode(Node node, Scene scene) {
    for(int i=0; i<node.MeshCount; i++) {
      var aMesh = scene.Meshes[node.MeshIndices[i]];
      _meshes.Add(ProcessMesh(aMesh, scene));
    }
  }

  private DataStructures.Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene) {
    List<Vertex> vertices = new List<Vertex>();
    List<int> indices = new List<int>();
    List<Textures.TextureStruct> textures = new List<Textures.TextureStruct>();

    Console.WriteLine($"Processing Mesh {mesh.Name}");
    for (int i=0; i<mesh.Vertices.Count; i++) {
      Vertex v = new();
      Vector3 vec = new();
      vec.X = mesh.Vertices[i].X;
      vec.Y = mesh.Vertices[i].Y;
      vec.Z = mesh.Vertices[i].Z;
      v.Position = vec;

      if(mesh.HasNormals) {
        vec.X = mesh.Normals[i].X;
        vec.Y = mesh.Normals[i].Y;
        vec.Z = mesh.Normals[i].Z;
        v.Normal = vec;
      }

      if(mesh.HasTextureCoords(0)) {
        Vector2 vec2 = new();
        vec2.X = mesh.TextureCoordinateChannels[0][i].X;
        vec2.Y = mesh.TextureCoordinateChannels[0][i].Y;
        v.TextureCoords = vec2;

        if(mesh.HasTangentBasis) {
          vec.X = mesh.Tangents[i].X;
          vec.Y = mesh.Tangents[i].Y;
          vec.Z = mesh.Tangents[i].Z;
          v.Tangent = vec;

          vec.X = mesh.BiTangents[i].X;
          vec.Y = mesh.BiTangents[i].Y;
          vec.Z = mesh.BiTangents[i].Z;
          v.Bitangent = vec;
        }
      } else {
        v.TextureCoords = new Vector2(0, 0);
      }

      vertices.Add(v);
    }

    for(int i = 0; i < mesh.FaceCount; i++) {
      var aFace = mesh.Faces[i];

      for(int j=0; j<aFace.IndexCount; j++) {
        indices.Add(aFace.Indices[j]);
      }
    }

    var aMaterial = scene.Materials[mesh.MaterialIndex];

    List<Textures.TextureStruct> diffuseMaps = LoadMaterialTextures(aMaterial, TextureType.Diffuse, TextureType.Diffuse);
    textures.InsertRange(textures.Count, diffuseMaps);

    List<Textures.TextureStruct> specularMaps = LoadMaterialTextures(aMaterial, TextureType.Specular, TextureType.Specular);
    textures.InsertRange(textures.Count, specularMaps);

    List<Textures.TextureStruct> normalMaps = LoadMaterialTextures(aMaterial, TextureType.Normals, TextureType.Normals);
    textures.InsertRange(textures.Count, normalMaps);

    List<Textures.TextureStruct> heightMaps = LoadMaterialTextures(aMaterial, TextureType.Height, TextureType.Height);
    textures.InsertRange(textures.Count, heightMaps);

    var returnMesh = new DataStructures.Mesh(vertices, indices, textures);
    Console.WriteLine($"Returning Mesh {mesh.Name}");

    return returnMesh;
  }

  private List<Textures.TextureStruct> LoadMaterialTextures(Assimp.Material material, Assimp.TextureType textureType, Assimp.TextureType typeName) {
    List<Textures.TextureStruct> materialTextures = new List<Textures.TextureStruct>();

    var count = material.GetMaterialTextureCount(textureType);
    for (int i=0; i<count; i++) {
      material.GetMaterialTexture(textureType, i, out Assimp.TextureSlot textureSlot);
      bool skip = false;
      for(int j=0; j<_textures.Count; j++) {
        if(_textures[j].Path == textureSlot.FilePath) {
          materialTextures.Add(_textures[j]);
          skip = true;
          break;
        }
      }
      if(!skip) {
        Textures.TextureStruct textureStruct;
        textureStruct.Id = Textures.Texture.FastTextureLoad($"Resources/{textureSlot.FilePath}").Handle;
        textureStruct.Type = typeName.ToString();
        textureStruct.Path = textureSlot.FilePath;
        materialTextures.Add(textureStruct);
      }
    }
    return materialTextures;
  }

  public MasterMesh LoadOld(string path) {
    var scene = _assimpContext.ImportFile($"{path}");

    _logger.Detach();

    List<DataStructures.Mesh> meshes = new();

    var node = scene.RootNode;

    foreach (var child in node.Children) {
      foreach (int index in child.MeshIndices) {
        List<Vector3> posList = new();
        List<Color4> colorList = new();
        List<Vector3> texList = new();
        List<Vector3> normalList = new();
        List<int> indices = new();

        var aMesh = scene.Meshes[index];

        foreach (Face face in aMesh.Faces) {
          for (int i = 0; i < face.IndexCount; i++) {
            int indice = face.Indices[i];

            indices.Add(indice);

            bool hasColors = aMesh.HasVertexColors(0);
            bool hasTexCoords = aMesh.HasTextureCoords(0);

            if (hasColors) {
              Color4 vertColor = FromColor(aMesh.VertexColorChannels[0][indice]);
              colorList.Add(vertColor);
            }
            if (aMesh.HasNormals) {
              Vector3 normal = FromVector(aMesh.Normals[indice]);
              normalList.Add(normal);
            }
            if (hasTexCoords) {
              Vector3 uvw = FromVector(aMesh.TextureCoordinateChannels[0][indice]);
              texList.Add(uvw);
            }
            Vector3 pos = FromVector(aMesh.Vertices[indice]);
            posList.Add(pos);
          }
        }

        DataStructures.Mesh mesh = new(
            posList,
            colorList,
            texList,
            normalList,
            indices
        );

        meshes.Add(mesh);
      }
    }

    return new MasterMesh(meshes, Enums.MeshRenderType.FbxModel);
  }

  private Vector3 FromVector(Vector3D vec) {
    Vector3 v;
    v.X = vec.X;
    v.Y = vec.Y;
    v.Z = vec.Z;
    return v;
  }

  private Color4 FromColor(Color4D color) {
    Color4 c;
    c.R = color.R;
    c.G = color.G;
    c.B = color.B;
    c.A = color.A;
    return c;
  }
}
