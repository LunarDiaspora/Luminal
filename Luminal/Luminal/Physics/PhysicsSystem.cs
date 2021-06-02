using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Logging;
using PhysX;
using PhysX.VisualDebugger;

namespace Luminal.Physics
{
    class ErrorOutput : ErrorCallback
    {
        public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
        {
            switch (errorCode)
            {
                case ErrorCode.DebugInfo:
                    Log.Debug($"PhysX ({file}:{lineNumber}): {message}");
                    break;
                case ErrorCode.InternalError:
                case ErrorCode.InvalidOperation:
                case ErrorCode.InvalidParameter:
                case ErrorCode.OutOfMemory:
                    Log.Error($"PhysX ERROR ({file}:{lineNumber}): {message}");
                    break;
                case ErrorCode.Warning:
                    Log.Warn($"PhysX ({file}:{lineNumber}): {message}");
                    break;
                default:
                    break;
            }
        }
    }

    public static class PhysicsSystem
    {
        public static PhysX.Physics Physics;
        public static PhysX.Foundation Foundation;
        public static Pvd PVD;
        public static Scene PhysicsScene;

        public static void Initialise()
        {
            var eo = new ErrorOutput();
            Foundation = new Foundation(eo);
            PVD = new Pvd(Foundation);
            Physics = new PhysX.Physics(Foundation, false, PVD);

            PhysicsScene = Physics.CreateScene(); // TODO: SceneDesc

            PhysicsScene.SetVisualizationParameter(VisualizationParameter.Scale, 2.0f);
            PhysicsScene.SetVisualizationParameter(VisualizationParameter.CollisionShapes, true);
            PhysicsScene.SetVisualizationParameter(VisualizationParameter.JointLocalFrames, true);
            PhysicsScene.SetVisualizationParameter(VisualizationParameter.JointLimits, true);
            PhysicsScene.SetVisualizationParameter(VisualizationParameter.ActorAxes, true);

            PVD.Connect("localhost");
        }
    }
}
