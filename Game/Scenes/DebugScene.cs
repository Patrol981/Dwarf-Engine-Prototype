using System.Reflection.Metadata;
using OpenTK.Mathematics;

using Voxelized.DataStructures;
using Voxelized.ECS;
using Voxelized.Loaders;
using Voxelized.Globals;
using Voxelized.Challanges;

namespace Voxelized.Scenes;

public class DebugScene : Scene {
  public DebugScene() : base() {
    var modelTest = new Entity();
    modelTest.AddComponent(new Transform(new Vector3(0,0,0)));
    modelTest.AddComponent(new Material(new Vector3(0.9f, 0.5f, 0.3f)));
    modelTest.AddComponent(new STLMeshLoader().Load("Resources/mod3.stl"));
    modelTest.AddComponent(new MeshRenderer());
    modelTest.GetComponent<MeshRenderer>().Init();
    modelTest.SetName("starship");

    var modelTest2 = new Entity();
    modelTest2.AddComponent(new Transform(new Vector3(5,0,0)));
    modelTest2.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    modelTest2.AddComponent(new STLMeshLoader().Load("Resources/mod3.stl"));
    modelTest2.AddComponent(new MeshRenderer());
    modelTest2.GetComponent<MeshRenderer>().Init();
    modelTest2.SetName("starship2");

    Entities.Add(modelTest);
    Entities.Add(modelTest2);

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);
  }

  internal override void RenderScene() {
    throw new NotImplementedException();
  }
}