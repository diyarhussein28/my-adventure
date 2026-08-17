#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SeasOfLegends.EditorTools
{
    /// <summary>
    /// Applies repeatable Unity import settings to all supplied Mixamo FBX files. Keep downloaded
    /// character meshes in Resources/Models/Characters and animations in Resources/Models/Animations.
    /// Unity invokes this automatically during the next import/reimport.
    /// </summary>
    public sealed class MixamoImportPostprocessor : AssetPostprocessor
    {
        private const string CharacterFolder = "Assets/Resources/Models/Characters/";
        private const string AnimationFolder = "Assets/Resources/Models/Animations/";

        private void OnPreprocessModel()
        {
            if (!assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase)) return;
            ModelImporter importer = (ModelImporter)assetImporter;
            if (assetPath.StartsWith(CharacterFolder))
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
                importer.optimizeGameObjects = true;
                importer.importBlendShapes = false;
            }
            else if (assetPath.StartsWith(AnimationFolder))
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = true;
                importer.importCameras = false;
                importer.importLights = false;
                importer.importBlendShapes = false;
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool importedWalk = importedAssets.Any(path => path.EndsWith("Resources/Models/Animations/UnarmedWalkForward.fbx", System.StringComparison.OrdinalIgnoreCase));
            if (importedWalk)
            {
                EditorApplication.delayCall += VanguardAnimatorControllerBuilder.BuildIfRequired;
            }
        }
    }
}
#endif
