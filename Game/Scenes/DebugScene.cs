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
    // TODO : FBX Loader
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
    terrain.GetComponent<MeshRenderer>().Init();
    terrain.SetName("new terrain");

    var objTest = new Entity();
    objTest.AddComponent(new Transform(new Vector3(2,0,0)));
    objTest.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    objTest.AddComponent(new SimpleObjLoader().Load("Resources/chr_knight"));
    objTest.AddComponent(new MeshRenderer());
    objTest.GetComponent<MeshRenderer>().Init();
    objTest.SetName("chr knight");

    int offsetX = 15;
    int offsetY = 15;

    for(int i=0; i<30; i++) {
      var monu1 = new Entity();
      monu1.AddComponent(new Transform(new Vector3(offsetX, 0, offsetY)));
      monu1.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
      monu1.AddComponent(new SimpleObjLoader().Load("Resources/monu1"));
      monu1.AddComponent(new MeshRenderer());
      monu1.GetComponent<MeshRenderer>().Init();
      monu1.SetName("monu1");
      Entities.Add(monu1);
      if(offsetX > 60) {
        offsetX = 15;
        offsetY += 15;
      } else {
        offsetX += 15;
      }
      
    }

    

    Entities.Add(terrain);
    
    Entities.Add(objTest);
    

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);
  }

  internal override void RenderScene() {
    throw new NotImplementedException();
  }
}