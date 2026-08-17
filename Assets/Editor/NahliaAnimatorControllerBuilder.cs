#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SeasOfLegends.EditorTools
{
    /// <summary>
    /// Builds the Lumenwake hero controller from free Mixamo source clips. It gracefully uses the
    /// existing in-place walk clip for the initial idle stance until a dedicated idle FBX is imported.
    /// </summary>
    public static class NahliaAnimatorControllerBuilder
    {
        private const string ControllerPath = "Assets/Resources/Lumenwake/Animations/NahliaLocomotion.controller";
        private const string WalkClipPath = "Assets/Resources/Models/Animations/UnarmedWalkForward.fbx";
        private const string IdleClipPath = "Assets/Resources/Lumenwake/Animations/Nahlia/StandingIdle03.fbx";

        [MenuItem("Seas of Legends/Build Lumenwake Nahlia Controller")]
        public static void Build()
        {
            AnimationClip walk = LoadClip(WalkClipPath);
            if (walk == null)
            {
                Debug.LogWarning("Lumenwake Nahlia controller needs UnarmedWalkForward.fbx before it can be generated.");
                return;
            }

            AnimationClip idle = LoadClip(IdleClipPath) ?? walk;
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);

            AnimatorStateMachine machine = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState state in machine.states)
                if (state.state != null) Object.DestroyImmediate(state.state, true);

            EnsureFloat(controller, "WalkRate");
            EnsureBool(controller, "Grounded");
            EnsureBool(controller, "Blocking");
            EnsureBool(controller, "Stunned");
            EnsureTrigger(controller, "Jump");
            EnsureTrigger(controller, "Dash");
            EnsureTrigger(controller, "Attack_Light_1");
            EnsureTrigger(controller, "Attack_Light_2");

            AnimatorState idleState = machine.AddState("Nahlia Idle");
            idleState.motion = idle;
            if (idle == walk) idleState.speed = 0f;

            AnimatorState walkState = machine.AddState("Nahlia Walk");
            walkState.motion = walk;
            walkState.speed = 1f;
            machine.defaultState = idleState;

            AnimatorStateTransition toWalk = idleState.AddTransition(walkState);
            toWalk.hasExitTime = false;
            toWalk.duration = 0.10f;
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.05f, "WalkRate");
            AnimatorStateTransition toIdle = walkState.AddTransition(idleState);
            toIdle.hasExitTime = false;
            toIdle.duration = 0.10f;
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.05f, "WalkRate");

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Built NahliaLocomotion.controller for the Lumenwake Reef vertical slice.");
        }

        public static void BuildIfSourceAvailable()
        {
            if (AssetDatabase.LoadMainAssetAtPath(WalkClipPath) != null) Build();
        }

        private static AnimationClip LoadClip(string path)
        {
            return AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<AnimationClip>()
                .FirstOrDefault(clip => !clip.name.StartsWith("__preview__"));
        }

        private static void EnsureFloat(AnimatorController controller, string name)
        {
            if (!controller.parameters.Any(parameter => parameter.name == name))
                controller.AddParameter(name, AnimatorControllerParameterType.Float);
        }

        private static void EnsureBool(AnimatorController controller, string name)
        {
            if (!controller.parameters.Any(parameter => parameter.name == name))
                controller.AddParameter(name, AnimatorControllerParameterType.Bool);
        }

        private static void EnsureTrigger(AnimatorController controller, string name)
        {
            if (!controller.parameters.Any(parameter => parameter.name == name))
                controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
        }
    }
}
#endif
