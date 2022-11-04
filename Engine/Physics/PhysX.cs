using PhysX;
using PhysX.VisualDebugger;

namespace Dwarf.Engine.Physics;

public class SampleFilterShader : SimulationFilterShader {
  public override FilterResult Filter(int attributes0, FilterData filterData0, int attributes1, FilterData filterData1) {
    return new FilterResult {
      FilterFlag = FilterFlag.Default,
      // Cause PhysX to report any contact of two shapes as a touch and call SimulationEventCallback.OnContact
      PairFlags = PairFlag.ContactDefault | PairFlag.NotifyTouchFound | PairFlag.NotifyTouchLost
    };
  }
}

public class ErrorOutput : ErrorCallback {
  public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber) {
    Console.WriteLine("PhysX: " + message);
  }
}

public class PhysXClass {
  private Scene _scene;
  private Physics _physics;

  private Action<SceneDesc> _sceneDescCallback;

  public PhysXClass() {
    Init();
  }

  void Init() {
    //var physics = new PhysX.Physics(foundation, true, pvd);

    var errorOutput = new ErrorOutput();
    var foundation = new Foundation(errorOutput);

    

    var desc = CreateSceneDescription(foundation);


  }

  public static void Test() {
    //var actor = new RigidActor()
    //var shape = RigidActorExt.CreateExclusiveShape()

    // var camera = 
  }

  private SceneDesc CreateSceneDescription(Foundation foundation) {
    var cuda = new CudaContextManager(foundation);
    var sceneDesc = new SceneDesc {
      Gravity = new System.Numerics.Vector3(0, -9.81f, 0),
      FilterShader = new SampleFilterShader()
    };
    sceneDesc.Flags |= SceneFlag.EnableGpuDynamics;
    sceneDesc.BroadPhaseType |= BroadPhaseType.Gpu;

    _sceneDescCallback?.Invoke(sceneDesc);
    return sceneDesc;
  }
}
