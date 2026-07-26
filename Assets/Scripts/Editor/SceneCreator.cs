using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Creates Petal Drift scenes programmatically for CI (no Unity Editor needed).
/// Called by BuildScript before the actual build.
/// </summary>
public static class SceneCreator
{
    private const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    /// <summary>
    /// Creates both scenes and registers them in Build Settings.
    /// </summary>
    public static void CreateAllScenes()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        RegisterTags();

        CreateMainMenuScene();
        CreateGameScene();

        // Create prefabs and assign them to GameController
        CreatePrefabs();

        // Register scenes in build settings
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MainMenuPath, true),
            new EditorBuildSettingsScene(GameScenePath, true),
        };

        Debug.Log("✓ All scenes and prefabs created and registered in Build Settings");
    }

    private static void RegisterTags()
    {
        // Register required tags for the game
        string[] requiredTags = { "Player", "Flower", "Hazard", "WindZone" };
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tagsProp = tagManager.FindProperty("tags");

        foreach (string tag in requiredTags)
        {
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
                Debug.Log($"✓ Registered tag: {tag}");
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private static void CreatePrefabs()
    {
        System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs");

        // Flower prefab
        var flowerGO = new GameObject("Flower", typeof(Rigidbody2D), typeof(CircleCollider2D));
        flowerGO.tag = "Flower";
        var fRB = flowerGO.GetComponent<Rigidbody2D>();
        fRB.gravityScale = 0f;
        var fCol = flowerGO.GetComponent<CircleCollider2D>();
        fCol.radius = 0.5f;
        fCol.isTrigger = true;
        var fSpr = new GameObject("FlowerSprite");
        fSpr.transform.SetParent(flowerGO.transform, false);
        fSpr.AddComponent<SpriteRenderer>();
        flowerGO.AddComponent<Flower>();
        var flowerPrefab = PrefabUtility.SaveAsPrefabAsset(flowerGO, "Assets/Resources/Prefabs/Flower.prefab");
        Object.DestroyImmediate(flowerGO);

        // Hazard prefab
        var hazardGO = new GameObject("Hazard", typeof(Rigidbody2D), typeof(BoxCollider2D));
        hazardGO.tag = "Hazard";
        var hRB = hazardGO.GetComponent<Rigidbody2D>();
        hRB.gravityScale = 0f;
        var hCol = hazardGO.GetComponent<BoxCollider2D>();
        hCol.size = new Vector2(2f, 1f);
        hCol.isTrigger = true;
        var hSpr = new GameObject("HazardSprite");
        hSpr.transform.SetParent(hazardGO.transform, false);
        hSpr.AddComponent<SpriteRenderer>();
        hazardGO.AddComponent<Hazard>();
        var hazardPrefab = PrefabUtility.SaveAsPrefabAsset(hazardGO, "Assets/Resources/Prefabs/Hazard.prefab");
        Object.DestroyImmediate(hazardGO);

        // WindZone prefab
        var windGO = new GameObject("WindZone", typeof(Rigidbody2D), typeof(BoxCollider2D));
        windGO.tag = "WindZone";
        var wRB = windGO.GetComponent<Rigidbody2D>();
        wRB.gravityScale = 0f;
        wRB.isKinematic = true;
        var wCol = windGO.GetComponent<BoxCollider2D>();
        wCol.size = new Vector2(4f, 2f);
        wCol.isTrigger = true;
        var wSpr = new GameObject("WindSprite");
        wSpr.transform.SetParent(windGO.transform, false);
        wSpr.AddComponent<SpriteRenderer>();
        windGO.AddComponent<WindZone>();
        var windPrefab = PrefabUtility.SaveAsPrefabAsset(windGO, "Assets/Resources/Prefabs/WindZone.prefab");
        Object.DestroyImmediate(windGO);

        // Assign prefabs to GameController in the Game scene
        var gameScene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Additive);
        var gc = Object.FindObjectOfType<GameController>();
        if (gc != null)
        {
            gc.flowerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Flower.prefab");
            gc.hazardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Hazard.prefab");
            gc.windZonePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/WindZone.prefab");
            EditorSceneManager.MarkSceneDirty(gameScene);
        }
        EditorSceneManager.SaveScene(gameScene);
        EditorSceneManager.CloseScene(gameScene, true);

        Debug.Log("✓ Created 3 prefabs: Flower, Hazard, WindZone");
    }

    private static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Main Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = new Color(0.02f, 0.01f, 0.06f);
        camGO.tag = "MainCamera";

        // EventSystem (required for UI)
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();

        // Background
        var bgGO = new GameObject("Background");
        bgGO.AddComponent<Background>();

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Title
        var titleGO = new GameObject("TitleLabel");
        titleGO.transform.SetParent(canvasGO.transform, false);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "Petal Drift";
        titleText.fontSize = 72;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = new Color(1f, 0.85f, 0.4f);
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(600, 100);

        // Start label
        var startGO = new GameObject("StartLabel");
        startGO.transform.SetParent(canvasGO.transform, false);
        var startText = startGO.AddComponent<Text>();
        startText.text = "Tap to Start";
        startText.fontSize = 36;
        startText.alignment = TextAnchor.MiddleCenter;
        startText.color = Color.white;
        var startRect = startGO.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0.45f);
        startRect.anchorMax = new Vector2(0.5f, 0.45f);
        startRect.pivot = new Vector2(0.5f, 0.5f);
        startRect.sizeDelta = new Vector2(400, 80);

        // High score label
        var hsGO = new GameObject("HighScoreLabel");
        hsGO.transform.SetParent(canvasGO.transform, false);
        var hsText = hsGO.AddComponent<Text>();
        hsText.text = "Best: 0";
        hsText.fontSize = 28;
        hsText.alignment = TextAnchor.MiddleCenter;
        hsText.color = new Color(0.7f, 0.7f, 0.9f);
        var hsRect = hsGO.GetComponent<RectTransform>();
        hsRect.anchorMin = new Vector2(0.5f, 0.35f);
        hsRect.anchorMax = new Vector2(0.5f, 0.35f);
        hsRect.pivot = new Vector2(0.5f, 0.5f);
        hsRect.sizeDelta = new Vector2(400, 60);

        // Instructions
        var instrGO = new GameObject("Instructions");
        instrGO.transform.SetParent(canvasGO.transform, false);
        var instrText = instrGO.AddComponent<Text>();
        instrText.text = "Left side ← → Right side\nRelease to float";
        instrText.fontSize = 22;
        instrText.alignment = TextAnchor.MiddleCenter;
        instrText.color = new Color(0.5f, 0.5f, 0.7f);
        var instrRect = instrGO.GetComponent<RectTransform>();
        instrRect.anchorMin = new Vector2(0.5f, 0.15f);
        instrRect.anchorMax = new Vector2(0.5f, 0.15f);
        instrRect.pivot = new Vector2(0.5f, 0.5f);
        instrRect.sizeDelta = new Vector2(500, 80);

        // MainMenu controller
        var mmGO = new GameObject("MainMenu");
        var mm = mmGO.AddComponent<MainMenu>();
        mm.highScoreText = hsText;
        mm.startText = startText;

        EditorSceneManager.SaveScene(scene, MainMenuPath);
        Debug.Log("✓ Created MainMenu scene");
    }

    private static void CreateGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Main Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = new Color(0.02f, 0.01f, 0.06f);
        camGO.tag = "MainCamera";

        // EventSystem
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();

        // Background
        var bgGO = new GameObject("Background");
        bgGO.AddComponent<Background>();

        // Player
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        var playerRB = playerGO.AddComponent<Rigidbody2D>();
        playerRB.gravityScale = 0f;
        playerRB.freezeRotation = true;
        var playerCol = playerGO.AddComponent<CircleCollider2D>();
        playerCol.radius = 0.5f;
        playerCol.isTrigger = true;
        var playerSprite = new GameObject("PetalSprite");
        playerSprite.transform.SetParent(playerGO.transform, false);
        var spr = playerSprite.AddComponent<SpriteRenderer>();
        spr.color = new Color(1f, 0.85f, 0.4f);
        playerGO.AddComponent<Player>();

        // Spawner parents
        var flowerSpawner = new GameObject("FlowerSpawner");
        var hazardSpawner = new GameObject("HazardSpawner");
        var windSpawner = new GameObject("WindSpawner");

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Score (top-left)
        var scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(canvasGO.transform, false);
        var scoreText = scoreGO.AddComponent<Text>();
        scoreText.text = "0";
        scoreText.fontSize = 48;
        scoreText.alignment = TextAnchor.UpperLeft;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.color = Color.white;
        var scoreRect = scoreGO.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(30, -30);
        scoreRect.sizeDelta = new Vector2(200, 80);

        // Combo (center)
        var comboGO = new GameObject("ComboText");
        comboGO.transform.SetParent(canvasGO.transform, false);
        var comboText = comboGO.AddComponent<Text>();
        comboText.text = "";
        comboText.fontSize = 72;
        comboText.alignment = TextAnchor.MiddleCenter;
        comboText.fontStyle = FontStyle.Bold;
        comboText.color = new Color(1f, 0.85f, 0.4f);
        var comboRect = comboGO.GetComponent<RectTransform>();
        comboRect.anchorMin = new Vector2(0.5f, 0.5f);
        comboRect.anchorMax = new Vector2(0.5f, 0.5f);
        comboRect.pivot = new Vector2(0.5f, 0.5f);
        comboRect.anchoredPosition = new Vector2(0, 0);
        comboRect.sizeDelta = new Vector2(400, 100);

        // GameOver Panel
        var panelGO = new GameObject("GameOverPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panel = panelGO.AddComponent<Image>();
        panel.color = new Color(0, 0, 0, 0.6f);
        var panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelGO.SetActive(false);

        // Final score label
        var finalGO = new GameObject("FinalScoreLabel");
        finalGO.transform.SetParent(panelGO.transform, false);
        var finalText = finalGO.AddComponent<Text>();
        finalText.text = "Score: 0";
        finalText.fontSize = 52;
        finalText.alignment = TextAnchor.MiddleCenter;
        finalText.fontStyle = FontStyle.Bold;
        finalText.color = Color.white;
        var finalRect = finalGO.GetComponent<RectTransform>();
        finalRect.anchorMin = new Vector2(0.5f, 0.6f);
        finalRect.anchorMax = new Vector2(0.5f, 0.6f);
        finalRect.pivot = new Vector2(0.5f, 0.5f);
        finalRect.sizeDelta = new Vector2(500, 80);

        // High score label (game over)
        var hsGO = new GameObject("HighScoreLabel");
        hsGO.transform.SetParent(panelGO.transform, false);
        var hsText = hsGO.AddComponent<Text>();
        hsText.text = "Best: 0";
        hsText.fontSize = 32;
        hsText.alignment = TextAnchor.MiddleCenter;
        hsText.color = new Color(1f, 0.85f, 0.4f);
        var hsRect = hsGO.GetComponent<RectTransform>();
        hsRect.anchorMin = new Vector2(0.5f, 0.45f);
        hsRect.anchorMax = new Vector2(0.5f, 0.45f);
        hsRect.pivot = new Vector2(0.5f, 0.5f);
        hsRect.sizeDelta = new Vector2(400, 60);

        // Tap to restart
        var tapGO = new GameObject("TapToRestart");
        tapGO.transform.SetParent(panelGO.transform, false);
        var tapText = tapGO.AddComponent<Text>();
        tapText.text = "Tap to Restart";
        tapText.fontSize = 30;
        tapText.alignment = TextAnchor.MiddleCenter;
        tapText.color = Color.white;
        var tapRect = tapGO.GetComponent<RectTransform>();
        tapRect.anchorMin = new Vector2(0.5f, 0.3f);
        tapRect.anchorMax = new Vector2(0.5f, 0.3f);
        tapRect.pivot = new Vector2(0.5f, 0.5f);
        tapRect.sizeDelta = new Vector2(400, 60);

        // GameManager (persistent singleton)
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        // GameController
        var gcGO = new GameObject("GameController");
        var gc = gcGO.AddComponent<GameController>();
        gc.flowerSpawner = flowerSpawner.transform;
        gc.hazardSpawner = hazardSpawner.transform;
        gc.windSpawner = windSpawner.transform;

        // GameUI
        var guiGO = new GameObject("GameUI");
        var gui = guiGO.AddComponent<GameUI>();
        gui.scoreText = scoreText;
        gui.comboText = comboText;
        gui.gameOverPanel = panelGO;
        gui.finalScoreText = finalText;
        gui.highScoreText = hsText;
        gui.tapToRestartText = tapText;

        // Auto-add Player tag (in case it doesn't exist)
        try { playerGO.tag = "Player"; } catch { Debug.LogWarning("Create 'Player' tag manually later"); }

        EditorSceneManager.SaveScene(scene, GameScenePath);
        Debug.Log("✓ Created Game scene");
    }
}
