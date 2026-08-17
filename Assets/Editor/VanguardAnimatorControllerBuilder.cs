#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SeasOfLegends.EditorTools
{
    /// <summary>
    /// Builds the first production Animator Controller after the Vanguard mesh and Mixamo walk
    /// clip are present. Additional imported clips can be added here without changing gameplay code.
    /// </summary>
    public static class VanguardAnimatorControllerBuilder
    {
        private const string ControllerPath = "Assets/Resources/Models/Animations/VanguardLocomotion.controller";
        private const string WalkClipPath = "Assets/Resources/Models/Animations/UnarmedWalkForward.fbx";

        [MenuItem("Seas of Legends/Build Vanguard Locomotion Controller")]
        public static void Build()
        {
            AnimationClip walkClip = AssetDatabase.LoadAllAssetsAtPath(WalkClipPath)
                .OfType<AnimationClip>()
                .FirstOrDefault(clip => !clip.name.StartsWith("__preview__"));
            if (walkClip == null)
            {
                Debug.LogWarning("Vanguard locomotion controller requires UnarmedWalkForward.fbx in Resources/Models/Animations.");
                return;
            }

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            AnimatorControllerLayer layer = controller.layers[0];
            AnimatorStateMachine machine = layer.stateMachine;
            foreach (ChildAnimatorState state in machine.states)
            {
                if (state.state != null) Object.DestroyImmediate(state.state, true);
            }

            AnimatorState idle = machine.AddState("Grounded Idle");
            // Until a dedicated idle clip is supplied, frame zero of the imported walk clip provides
            // a grounded neutral stance instead of exposing the character's bind-pose silhouette.
            idle.motion = walkClip;
            idle.speed = 0f;
            AnimatorState walk = machine.AddState("Walk");
            walk.motion = walkClip;
            walk.speed = 1f;
            machine.defaultState = idle;

            EnsureFloat(controller, "WalkRate");
            AnimatorStateTransition toWalk = idle.AddTransition(walk);
            toWalk.hasExitTime = false;
            toWalk.duration = 0.12f;
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.05f, "WalkRate");
            AnimatorStateTransition toIdle = walk.AddTransition(idle);
            toIdle.hasExitTime = false;
            toIdle.duration = 0.12f;
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.05f, "WalkRate");

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Built VanguardLocomotion.controller with the Mixamo walk clip.");
        }

        public static void BuildIfRequired()
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) == null &&
                AssetDatabase.LoadMainAssetAtPath(WalkClipPath) != null)
                Build();
        }

        private static void EnsureFloat(AnimatorController controller, string name)
        {
            if (!controller.parameters.Any(parameter => parameter.name == name))
                controller.AddParameter(name, AnimatorControllerParameterType.Float);
        }
    }
}
#endif
