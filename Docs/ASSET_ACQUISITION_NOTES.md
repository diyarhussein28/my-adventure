# External Asset Acquisition Notes

## Mixamo humanoid source

The signed-in user account at [Mixamo](https://www.mixamo.com/) was used to select the **Vanguard by T. Choonyung** humanoid character. The user downloaded and attached the character FBX, which is now stored in the Unity project as `Assets/Resources/Models/Characters/VanguardHero.fbx`.

Mixamo’s interface describes its library as providing textured and rigged 3D characters, automatic character rigging, motion-captured animations, and multi-format exports. The current target format is FBX compatible with Unity. The model asset was supplied by the user under their own Mixamo account.

## Animation acquisition status

The current browser session has selected **Unarmed Walk Forward** on the Vanguard character. The in-place option was enabled in the animation preview. The export dialog is open. Recommended export settings are `FBX for Unity` where selectable, `Without Skin`, `30 FPS`, and `In Place` for locomotion clips. Browser downloads land on the user’s local machine and must be attached to this chat before they can be imported into the repository.

The initial animation set should include idle, walk, run, jump, a melee/sword attack, hit reaction, and block. Imported animation FBX files belong under `Assets/Resources/Models/Animations/`. The project contains `Assets/Editor/MixamoImportPostprocessor.cs` to configure character and animation FBXs as Unity Humanoid imports.

## Environment source research

A researched example for a commercial-quality tropical environment is [Tropical Island Environment: Props & VFX](https://assetstore.unity.com/packages/3d/environments/landscapes/tropical-island-environment-props-vfx-305720) on the Unity Asset Store. The page advertises PBR materials, tropical vegetation, ocean/beach environment content, and HDRP compatibility. Acquisition must use the user’s own Unity Asset Store account and comply with the asset’s license and pipeline compatibility requirements.

A publicly documented free animation alternative is [Human Basic Motions FREE](https://assetstore.unity.com/packages/3d/animations/human-basic-motions-free-154271), but it has not been acquired or imported.
