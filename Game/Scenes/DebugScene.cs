using Dwarf.Engine.Cameras;
using Dwarf.Engine.Controllers;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Generators;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Loaders;
using Dwarf.Engine.Physics;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Scenes;

public class DebugScene : Scene {
  public DebugScene() : base() {
    var window = WindowGlobalState.GetWindow();

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0, 5, 0)));
    camera.AddComponent(new FreeCamera(window.Size.X / (float)window.Size.Y));
    // camera.AddComponent(new ThirdPersonCamera(window.Size.X / (float)window.Size.Y, null!));
    CameraGlobalState.SetCameraEntity(camera);
    // CameraGlobalState.SetCamera(camera.GetComponent<ThirdPersonCamera>());
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());

    // TODO : Marching cubes
    // TODO : Procedural Mesh Generation

    // var window = WindowGlobalState.GetWindow();

    var objTest = new Entity();
    Entities.Add(objTest);
    objTest.AddComponent(new Transform(new Vector3(2,0,0)));
    objTest.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
    objTest.AddComponent(new ObjLoader().Load("Resources/chr_knight"));
    objTest.AddComponent(new MeshRenderer());
    //objTest.AddComponent(new TransformController(1.5f));
    objTest.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    objTest.AddComponent(new BoundingBox());
    objTest.GetComponent<BoundingBox>().Setup(objTest.GetComponent<MasterMesh>());
    objTest.SetName("chr knight");

    var fbx = new Entity();
    fbx.SetName("Yuna");
    fbx.AddComponent(new Transform(new Vector3(0, 0, 0)));
    fbx.GetComponent<Transform>().Rotation = new Vector3(-90, 180, 0);
    fbx.AddComponent(new Material(new Vector3(1, 1, 1)));
    fbx.AddComponent(new FbxLoader().Load($"Resources/{fbx.GetName()}"));
    fbx.AddComponent(new MeshRenderer());
    fbx.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    // fbx.AddComponent(new TransformController(1.5f));
    Entities.Add(fbx);

    var newTerrain = new Entity();
    Entities.Add(newTerrain);
    newTerrain.AddComponent(new Transform(new Vector3(-100, 0, -100)));
    newTerrain.AddComponent(new Material(new Vector3(1, 1, 1)));
    newTerrain.AddComponent(new MasterMesh());
    newTerrain.AddComponent(new MeshRenderer());

    var chunk = Chunk.SetupMesh(100);
    newTerrain.GetComponent<MasterMesh>().Meshes.Add(chunk);

    newTerrain.GetComponent<MeshRenderer>().Init("./Shaders/terrain.vert", "./Shaders/terrain.frag");
    newTerrain.SetName("chunk");


    var terrain = new Entity();
    terrain.AddComponent(new Transform(new Vector3(0, 0, 0)));
    terrain.AddComponent(new Material(new Vector3(1.0f, 1.0f, 1.0f)));
    terrain.AddComponent(new MeshGenerator());
    terrain.AddComponent(new MeshRenderer());
    terrain.GetComponent<MeshGenerator>().SetupPlane(0, 0);
    terrain.GetComponent<MeshRenderer>().Init("./Shaders/terrain.vert", "./Shaders/terrain.frag");
    terrain.SetName("terrain");
    Entities.Add(terrain);

    int offsetX = 15;
    int offsetY = 15;

 
    for(int i=0; i<15; i++) {
      var monu1 = new Entity();
      monu1.SetName("monu1");
      monu1.AddComponent(new Transform(new Vector3(offsetX, 0, offsetY)));
      monu1.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
      monu1.AddComponent(new ObjLoader().Load($"Resources/{monu1.GetName()}"));
      monu1.AddComponent(new MeshRenderer());
      monu1.AddComponent(new BoundingBox());
      monu1.GetComponent<BoundingBox>().Setup(monu1.GetComponent<MasterMesh>());
      monu1.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
      Entities.Add(monu1);
      if(offsetX > 60) {
        offsetX = 15;
        offsetY += 15;
      } else {
        offsetX += 15;
      }
      
    }

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);

    // camera.GetComponent<ThirdPersonCamera>().FollowTarget = objTest;
  }

  public override void RenderScene() {
    throw new NotImplementedException();
  }
}