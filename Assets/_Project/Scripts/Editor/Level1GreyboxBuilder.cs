#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SiagaBencana.Core;
using SiagaBencana.Gameplay;
using SiagaBencana.UI;

namespace SiagaBencana.Editor
{
    public static class Level1GreyboxBuilder
    {
        static Material matFloor, matFloorTile, matWall, matCeiling, matTable, matBed;
        static Material matSafe, matPlayer, matPlayerSkin, matPlayerHair;
        static Material matDoor, matWindow;
        static Material matGrass, matSofa, matTV, matRug, matPillow, matWardrobeDark, matStove;

        [MenuItem("Tools/Siaga Bencana/Generate Level 1 (Rumah - Gempa)")]
        public static void Generate()
        {
            if (!EditorUtility.DisplayDialog("Siaga Bencana",
                "Generate ulang layout Level 1 (Rumah)?",
                "Buat Sekarang", "Batal")) return;

            var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var r in roots)
            {
                if (r.name.Contains("LEVEL 1") || r.name.Contains("Canvas") || r.name.Contains("EventSystem") || r.name.Contains("Managers"))
                {
                    Object.DestroyImmediate(r);
                }
            }

            CreateMaterials();

            var root = new GameObject("[LEVEL 1 - RUMAH GEMPA]");
            Undo.RegisterCreatedObjectUndo(root, "Generate Level 1");

            var playerTf = BuildHouse(root.transform);
            BuildDisasterSystem(root.transform);
            BuildManagers(root.transform);
            BuildCamera(root.transform, playerTf);
            BuildSafeZone(root.transform);
            BuildUI(root.transform);
            BuildLighting(root.transform);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = playerTf.gameObject;
            Debug.Log("[Siaga Bencana] Level 1 berhasil dibuat.");
        }

        static void CreateMaterials()
        {
            matFloor        = GetOrCreateMat("Mat_Floor",        new Color(0.85f, 0.72f, 0.55f));
            matFloorTile    = GetOrCreateMat("Mat_FloorTile",    new Color(0.90f, 0.88f, 0.84f));
            matGrass        = GetOrCreateMat("Mat_Grass",        new Color(0.40f, 0.72f, 0.30f));
            matWall         = GetOrCreateMat("Mat_Wall",         new Color(0.96f, 0.94f, 0.88f));
            matCeiling      = GetOrCreateMat("Mat_Ceiling",      new Color(0.97f, 0.97f, 0.95f));
            matDoor         = GetOrCreateMat("Mat_Door",         new Color(0.50f, 0.30f, 0.12f));
            matWindow       = GetOrCreateMat("Mat_Window",       new Color(0.65f, 0.85f, 1.00f, 0.5f), true);
            matTable        = GetOrCreateMat("Mat_Table",        new Color(0.52f, 0.30f, 0.12f));
            matBed          = GetOrCreateMat("Mat_Bed",          new Color(0.30f, 0.48f, 0.78f));
            matPillow       = GetOrCreateMat("Mat_Pillow",       new Color(0.95f, 0.95f, 1.00f));
            matSofa         = GetOrCreateMat("Mat_Sofa",         new Color(0.55f, 0.25f, 0.15f));
            matTV           = GetOrCreateMat("Mat_TV",           new Color(0.12f, 0.12f, 0.12f));
            matRug          = GetOrCreateMat("Mat_Rug",          new Color(0.70f, 0.20f, 0.20f));
            matWardrobeDark = GetOrCreateMat("Mat_WardrobeDark", new Color(0.30f, 0.18f, 0.08f));
            matStove        = GetOrCreateMat("Mat_Stove",        new Color(0.22f, 0.22f, 0.26f));
            matPlayer       = GetOrCreateMat("Mat_PlayerShirt",  new Color(0.20f, 0.45f, 0.85f));
            matPlayerSkin   = GetOrCreateMat("Mat_PlayerSkin",   new Color(0.96f, 0.78f, 0.62f));
            matPlayerHair   = GetOrCreateMat("Mat_PlayerHair",   new Color(0.25f, 0.16f, 0.08f));
            matSafe         = GetOrCreateMat("Mat_SafeZone",     new Color(0.20f, 0.85f, 0.30f, 0.45f), true);
        }

        static Transform BuildHouse(Transform parent)
        {
            var env = new GameObject("Environment").transform;
            env.SetParent(parent);

            float wH = 4.5f;
            float wY = 1.25f;
            float wT = 0.8f;

            // Ground & floor
            Box("Ground_Surrounding", V3(0, -0.55f, 4), V3(120f, 1.0f, 120f), matGrass, env);
            Box("Floor_Solid_Home", V3(0, -0.5f, 0), V3(18.5f, 1.0f, 14.5f), matFloor, env);
            Box("Rug", V3(4, 0.01f, 2), V3(4, 0.02f, 3), matRug, env, true);

            // Dinding luar
            Box("Wall_Back",   V3(0, wY, -7.3f),    V3(19f, wH, wT), matWall, env);
            Box("Wall_Left",   V3(-9.3f, wY, 0),    V3(wT, wH, 15f), matWall, env);
            Box("Wall_Right",  V3(9.3f, wY, 0),     V3(wT, wH, 15f), matWall, env);
            Box("Wall_FrontL", V3(-5.5f, wY, 7.3f), V3(7.5f, wH, wT), matWall, env);
            Box("Wall_FrontR", V3(5.5f, wY, 7.3f),  V3(7.5f, wH, wT), matWall, env);

            // Partisi kamar tidur
            Box("Part_Bedroom_Back",  V3(-3, wY, -3.5f), V3(wT, wH, 7.5f), matWall, env);
            Box("Part_Bedroom_Front", V3(-3, wY, 4.5f),  V3(wT, wH, 5.0f), matWall, env);
            Box("Door_Lintel",        V3(-3, 3.0f, 1.125f), V3(wT, 1.0f, 1.75f), matWall, env);

            BuildTable_CoverZone(env);

            // Perabot
            Box("Sofa", V3(5f, 0.3f, 4.5f), V3(3f, 0.6f, 1f), matSofa, env);
            Box("TV", V3(5f, 0.8f, -0.5f), V3(2f, 1f, 0.2f), matTV, env);
            Box("Bed", V3(-6.5f, 0.3f, -3.5f), V3(2.5f, 0.6f, 3.5f), matBed, env);
            Box("KitchenCounter", V3(6.5f, 0.5f, -5.5f), V3(5f, 1f, 0.9f), matStove, env);

            // Player
            var playerTf = BuildBlockyCharacter("Player", V3(2, 0.05f, 2),
                matPlayer, matPlayerSkin, matPlayerHair, 1.0f, parent);
            var pc = playerTf.gameObject;
            var cc = pc.AddComponent<CharacterController>();
            cc.height = 1.8f; 
            cc.radius = 0.32f; 
            cc.center = V3(0, 0.9f, 0);
            cc.skinWidth = 0.02f;
            cc.stepOffset = 0.15f;
            cc.minMoveDistance = 0f;
            pc.AddComponent<PlayerController>();

            return playerTf;
        }

        static void BuildTable_CoverZone(Transform env)
        {
            var table = Box("Table_CoverZone", V3(3f, 0.88f, 2f), V3(2.4f, 0.12f, 1.4f), matTable, env);
            var coverCol = table.GetComponent<BoxCollider>();
            coverCol.isTrigger = true;
            coverCol.size = new Vector3(1.8f, 1.5f, 1.4f);
            coverCol.center = new Vector3(0, -0.3f, 0);
            table.AddComponent<CoverZone>();

            // Kaki meja tanpa collider agar karakter leluasa merunduk di bawah meja
            Box("TL1", V3(2.1f, 0.44f, 1.45f), V3(0.14f, 0.88f, 0.14f), matTable, env, true);
            Box("TL2", V3(3.9f, 0.44f, 1.45f), V3(0.14f, 0.88f, 0.14f), matTable, env, true);
            Box("TL3", V3(2.1f, 0.44f, 2.55f), V3(0.14f, 0.88f, 0.14f), matTable, env, true);
            Box("TL4", V3(3.9f, 0.44f, 2.55f), V3(0.14f, 0.88f, 0.14f), matTable, env, true);
        }

        static Transform BuildBlockyCharacter(string name, Vector3 pos,
            Material shirtMat, Material skinMat, Material hairMat, float scale, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.position = pos;
            root.transform.SetParent(parent);

            var visual = new GameObject("Visual");
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = Vector3.one * scale;

            Box("Head", V3(0, 1.65f, 0), V3(0.52f, 0.52f, 0.52f), skinMat, visual.transform, true);
            if (hairMat != null)
                Box("Hair", V3(0, 1.93f, 0), V3(0.54f, 0.08f, 0.54f), hairMat, visual.transform, true);

            var eyePupil = GetOrCreateMat("Mat_EyePupil", new Color(0.1f, 0.08f, 0.05f));
            Box("EyeL", V3(-0.13f, 1.69f, 0.28f), V3(0.08f, 0.08f, 0.02f), eyePupil, visual.transform, true);
            Box("EyeR", V3( 0.13f, 1.69f, 0.28f), V3(0.08f, 0.08f, 0.02f), eyePupil, visual.transform, true);

            Box("Body", V3(0, 1.10f, 0), V3(0.52f, 0.62f, 0.30f), shirtMat, visual.transform, true);
            Box("ArmL", V3(-0.39f, 1.10f, 0), V3(0.22f, 0.60f, 0.26f), shirtMat, visual.transform, true);
            Box("ArmR", V3( 0.39f, 1.10f, 0), V3(0.22f, 0.60f, 0.26f), shirtMat, visual.transform, true);

            var legMat = GetOrCreateMat("Mat_Pants_" + name, new Color(0.18f, 0.18f, 0.32f));
            Box("LegL", V3(-0.13f, 0.45f, 0), V3(0.22f, 0.76f, 0.24f), legMat, visual.transform, true);
            Box("LegR", V3( 0.13f, 0.45f, 0), V3(0.22f, 0.76f, 0.24f), legMat, visual.transform, true);

            var shoeMat = GetOrCreateMat("Mat_Shoes_" + name, new Color(0.10f, 0.10f, 0.12f));
            Box("ShoeL", V3(-0.13f, 0.05f, 0.02f), V3(0.24f, 0.10f, 0.28f), shoeMat, visual.transform, true);
            Box("ShoeR", V3( 0.13f, 0.05f, 0.02f), V3(0.24f, 0.10f, 0.28f), shoeMat, visual.transform, true);

            return root.transform;
        }

        static void BuildDisasterSystem(Transform parent)
        {
            var root = new GameObject("Disaster_Earthquake");
            root.transform.SetParent(parent);
            root.AddComponent<EarthquakeController>();
        }

        static void BuildManagers(Transform parent)
        {
            var mgr = new GameObject("Managers");
            mgr.transform.SetParent(parent);
            mgr.AddComponent<GameManager>();
            mgr.AddComponent<LevelManager>();
        }

        static void BuildCamera(Transform parent, Transform playerTf)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var co = new GameObject("Main Camera");
                cam = co.AddComponent<Camera>();
                co.tag = "MainCamera";
                co.AddComponent<AudioListener>();
            }
            cam.transform.SetParent(parent);
            cam.transform.position = playerTf.position + V3(0, 9, -6);
            cam.transform.rotation = Quaternion.Euler(55, 0, 0);

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.40f, 0.72f, 0.30f);

            var followCam = cam.gameObject.GetComponent<CameraFollowController>() ??
                            cam.gameObject.AddComponent<CameraFollowController>();
            followCam.SetTarget(playerTf);
        }

        static void BuildSafeZone(Transform parent)
        {
            var sz = Box("SafeZone", V3(0, 0.02f, 13), V3(7, 0.05f, 5), matSafe, parent);
            sz.GetComponent<BoxCollider>().isTrigger = true;
            sz.AddComponent<SafeZone>();

            Box("Sign_Board", V3(0, 2.5f, 10.4f), V3(4f, 0.7f, 0.1f),
                GetOrCreateMat("Mat_Sign", new Color(0.1f, 0.65f, 0.2f)), parent);
        }

        static void BuildLighting(Transform parent)
        {
            var existingLight = Object.FindAnyObjectByType<Light>();
            Light dirLight = (existingLight != null && existingLight.type == LightType.Directional) 
                ? existingLight 
                : new GameObject("Directional Light").AddComponent<Light>();

            dirLight.type = LightType.Directional;
            dirLight.transform.SetParent(parent);
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            dirLight.intensity = 1.2f;
        }

        static void BuildUI(Transform parent)
        {
            var canvasObj = new GameObject("Canvas_SiagaBencana");
            canvasObj.transform.SetParent(parent);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.transform.SetParent(parent);
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            var uiMgr = canvasObj.AddComponent<UIManager>();

            // 1. TUTORIAL PANEL
            var tutPanel = MakePanel("Tutorial_Panel", canvasObj.transform, new Color(0.08f, 0.08f, 0.12f, 0.95f));
            var tutTitle = MakeTMP("TutTitle", tutPanel.transform, "Simulasi Siaga Bencana", 34, V2(0, 180), V2(700, 60), TextAlignmentOptions.Center, Color.white);
            var tutDesc = MakeTMP("TutDesc", tutPanel.transform, "", 22, V2(0, 60), V2(680, 150), TextAlignmentOptions.Center, Color.white);
            var tutCtrl = MakeTMP("TutCtrl", tutPanel.transform, "", 26, V2(0, -60), V2(550, 60), TextAlignmentOptions.Center, new Color(0.3f, 1f, 0.5f));
            var tutStep = MakeTMP("TutStep", tutPanel.transform, "1/4", 18, V2(0, -130), V2(200, 30), TextAlignmentOptions.Center, Color.gray);
            var (tutBtn, tutBtnTxt) = MakeButton("TutNextBtn", tutPanel.transform, "Lanjut >>", V2(0, -220), V2(260, 60), new Color(0.15f, 0.65f, 0.35f));

            // HUD
            var hudPanel = MakePanel("HUD_Panel", canvasObj.transform, new Color(0, 0, 0, 0));
            var timerText = MakeTMP("Timer", hudPanel.transform, "00:15", 32, V2(0, -30), V2(200, 45), TextAlignmentOptions.Center, Color.white);
            timerText.GetComponent<RectTransform>().anchorMin = V2(0.5f, 1);
            timerText.GetComponent<RectTransform>().anchorMax = V2(0.5f, 1);

            var phaseText = MakeTMP("Phase", hudPanel.transform, "PERSIAPAN", 20, V2(0, -75), V2(500, 35), TextAlignmentOptions.Center, Color.yellow);
            phaseText.GetComponent<RectTransform>().anchorMin = V2(0.5f, 1);
            phaseText.GetComponent<RectTransform>().anchorMax = V2(0.5f, 1);

            var warnBanner = new GameObject("WarningBanner");
            warnBanner.transform.SetParent(hudPanel.transform, false);
            var warnRect = warnBanner.AddComponent<RectTransform>();
            warnRect.anchorMin = V2(0.5f, 0.5f); warnRect.anchorMax = V2(0.5f, 0.5f);
            warnRect.anchoredPosition = V2(0, 200);
            warnRect.sizeDelta = V2(750, 55);
            var warnBg = warnBanner.AddComponent<Image>();
            warnBg.color = new Color(0.8f, 0.1f, 0.1f, 0.85f);
            var warnText = MakeTMP("WarnText", warnBanner.transform, "", 22, V2(0, 0), V2(720, 50), TextAlignmentOptions.Center, Color.white);

            // Decision
            var decPanel = MakePanel("Decision_Panel", canvasObj.transform, new Color(0, 0, 0, 0.88f));
            var decQ = MakeTMP("DecQuestion", decPanel.transform, "", 26, V2(0, 140), V2(750, 100), TextAlignmentOptions.Center, Color.yellow);
            var (cBtn1, cTxt1) = MakeButton("Choice1", decPanel.transform, "", V2(0, 20), V2(700, 65), new Color(0.15f, 0.45f, 0.85f));
            var (cBtn2, cTxt2) = MakeButton("Choice2", decPanel.transform, "", V2(0, -70), V2(700, 65), new Color(0.25f, 0.25f, 0.35f));

            // Feedback
            var fbPanel = MakePanel("Feedback_Panel", canvasObj.transform, new Color(0, 0, 0, 0.92f));
            var fbTitle = MakeTMP("FBTitle", fbPanel.transform, "", 30, V2(0, 130), V2(700, 60), TextAlignmentOptions.Center, Color.white);
            var fbDesc = MakeTMP("FBDesc", fbPanel.transform, "", 20, V2(0, 10), V2(680, 140), TextAlignmentOptions.Center, new Color(0.9f, 0.9f, 0.9f));
            var (fbBtn, _) = MakeButton("FBBtn", fbPanel.transform, "Lanjutkan Simulasi", V2(0, -140), V2(280, 55), new Color(0.15f, 0.6f, 0.3f));

            // Debrief
            var dbPanel = MakePanel("Debrief_Panel", canvasObj.transform, new Color(0, 0, 0, 0.9f));
            var dbTitle = MakeTMP("DbTitle", dbPanel.transform, "EVAKUASI SELESAI!", 36, V2(0, 140), V2(600, 60), TextAlignmentOptions.Center, Color.green);
            var dbStatus = MakeTMP("DbStatus", dbPanel.transform, "", 22, V2(0, 20), V2(650, 100), TextAlignmentOptions.Center, Color.white);
            var (dbRestart, _) = MakeButton("DbRestart", dbPanel.transform, "Main Lagi (Ulangi)", V2(0, -140), V2(260, 55), new Color(0.2f, 0.5f, 0.9f));

            // Wiring
            var so = new SerializedObject(uiMgr);
            so.FindProperty("_tutorialPanel").objectReferenceValue = tutPanel;
            so.FindProperty("_tutorialTitle").objectReferenceValue = tutTitle;
            so.FindProperty("_tutorialDesc").objectReferenceValue = tutDesc;
            so.FindProperty("_tutorialControl").objectReferenceValue = tutCtrl;
            so.FindProperty("_tutorialStepCounter").objectReferenceValue = tutStep;
            so.FindProperty("_tutorialNextBtn").objectReferenceValue = tutBtn;
            so.FindProperty("_tutorialNextBtnText").objectReferenceValue = tutBtnTxt;

            so.FindProperty("_hudPanel").objectReferenceValue = hudPanel;
            so.FindProperty("_timerText").objectReferenceValue = timerText;
            so.FindProperty("_phaseText").objectReferenceValue = phaseText;
            so.FindProperty("_warningBanner").objectReferenceValue = warnBanner;
            so.FindProperty("_warningText").objectReferenceValue = warnText;

            so.FindProperty("_decisionPanel").objectReferenceValue = decPanel;
            so.FindProperty("_decisionQuestionText").objectReferenceValue = decQ;
            so.FindProperty("_choiceBtn1").objectReferenceValue = cBtn1;
            so.FindProperty("_choiceTxt1").objectReferenceValue = cTxt1;
            so.FindProperty("_choiceBtn2").objectReferenceValue = cBtn2;
            so.FindProperty("_choiceTxt2").objectReferenceValue = cTxt2;

            so.FindProperty("_feedbackPanel").objectReferenceValue = fbPanel;
            so.FindProperty("_feedbackTitle").objectReferenceValue = fbTitle;
            so.FindProperty("_feedbackDesc").objectReferenceValue = fbDesc;
            so.FindProperty("_feedbackContinueBtn").objectReferenceValue = fbBtn;

            so.FindProperty("_debriefPanel").objectReferenceValue = dbPanel;
            so.FindProperty("_debriefTitle").objectReferenceValue = dbTitle;
            so.FindProperty("_debriefStatus").objectReferenceValue = dbStatus;
            so.FindProperty("_debriefRestartBtn").objectReferenceValue = dbRestart;

            so.ApplyModifiedProperties();
        }

        static Vector3 V3(float x, float y, float z) => new Vector3(x, y, z);
        static Vector2 V2(float x, float y) => new Vector2(x, y);

        static GameObject Box(string name, Vector3 pos, Vector3 size, Material mat, Transform parent, bool removeCollider = false)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = pos;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = size;
            if (mat != null) obj.GetComponent<Renderer>().sharedMaterial = mat;
            if (removeCollider)
            {
                var col = obj.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }
            return obj;
        }

        static Material GetOrCreateMat(string name, Color color, bool transparent = false)
        {
            string path = $"Assets/_Project/Art/Materials/{name}.mat";
            var ex = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (ex != null) return ex;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            if (transparent)
            {
                mat.SetFloat("_Surface", 1);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        static GameObject MakePanel(string name, Transform parent, Color bgColor)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var r = panel.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.sizeDelta = Vector2.zero;
            if (bgColor.a > 0.01f)
            {
                var img = panel.AddComponent<Image>();
                img.color = bgColor;
            }
            return panel;
        }

        static TextMeshProUGUI MakeTMP(string name, Transform parent, string text, float size, Vector2 pos, Vector2 sizeDelta, TextAlignmentOptions align, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var r = obj.AddComponent<RectTransform>();
            r.anchoredPosition = pos;
            r.sizeDelta = sizeDelta;
            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = color;
            return tmp;
        }

        static (Button, TextMeshProUGUI) MakeButton(string name, Transform parent, string text, Vector2 pos, Vector2 sizeDelta, Color bgColor)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var r = obj.AddComponent<RectTransform>();
            r.anchoredPosition = pos;
            r.sizeDelta = sizeDelta;
            var img = obj.AddComponent<Image>();
            img.color = bgColor;
            var btn = obj.AddComponent<Button>();

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(obj.transform, false);
            var tR = txtObj.AddComponent<RectTransform>();
            tR.anchorMin = Vector2.zero; tR.anchorMax = Vector2.one; tR.sizeDelta = Vector2.zero;
            var tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return (btn, tmp);
        }
    }
}
#endif
