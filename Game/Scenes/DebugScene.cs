using System.Reflection.Metadata;
using OpenTK.Mathematics;

using Voxelized.Engine.DataStructures;
using Voxelized.Engine.ECS;
using Voxelized.Engine.Loaders;
using Voxelized.Engine.Globals;
using Voxelized.Engine.Generators;
using Voxelized.Engine.Cameras;

namespace Voxelized.Engine.Scenes;

public class DebugScene : Scene {
  public DebugScene() : base() {
    // TODO : Marching cubes
    // TODO : Procedural Mesh Generation

    var window = WindowGlobalState.GetWindow();

    var camera = new Entity();
    camera.AddComponent(new Transform(new Vector3(0, 0, 1)));
    camera.AddComponent(new FreeCamera(Vector3.UnitZ * 3, window.Size.X / (float)window.Size.Y));
    CameraGlobalState.SetCameraEntity(camera);
    CameraGlobalState.SetCamera(camera.GetComponent<FreeCamera>());

    var terrain = new Entity();
    terrain.AddComponent(new Transform(new Vector3(0, 0, 0)));
    terrain.AddComponent(new Material(new Vector3(1.0f, 1.0f, 1.0f)));
    terrain.AddComponent(new MeshGenerator());
    terrain.AddComponent(new MeshRenderer());
    terrain.GetComponent<MeshGenerator>().SetupPlane(0, 0);
    terrain.GetComponent<MeshRenderer>().Init("./Shaders/terrain.vert", "./Shaders/terrain.frag");
    terrain.SetName("new terrain");

    var objTest = new Entity();
    objTest.AddComponent(new Transform(new Vector3(2,0,0)));
    objTest.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    objTest.AddComponent(new SimpleObjLoader().Load("Resources/chr_knight"));
    objTest.AddComponent(new MeshRenderer());
    objTest.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    objTest.SetName("chr knight");

    int offsetX = 15;
    int offsetY = 15;

    for(int i=0; i<1; i++) {
      var monu1 = new Entity();
      monu1.AddComponent(new Transform(new Vector3(offsetX, 0, offsetY)));
      monu1.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
      monu1.AddComponent(new SimpleObjLoader().Load("Resources/monu1"));
      monu1.AddComponent(new MeshRenderer());
      monu1.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
      monu1.SetName("monu1");
      Entities.Add(monu1);
      if(offsetX > 60) {
        offsetX = 15;
        offsetY += 15;
      } else {
        offsetX += 15;
      }
      
    }

    var box = new Entity();
    box.AddComponent(new Transform(new Vector3(0, 0, 0)));
    box.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    box.AddComponent(new SimpleObjLoader().Load("Resources/box"));
    box.AddComponent(new MeshRenderer());
    box.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    box.SetName("box");
    Entities.Add(box);

    var fbx = new Entity();
    fbx.AddComponent(new Transform(new Vector3(2, 0, 2)));
    fbx.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    fbx.AddComponent(new FbxLoader().Load("Resources/Yuna@Female Standing Pose"));
    fbx.AddComponent(new MeshRenderer());
    fbx.GetComponent<MeshRenderer>().Init("./Shaders/vertexShader.vert", "./Shaders/fragmentShader.frag");
    fbx.SetName("fbx");
    Entities.Add(fbx);



    Entities.Add(terrain);
    
    Entities.Add(objTest);
    

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);
  }

  internal override void RenderScene() {
    throw new NotImplementedException();
  }
}