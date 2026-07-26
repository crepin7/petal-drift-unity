using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

/// <summary>
/// Build script for CI/CD (game-ci).
/// In CI: first creates all scenes + prefabs, then builds the APK.
/// </summary>
public class BuildScript
{
    private static readonly string[] Scenes = new[]
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Game.unity",
    };

    [MenuItem("Build/Create Scenes and Prefabs")]
    public static void CreateScenes()
    {
        SceneCreator.CreateAllScenes();
        Debug.Log("✓ Scenes and prefabs created. You can now build.");
    }

    [MenuItem("Build/Build Android APK")]
    public static void PerformBuild()
    {
        PerformBuild(BuildTarget.Android);
    }

    public static void PerformBuild(BuildTarget target)
    {
        // Step 1: Create scenes and prefabs
        Debug.Log("=== Step 1: Creating scenes and prefabs ===");
        SceneCreator.CreateAllScenes();

        // Step 2: Build
        string buildPath = GetBuildPath(target);
        Debug.Log($"=== Step 2: Building {target} → {buildPath} ===");

        string[] validScenes = Scenes.Where(s => System.IO.File.Exists(s)).ToArray();
        if (validScenes.Length == 0)
        {
            Debug.LogError("No scene files found! Scene creation may have failed.");
            EditorApplication.Exit(1);
            return;
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = validScenes,
            locationPathName = buildPath,
            target = target,
            options = BuildOptions.None,
        };

        // Set Android-specific settings
        if (target == BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.bundleVersion = "1.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.nousresearch.petaldrift");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        }

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        Debug.Log($"Build result: {summary.result}");
        Debug.Log($"Total size: {summary.totalSize} bytes");
        Debug.Log($"Total time: {summary.totalTime}");

        if (summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"Build failed with {summary.totalErrors} error(s)");
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("✓ Build succeeded!");
        }
    }

    private static string GetBuildPath(BuildTarget target)
    {
        string suffix = target == BuildTarget.Android ? ".apk" : "";
        string path = System.IO.Path.Combine("Builds", target.ToString(), "petal-drift" + suffix);
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        return path;
    }
}
