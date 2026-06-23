using EmeToDia.Gameplay;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EmeToDia.Editor
{
    // Eme_to_Dia_Promotion 씬을 과제 요구에 맞게 재구성하는 에디터 도구입니다.
    // UI는 프리팹을 먼저 만들고 씬에 Instantiate하며, 월드 아이템도 Addressables 프리팹으로만 배치합니다.
    public static class DaniTechPromotionSceneBuilder
    {
        private const string ScenePath = "Assets/Eme_to_Dia_Promotion.unity";
        private const string MaterialFolderPath = "Assets/DaniTech/Materials";
        private const string TerrainFolderPath = "Assets/DaniTech/Terrain";
        private const string PrefabFolderPath = "Assets/DaniTech/Prefabs";
        private const string ItemPrefabFolderPath = PrefabFolderPath + "/Items";
        private const string UiPrefabFolderPath = PrefabFolderPath + "/UI";
        private const string ExternalFolderPath = "Assets/ThirdParty/DaniTechImported";
        private const string AddressableGroupName = "DaniTechPromotion";
        private const string BrooklynSourceRoot = "D:/3D_Basic_Assets/0_배경/Kitbash3D Brooklyn-004/Kitbash3D Brooklyn-004/Kitbash3D Brooklyn/Kitbash3D Brooklyn Unity BuiltIn/KB3D_Brooklyn_UnityBuiltIn/Assets/KB3D/Brooklyn";
        private const string AsteroidCrystalPackageRoot = "D:/3D_Basic_Assets/3_프롭/Asteroid Crystal Set 1.unitypackage";
        private const string MagePackageRoot = "D:/3D_Basic_Assets/1_캐릭터/Stylized Mages - 8 colour variations Low Poly v1.4.unitypackage";
        private const string TownConstructorPackageRoot = "D:/3D_Basic_Assets/0_배경/drive-download-20260617T045914Z-3-001/Town Constructor 2.unitypackage";
        private const string ForestHousePackageRoot = "D:/3D_Basic_Assets/0_배경/drive-download-20260617T045914Z-3-004/Forest House Environment v1.0.unitypackage";
        private const string BrooklynImportedFolderPath = ExternalFolderPath + "/KB3D_Brooklyn";
        private const string AsteroidImportedFolderPath = ExternalFolderPath + "/Asteroid_Crystal_Set_01";
        private const string MageImportedFolderPath = ExternalFolderPath + "/Stylized_Mages";
        private const string TownImportedFolderPath = ExternalFolderPath + "/TownConstructor2";
        private const string ForestHouseImportedFolderPath = ExternalFolderPath + "/ForestHouseEnvironment";

        private const string EmeraldItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_EmeraldCore.prefab";
        private const string DiamondItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_DiamondTonic.prefab";
        private const string ShieldItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_PrismShield.prefab";
        private const string SwiftItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_SwiftSigil.prefab";
        private const string UiPrefabPath = UiPrefabFolderPath + "/PF_DaniTechPromotionUI.prefab";
        private const string UseEffectPrefabPath = PrefabFolderPath + "/PF_DaniTech_UseEffect.prefab";
        private const string CrystalLargeAssetPath = AsteroidImportedFolderPath + "/Models/Crystal_Lrg_A_01.FBX";
        private const string CrystalMediumAssetPath = AsteroidImportedFolderPath + "/Models/Crystal_Med_A_01.FBX";
        private const string CrystalSmallAAssetPath = AsteroidImportedFolderPath + "/Models/Crystal_Sml_A_01.FBX";
        private const string CrystalSmallBAssetPath = AsteroidImportedFolderPath + "/Models/Crystal_Sml_B_01.FBX";
        private const string CrystalSmallCAssetPath = AsteroidImportedFolderPath + "/Models/Crystal_Sml_C_01.FBX";
        private const string AsteroidSmallAAssetPath = AsteroidImportedFolderPath + "/Models/Asteroid_Sml_A_01.FBX";
        private const string AsteroidSmallBAssetPath = AsteroidImportedFolderPath + "/Models/Asteroid_Sml_B_01.FBX";
        private const string PlayerCharacterPrefabPath = MageImportedFolderPath + "/Models/Characters/Fantasy/Mages/MageBlue.prefab";
        private const string TownFloorAssetPath = TownImportedFolderPath + "/19ThTownPack/Standard Assets/Prototyping/Models/FloorPrototype64x01x64.fbx";
        private const string TownRoadAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/Road.fbx";
        private const string TownSideWalkAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/SideWalk.fbx";
        private const string TownRoadEdgeAAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/RoadEdge_01.fbx";
        private const string TownRoadEdgeBAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/RoadEdge_02.fbx";
        private const string TownStoneChunksAAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/StoneChunks1.FBX";
        private const string TownStoneChunksBAssetPath = TownImportedFolderPath + "/19ThTownPack/ModularParts/Roads/StoneChunks3.FBX";
        private const string TownStreetLightAssetPath = TownImportedFolderPath + "/19ThTownPack/Props/StreetLight2.FBX";
        private const string ForestGrassAlbedoPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Grass_1/FHE_Grass_1_AlbedoSmoothness.png";
        private const string ForestGrassNormalPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Grass_1/FHE_Grass_1_Normal.png";
        private const string ForestMudAlbedoPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Mud_2/FHE_Mud_2_AlbedoSmoothness.png";
        private const string ForestMudNormalPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Mud_2/FHE_Mud_2_Normal.png";
        private const string ForestRockAlbedoPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Rocks_2/FHE_Rocks_2_AlbedoSmoothness.png";
        private const string ForestRockNormalPath = ForestHouseImportedFolderPath + "/Forest House Environment/Textures/Terrain Ground/Rocks_2/FHE_Rocks_2_Normal.png";
        private const string ForestTreeOnePrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Tree_1.prefab";
        private const string ForestTreeTwoPrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Tree_2.prefab";
        private const string ForestBushOnePrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Bush_1.prefab";
        private const string ForestBushTwoPrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Bush_2.prefab";
        private const string ForestRockOnePrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Rock_1.prefab";
        private const string ForestRockTwoPrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Rock_2.prefab";
        private const string ForestRockThreePrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Rock_3.prefab";
        private const string ForestRockFourPrefabPath = ForestHouseImportedFolderPath + "/Forest House Environment/Prefabs/FHE_Rock_4.prefab";
        private const string TerrainDataPath = TerrainFolderPath + "/Terrain_DaniTech_HeroValley.asset";
        private const string GrassTerrainLayerPath = TerrainFolderPath + "/TL_DaniTech_Grass.asset";
        private const string MudTerrainLayerPath = TerrainFolderPath + "/TL_DaniTech_Mud.asset";
        private const string RockTerrainLayerPath = TerrainFolderPath + "/TL_DaniTech_Rock.asset";
        private const string SkyboxMaterialPath = MaterialFolderPath + "/MAT_DaniTech_TwilightSky.mat";

        [MenuItem("EmeToDia/Rebuild Promotion Scene")]
        public static void RebuildPromotionScene()
        {
            EnsureFolders();
            ImportExternalAssets();
            CreateMaterials();
            CreateItemPrefabs();
            CreateUseEffectPrefab();
            CreateUiPrefab();
            RegisterAddressables();
            RebuildScene();
            RegisterSceneForPlay();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("DaniTechPromotionSceneBuilder: Eme_to_Dia_Promotion scene setup finished.");
        }

        [InitializeOnLoadMethod]
        private static void SetFastAddressablesPlayMode()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            if (settings == null)
            {
                return;
            }

            int fastModeIndex = GetDataBuilderIndex<BuildScriptFastMode>(settings);
            if (fastModeIndex < 0 || settings.ActivePlayModeDataBuilderIndex == fastModeIndex)
            {
                return;
            }

            settings.ActivePlayModeDataBuilderIndex = fastModeIndex;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "DaniTech");
            EnsureFolder("Assets/DaniTech", "Materials");
            EnsureFolder("Assets/DaniTech", "Terrain");
            EnsureFolder("Assets/DaniTech", "Prefabs");
            EnsureFolder(PrefabFolderPath, "Items");
            EnsureFolder(PrefabFolderPath, "UI");
            EnsureFolder("Assets", "ThirdParty");
            EnsureFolder("Assets/ThirdParty", "DaniTechImported");
            EnsureFolder(ExternalFolderPath, "KB3D_Brooklyn");
            EnsureFolder(ExternalFolderPath, "Asteroid_Crystal_Set_01");
            EnsureFolder(ExternalFolderPath, "Stylized_Mages");
            EnsureFolder(ExternalFolderPath, "TownConstructor2");
            EnsureFolder(ExternalFolderPath, "ForestHouseEnvironment");
        }

        private static void ImportExternalAssets()
        {
            CopyExternalFolder(BrooklynSourceRoot + "/Materials", BrooklynImportedFolderPath + "/Materials");
            CopyExternalAsset("KB3D_BRK_BldgLG_C.fbx");
            CopyExternalAsset("KB3D_BRK_BldgLG_A.fbx");
            CopyExternalAsset("KB3D_BRK_BldgLG_D.fbx");
            CopyExternalAsset("KB3D_BRK_BldgLG_E.fbx");
            CopyExternalAsset("KB3D_BRK_BldgMD_F.fbx");
            CopyExternalAsset("KB3D_BRK_BldgSM_F.fbx");
            CopyExternalAsset("KB3D_BRK_BldgSM_I.fbx");
            CopyExternalAsset("KB3D_BRK_WaterTower_A.fbx");
            CopyExternalAsset("KB3D_BRK_Lamp_A.fbx");
            CopyExternalAsset("KB3D_BRK_RooftopProp_A.fbx");
            CopyExternalAsset("KB3D_BRK_FireScape_A.fbx");
            ImportUnityPackageFolder(AsteroidCrystalPackageRoot, ExternalFolderPath);
            ImportUnityPackageAssets(MagePackageRoot, MageImportedFolderPath, GetMageImportPaths());
            ImportUnityPackageAssets(TownConstructorPackageRoot, TownImportedFolderPath, GetTownConstructorImportPaths());
            ImportUnityPackageAssets(ForestHousePackageRoot, ForestHouseImportedFolderPath, GetForestHouseImportPaths());
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(BrooklynImportedFolderPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset(AsteroidImportedFolderPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset(MageImportedFolderPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset(TownImportedFolderPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            AssetDatabase.ImportAsset(ForestHouseImportedFolderPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            ConfigureForestTextureImporters();
        }

        private static string[] GetMageImportPaths()
        {
            return new[]
            {
                "Assets/Models/Characters/Fantasy/Mages/MageBlue.prefab",
                "Assets/Models/Characters/Fantasy/Mages/Weapons/StaffBlue.prefab",
                "Assets/Models/Characters/Fantasy/Mages/Textures/MagesTexture.png",
                "Assets/Models/Characters/Fantasy/Mages/Textures/MagesTexture2.png",
                "Assets/Models/Characters/Fantasy/Mages/FBX/Mages.fbx",
                "Assets/Models/Characters/Fantasy/Mages/FBX/Materials/MagesTexture.mat",
                "Assets/Models/Characters/Fantasy/Mages/FBX/Materials/MagesTexture2.mat"
            };
        }

        private static string[] GetTownConstructorImportPaths()
        {
            return new[]
            {
                "Assets/19ThTownPack/Standard Assets/Prototyping/Models/FloorPrototype64x01x64.fbx",
                "Assets/19ThTownPack/ModularParts/Roads/Road.fbx",
                "Assets/19ThTownPack/ModularParts/Roads/SideWalk.fbx",
                "Assets/19ThTownPack/ModularParts/Roads/RoadEdge_01.fbx",
                "Assets/19ThTownPack/ModularParts/Roads/RoadEdge_02.fbx",
                "Assets/19ThTownPack/ModularParts/Roads/StoneChunks1.FBX",
                "Assets/19ThTownPack/ModularParts/Roads/StoneChunks3.FBX",
                "Assets/19ThTownPack/Props/StreetLight2.FBX"
            };
        }

        private static string[] GetForestHouseImportPaths()
        {
            List<string> assetPaths = new List<string>();
            CollectUnityPackagePaths(
                ForestHousePackageRoot,
                assetPaths,
                IsForestHouseTerrainAsset);
            CollectUnityPackagePaths(
                ForestHousePackageRoot,
                assetPaths,
                IsForestHouseRockAsset);
            CollectUnityPackagePaths(
                ForestHousePackageRoot,
                assetPaths,
                IsForestHouseTreeAsset);
            return assetPaths.ToArray();
        }

        private static void CollectUnityPackagePaths(string packageFolderPath, List<string> assetPaths, System.Predicate<string> selector)
        {
            if (Directory.Exists(packageFolderPath) == false)
            {
                return;
            }

            string[] packageEntryPaths = Directory.GetDirectories(packageFolderPath);
            for (int i = 0; i < packageEntryPaths.Length; i++)
            {
                string pathnamePath = Path.Combine(packageEntryPaths[i], "pathname");
                if (File.Exists(pathnamePath) == false)
                {
                    continue;
                }

                string[] packagePathLines = File.ReadAllLines(pathnamePath);
                if (packagePathLines.Length <= 0)
                {
                    continue;
                }

                string packageAssetPath = packagePathLines[0].Trim().Replace("\\", "/");
                if (selector(packageAssetPath) && assetPaths.Contains(packageAssetPath) == false)
                {
                    assetPaths.Add(packageAssetPath);
                }
            }
        }

        private static bool IsForestHouseTerrainAsset(string assetPath)
        {
            if (assetPath.StartsWith("Assets/Forest House Environment/Textures/Terrain Ground/Grass_1/"))
            {
                return true;
            }

            if (assetPath.StartsWith("Assets/Forest House Environment/Textures/Terrain Ground/Mud_2/"))
            {
                return true;
            }

            if (assetPath.StartsWith("Assets/Forest House Environment/Textures/Terrain Ground/Rocks_2/"))
            {
                return true;
            }

            if (assetPath == "Assets/Forest House Environment/Materials/Ground/FHE_Grass_1.mat")
            {
                return true;
            }

            if (assetPath == "Assets/Forest House Environment/Materials/Ground/FHE_Mud_2.mat")
            {
                return true;
            }

            if (assetPath == "Assets/Forest House Environment/Materials/Ground/FHE_Rocks_2.mat")
            {
                return true;
            }

            return assetPath == "Assets/Forest House Environment/Terrain Layers/FHE_Grass_1.terrainlayer"
                || assetPath == "Assets/Forest House Environment/Terrain Layers/FHE_Mud_2.terrainlayer"
                || assetPath == "Assets/Forest House Environment/Terrain Layers/FHE_Rocks_2.terrainlayer";
        }

        private static bool IsForestHouseRockAsset(string assetPath)
        {
            bool isRockPath = assetPath.Contains("/Materials/Rocks/")
                || assetPath.Contains("/Textures/Rocks/")
                || assetPath.Contains("/Models/")
                || assetPath.Contains("/Prefabs/");

            if (isRockPath == false)
            {
                return false;
            }

            return assetPath.Contains("FHE_Rock_1")
                || assetPath.Contains("FHE_Rock_2")
                || assetPath.Contains("FHE_Rock_3")
                || assetPath.Contains("FHE_Rock_4");
        }

        private static bool IsForestHouseTreeAsset(string assetPath)
        {
            bool isTreePath = assetPath.Contains("/Materials/Trees/")
                || assetPath.Contains("/Textures/Trees/")
                || assetPath.Contains("/Models/")
                || assetPath.Contains("/Prefabs/");

            if (isTreePath == false)
            {
                return false;
            }

            return assetPath.Contains("FHE_Tree_1")
                || assetPath.Contains("FHE_Tree_2")
                || assetPath.Contains("FHE_Bush_1")
                || assetPath.Contains("FHE_Bush_2")
                || assetPath.Contains("/Tree_1/")
                || assetPath.Contains("/Tree_2/")
                || assetPath.Contains("/Bush_1/")
                || assetPath.Contains("/Bush_2/");
        }

        private static void CopyExternalAsset(string fileName)
        {
            string sourcePath = Path.Combine(BrooklynSourceRoot, fileName);
            if (File.Exists(sourcePath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: external asset was not found. " + sourcePath);
                return;
            }

            string targetAssetPath = BrooklynImportedFolderPath + "/" + fileName;
            CopyProjectFile(sourcePath, targetAssetPath);
            CopyProjectFile(sourcePath + ".meta", targetAssetPath + ".meta");
            AssetDatabase.ImportAsset(targetAssetPath, ImportAssetOptions.ForceUpdate);
        }

        private static void CopyExternalFolder(string sourceFolderPath, string targetAssetFolderPath)
        {
            if (Directory.Exists(sourceFolderPath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: external folder was not found. " + sourceFolderPath);
                return;
            }

            string[] sourceFilePaths = Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < sourceFilePaths.Length; i++)
            {
                string relativePath = sourceFilePaths[i].Substring(sourceFolderPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string targetAssetPath = (targetAssetFolderPath + "/" + relativePath).Replace("\\", "/");
                CopyProjectFile(sourceFilePaths[i], targetAssetPath);
            }
        }

        private static void ImportUnityPackageFolder(string packageFolderPath, string targetRootPath)
        {
            if (Directory.Exists(packageFolderPath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: package folder was not found. " + packageFolderPath);
                return;
            }

            string[] packageEntryPaths = Directory.GetDirectories(packageFolderPath);
            for (int i = 0; i < packageEntryPaths.Length; i++)
            {
                ImportUnityPackageEntry(packageEntryPaths[i], targetRootPath);
            }
        }

        private static void ImportUnityPackageAssets(string packageFolderPath, string targetRootPath, string[] packageAssetPaths)
        {
            if (Directory.Exists(packageFolderPath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: package folder was not found. " + packageFolderPath);
                return;
            }

            HashSet<string> packageAssetPathSet = new HashSet<string>(packageAssetPaths);
            string[] packageEntryPaths = Directory.GetDirectories(packageFolderPath);
            for (int i = 0; i < packageEntryPaths.Length; i++)
            {
                string pathnamePath = Path.Combine(packageEntryPaths[i], "pathname");
                if (File.Exists(pathnamePath) == false)
                {
                    continue;
                }

                string[] packagePathLines = File.ReadAllLines(pathnamePath);
                if (packagePathLines.Length <= 0)
                {
                    continue;
                }

                string packageAssetPath = packagePathLines[0].Trim().Replace("\\", "/");
                if (packageAssetPathSet.Contains(packageAssetPath))
                {
                    ImportUnityPackageEntry(packageEntryPaths[i], targetRootPath);
                }
            }
        }

        private static void ImportUnityPackageEntry(string packageEntryPath, string targetRootPath)
        {
            string pathnamePath = Path.Combine(packageEntryPath, "pathname");
            if (File.Exists(pathnamePath) == false)
            {
                return;
            }

            string[] packagePathLines = File.ReadAllLines(pathnamePath);
            if (packagePathLines.Length <= 0)
            {
                return;
            }

            string packageAssetPath = packagePathLines[0].Trim().Replace("\\", "/");
            if (packageAssetPath.StartsWith("Assets/") == false)
            {
                return;
            }

            string relativeAssetPath = packageAssetPath.Substring("Assets/".Length);
            string targetAssetPath = targetRootPath + "/" + relativeAssetPath;
            string sourceAssetPath = Path.Combine(packageEntryPath, "asset");
            string sourceMetaPath = Path.Combine(packageEntryPath, "metaData");

            if (File.Exists(sourceAssetPath))
            {
                CopyProjectFile(sourceAssetPath, targetAssetPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetFullPath(targetAssetPath));
            }

            CopyUnityPackageMetaIfValid(sourceMetaPath, targetAssetPath + ".meta");
        }

        private static void CopyUnityPackageMetaIfValid(string sourceMetaPath, string targetMetaAssetPath)
        {
            if (File.Exists(sourceMetaPath) == false)
            {
                return;
            }

            if (IsUnityMetaFile(sourceMetaPath))
            {
                CopyProjectFile(sourceMetaPath, targetMetaAssetPath);
                return;
            }

            string targetMetaFullPath = Path.GetFullPath(targetMetaAssetPath);
            if (File.Exists(targetMetaFullPath))
            {
                File.Delete(targetMetaFullPath);
            }
        }

        private static bool IsUnityMetaFile(string sourceMetaPath)
        {
            byte[] headerBuffer = new byte[32];
            using (FileStream stream = File.OpenRead(sourceMetaPath))
            {
                int readLength = stream.Read(headerBuffer, 0, headerBuffer.Length);
                if (readLength <= 0)
                {
                    return false;
                }

                string headerText = System.Text.Encoding.UTF8.GetString(headerBuffer, 0, readLength);
                return headerText.StartsWith("fileFormatVersion:");
            }
        }

        private static void CopyProjectFile(string sourcePath, string targetAssetPath)
        {
            if (File.Exists(sourcePath) == false)
            {
                return;
            }

            string targetFullPath = Path.GetFullPath(targetAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));
            File.Copy(sourcePath, targetFullPath, true);
        }

        private static void ConfigureForestTextureImporters()
        {
            ConfigureTextureImporter(ForestGrassAlbedoPath, TextureImporterType.Default, true);
            ConfigureTextureImporter(ForestGrassNormalPath, TextureImporterType.NormalMap, false);
            ConfigureTextureImporter(ForestMudAlbedoPath, TextureImporterType.Default, true);
            ConfigureTextureImporter(ForestMudNormalPath, TextureImporterType.NormalMap, false);
            ConfigureTextureImporter(ForestRockAlbedoPath, TextureImporterType.Default, true);
            ConfigureTextureImporter(ForestRockNormalPath, TextureImporterType.NormalMap, false);
            AssetDatabase.Refresh();
        }

        private static void ConfigureTextureImporter(string texturePath, TextureImporterType textureType, bool isReadable)
        {
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = textureType;
            importer.isReadable = isReadable;
            importer.mipmapEnabled = true;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.SaveAndReimport();
        }

        private static void CreateMaterials()
        {
            GetOrCreateMaterial("MAT_DaniTech_Ground", new Color(0.09f, 0.13f, 0.16f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Path", new Color(0.16f, 0.18f, 0.18f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Emerald", new Color(0.04f, 0.82f, 0.46f, 1f), 0.35f);
            GetOrCreateMaterial("MAT_DaniTech_Diamond", new Color(0.58f, 0.92f, 1f, 1f), 0.55f);
            GetOrCreateMaterial("MAT_DaniTech_Shield", new Color(0.28f, 0.48f, 1f, 1f), 0.45f);
            GetOrCreateMaterial("MAT_DaniTech_Swift", new Color(1f, 0.62f, 0.18f, 1f), 0.25f);
            GetOrCreateMaterial("MAT_DaniTech_DarkMetal", new Color(0.08f, 0.09f, 0.11f, 1f), 0.05f);
            GetOrCreateMaterial("MAT_DaniTech_Light", new Color(1f, 0.84f, 0.36f, 1f), 0.7f);
            GetOrCreateMaterial("MAT_DaniTech_Asphalt", new Color(0.055f, 0.06f, 0.065f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Concrete", new Color(0.31f, 0.33f, 0.32f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Brick", new Color(0.30f, 0.17f, 0.13f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Window", new Color(0.08f, 0.20f, 0.25f, 1f), 0.12f);
            GetOrCreateMaterial("MAT_DaniTech_Bronze", new Color(0.42f, 0.27f, 0.13f, 1f), 0.03f);
            GetOrCreateMaterial("MAT_DaniTech_CrystalCyan", new Color(0.15f, 0.72f, 0.88f, 1f), 0.32f);
            GetOrCreateMaterial("MAT_DaniTech_CrystalViolet", new Color(0.42f, 0.26f, 0.72f, 1f), 0.26f);
            GetOrCreateMaterial("MAT_DaniTech_HeroBlue", new Color(0.08f, 0.26f, 0.55f, 1f), 0.05f);
            GetOrCreateMaterial("MAT_DaniTech_HeroGold", new Color(0.92f, 0.66f, 0.20f, 1f), 0.08f);
            GetOrCreateMaterial("MAT_DaniTech_HeroSkin", new Color(0.82f, 0.58f, 0.42f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_Leather", new Color(0.28f, 0.16f, 0.08f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_LeatherDark", new Color(0.15f, 0.08f, 0.04f, 1f), 0f);
            GetOrCreateTexturedMaterial("MAT_DaniTech_ForestGrass", ForestGrassAlbedoPath, ForestGrassNormalPath, new Color(0.62f, 0.78f, 0.48f, 1f), 0.22f);
            GetOrCreateTexturedMaterial("MAT_DaniTech_ForestMud", ForestMudAlbedoPath, ForestMudNormalPath, new Color(0.42f, 0.29f, 0.18f, 1f), 0.12f);
            GetOrCreateTexturedMaterial("MAT_DaniTech_ForestRock", ForestRockAlbedoPath, ForestRockNormalPath, new Color(0.48f, 0.52f, 0.56f, 1f), 0.2f);
            GetOrCreateMaterial("MAT_DaniTech_ForestFoliage", new Color(0.20f, 0.43f, 0.25f, 1f), 0f);
            GetOrCreateMaterial("MAT_DaniTech_ForestBark", new Color(0.27f, 0.17f, 0.10f, 1f), 0f);
        }

        private static void CreateItemPrefabs()
        {
            CreateCrystalItemPrefab(EmeraldItemPrefabPath, "emerald_core", "에메랄드 코어", "MAT_DaniTech_Emerald", CrystalLargeAssetPath, AsteroidSmallAAssetPath);
            CreateCrystalItemPrefab(DiamondItemPrefabPath, "diamond_tonic", "다이아 회복제", "MAT_DaniTech_Diamond", CrystalMediumAssetPath, AsteroidSmallBAssetPath);
            CreateCrystalItemPrefab(ShieldItemPrefabPath, "prism_shield", "프리즘 보호막", "MAT_DaniTech_Shield", CrystalSmallBAssetPath, AsteroidSmallAAssetPath);
            CreateCrystalItemPrefab(SwiftItemPrefabPath, "swift_sigil", "신속의 인장", "MAT_DaniTech_Swift", CrystalSmallCAssetPath, AsteroidSmallBAssetPath);
        }

        private static void CreateCrystalItemPrefab(
            string prefabPath,
            string itemId,
            string label,
            string materialName,
            string crystalAssetPath,
            string baseAssetPath)
        {
            GameObject rootObject = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            rootObject.transform.localScale = Vector3.one;

            GameObject visualRoot = CreateEmpty("VisualRoot", rootObject.transform, Vector3.zero);
            Material itemMaterial = LoadMaterial(materialName);
            bool hasCrystalAsset = CreateImportedAssetVisual(
                crystalAssetPath,
                "Visual_ImportedCrystal",
                visualRoot.transform,
                Vector3.zero,
                Quaternion.identity,
                1.35f,
                itemMaterial);

            bool hasBaseAsset = CreateImportedAssetVisual(
                baseAssetPath,
                "Visual_ImportedStoneBase",
                visualRoot.transform,
                new Vector3(0f, -0.58f, 0f),
                Quaternion.identity,
                0.34f,
                LoadMaterial("MAT_DaniTech_DarkMetal"));

            if (hasCrystalAsset == false)
            {
                CreateDiamond("Fallback_Gem_Core", visualRoot.transform, Vector3.zero, new Vector3(0.65f, 0.92f, 0.65f), itemMaterial);
            }

            if (hasBaseAsset == false)
            {
                CreatePrimitive(PrimitiveType.Cylinder, "Fallback_Gem_Base", visualRoot.transform, new Vector3(0f, -0.55f, 0f), new Vector3(0.72f, 0.12f, 0.72f), LoadMaterial("MAT_DaniTech_DarkMetal"));
            }

            GameObject lightObject = CreateEmpty("Light_ItemGlow", rootObject.transform, new Vector3(0f, 0.35f, 0f));
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = itemMaterial.color;
            light.range = 3.5f;
            light.intensity = 1.2f;

            TextMesh labelText = CreateWorldText(rootObject.transform, "Text_ItemLabel", label, new Vector3(0f, 1.45f, 0f), 42);

            SphereCollider collider = rootObject.AddComponent<SphereCollider>();
            collider.radius = 1.05f;
            collider.center = new Vector3(0f, 0.25f, 0f);

            DaniTechFloatingItemView itemView = rootObject.AddComponent<DaniTechFloatingItemView>();
            DaniTechItemPickup itemPickup = rootObject.AddComponent<DaniTechItemPickup>();
            SetObjectReference(itemView, "_visualRoot", visualRoot.transform);
            SetObjectReference(itemView, "_labelText", labelText);
            SetString(itemPickup, "_itemId", itemId);
            SetInt(itemPickup, "_amount", 1);
            SetObjectReference(itemPickup, "_itemView", itemView);

            SavePrefab(rootObject, prefabPath);
        }

        private static void CreateUseEffectPrefab()
        {
            GameObject effectObject = new GameObject("PF_DaniTech_UseEffect");
            ParticleSystem particleSystem = effectObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particleSystem.main;
            main.duration = 1.4f;
            main.startLifetime = 0.75f;
            main.startSpeed = 1.7f;
            main.startSize = 0.16f;
            main.startColor = new Color(0.54f, 0.93f, 1f, 0.9f);
            main.maxParticles = 80;

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 38) });

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.45f;

            SavePrefab(effectObject, UseEffectPrefabPath);
        }

        private static void CreateUiPrefab()
        {
            GameObject canvasObject = new GameObject("PF_DaniTechPromotionUI");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();

            RectTransform hudRoot = CreatePanel(canvasObject.transform, "Panel_PlayerHUD", new Color(0.03f, 0.035f, 0.04f, 0.88f));
            SetRect(hudRoot, new Vector2(0f, 1f), new Vector2(680f, 440f), new Vector2(22f, -22f));
            DaniTechPlayerHUD playerHUD = hudRoot.gameObject.AddComponent<DaniTechPlayerHUD>();
            Text titleText = CreateText(hudRoot, "Text_HudTitle", "PLAYER STATUS", 26, TextAnchor.MiddleLeft);
            SetRect(titleText.rectTransform, new Vector2(0f, 1f), new Vector2(360f, 34f), new Vector2(24f, -18f));
            Text healthText = CreateText(hudRoot, "Text_HP", "HP 62 / 100", 25, TextAnchor.MiddleLeft);
            SetRect(healthText.rectTransform, new Vector2(0f, 1f), new Vector2(160f, 32f), new Vector2(24f, -64f));
            Text staminaText = CreateText(hudRoot, "Text_Stamina", "STM 70 / 100", 25, TextAnchor.MiddleLeft);
            SetRect(staminaText.rectTransform, new Vector2(0f, 1f), new Vector2(160f, 32f), new Vector2(24f, -106f));
            Text shieldText = CreateText(hudRoot, "Text_Shield", "SHD 0 / 60", 25, TextAnchor.MiddleLeft);
            SetRect(shieldText.rectTransform, new Vector2(0f, 1f), new Vector2(160f, 32f), new Vector2(24f, -148f));
            Image healthFill = CreateBar(hudRoot, "Bar_HP", new Vector2(200f, -70f), new Vector2(400f, 22f), new Color(0.90f, 0.12f, 0.14f, 1f));
            Image staminaFill = CreateBar(hudRoot, "Bar_Stamina", new Vector2(200f, -112f), new Vector2(400f, 22f), new Color(0.16f, 0.72f, 0.30f, 1f));
            Image shieldFill = CreateBar(hudRoot, "Bar_Shield", new Vector2(200f, -154f), new Vector2(400f, 22f), new Color(0.14f, 0.42f, 0.86f, 1f));
            healthFill.fillAmount = 0.62f;
            staminaFill.fillAmount = 0.7f;
            shieldFill.fillAmount = 0f;
            Text statusText = CreateText(hudRoot, "Text_Status", "Selected: None\nSpeed x1.00 (0.0s)", 23, TextAnchor.UpperLeft);
            SetRect(statusText.rectTransform, new Vector2(0f, 1f), new Vector2(620f, 58f), new Vector2(24f, -190f));
            Text tooltipText = CreateText(hudRoot, "Text_Tooltip", "Tip: Aim at an item and press E to pick it up.", 21, TextAnchor.UpperLeft);
            SetRect(tooltipText.rectTransform, new Vector2(0f, 1f), new Vector2(620f, 38f), new Vector2(24f, -254f));
            Text inventorySummaryText = CreateText(hudRoot, "Text_InventorySummary", "BAG  Tab/I\nEmpty\nPick up items with E", 22, TextAnchor.UpperLeft);
            SetRect(inventorySummaryText.rectTransform, new Vector2(0f, 1f), new Vector2(620f, 112f), new Vector2(24f, -302f));
            Text guideText = CreateText(hudRoot, "Text_Guide", "Right drag: look  /  Q,C: turn  /  Wheel: zoom  /  Tab/I: bag", 19, TextAnchor.LowerLeft);
            SetRect(guideText.rectTransform, new Vector2(0f, 0f), new Vector2(590f, 34f), new Vector2(24f, 16f));
            SetObjectReference(playerHUD, "_statusText", statusText);
            SetObjectReference(playerHUD, "_guideText", guideText);
            SetObjectReference(playerHUD, "_healthText", healthText);
            SetObjectReference(playerHUD, "_staminaText", staminaText);
            SetObjectReference(playerHUD, "_shieldText", shieldText);
            SetObjectReference(playerHUD, "_tooltipText", tooltipText);
            SetObjectReference(playerHUD, "_inventorySummaryText", inventorySummaryText);
            SetObjectReference(playerHUD, "_healthFillImage", healthFill);
            SetObjectReference(playerHUD, "_staminaFillImage", staminaFill);
            SetObjectReference(playerHUD, "_shieldFillImage", shieldFill);

            RectTransform logRoot = CreatePanel(canvasObject.transform, "Panel_FeedbackLog", new Color(0.04f, 0.04f, 0.05f, 0.75f));
            SetRect(logRoot, new Vector2(0f, 0f), new Vector2(840f, 190f), new Vector2(22f, 22f));
            DaniTechFeedbackLogView feedbackLogView = logRoot.gameObject.AddComponent<DaniTechFeedbackLogView>();
            Text logText = CreateText(logRoot, "Text_Log", "", 20, TextAnchor.LowerLeft);
            SetStretch(logText.rectTransform, new Vector2(22f, 18f), new Vector2(-22f, -18f));
            SetObjectReference(feedbackLogView, "_logText", logText);

            RectTransform inventoryRoot = CreatePanel(canvasObject.transform, "Panel_InventoryRoot", new Color(0.18f, 0.10f, 0.055f, 0.97f));
            SetRect(inventoryRoot, new Vector2(0.5f, 0.5f), new Vector2(1320f, 820f), Vector2.zero);
            DaniTechInventoryUI inventoryUI = inventoryRoot.gameObject.AddComponent<DaniTechInventoryUI>();
            BuildInventoryPanel(inventoryRoot, inventoryUI);

            SetObjectReference(inventoryUI, "_rootObject", inventoryRoot.gameObject);
            inventoryRoot.gameObject.SetActive(false);
            SavePrefab(canvasObject, UiPrefabPath);
        }

        private static void BuildInventoryPanel(RectTransform inventoryRoot, DaniTechInventoryUI inventoryUI)
        {
            Text titleText = CreateText(inventoryRoot, "Text_Title", "Field Bag Inventory", 40, TextAnchor.MiddleCenter);
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(1180f, 58f), new Vector2(0f, -44f));

            Text messageText = CreateText(inventoryRoot, "Text_Message", "", 23, TextAnchor.MiddleCenter);
            SetRect(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(1140f, 44f), new Vector2(0f, 38f));

            RectTransform slotPanel = CreatePanel(inventoryRoot, "Panel_Slots", new Color(0.24f, 0.15f, 0.08f, 0.94f));
            SetRect(slotPanel, new Vector2(0f, 0.5f), new Vector2(760f, 560f), new Vector2(50f, -16f));
            GridLayoutGroup grid = slotPanel.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(170f, 130f);
            grid.spacing = new Vector2(16f, 16f);
            grid.padding = new RectOffset(24, 24, 24, 24);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;

            DaniTechInventorySlotView[] slotViews = new DaniTechInventorySlotView[DaniTechInventoryModel.InventorySlotCount];
            for (int i = 0; i < slotViews.Length; i++)
            {
                slotViews[i] = CreateSlot(slotPanel, i);
            }

            RectTransform detailPanel = CreatePanel(inventoryRoot, "Panel_Detail", new Color(0.22f, 0.13f, 0.07f, 0.96f));
            SetRect(detailPanel, new Vector2(1f, 0.55f), new Vector2(440f, 500f), new Vector2(-50f, -18f));
            Text selectedText = CreateText(detailPanel, "Text_Selected", "", 28, TextAnchor.UpperLeft);
            SetRect(selectedText.rectTransform, new Vector2(0f, 1f), new Vector2(392f, 84f), new Vector2(24f, -24f));
            Text detailText = CreateText(detailPanel, "Text_Detail", "", 22, TextAnchor.UpperLeft);
            SetRect(detailText.rectTransform, new Vector2(0f, 1f), new Vector2(392f, 270f), new Vector2(24f, -124f));

            Button useButton = CreateButton(detailPanel, "Button_Use", "Use", new Vector2(24f, 48f), new Vector2(116f, 54f), new Color(0.20f, 0.43f, 0.25f, 1f));
            Button dropButton = CreateButton(detailPanel, "Button_Drop", "Drop", new Vector2(158f, 48f), new Vector2(116f, 54f), new Color(0.48f, 0.20f, 0.12f, 1f));
            Button closeButton = CreateButton(detailPanel, "Button_Close", "Close", new Vector2(292f, 48f), new Vector2(116f, 54f), new Color(0.31f, 0.21f, 0.13f, 1f));

            RectTransform sortPanel = CreatePanel(inventoryRoot, "Panel_Sort", new Color(0.13f, 0.075f, 0.04f, 0.9f));
            SetRect(sortPanel, new Vector2(0.5f, 0f), new Vector2(790f, 84f), new Vector2(-188f, 122f));
            Button sortNameButton = CreateButton(sortPanel, "Button_SortName", "Name", new Vector2(20f, 15f), new Vector2(130f, 54f), new Color(0.35f, 0.23f, 0.13f, 1f));
            Button sortTypeButton = CreateButton(sortPanel, "Button_SortType", "Type", new Vector2(168f, 15f), new Vector2(130f, 54f), new Color(0.35f, 0.23f, 0.13f, 1f));
            Button sortSequenceButton = CreateButton(sortPanel, "Button_SortSequence", "Recent", new Vector2(316f, 15f), new Vector2(146f, 54f), new Color(0.35f, 0.23f, 0.13f, 1f));

            RectTransform dropTargetRoot = CreatePanel(sortPanel, "Panel_DropTarget", new Color(0.42f, 0.16f, 0.10f, 0.92f));
            SetRect(dropTargetRoot, new Vector2(1f, 0.5f), new Vector2(230f, 54f), new Vector2(-20f, 0f));
            Text dropTargetText = CreateText(dropTargetRoot, "Text_DropTarget", "Drag Here To Drop", 20, TextAnchor.MiddleCenter);
            SetStretch(dropTargetText.rectTransform, Vector2.zero, Vector2.zero);
            DaniTechInventoryDropTarget dropTarget = dropTargetRoot.gameObject.AddComponent<DaniTechInventoryDropTarget>();
            SetObjectReference(dropTarget, "_backgroundImage", dropTargetRoot.GetComponent<Image>());
            SetObjectReference(dropTarget, "_labelText", dropTargetText);

            SetObjectReference(inventoryUI, "_titleText", titleText);
            SetObjectReference(inventoryUI, "_messageText", messageText);
            SetObjectReference(inventoryUI, "_selectedItemText", selectedText);
            SetObjectReference(inventoryUI, "_detailText", detailText);
            SetObjectReference(inventoryUI, "_useButton", useButton);
            SetObjectReference(inventoryUI, "_dropButton", dropButton);
            SetObjectReference(inventoryUI, "_sortNameButton", sortNameButton);
            SetObjectReference(inventoryUI, "_sortTypeButton", sortTypeButton);
            SetObjectReference(inventoryUI, "_sortSequenceButton", sortSequenceButton);
            SetObjectReference(inventoryUI, "_closeButton", closeButton);
            SetObjectReference(inventoryUI, "_dropTarget", dropTarget);
            SetObjectArray(inventoryUI, "_slotViews", slotViews);
        }

        private static DaniTechInventorySlotView CreateSlot(RectTransform parent, int slotIndex)
        {
            RectTransform slotRoot = CreatePanel(parent, "Slot_Item_" + slotIndex.ToString("00"), new Color(0.16f, 0.095f, 0.055f, 0.86f));
            Button button = slotRoot.gameObject.AddComponent<Button>();
            Text iconText = CreateText(slotRoot, "Text_Icon", "", 32, TextAnchor.MiddleCenter);
            SetRect(iconText.rectTransform, new Vector2(0.5f, 1f), new Vector2(136f, 38f), new Vector2(0f, -18f));
            Text nameText = CreateText(slotRoot, "Text_Name", "", 18, TextAnchor.MiddleCenter);
            SetRect(nameText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(148f, 46f), new Vector2(0f, -2f));
            Text amountText = CreateText(slotRoot, "Text_Amount", "", 18, TextAnchor.LowerRight);
            SetRect(amountText.rectTransform, new Vector2(1f, 0f), new Vector2(72f, 28f), new Vector2(-10f, 10f));
            Text cooldownText = CreateText(slotRoot, "Text_Cooldown", "", 17, TextAnchor.LowerLeft);
            SetRect(cooldownText.rectTransform, new Vector2(0f, 0f), new Vector2(86f, 28f), new Vector2(10f, 10f));

            DaniTechInventorySlotView slotView = slotRoot.gameObject.AddComponent<DaniTechInventorySlotView>();
            SetObjectReference(slotView, "_button", button);
            SetObjectReference(slotView, "_backgroundImage", slotRoot.GetComponent<Image>());
            SetObjectReference(slotView, "_iconText", iconText);
            SetObjectReference(slotView, "_nameText", nameText);
            SetObjectReference(slotView, "_amountText", amountText);
            SetObjectReference(slotView, "_cooldownText", cooldownText);
            return slotView;
        }

        private static void RegisterAddressables()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            SetFastAddressablesPlayMode();
            AddressableAssetGroup group = settings.FindGroup(AddressableGroupName);
            if (group == null)
            {
                group = settings.CreateGroup(
                    AddressableGroupName,
                    false,
                    false,
                    true,
                    null,
                    typeof(BundledAssetGroupSchema),
                    typeof(ContentUpdateGroupSchema));
            }

            ClearAddressableGroup(settings, group);
            RegisterAddressable(settings, group, EmeraldItemPrefabPath, "DaniTech/Items/EmeraldCore");
            RegisterAddressable(settings, group, DiamondItemPrefabPath, "DaniTech/Items/DiamondTonic");
            RegisterAddressable(settings, group, ShieldItemPrefabPath, "DaniTech/Items/PrismShield");
            RegisterAddressable(settings, group, SwiftItemPrefabPath, "DaniTech/Items/SwiftSigil");
            RegisterAddressable(settings, group, UiPrefabPath, "DaniTech/UI/PromotionUI");
            RegisterAddressable(settings, group, UseEffectPrefabPath, "DaniTech/VFX/UseEffect");
            RegisterAddressableFolderAssets(settings, group, BrooklynImportedFolderPath, "DaniTech/Imported/KB3D_Brooklyn");
            RegisterAddressableFolderAssets(settings, group, AsteroidImportedFolderPath, "DaniTech/Imported/Asteroid_Crystal_Set_01");
            RegisterAddressableFolderAssets(settings, group, MageImportedFolderPath, "DaniTech/Imported/Stylized_Mages");
            RegisterAddressableFolderAssets(settings, group, TownImportedFolderPath, "DaniTech/Imported/TownConstructor2");
            RegisterAddressableFolderAssets(settings, group, ForestHouseImportedFolderPath, "DaniTech/Imported/ForestHouseEnvironment");
            RegisterAddressableFolderAssets(settings, group, TerrainFolderPath, "DaniTech/Terrain");
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        }

        private static void ClearAddressableGroup(AddressableAssetSettings settings, AddressableAssetGroup group)
        {
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>(group.entries);
            for (int i = 0; i < entries.Count; i++)
            {
                settings.RemoveAssetEntry(entries[i].guid);
            }
        }

        private static int GetDataBuilderIndex<T>(AddressableAssetSettings settings) where T : ScriptableObject
        {
            if (settings == null)
            {
                return -1;
            }

            for (int i = 0; i < settings.DataBuilders.Count; i++)
            {
                if (settings.DataBuilders[i] is T)
                {
                    return i;
                }
            }

            return -1;
        }

        private static void RegisterSceneForPlay()
        {
            EditorBuildSettingsScene scene = new EditorBuildSettingsScene(ScenePath, true);
            string sceneGuid = AssetDatabase.AssetPathToGUID(ScenePath);
            if (string.IsNullOrEmpty(sceneGuid) == false)
            {
                scene.guid = new GUID(sceneGuid);
            }

            EditorBuildSettings.scenes = new[] { scene };
        }

        private static void RebuildScene()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            ClearScene(scene);
            CreateLighting(scene);
            CreateEnvironment(scene);
            GameObject playerObject = CreatePlayer(scene);
            GameObject managerGroup = CreateManagers(scene, playerObject);
            CreateSpawnPoints(managerGroup.transform);
            CreateUi(scene, managerGroup);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void CreateLighting(Scene scene)
        {
            GameObject lightRoot = CreateRoot(scene, "Lighting");
            GameObject sunObject = CreateEmpty("DirectionalLight_Sun", lightRoot.transform, new Vector3(0f, 8f, 0f));
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(0.82f, 0.90f, 1f, 1f);
            sun.intensity = 1.05f;
            sunObject.transform.rotation = Quaternion.Euler(36f, -38f, 0f);

            CreatePointLight(lightRoot.transform, "PointLight_HeroGate", new Vector3(0f, 12f, 22f), new Color(0.48f, 0.82f, 1f, 1f), 1.45f, 46f);
            CreatePointLight(lightRoot.transform, "PointLight_WarmFill", new Vector3(-24f, 10f, -18f), new Color(1f, 0.66f, 0.42f, 1f), 0.55f, 62f);
            RenderSettings.skybox = GetOrCreateSkyboxMaterial();
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.73f, 0.92f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.54f, 0.63f, 0.76f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.30f, 0.37f, 0.43f, 1f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.62f, 0.75f, 0.88f, 1f);
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 85f;
            RenderSettings.fogEndDistance = 360f;
        }

        private static void CreateEnvironment(Scene scene)
        {
            GameObject environmentRoot = CreateRoot(scene, "Environment_HeroPlaza");
            Material groundMaterial = LoadMaterial("MAT_DaniTech_Asphalt");
            Material pathMaterial = LoadMaterial("MAT_DaniTech_Concrete");
            Material diamondMaterial = LoadMaterial("MAT_DaniTech_CrystalCyan");
            Material lightMaterial = LoadMaterial("MAT_DaniTech_Light");

            CreateTerrainGround(environmentRoot.transform);
            CreatePrimitive(PrimitiveType.Cube, "Ground_HeroPath_North", environmentRoot.transform, new Vector3(0f, 0.02f, 40f), new Vector3(18f, 0.08f, 112f), pathMaterial);
            CreatePrimitive(PrimitiveType.Cube, "Ground_HeroPath_East", environmentRoot.transform, new Vector3(40f, 0.03f, 0f), new Vector3(112f, 0.08f, 18f), pathMaterial);
            CreatePrimitive(PrimitiveType.Cube, "Ground_HeroPath_West", environmentRoot.transform, new Vector3(-40f, 0.03f, 0f), new Vector3(112f, 0.08f, 18f), pathMaterial);

            CreateTownFloorTiles(environmentRoot.transform, groundMaterial);
            CreateTownRoadNetwork(environmentRoot.transform, pathMaterial, LoadMaterial("MAT_DaniTech_Bronze"));

            CreateExternalModel("KB3D_BRK_BldgLG_C.fbx", environmentRoot.transform, new Vector3(-82f, 0f, 48f), Quaternion.Euler(0f, 28f, 0f), new Vector3(0.28f, 0.28f, 0.28f));
            CreateExternalModel("KB3D_BRK_BldgLG_E.fbx", environmentRoot.transform, new Vector3(86f, 0f, 52f), Quaternion.Euler(0f, -34f, 0f), new Vector3(0.31f, 0.31f, 0.31f));
            CreateExternalModel("KB3D_BRK_BldgLG_A.fbx", environmentRoot.transform, new Vector3(-88f, 0f, -62f), Quaternion.Euler(0f, 150f, 0f), new Vector3(0.26f, 0.26f, 0.26f));
            CreateExternalModel("KB3D_BRK_BldgLG_D.fbx", environmentRoot.transform, new Vector3(84f, 0f, -66f), Quaternion.Euler(0f, -146f, 0f), new Vector3(0.27f, 0.27f, 0.27f));
            CreateExternalModel("KB3D_BRK_BldgMD_F.fbx", environmentRoot.transform, new Vector3(0f, 0f, 98f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.26f, 0.26f, 0.26f));
            CreateExternalModel("KB3D_BRK_BldgSM_F.fbx", environmentRoot.transform, new Vector3(-112f, 0f, -8f), Quaternion.Euler(0f, 82f, 0f), new Vector3(0.25f, 0.25f, 0.25f));
            CreateExternalModel("KB3D_BRK_BldgSM_I.fbx", environmentRoot.transform, new Vector3(112f, 0f, -6f), Quaternion.Euler(0f, -88f, 0f), new Vector3(0.25f, 0.25f, 0.25f));
            CreateExternalModel("KB3D_BRK_WaterTower_A.fbx", environmentRoot.transform, new Vector3(-48f, 0f, -82f), Quaternion.Euler(0f, 12f, 0f), new Vector3(0.34f, 0.34f, 0.34f));
            CreateExternalModel("KB3D_BRK_RooftopProp_A.fbx", environmentRoot.transform, new Vector3(50f, 0.1f, -84f), Quaternion.identity, new Vector3(0.48f, 0.48f, 0.48f));
            CreateExternalModel("KB3D_BRK_FireScape_A.fbx", environmentRoot.transform, new Vector3(-54f, 0.2f, 18f), Quaternion.Euler(0f, 78f, 0f), new Vector3(0.38f, 0.38f, 0.38f));
            CreateForestBackdrop(environmentRoot.transform);

            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f;
                Vector3 position = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, 24f);
                bool hasGemAsset = CreateImportedAssetVisual(
                    i % 2 == 0 ? CrystalLargeAssetPath : CrystalMediumAssetPath,
                    "Asset_Gem_Pillar_" + i.ToString("00"),
                    environmentRoot.transform,
                    position + Vector3.up * 4.8f,
                    Quaternion.Euler(0f, angle, 0f),
                    7.2f,
                    i % 2 == 0 ? LoadMaterial("MAT_DaniTech_CrystalCyan") : LoadMaterial("MAT_DaniTech_CrystalViolet"));
                bool hasBaseAsset = CreateImportedAssetVisual(
                    i % 2 == 0 ? AsteroidSmallAAssetPath : AsteroidSmallBAssetPath,
                    "Asset_Pillar_Base_" + i.ToString("00"),
                    environmentRoot.transform,
                    position + Vector3.up * 0.45f,
                    Quaternion.Euler(0f, angle, 0f),
                    2.4f,
                    LoadMaterial("MAT_DaniTech_DarkMetal"));

                if (hasGemAsset == false)
                {
                    CreateDiamond("Fallback_Gem_Pillar_" + i.ToString("00"), environmentRoot.transform, position + Vector3.up * 4.8f, new Vector3(3f, 5.4f, 3f), diamondMaterial);
                }

                if (hasBaseAsset == false)
                {
                    CreatePrimitive(PrimitiveType.Cylinder, "Fallback_Pillar_Base_" + i.ToString("00"), environmentRoot.transform, position + Vector3.up * 1.1f, new Vector3(3f, 2.2f, 3f), LoadMaterial("MAT_DaniTech_DarkMetal"));
                }

                CreatePointLight(environmentRoot.transform, "PointLight_Gem_" + i.ToString("00"), position + Vector3.up * 8f, lightMaterial.color, 2.1f, 18f);
            }
        }

        private static void CreateTerrainGround(Transform environmentRoot)
        {
            TerrainData terrainData = CreateHeroTerrainData();
            GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
            terrainObject.name = "Terrain_HeroValley_260";
            terrainObject.transform.SetParent(environmentRoot);
            terrainObject.transform.localPosition = new Vector3(-130f, -0.65f, -130f);
            terrainObject.transform.localRotation = Quaternion.identity;
            terrainObject.transform.localScale = Vector3.one;
            terrainObject.isStatic = true;

            Terrain terrain = terrainObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                terrain.materialTemplate = null;
                terrain.heightmapPixelError = 3f;
                terrain.basemapDistance = 1200f;
                terrain.treeDistance = 420f;
            }
        }

        private static TerrainData CreateHeroTerrainData()
        {
            TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(TerrainDataPath);
            if (terrainData == null)
            {
                terrainData = new TerrainData();
                AssetDatabase.CreateAsset(terrainData, TerrainDataPath);
            }

            terrainData.heightmapResolution = 257;
            terrainData.alphamapResolution = 192;
            terrainData.size = new Vector3(260f, 12f, 260f);
            terrainData.terrainLayers = CreateHeroTerrainLayers();
            terrainData.SetHeights(0, 0, CreateHeroTerrainHeights(terrainData.heightmapResolution));
            terrainData.SetAlphamaps(0, 0, CreateHeroTerrainAlphamaps(terrainData.alphamapResolution));
            EditorUtility.SetDirty(terrainData);
            return terrainData;
        }

        private static TerrainLayer[] CreateHeroTerrainLayers()
        {
            TerrainLayer grassLayer = GetOrCreateTerrainLayer(
                GrassTerrainLayerPath,
                ForestGrassAlbedoPath,
                ForestGrassNormalPath,
                new Vector2(9f, 9f),
                0f,
                0.24f);
            TerrainLayer mudLayer = GetOrCreateTerrainLayer(
                MudTerrainLayerPath,
                ForestMudAlbedoPath,
                ForestMudNormalPath,
                new Vector2(10f, 10f),
                0f,
                0.16f);
            TerrainLayer rockLayer = GetOrCreateTerrainLayer(
                RockTerrainLayerPath,
                ForestRockAlbedoPath,
                ForestRockNormalPath,
                new Vector2(13f, 13f),
                0f,
                0.22f);
            return new[] { grassLayer, mudLayer, rockLayer };
        }

        private static TerrainLayer GetOrCreateTerrainLayer(
            string layerPath,
            string diffuseTexturePath,
            string normalTexturePath,
            Vector2 tileSize,
            float metallic,
            float smoothness)
        {
            TerrainLayer terrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
            if (terrainLayer == null)
            {
                terrainLayer = new TerrainLayer();
                AssetDatabase.CreateAsset(terrainLayer, layerPath);
            }

            terrainLayer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(diffuseTexturePath);
            terrainLayer.normalMapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalTexturePath);
            terrainLayer.tileSize = tileSize;
            terrainLayer.metallic = metallic;
            terrainLayer.smoothness = smoothness;
            EditorUtility.SetDirty(terrainLayer);
            return terrainLayer;
        }

        private static float[,] CreateHeroTerrainHeights(int resolution)
        {
            float[,] heights = new float[resolution, resolution];
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float worldX = Mathf.Lerp(-130f, 130f, x / (resolution - 1f));
                    float worldZ = Mathf.Lerp(-130f, 130f, z / (resolution - 1f));
                    heights[z, x] = GetHeroTerrainNormalizedHeight(worldX, worldZ);
                }
            }

            return heights;
        }

        private static float[,,] CreateHeroTerrainAlphamaps(int resolution)
        {
            float[,,] alphamaps = new float[resolution, resolution, 3];
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float worldX = Mathf.Lerp(-130f, 130f, x / (resolution - 1f));
                    float worldZ = Mathf.Lerp(-130f, 130f, z / (resolution - 1f));
                    float distance = new Vector2(worldX, worldZ).magnitude;
                    float pathMask = Mathf.Max(
                        1f - Mathf.InverseLerp(14f, 32f, Mathf.Abs(worldX)),
                        1f - Mathf.InverseLerp(14f, 32f, Mathf.Abs(worldZ)));
                    float edgeMask = Mathf.InverseLerp(70f, 132f, distance);
                    float noise = Mathf.PerlinNoise(worldX * 0.036f + 18.6f, worldZ * 0.036f + 3.2f);
                    float mud = Mathf.Clamp01(pathMask * 0.50f + noise * 0.18f);
                    float rock = Mathf.Clamp01(edgeMask * 0.64f + Mathf.Max(0f, noise - 0.62f) * 0.74f);
                    float grass = Mathf.Clamp01(1f - mud * 0.55f - rock * 0.70f);
                    float total = grass + mud + rock;

                    alphamaps[z, x, 0] = grass / total;
                    alphamaps[z, x, 1] = mud / total;
                    alphamaps[z, x, 2] = rock / total;
                }
            }

            return alphamaps;
        }

        private static float GetHeroTerrainNormalizedHeight(float worldX, float worldZ)
        {
            float distance = new Vector2(worldX, worldZ).magnitude;
            float crossRoadMask = Mathf.Max(
                1f - Mathf.InverseLerp(14f, 44f, Mathf.Abs(worldX)),
                1f - Mathf.InverseLerp(14f, 44f, Mathf.Abs(worldZ)));
            float plazaMask = 1f - Mathf.InverseLerp(24f, 62f, distance);
            float flatMask = Mathf.Clamp01(Mathf.Max(crossRoadMask, plazaMask));
            float edgeRise = Mathf.InverseLerp(62f, 130f, distance) * 0.075f;
            float lowNoise = Mathf.PerlinNoise(worldX * 0.020f + 5.4f, worldZ * 0.020f + 33.7f) * 0.036f;
            float highNoise = Mathf.PerlinNoise(worldX * 0.062f + 91.2f, worldZ * 0.062f + 12.4f) * 0.018f;
            return Mathf.Clamp01((edgeRise + lowNoise + highNoise) * (1f - flatMask * 0.88f) + 0.012f);
        }

        private static float GetHeroTerrainWorldHeight(float worldX, float worldZ)
        {
            return -0.65f + GetHeroTerrainNormalizedHeight(worldX, worldZ) * 12f;
        }

        private static void CreateForestBackdrop(Transform environmentRoot)
        {
            string[] treeAssetPaths =
            {
                ForestTreeOnePrefabPath,
                ForestTreeTwoPrefabPath,
                ForestBushOnePrefabPath,
                ForestBushTwoPrefabPath
            };

            for (int i = 0; i < 44; i++)
            {
                float angle = i * 8.2f + (i % 5) * 3.1f;
                float radius = 92f + (i % 4) * 10f;
                Vector3 position = GetRingPosition(angle, radius);
                position.y = GetHeroTerrainWorldHeight(position.x, position.z) - 0.05f;
                string assetPath = treeAssetPaths[i % treeAssetPaths.Length];
                float scale = assetPath.Contains("Bush") ? 5.2f + (i % 3) * 1.1f : 8.5f + (i % 5) * 1.35f;
                CreateForestAsset(
                    assetPath,
                    "Asset_ForestBackdrop_" + i.ToString("00"),
                    environmentRoot,
                    position,
                    Quaternion.Euler(0f, angle + 180f, 0f),
                    Vector3.one * scale,
                    true);
            }

            string[] rockAssetPaths =
            {
                ForestRockOnePrefabPath,
                ForestRockTwoPrefabPath,
                ForestRockThreePrefabPath,
                ForestRockFourPrefabPath
            };

            for (int i = 0; i < 28; i++)
            {
                float angle = i * 12.85f + 6f;
                float radius = 68f + (i % 5) * 11f;
                Vector3 position = GetRingPosition(angle, radius);
                position.y = GetHeroTerrainWorldHeight(position.x, position.z) + 0.08f;
                CreateForestAsset(
                    rockAssetPaths[i % rockAssetPaths.Length],
                    "Asset_ForestRock_" + i.ToString("00"),
                    environmentRoot,
                    position,
                    Quaternion.Euler(0f, angle * 0.6f, 0f),
                    Vector3.one * (3.6f + (i % 4) * 0.8f),
                    false);
            }
        }

        private static Vector3 GetRingPosition(float angle, float radius)
        {
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            return rotation * new Vector3(0f, 0f, radius);
        }

        private static void CreateForestAsset(
            string assetPath,
            string objectName,
            Transform parent,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            bool isTree)
        {
            GameObject assetObject = CreateImportedSceneAsset(
                assetPath,
                objectName,
                parent,
                position,
                rotation,
                scale);
            if (assetObject != null)
            {
                ApplyFallbackMaterialToRenderers(
                    assetObject,
                    isTree ? LoadMaterial("MAT_DaniTech_ForestFoliage") : LoadMaterial("MAT_DaniTech_ForestRock"));
                return;
            }

            if (isTree)
            {
                CreateFallbackTree(objectName, parent, position, rotation, scale);
                return;
            }

            CreatePrimitive(PrimitiveType.Sphere, "Fallback_" + objectName, parent, position, scale, LoadMaterial("MAT_DaniTech_ForestRock"));
        }

        private static void CreateFallbackTree(string objectName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject treeRoot = CreateEmpty("Fallback_" + objectName, parent, position);
            treeRoot.transform.localRotation = rotation;
            treeRoot.transform.localScale = scale;
            CreatePrimitive(PrimitiveType.Cylinder, "Trunk", treeRoot.transform, new Vector3(0f, 0.38f, 0f), new Vector3(0.12f, 0.82f, 0.12f), LoadMaterial("MAT_DaniTech_ForestBark"));
            CreatePrimitive(PrimitiveType.Sphere, "Canopy", treeRoot.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.58f, 0.72f, 0.58f), LoadMaterial("MAT_DaniTech_ForestFoliage"));
        }

        private static void CreateTownFloorTiles(Transform environmentRoot, Material groundMaterial)
        {
            Vector3[] floorPositions =
            {
                new Vector3(-64f, 0.06f, -64f),
                new Vector3(64f, 0.06f, -64f),
                new Vector3(-64f, 0.06f, 64f),
                new Vector3(64f, 0.06f, 64f)
            };

            for (int i = 0; i < floorPositions.Length; i++)
            {
                CreateImportedSceneAsset(
                    TownFloorAssetPath,
                    "Asset_TownFloor_" + i.ToString("00"),
                    environmentRoot,
                    floorPositions[i],
                    Quaternion.identity,
                    new Vector3(2.1f, 1f, 2.1f),
                    groundMaterial,
                    LoadMaterial("MAT_DaniTech_Concrete"));
            }
        }

        private static void CreateTownRoadNetwork(Transform environmentRoot, Material pathMaterial, Material edgeMaterial)
        {
            for (int i = -5; i <= 5; i++)
            {
                float offset = i * 18f;
                CreateImportedSceneAsset(TownRoadAssetPath, "Asset_Road_North_" + i.ToString("00"), environmentRoot, new Vector3(0f, 0.12f, offset), Quaternion.identity, new Vector3(2.9f, 1f, 2.9f), pathMaterial);
                CreateImportedSceneAsset(TownRoadAssetPath, "Asset_Road_East_" + i.ToString("00"), environmentRoot, new Vector3(offset, 0.13f, 0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(2.9f, 1f, 2.9f), pathMaterial);

                CreateImportedSceneAsset(TownSideWalkAssetPath, "Asset_SideWalk_Left_" + i.ToString("00"), environmentRoot, new Vector3(-12f, 0.16f, offset), Quaternion.identity, new Vector3(2.45f, 1f, 2.45f), edgeMaterial);
                CreateImportedSceneAsset(TownSideWalkAssetPath, "Asset_SideWalk_Right_" + i.ToString("00"), environmentRoot, new Vector3(12f, 0.16f, offset), Quaternion.Euler(0f, 180f, 0f), new Vector3(2.45f, 1f, 2.45f), edgeMaterial);
            }

            CreateImportedSceneAsset(TownRoadEdgeAAssetPath, "Asset_RoadEdge_West", environmentRoot, new Vector3(-58f, 0.18f, 0f), Quaternion.Euler(0f, 90f, 0f), new Vector3(5.5f, 1f, 5.5f), edgeMaterial);
            CreateImportedSceneAsset(TownRoadEdgeBAssetPath, "Asset_RoadEdge_East", environmentRoot, new Vector3(58f, 0.18f, 0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(5.5f, 1f, 5.5f), edgeMaterial);
            CreateImportedSceneAsset(TownStoneChunksAAssetPath, "Asset_StoneChunks_West", environmentRoot, new Vector3(-34f, 0.2f, -32f), Quaternion.Euler(0f, 18f, 0f), new Vector3(2.8f, 2.8f, 2.8f), LoadMaterial("MAT_DaniTech_DarkMetal"), pathMaterial);
            CreateImportedSceneAsset(TownStoneChunksBAssetPath, "Asset_StoneChunks_East", environmentRoot, new Vector3(35f, 0.2f, 30f), Quaternion.Euler(0f, -28f, 0f), new Vector3(2.6f, 2.6f, 2.6f), LoadMaterial("MAT_DaniTech_DarkMetal"), pathMaterial);

            Vector3[] lightPositions =
            {
                new Vector3(-18f, 0.18f, -48f),
                new Vector3(18f, 0.18f, -48f),
                new Vector3(-18f, 0.18f, 48f),
                new Vector3(18f, 0.18f, 48f)
            };

            for (int i = 0; i < lightPositions.Length; i++)
            {
                CreateImportedSceneAsset(
                    TownStreetLightAssetPath,
                    "Asset_StreetLight_" + i.ToString("00"),
                    environmentRoot,
                    lightPositions[i],
                    Quaternion.Euler(0f, i < 2 ? 0f : 180f, 0f),
                    new Vector3(2.2f, 2.2f, 2.2f),
                    LoadMaterial("MAT_DaniTech_DarkMetal"),
                    LoadMaterial("MAT_DaniTech_Light"));
            }
        }

        private static GameObject CreatePlayer(Scene scene)
        {
            GameObject playerObject = CreateRoot(scene, "Player_DaniTech");
            playerObject.tag = "Player";
            playerObject.transform.position = new Vector3(0f, 1.1f, -7.2f);
            CharacterController characterController = playerObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.32f;
            characterController.center = new Vector3(0f, 0.9f, 0f);
            playerObject.AddComponent<DaniTechPlayerMovement>();
            DaniTechPlayerView playerView = playerObject.AddComponent<DaniTechPlayerView>();
            DaniTechPlayerInteraction interaction = playerObject.AddComponent<DaniTechPlayerInteraction>();
            Transform playerVisualRoot = CreatePlayerCharacterVisual(playerObject.transform);

            GameObject cameraObject = CreateEmpty("Camera_FirstPerson", playerObject.transform, new Vector3(0f, 1.58f, 0f));
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 70f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 600f;
            camera.tag = "MainCamera";
            cameraObject.AddComponent<AudioListener>();

            SetObjectReference(playerView, "_cameraTransform", cameraObject.transform);
            SetObjectReference(playerView, "_characterVisualRoot", playerVisualRoot);
            SetObjectReference(interaction, "_cameraTransform", cameraObject.transform);
            return playerObject;
        }

        private static Transform CreatePlayerCharacterVisual(Transform parent)
        {
            GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerCharacterPrefabPath);
            GameObject visualObject;
            if (characterPrefab != null)
            {
                visualObject = PrefabUtility.InstantiatePrefab(characterPrefab, parent) as GameObject;
                if (visualObject != null)
                {
                    visualObject.name = "Visual_PlayerCharacter_HeroMage";
                    visualObject.transform.localPosition = Vector3.zero;
                    visualObject.transform.localRotation = Quaternion.identity;
                    visualObject.transform.localScale = Vector3.one;
                    ScaleToHeight(visualObject, 1.85f);
                    ApplyMaterialPaletteToRenderers(
                        visualObject,
                        LoadMaterial("MAT_DaniTech_HeroBlue"),
                        LoadMaterial("MAT_DaniTech_HeroGold"),
                        LoadMaterial("MAT_DaniTech_HeroSkin"),
                        LoadMaterial("MAT_DaniTech_DarkMetal"));
                    visualObject.SetActive(false);
                    return visualObject.transform;
                }
            }

            visualObject = CreatePrimitive(PrimitiveType.Capsule, "Fallback_PlayerCharacter", parent, new Vector3(0f, 0.9f, 0f), new Vector3(0.65f, 0.9f, 0.65f), LoadMaterial("MAT_DaniTech_HeroBlue"));
            visualObject.SetActive(false);
            return visualObject.transform;
        }

        private static GameObject CreateManagers(Scene scene, GameObject playerObject)
        {
            GameObject managerGroup = CreateRoot(scene, "Managers");
            GameObject gameManagerObject = CreateEmpty("GameManager", managerGroup.transform, Vector3.zero);
            GameObject dataManagerObject = CreateEmpty("GameDataManager", managerGroup.transform, Vector3.zero);
            GameObject promotionManagerObject = CreateEmpty("DaniTechPromotionManager", managerGroup.transform, Vector3.zero);
            GameObject itemRoot = CreateEmpty("Transform_AddressableItems", managerGroup.transform, Vector3.zero);

            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            GameDataManager dataManager = dataManagerObject.AddComponent<GameDataManager>();
            DaniTechPromotionManager promotionManager = promotionManagerObject.AddComponent<DaniTechPromotionManager>();
            DaniTechAddressableItemSpawner spawner = promotionManagerObject.AddComponent<DaniTechAddressableItemSpawner>();

            SetObjectReference(gameManager, "_gameDataManager", dataManager);
            SetObjectReference(gameManager, "_promotionManager", promotionManager);
            SetObjectReference(promotionManager, "_itemSpawner", spawner);
            SetObjectReference(promotionManager, "_playerInteraction", playerObject.GetComponent<DaniTechPlayerInteraction>());
            SetObjectReference(spawner, "_itemRoot", itemRoot.transform);
            SetObjectReference(spawner, "_dropAnchor", playerObject.transform);
            SetObjectReference(spawner, "_useEffectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(UseEffectPrefabPath));
            return managerGroup;
        }

        private static void CreateSpawnPoints(Transform managerGroup)
        {
            Transform spawnRoot = CreateEmpty("SpawnPoints_ItemAddressables", managerGroup, Vector3.zero).transform;
            CreateSpawnPoint(spawnRoot, "Spawn_EmeraldCore_A", "emerald_core", 2, new Vector3(-14f, 0.8f, -4f));
            CreateSpawnPoint(spawnRoot, "Spawn_EmeraldCore_B", "emerald_core", 1, new Vector3(18f, 0.8f, -16f));
            CreateSpawnPoint(spawnRoot, "Spawn_DiamondTonic", "diamond_tonic", 1, new Vector3(22f, 0.8f, 18f));
            CreateSpawnPoint(spawnRoot, "Spawn_PrismShield", "prism_shield", 1, new Vector3(-26f, 0.8f, 20f));
            CreateSpawnPoint(spawnRoot, "Spawn_SwiftSigil", "swift_sigil", 1, new Vector3(0f, 0.8f, 34f));

            DaniTechAddressableItemSpawner spawner = managerGroup.GetComponentInChildren<DaniTechAddressableItemSpawner>();
            if (spawner != null)
            {
                SetObjectArray(spawner, "_spawnPoints", spawnRoot.GetComponentsInChildren<DaniTechItemSpawnPoint>());
            }
        }

        private static void CreateSpawnPoint(Transform parent, string objectName, string itemId, int amount, Vector3 position)
        {
            GameObject spawnPointObject = CreateEmpty(objectName, parent, position);
            DaniTechItemSpawnPoint spawnPoint = spawnPointObject.AddComponent<DaniTechItemSpawnPoint>();
            SetString(spawnPoint, "_itemId", itemId);
            SetInt(spawnPoint, "_amount", amount);
        }

        private static void CreateUi(Scene scene, GameObject managerGroup)
        {
            GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(UiPrefabPath);
            GameObject uiObject = PrefabUtility.InstantiatePrefab(uiPrefab) as GameObject;
            SceneManager.MoveGameObjectToScene(uiObject, scene);
            uiObject.name = "Canvas_DaniTechPromotionUI";

            EnsureEventSystem(scene);
            DaniTechPromotionManager promotionManager = managerGroup.GetComponentInChildren<DaniTechPromotionManager>();
            DaniTechInventoryUI inventoryUI = uiObject.GetComponentInChildren<DaniTechInventoryUI>(true);
            DaniTechPlayerHUD playerHUD = uiObject.GetComponentInChildren<DaniTechPlayerHUD>(true);
            DaniTechFeedbackLogView feedbackLogView = uiObject.GetComponentInChildren<DaniTechFeedbackLogView>(true);

            SetObjectReference(promotionManager, "_inventoryUI", inventoryUI);
            SetObjectReference(promotionManager, "_playerHUD", playerHUD);
            SetObjectReference(promotionManager, "_feedbackLogView", feedbackLogView);
        }

        private static void EnsureEventSystem(Scene scene)
        {
            GameObject eventSystemObject = CreateRoot(scene, "EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static Material GetOrCreateMaterial(string materialName, Color color, float emission)
        {
            string materialPath = MaterialFolderPath + "/" + materialName + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                Shader shader = Shader.Find("Standard");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }

            material.color = color;
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            if (emission > 0f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * emission);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.black);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material GetOrCreateTexturedMaterial(
            string materialName,
            string albedoTexturePath,
            string normalTexturePath,
            Color fallbackColor,
            float smoothness)
        {
            Material material = GetOrCreateMaterial(materialName, fallbackColor, 0f);
            Texture2D albedoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoTexturePath);
            Texture2D normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalTexturePath);

            if (albedoTexture != null)
            {
                material.mainTexture = albedoTexture;
                material.SetTexture("_MainTex", albedoTexture);
                material.SetTextureScale("_MainTex", new Vector2(0.22f, 0.22f));
            }

            if (normalTexture != null)
            {
                material.EnableKeyword("_NORMALMAP");
                material.SetTexture("_BumpMap", normalTexture);
                material.SetFloat("_BumpScale", 0.72f);
            }

            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", smoothness);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material GetOrCreateSkyboxMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(SkyboxMaterialPath);
            if (material == null)
            {
                Shader shader = Shader.Find("Skybox/Procedural");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, SkyboxMaterialPath);
            }

            if (material.HasProperty("_SkyTint"))
            {
                material.SetColor("_SkyTint", new Color(0.52f, 0.68f, 0.96f, 1f));
            }

            if (material.HasProperty("_GroundColor"))
            {
                material.SetColor("_GroundColor", new Color(0.50f, 0.63f, 0.74f, 1f));
            }

            if (material.HasProperty("_Exposure"))
            {
                material.SetFloat("_Exposure", 1.18f);
            }

            if (material.HasProperty("_AtmosphereThickness"))
            {
                material.SetFloat("_AtmosphereThickness", 1.46f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material LoadMaterial(string materialName)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(MaterialFolderPath + "/" + materialName + ".mat");
        }

        private static Material LoadBrooklynMaterial(string materialName)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(BrooklynImportedFolderPath + "/Materials/" + materialName + ".mat");
        }

        private static void RegisterAddressable(AddressableAssetSettings settings, AddressableAssetGroup group, string assetPath, string address)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: addressable asset GUID missing. " + assetPath);
                return;
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = address;
            entry.SetLabel("DaniTechPromotion", true, true);
            EditorUtility.SetDirty(settings);
        }

        private static void RegisterAddressableFolderAssets(AddressableAssetSettings settings, AddressableAssetGroup group, string folderPath, string addressRoot)
        {
            if (AssetDatabase.IsValidFolder(folderPath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: addressable folder missing. " + folderPath);
                return;
            }

            string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.IsNullOrEmpty(assetPath) || AssetDatabase.IsValidFolder(assetPath))
                {
                    continue;
                }

                string relativePath = assetPath.Substring(folderPath.Length).TrimStart('/').Replace("\\", "/");
                RegisterAddressable(settings, group, assetPath, addressRoot + "/" + relativePath);
            }
        }

        private static void CreateExternalModel(string fileName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            string assetPath = BrooklynImportedFolderPath + "/" + fileName;
            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (modelPrefab == null)
            {
                return;
            }

            GameObject modelObject = PrefabUtility.InstantiatePrefab(modelPrefab, parent) as GameObject;
            modelObject.name = "External_" + Path.GetFileNameWithoutExtension(fileName);
            modelObject.transform.localPosition = position;
            modelObject.transform.localRotation = rotation;
            modelObject.transform.localScale = scale;
            ApplyMaterialPaletteToRenderers(
                modelObject,
                LoadMaterial("MAT_DaniTech_Brick"),
                LoadMaterial("MAT_DaniTech_Concrete"),
                LoadMaterial("MAT_DaniTech_Window"),
                LoadMaterial("MAT_DaniTech_Bronze"),
                LoadMaterial("MAT_DaniTech_DarkMetal"));
        }

        private static GameObject CreateImportedSceneAsset(
            string assetPath,
            string objectName,
            Transform parent,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            params Material[] materials)
        {
            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (modelPrefab == null)
            {
                return null;
            }

            GameObject modelObject = PrefabUtility.InstantiatePrefab(modelPrefab, parent) as GameObject;
            if (modelObject == null)
            {
                return null;
            }

            modelObject.name = objectName;
            modelObject.transform.localPosition = position;
            modelObject.transform.localRotation = rotation;
            modelObject.transform.localScale = scale;
            ApplyMaterialPaletteToRenderers(modelObject, materials);
            return modelObject;
        }

        private static bool CreateImportedAssetVisual(
            string assetPath,
            string objectName,
            Transform parent,
            Vector3 position,
            Quaternion rotation,
            float targetHeight,
            Material fallbackMaterial)
        {
            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (modelPrefab == null)
            {
                return false;
            }

            GameObject modelObject = PrefabUtility.InstantiatePrefab(modelPrefab, parent) as GameObject;
            if (modelObject == null)
            {
                return false;
            }

            modelObject.name = objectName;
            modelObject.transform.localPosition = position;
            modelObject.transform.localRotation = rotation;
            modelObject.transform.localScale = Vector3.one;
            ScaleToHeight(modelObject, targetHeight);
            ApplyMaterialToRenderers(modelObject, fallbackMaterial);
            return true;
        }

        private static void ApplyMaterialToRenderers(GameObject targetObject, Material material)
        {
            if (targetObject == null || material == null)
            {
                return;
            }

            ApplyMaterialPaletteToRenderers(targetObject, material);
        }

        private static void ApplyMaterialPaletteToRenderers(GameObject targetObject, params Material[] materials)
        {
            if (targetObject == null || materials == null || materials.Length <= 0)
            {
                return;
            }

            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] sharedMaterials = renderers[i].sharedMaterials;
                if (sharedMaterials == null || sharedMaterials.Length <= 0)
                {
                    continue;
                }

                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    Material material = materials[(i + j) % materials.Length];
                    if (material != null)
                    {
                        sharedMaterials[j] = material;
                    }
                }

                renderers[i].sharedMaterials = sharedMaterials;
            }
        }

        private static void ApplyFallbackMaterialToRenderers(GameObject targetObject, Material material)
        {
            if (targetObject == null || material == null)
            {
                return;
            }

            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] sharedMaterials = renderers[i].sharedMaterials;
                bool hasChanged = false;
                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    if (ShouldReplaceMaterial(sharedMaterials[j]))
                    {
                        sharedMaterials[j] = material;
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    renderers[i].sharedMaterials = sharedMaterials;
                }
            }
        }

        private static bool ShouldReplaceMaterial(Material material)
        {
            if (material == null)
            {
                return true;
            }

            return material.name.Contains("Default") || material.name.Contains("No Name");
        }

        private static void ScaleToHeight(GameObject targetObject, float targetHeight)
        {
            if (targetObject == null || targetHeight <= 0f)
            {
                return;
            }

            Bounds bounds = CalculateRendererBounds(targetObject);
            if (bounds.size.y <= 0.001f)
            {
                return;
            }

            float scaleRate = targetHeight / bounds.size.y;
            targetObject.transform.localScale *= scaleRate;
        }

        private static Bounds CalculateRendererBounds(GameObject targetObject)
        {
            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return new Bounds(targetObject.transform.position, Vector3.one);
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private static RectTransform CreatePanel(Transform parent, string objectName, Color color)
        {
            GameObject panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(parent);
            panelObject.transform.localRotation = Quaternion.identity;
            panelObject.transform.localScale = Vector3.one;
            RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
            Image image = panelObject.AddComponent<Image>();
            image.color = color;
            return rectTransform;
        }

        private static Text CreateText(Transform parent, string objectName, string text, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent);
            textObject.transform.localRotation = Quaternion.identity;
            textObject.transform.localScale = Vector3.one;
            RectTransform rectTransform = textObject.AddComponent<RectTransform>();
            Text uiText = textObject.AddComponent<Text>();
            uiText.text = text;
            uiText.fontSize = fontSize;
            uiText.alignment = alignment;
            uiText.color = Color.white;
            uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
            uiText.verticalOverflow = VerticalWrapMode.Truncate;
            uiText.raycastTarget = false;
            return uiText;
        }

        private static TextMesh CreateWorldText(Transform parent, string objectName, string text, Vector3 localPosition, int fontSize)
        {
            GameObject textObject = CreateEmpty(objectName, parent, localPosition);
            TextMesh textMesh = textObject.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;
            textObject.transform.localScale = Vector3.one * 0.025f;
            return textMesh;
        }

        private static Image CreateBar(RectTransform parent, string objectName, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            RectTransform background = CreatePanel(parent, objectName + "_Bg", new Color(0f, 0f, 0f, 0.35f));
            SetRect(background, new Vector2(0f, 1f), size, anchoredPosition);

            RectTransform fill = CreatePanel(background, objectName + "_Fill", color);
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            Image image = fill.GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = 0;
            image.fillAmount = 1f;
            return image;
        }

        private static Button CreateButton(Transform parent, string objectName, string label, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            RectTransform buttonRoot = CreatePanel(parent, objectName, color);
            SetRect(buttonRoot, new Vector2(0f, 0f), size, anchoredPosition);
            Button button = buttonRoot.gameObject.AddComponent<Button>();
            Text labelText = CreateText(buttonRoot, "Text_" + objectName, label, 20, TextAnchor.MiddleCenter);
            SetStretch(labelText.rectTransform, Vector2.zero, Vector2.zero);
            return button;
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string objectName, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject createdObject = GameObject.CreatePrimitive(type);
            createdObject.name = objectName;
            createdObject.transform.SetParent(parent);
            createdObject.transform.localPosition = localPosition;
            createdObject.transform.localRotation = Quaternion.identity;
            createdObject.transform.localScale = localScale;

            Renderer renderer = createdObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            return createdObject;
        }

        private static GameObject CreateDiamond(string objectName, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject diamondObject = new GameObject(objectName);
            diamondObject.transform.SetParent(parent);
            diamondObject.transform.localPosition = localPosition;
            diamondObject.transform.localRotation = Quaternion.identity;
            diamondObject.transform.localScale = localScale;

            MeshFilter meshFilter = diamondObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = diamondObject.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = CreateDiamondMesh();
            meshRenderer.sharedMaterial = material;
            return diamondObject;
        }

        private static Mesh CreateDiamondMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "DaniTech_DiamondMesh";
            Vector3 top = new Vector3(0f, 0.75f, 0f);
            Vector3 bottom = new Vector3(0f, -0.75f, 0f);
            Vector3 left = new Vector3(-0.55f, 0f, 0f);
            Vector3 right = new Vector3(0.55f, 0f, 0f);
            Vector3 front = new Vector3(0f, 0f, 0.55f);
            Vector3 back = new Vector3(0f, 0f, -0.55f);

            mesh.vertices = new[]
            {
                top, front, right,
                top, right, back,
                top, back, left,
                top, left, front,
                bottom, right, front,
                bottom, back, right,
                bottom, left, back,
                bottom, front, left
            };

            mesh.triangles = new[]
            {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8,
                9, 10, 11,
                12, 13, 14,
                15, 16, 17,
                18, 19, 20,
                21, 22, 23
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static GameObject CreateEmpty(string objectName, Transform parent, Vector3 localPosition)
        {
            GameObject createdObject = new GameObject(objectName);
            createdObject.transform.SetParent(parent);
            createdObject.transform.localPosition = localPosition;
            createdObject.transform.localRotation = Quaternion.identity;
            createdObject.transform.localScale = Vector3.one;
            return createdObject;
        }

        private static GameObject CreateRoot(Scene scene, string objectName)
        {
            GameObject rootObject = new GameObject(objectName);
            SceneManager.MoveGameObjectToScene(rootObject, scene);
            return rootObject;
        }

        private static void CreatePointLight(Transform parent, string objectName, Vector3 localPosition, Color color, float intensity, float range)
        {
            GameObject lightObject = CreateEmpty(objectName, parent, localPosition);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
        }

        private static void SavePrefab(GameObject rootObject, string prefabPath)
        {
            PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            Object.DestroyImmediate(rootObject);
        }

        private static void SetRect(RectTransform rectTransform, Vector2 anchor, Vector2 size, Vector2 anchoredPosition)
        {
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        private static void SetStretch(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            AssetDatabase.CreateFolder(parent, child);
        }

        private static void ClearScene(Scene scene)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                Object.DestroyImmediate(rootObjects[i]);
            }
        }

        private static void SetObjectReference(Object targetObject, string propertyName, Object value)
        {
            SerializedObject serializedObject = new SerializedObject(targetObject);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: property was not found. " + propertyName);
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(targetObject);
        }

        private static void SetObjectArray<TComponent>(Object targetObject, string propertyName, TComponent[] components)
            where TComponent : Object
        {
            SerializedObject serializedObject = new SerializedObject(targetObject);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: property was not found. " + propertyName);
                return;
            }

            property.arraySize = components.Length;
            for (int i = 0; i < components.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = components[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(targetObject);
        }

        private static void SetString(Object targetObject, string propertyName, string value)
        {
            SerializedObject serializedObject = new SerializedObject(targetObject);
            serializedObject.FindProperty(propertyName).stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(targetObject);
        }

        private static void SetInt(Object targetObject, string propertyName, int value)
        {
            SerializedObject serializedObject = new SerializedObject(targetObject);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(targetObject);
        }
    }
}
