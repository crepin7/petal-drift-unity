using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

/// <summary>
/// Build script for CI/CD (game-ci).
/// Usage: unity -batchmode -quit -executeMethod BuildScript.PerformBuild
/// </summary>
public class BuildScript
{
    private static readonly string[] Scenes = new[]
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Game.unity",
    };

    [MenuItem("Build/Build Android APK")]
    public static void PerformBuild()
    {
        PerformBuild(BuildTarget.Android);
    }

    public static void PerformBuild(BuildTarget target)
    {
        string buildPath = GetBuildPath(target);
        Debug.Log($"Building {target} to {buildPath}");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = Scenes.Where(s => System.IO.File.Exists(s)).ToArray(),
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
            Debug.Log("Build succeeded!");
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
