using System.Reflection.Metadata;
using OpenTK.Mathematics;

using Voxelized.DataStructures;
using Voxelized.ECS;
using Voxelized.Loaders;
using Voxelized.Globals;

namespace Voxelized.Scenes;

public class DebugScene : Scene {
  public DebugScene() : base() {
    // TODO : FBX Loader
    // TODO : Marching cubes
    // TODO : Procedural Mesh Generation


    var objTest = new Entity();
    objTest.AddComponent(new Transform(new Vector3(10,0,0)));
    objTest.AddComponent(new Material(new Vector3(1f, 0.2f, 0.0f)));
    objTest.AddComponent(new ObjLoader().LoadObj("Resources/chr_knight"));
    objTest.AddComponent(new MeshRenderer());
    objTest.GetComponent<MeshRenderer>().Init();
    objTest.SetName("chr knight");

    Entities.Add(objTest);

    EntityGlobalState.ClearEntities();
    EntityGlobalState.SetEntities(Entities);
  }

  internal override void RenderScene() {
    throw new NotImplementedException();
  }
}