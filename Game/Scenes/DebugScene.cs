using Dwarf.Engine.Cameras;
using Dwarf.Engine.Controllers;
using Dwarf.Engine.DataStructures;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Generators;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Loaders;
using OpenTK.Mathematics;

namespace Dwarf.Engine.Scenes;

public class DebugScene : Scene {
  public DebugScene() : base() {
    var window = WindowGlobalState.GetWindow();

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0, 0, 3)));
    camera.AddComponent(new FreeCamera(Vector3.UnitZ * 3, window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCameraEntity(camera);
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());

    // TODO : Marching cubes
    // TODO : Procedural Mesh Generation

    // var window = WindowGlobalState.GetWindow();

    var terrain = new Entity();
    terrain.AddComponent(new Transform(new Vector3(0, 0, 0)));
    terrain.AddComponent(new Material(new Vector3(1.0f, 1.0f, 1.0f)));
    terrain.AddComponent(new MeshGenerator());
    terrain.AddComponent(new MeshRenderer());
    terrain.GetComponent<MeshGenerator>().SetupPlane(0, 0);
    terrain.GetComponent<MeshRenderer>().Init("./Shaders/terrain.vert", "./Shaders/terrain.frag");
    terrain.SetName("new terrain");
    Entities.Add(terrain);

    /*
    var objTest = new Entity();
    objTest.AddComponent(new Transform(new Vector3(2,0,0)));
    objTest.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    objTest.AddComponent(new SimpleObjLoader().Load("Resources/chr_knight"));
    objTest.AddComponent(new MeshRenderer());
    objTest.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    objTest.SetName("chr knight");
    Entities.Add(objTest);
    */

    int offsetX = 15;
    int offsetY = 15;

 
    for(int i=0; i<15; i++) {
      var monu1 = new Entity();
      monu1.SetName("monu1");
      monu1.AddComponent(new Transform(new Vector3(offsetX, 0, offsetY)));
      monu1.AddComponent(new Material(new Vector3(1f, 1f, 1f)));
      monu1.AddComponent(new ObjLoader().Load($"Resources/{monu1.GetName()}"));
      monu1.AddComponent(new MeshRenderer());
      monu1.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
      Entities.Add(monu1);
      if(offsetX > 60) {
        offsetX = 15;
        offsetY += 15;
      } else {
        offsetX += 15;
      }
      
    }

    /*
    var box = new Entity();
    box.AddComponent(new Transform(new Vector3(4, 0, 0)));
    box.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    box.AddComponent(new SimpleObjLoader().Load("Resources/box"));
    box.AddComponent(new MeshRenderer());
    box.GetComponent<MeshRenderer>().Init("./Shaders/cubeComponent.vert", "./Shaders/cubeComponent.frag");
    box.SetName("box");
    Entities.Add(box);
    */

    /*
    var cube = new Entity();
    cube.AddComponent(new Transform(new Vector3(2, 0, 0)));
    cube.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    cube.AddComponent(new CubeComponent().Load());
    cube.AddComponent(new MeshRenderer());
    cube.GetComponent<MeshRenderer>().Init("./Shaders/cube.vert", "./Shaders/cube.frag");
    cube.SetName("cube");
    Entities.Add(cube);
    */

    var fbx = new Entity();
    fbx.SetName("Yuna");
    fbx.AddComponent(new Transform(new Vector3(0, 0, 0)));
    fbx.GetComponent<Transform>().Rotation = new Vector3(-90, 0, 0);
    fbx.AddComponent(new Material(new Vector3(1, 1, 1)));
    fbx.AddComponent(new FbxLoader().Load($"Resources/{fbx.GetName()}"));
    fbx.AddComponent(new MeshRenderer());
    //fbx.GetComponent<MeshRenderer>().Init("./Shaders/cubeComponent.vert", "./Shaders/cubeComponent.frag");
    fbx.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    fbx.AddComponent(new TransformController(1.5f));
    Entities.Add(fbx);

    /*
    var fbx2 = new Entity();
    fbx2.SetName("fbx2");
    fbx2.AddComponent(new Transform(new Vector3(2, 0, 2)));
    fbx2.AddComponent(new Material(new Vector3(1, 0.5f, 0.3f)));
    fbx2.AddComponent(new FbxLoader().Load("Resources/Yuna"));
    fbx2.AddComponent(new MeshRenderer());
    fbx2.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    Entities.Add(fbx2);
    */

    /*
    var backpack = new Entity();
    backpack.SetName("backpack");
    backpack.AddComponent(new Transform(new Vector3(4, 0, 0)));
    backpack.AddComponent(new Material(new Vector3(1, 0.5f, 0.3f)));
    backpack.AddComponent(new ObjLoader().Load("Resources/backpack"));
    backpack.AddComponent(new MeshRenderer());
    backpack.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    Entities.Add(backpack);
    */

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);
  }

  public override void RenderScene() {
    throw new NotImplementedException();
  }
}