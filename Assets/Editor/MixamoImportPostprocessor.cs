#if UNITY_EDITOR
using System;
using UnityEditor;

namespace SeasOfLegends.EditorTools
{
    /// <summary>
    /// Normalizes the project-owned Mixamo FBX folders for reliable Unity Humanoid import.
    /// Character meshes retain an Avatar; animation FBXs are imported as Humanoid clips.
    /// </summary>
    public sealed class MixamoImportPostprocessor : AssetPostprocessor
    {
        private const string LegacyCharacterFolder = "Assets/Resources/Models/Characters/";
        private const string LegacyAnimationFolder = "Assets/Resources/Models/Animations/";
        private const string LumenwakeCharacterFolder = "Assets/Resources/Lumenwake/Characters/";
        private const string LumenwakeAnimationFolder = "Assets/Resources/Lumenwake/Animations/";

        private void OnPreprocessModel()
        {
            if (!assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) return;

            ModelImporter importer = (ModelImporter)assetImporter;
            if (assetPath.StartsWith(LegacyCharacterFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.StartsWith(LumenwakeCharacterFolder, StringComparison.OrdinalIgnoreCase))
            {
                ConfigureHumanoidMesh(importer);
                return;
            }

            if (assetPath.StartsWith(LegacyAnimationFolder, StringComparison.OrdinalIgnoreCase) ||
                assetPath.StartsWith(LumenwakeAnimationFolder, StringComparison.OrdinalIgnoreCase))
            {
                ConfigureHumanoidMotion(importer);
            }
        }

        private static void ConfigureHumanoidMesh(ModelImporter importer)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.importCameras = false;
            importer.importLights = false;
            importer.importBlendShapes = false;
            importer.optimizeGameObjects = true;
        }

        private static void ConfigureHumanoidMotion(ModelImporter importer)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.importCameras = false;
            importer.importLights = false;
            importer.importBlendShapes = false;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path.StartsWith(LumenwakeCharacterFolder, StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(LumenwakeAnimationFolder, StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith("Resources/Models/Animations/UnarmedWalkForward.fbx", StringComparison.OrdinalIgnoreCase))
                {
                    EditorApplication.delayCall += NahliaAnimatorControllerBuilder.BuildIfSourceAvailable;
                    break;
                }
            }
        }
    }
}
#endif
