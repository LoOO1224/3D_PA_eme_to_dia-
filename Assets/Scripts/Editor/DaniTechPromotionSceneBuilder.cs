using EmeToDia.Gameplay;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private const string PrefabFolderPath = "Assets/DaniTech/Prefabs";
        private const string ItemPrefabFolderPath = PrefabFolderPath + "/Items";
        private const string UiPrefabFolderPath = PrefabFolderPath + "/UI";
        private const string ExternalFolderPath = "Assets/ThirdParty/DaniTechImported";
        private const string AddressableGroupName = "DaniTechPromotion";
        private const string BrooklynSourceRoot = "D:/3D_Basic_Assets/0_배경/Kitbash3D Brooklyn-004/Kitbash3D Brooklyn-004/Kitbash3D Brooklyn/Kitbash3D Brooklyn Unity BuiltIn/KB3D_Brooklyn_UnityBuiltIn/Assets/KB3D/Brooklyn";

        private const string EmeraldItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_EmeraldCore.prefab";
        private const string DiamondItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_DiamondTonic.prefab";
        private const string ShieldItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_PrismShield.prefab";
        private const string SwiftItemPrefabPath = ItemPrefabFolderPath + "/PF_DaniTech_Item_SwiftSigil.prefab";
        private const string UiPrefabPath = UiPrefabFolderPath + "/PF_DaniTechPromotionUI.prefab";
        private const string UseEffectPrefabPath = PrefabFolderPath + "/PF_DaniTech_UseEffect.prefab";

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

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "DaniTech");
            EnsureFolder("Assets/DaniTech", "Materials");
            EnsureFolder("Assets/DaniTech", "Prefabs");
            EnsureFolder(PrefabFolderPath, "Items");
            EnsureFolder(PrefabFolderPath, "UI");
            EnsureFolder("Assets", "ThirdParty");
            EnsureFolder("Assets/ThirdParty", "DaniTechImported");
            EnsureFolder(ExternalFolderPath, "KB3D_Brooklyn");
        }

        private static void ImportExternalAssets()
        {
            CopyExternalAsset("KB3D_BRK_BldgLG_C.fbx");
            CopyExternalAsset("KB3D_BRK_BldgMD_F.fbx");
            CopyExternalAsset("KB3D_BRK_WaterTower_A.fbx");
            CopyExternalAsset("KB3D_BRK_Lamp_A.fbx");
            CopyExternalAsset("KB3D_BRK_RooftopProp_A.fbx");
            AssetDatabase.Refresh();
        }

        private static void CopyExternalAsset(string fileName)
        {
            string sourcePath = Path.Combine(BrooklynSourceRoot, fileName);
            if (File.Exists(sourcePath) == false)
            {
                Debug.LogWarning("DaniTechPromotionSceneBuilder: external asset was not found. " + sourcePath);
                return;
            }

            string targetAssetPath = ExternalFolderPath + "/KB3D_Brooklyn/" + fileName;
            string targetFullPath = Path.GetFullPath(targetAssetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));

            if (File.Exists(targetFullPath) == false)
            {
                FileUtil.CopyFileOrDirectory(sourcePath, targetFullPath);
            }

            AssetDatabase.ImportAsset(targetAssetPath, ImportAssetOptions.ForceUpdate);
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
        }

        private static void CreateItemPrefabs()
        {
            CreateCrystalItemPrefab(EmeraldItemPrefabPath, "emerald_core", "에메랄드 코어", "MAT_DaniTech_Emerald");
            CreateCrystalItemPrefab(DiamondItemPrefabPath, "diamond_tonic", "다이아 회복제", "MAT_DaniTech_Diamond");
            CreateCrystalItemPrefab(ShieldItemPrefabPath, "prism_shield", "프리즘 보호막", "MAT_DaniTech_Shield");
            CreateCrystalItemPrefab(SwiftItemPrefabPath, "swift_sigil", "신속의 인장", "MAT_DaniTech_Swift");
        }

        private static void CreateCrystalItemPrefab(string prefabPath, string itemId, string label, string materialName)
        {
            GameObject rootObject = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            rootObject.transform.localScale = Vector3.one;

            GameObject visualRoot = CreateEmpty("VisualRoot", rootObject.transform, Vector3.zero);
            Material itemMaterial = LoadMaterial(materialName);
            CreateDiamond("Visual_Gem_Core", visualRoot.transform, Vector3.zero, new Vector3(0.65f, 0.92f, 0.65f), itemMaterial);
            CreatePrimitive(PrimitiveType.Cylinder, "Visual_Gem_Base", visualRoot.transform, new Vector3(0f, -0.55f, 0f), new Vector3(0.72f, 0.12f, 0.72f), LoadMaterial("MAT_DaniTech_DarkMetal"));

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

            RectTransform hudRoot = CreatePanel(canvasObject.transform, "Panel_PlayerHUD", new Color(0.03f, 0.05f, 0.07f, 0.82f));
            SetRect(hudRoot, new Vector2(0f, 1f), new Vector2(430f, 190f), new Vector2(22f, -22f));
            DaniTechPlayerHUD playerHUD = hudRoot.gameObject.AddComponent<DaniTechPlayerHUD>();
            Text statusText = CreateText(hudRoot, "Text_Status", "", 20, TextAnchor.UpperLeft);
            SetRect(statusText.rectTransform, new Vector2(0f, 1f), new Vector2(386f, 120f), new Vector2(22f, -18f));
            Text guideText = CreateText(hudRoot, "Text_Guide", "", 16, TextAnchor.LowerLeft);
            SetRect(guideText.rectTransform, new Vector2(0f, 0f), new Vector2(386f, 34f), new Vector2(22f, 16f));
            Image healthFill = CreateBar(hudRoot, "Bar_HP", new Vector2(22f, -142f), new Color(0.9f, 0.14f, 0.16f, 1f));
            Image staminaFill = CreateBar(hudRoot, "Bar_Stamina", new Vector2(22f, -160f), new Color(0.2f, 0.82f, 0.36f, 1f));
            Image shieldFill = CreateBar(hudRoot, "Bar_Shield", new Vector2(22f, -178f), new Color(0.18f, 0.48f, 0.95f, 1f));
            SetObjectReference(playerHUD, "_statusText", statusText);
            SetObjectReference(playerHUD, "_guideText", guideText);
            SetObjectReference(playerHUD, "_healthFillImage", healthFill);
            SetObjectReference(playerHUD, "_staminaFillImage", staminaFill);
            SetObjectReference(playerHUD, "_shieldFillImage", shieldFill);

            RectTransform logRoot = CreatePanel(canvasObject.transform, "Panel_FeedbackLog", new Color(0.04f, 0.04f, 0.05f, 0.75f));
            SetRect(logRoot, new Vector2(0f, 0f), new Vector2(650f, 160f), new Vector2(22f, 22f));
            DaniTechFeedbackLogView feedbackLogView = logRoot.gameObject.AddComponent<DaniTechFeedbackLogView>();
            Text logText = CreateText(logRoot, "Text_Log", "", 18, TextAnchor.LowerLeft);
            SetStretch(logText.rectTransform, new Vector2(18f, 14f), new Vector2(-18f, -14f));
            SetObjectReference(feedbackLogView, "_logText", logText);

            RectTransform inventoryRoot = CreatePanel(canvasObject.transform, "Panel_InventoryRoot", new Color(0.025f, 0.035f, 0.045f, 0.96f));
            SetRect(inventoryRoot, new Vector2(0.5f, 0.5f), new Vector2(980f, 620f), Vector2.zero);
            DaniTechInventoryUI inventoryUI = inventoryRoot.gameObject.AddComponent<DaniTechInventoryUI>();
            BuildInventoryPanel(inventoryRoot, inventoryUI);

            SetObjectReference(inventoryUI, "_rootObject", inventoryRoot.gameObject);
            inventoryRoot.gameObject.SetActive(false);
            SavePrefab(canvasObject, UiPrefabPath);
        }

        private static void BuildInventoryPanel(RectTransform inventoryRoot, DaniTechInventoryUI inventoryUI)
        {
            Text titleText = CreateText(inventoryRoot, "Text_Title", "Diamond Promotion Inventory", 30, TextAnchor.MiddleCenter);
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(880f, 46f), new Vector2(0f, -34f));

            Text messageText = CreateText(inventoryRoot, "Text_Message", "", 18, TextAnchor.MiddleCenter);
            SetRect(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(850f, 34f), new Vector2(0f, 30f));

            RectTransform slotPanel = CreatePanel(inventoryRoot, "Panel_Slots", new Color(0.07f, 0.09f, 0.11f, 0.92f));
            SetRect(slotPanel, new Vector2(0f, 0.5f), new Vector2(560f, 420f), new Vector2(42f, -4f));
            GridLayoutGroup grid = slotPanel.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(126f, 92f);
            grid.spacing = new Vector2(10f, 10f);
            grid.padding = new RectOffset(18, 18, 18, 18);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;

            DaniTechInventorySlotView[] slotViews = new DaniTechInventorySlotView[DaniTechInventoryModel.InventorySlotCount];
            for (int i = 0; i < slotViews.Length; i++)
            {
                slotViews[i] = CreateSlot(slotPanel, i);
            }

            RectTransform detailPanel = CreatePanel(inventoryRoot, "Panel_Detail", new Color(0.07f, 0.08f, 0.10f, 0.94f));
            SetRect(detailPanel, new Vector2(1f, 0.55f), new Vector2(330f, 360f), new Vector2(-42f, -8f));
            Text selectedText = CreateText(detailPanel, "Text_Selected", "", 22, TextAnchor.UpperLeft);
            SetRect(selectedText.rectTransform, new Vector2(0f, 1f), new Vector2(292f, 64f), new Vector2(18f, -18f));
            Text detailText = CreateText(detailPanel, "Text_Detail", "", 17, TextAnchor.UpperLeft);
            SetRect(detailText.rectTransform, new Vector2(0f, 1f), new Vector2(292f, 188f), new Vector2(18f, -90f));

            Button useButton = CreateButton(detailPanel, "Button_Use", "사용", new Vector2(18f, 40f), new Vector2(90f, 44f), new Color(0.12f, 0.36f, 0.25f, 1f));
            Button dropButton = CreateButton(detailPanel, "Button_Drop", "버리기", new Vector2(120f, 40f), new Vector2(90f, 44f), new Color(0.42f, 0.16f, 0.15f, 1f));
            Button closeButton = CreateButton(detailPanel, "Button_Close", "닫기", new Vector2(222f, 40f), new Vector2(90f, 44f), new Color(0.18f, 0.20f, 0.23f, 1f));

            RectTransform sortPanel = CreatePanel(inventoryRoot, "Panel_Sort", new Color(0.05f, 0.06f, 0.075f, 0.88f));
            SetRect(sortPanel, new Vector2(0.5f, 0f), new Vector2(610f, 68f), new Vector2(-150f, 98f));
            Button sortNameButton = CreateButton(sortPanel, "Button_SortName", "이름", new Vector2(18f, 12f), new Vector2(110f, 44f), new Color(0.18f, 0.22f, 0.26f, 1f));
            Button sortTypeButton = CreateButton(sortPanel, "Button_SortType", "타입", new Vector2(142f, 12f), new Vector2(110f, 44f), new Color(0.18f, 0.22f, 0.26f, 1f));
            Button sortSequenceButton = CreateButton(sortPanel, "Button_SortSequence", "획득순", new Vector2(266f, 12f), new Vector2(122f, 44f), new Color(0.18f, 0.22f, 0.26f, 1f));

            RectTransform dropTargetRoot = CreatePanel(sortPanel, "Panel_DropTarget", new Color(0.36f, 0.12f, 0.12f, 0.9f));
            SetRect(dropTargetRoot, new Vector2(1f, 0.5f), new Vector2(170f, 44f), new Vector2(-18f, 0f));
            Text dropTargetText = CreateText(dropTargetRoot, "Text_DropTarget", "Drop / 버리기", 16, TextAnchor.MiddleCenter);
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
            RectTransform slotRoot = CreatePanel(parent, "Slot_Item_" + slotIndex.ToString("00"), new Color(0.08f, 0.10f, 0.12f, 0.82f));
            Button button = slotRoot.gameObject.AddComponent<Button>();
            Text iconText = CreateText(slotRoot, "Text_Icon", "", 22, TextAnchor.MiddleCenter);
            SetRect(iconText.rectTransform, new Vector2(0.5f, 1f), new Vector2(104f, 30f), new Vector2(0f, -16f));
            Text nameText = CreateText(slotRoot, "Text_Name", "", 13, TextAnchor.MiddleCenter);
            SetRect(nameText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(112f, 32f), new Vector2(0f, -2f));
            Text amountText = CreateText(slotRoot, "Text_Amount", "", 14, TextAnchor.LowerRight);
            SetRect(amountText.rectTransform, new Vector2(1f, 0f), new Vector2(56f, 22f), new Vector2(-8f, 8f));
            Text cooldownText = CreateText(slotRoot, "Text_Cooldown", "", 13, TextAnchor.LowerLeft);
            SetRect(cooldownText.rectTransform, new Vector2(0f, 0f), new Vector2(62f, 22f), new Vector2(8f, 8f));

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
            settings.ActivePlayerDataBuilderIndex = 0;
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

            RegisterAddressable(settings, group, EmeraldItemPrefabPath, "DaniTech/Items/EmeraldCore");
            RegisterAddressable(settings, group, DiamondItemPrefabPath, "DaniTech/Items/DiamondTonic");
            RegisterAddressable(settings, group, ShieldItemPrefabPath, "DaniTech/Items/PrismShield");
            RegisterAddressable(settings, group, SwiftItemPrefabPath, "DaniTech/Items/SwiftSigil");
            RegisterAddressable(settings, group, UiPrefabPath, "DaniTech/UI/PromotionUI");
            RegisterAddressable(settings, group, UseEffectPrefabPath, "DaniTech/VFX/UseEffect");
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
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
            sun.color = new Color(1f, 0.92f, 0.82f, 1f);
            sun.intensity = 1.45f;
            sunObject.transform.rotation = Quaternion.Euler(52f, -38f, 0f);

            CreatePointLight(lightRoot.transform, "PointLight_DiamondGate", new Vector3(0f, 4f, 7f), new Color(0.42f, 0.92f, 1f, 1f), 2.4f, 11f);
            RenderSettings.ambientLight = new Color(0.22f, 0.25f, 0.28f, 1f);
        }

        private static void CreateEnvironment(Scene scene)
        {
            GameObject environmentRoot = CreateRoot(scene, "Environment_DiamondDistrict");
            Material groundMaterial = LoadMaterial("MAT_DaniTech_Ground");
            Material pathMaterial = LoadMaterial("MAT_DaniTech_Path");
            Material diamondMaterial = LoadMaterial("MAT_DaniTech_Diamond");
            Material lightMaterial = LoadMaterial("MAT_DaniTech_Light");

            CreatePrimitive(PrimitiveType.Cube, "Ground_MainPlaza", environmentRoot.transform, new Vector3(0f, -0.08f, 0f), new Vector3(28f, 0.16f, 28f), groundMaterial);
            CreatePrimitive(PrimitiveType.Cube, "Ground_DiamondPath_North", environmentRoot.transform, new Vector3(0f, 0.02f, 6.4f), new Vector3(4.2f, 0.08f, 13f), pathMaterial);
            CreatePrimitive(PrimitiveType.Cube, "Ground_DiamondPath_East", environmentRoot.transform, new Vector3(6.4f, 0.03f, 0f), new Vector3(13f, 0.08f, 4.2f), pathMaterial);

            CreateExternalModel("KB3D_BRK_BldgLG_C.fbx", environmentRoot.transform, new Vector3(-11f, 0f, 7f), Quaternion.Euler(0f, 30f, 0f), new Vector3(0.035f, 0.035f, 0.035f));
            CreateExternalModel("KB3D_BRK_BldgMD_F.fbx", environmentRoot.transform, new Vector3(11f, 0f, 5f), Quaternion.Euler(0f, -30f, 0f), new Vector3(0.04f, 0.04f, 0.04f));
            CreateExternalModel("KB3D_BRK_WaterTower_A.fbx", environmentRoot.transform, new Vector3(-6f, 0f, -8f), Quaternion.Euler(0f, 12f, 0f), new Vector3(0.08f, 0.08f, 0.08f));
            CreateExternalModel("KB3D_BRK_RooftopProp_A.fbx", environmentRoot.transform, new Vector3(7f, 0.1f, -8f), Quaternion.identity, new Vector3(0.12f, 0.12f, 0.12f));

            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f;
                Vector3 position = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, 8.8f);
                CreateDiamond("Gem_Pillar_" + i.ToString("00"), environmentRoot.transform, position + Vector3.up * 1.35f, new Vector3(0.7f, 1.2f, 0.7f), diamondMaterial);
                CreatePrimitive(PrimitiveType.Cylinder, "Pillar_Base_" + i.ToString("00"), environmentRoot.transform, position + Vector3.up * 0.4f, new Vector3(0.7f, 0.8f, 0.7f), LoadMaterial("MAT_DaniTech_DarkMetal"));
                CreatePointLight(environmentRoot.transform, "PointLight_Gem_" + i.ToString("00"), position + Vector3.up * 2.4f, lightMaterial.color, 1.1f, 5f);
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
            playerObject.AddComponent<DaniTechPlayerView>();
            DaniTechPlayerInteraction interaction = playerObject.AddComponent<DaniTechPlayerInteraction>();

            GameObject cameraObject = CreateEmpty("Camera_FirstPerson", playerObject.transform, new Vector3(0f, 1.58f, 0f));
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 70f;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 600f;
            camera.tag = "MainCamera";
            cameraObject.AddComponent<AudioListener>();

            SetObjectReference(interaction, "_cameraTransform", cameraObject.transform);
            return playerObject;
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
            SetObjectReference(spawner, "_itemRoot", itemRoot.transform);
            SetObjectReference(spawner, "_dropAnchor", playerObject.transform);
            SetObjectReference(spawner, "_useEffectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(UseEffectPrefabPath));
            return managerGroup;
        }

        private static void CreateSpawnPoints(Transform managerGroup)
        {
            Transform spawnRoot = CreateEmpty("SpawnPoints_ItemAddressables", managerGroup, Vector3.zero).transform;
            CreateSpawnPoint(spawnRoot, "Spawn_EmeraldCore_A", "emerald_core", 2, new Vector3(-3.4f, 0.55f, -0.4f));
            CreateSpawnPoint(spawnRoot, "Spawn_EmeraldCore_B", "emerald_core", 1, new Vector3(2.2f, 0.55f, -2.6f));
            CreateSpawnPoint(spawnRoot, "Spawn_DiamondTonic", "diamond_tonic", 1, new Vector3(3.7f, 0.55f, 2.8f));
            CreateSpawnPoint(spawnRoot, "Spawn_PrismShield", "prism_shield", 1, new Vector3(-4.4f, 0.55f, 3.2f));
            CreateSpawnPoint(spawnRoot, "Spawn_SwiftSigil", "swift_sigil", 1, new Vector3(0f, 0.55f, 5.4f));

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
            if (emission > 0f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * emission);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material LoadMaterial(string materialName)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(MaterialFolderPath + "/" + materialName + ".mat");
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

        private static void CreateExternalModel(string fileName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            string assetPath = ExternalFolderPath + "/KB3D_Brooklyn/" + fileName;
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
            ApplyMaterialToRenderers(modelObject, LoadMaterial("MAT_DaniTech_DarkMetal"));
        }

        private static void ApplyMaterialToRenderers(GameObject targetObject, Material material)
        {
            if (targetObject == null || material == null)
            {
                return;
            }

            Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial = material;
            }
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

        private static Image CreateBar(RectTransform parent, string objectName, Vector2 anchoredPosition, Color color)
        {
            RectTransform background = CreatePanel(parent, objectName + "_Bg", new Color(0f, 0f, 0f, 0.35f));
            SetRect(background, new Vector2(0f, 1f), new Vector2(386f, 10f), anchoredPosition);

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
            Text labelText = CreateText(buttonRoot, "Text_" + objectName, label, 17, TextAnchor.MiddleCenter);
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
