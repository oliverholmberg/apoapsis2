using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Linq;

public class BuildScript
{
    private const string DEFAULT_BUNDLE_ID = "com.oliverholmberg.apoapsis2";
    private const string IOS_OUTPUT_PATH = "Builds/iOS";

    [MenuItem("Build/iOS - Simulator")]
    public static void BuildiOSSimulator()
    {
        BuildiOS(iOSSdkVersion.SimulatorSDK);
    }

    [MenuItem("Build/iOS - Device")]
    public static void BuildiOSDevice()
    {
        BuildiOS(iOSSdkVersion.DeviceSDK);
    }

    private static void BuildiOS(iOSSdkVersion targetSDK)
    {
        string bundleId = GetEnvOrDefault("BUNDLE_ID", DEFAULT_BUNDLE_ID);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);

        PlayerSettings.iOS.sdkVersion = targetSDK;
        // Simulator uses iOS 17 (x86_64 Rosetta), device targets iOS 26
        PlayerSettings.iOS.targetOSVersionString = targetSDK == iOSSdkVersion.SimulatorSDK ? "17.0" : "26.0";

        // Code signing
        string teamId = Environment.GetEnvironmentVariable("APPLE_TEAM_ID");
        if (!string.IsNullOrEmpty(teamId))
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = teamId;
        }
        else
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        }

        // Gather enabled scenes
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("No scenes found in Build Settings!");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"Building iOS ({targetSDK}) with {scenes.Length} scene(s)...");
        Debug.Log($"Bundle ID: {bundleId}");
        Debug.Log($"Output: {IOS_OUTPUT_PATH}");

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = IOS_OUTPUT_PATH,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize} bytes, {summary.totalTime}");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"Build failed: {summary.result}");
            foreach (var step in report.steps)
            {
                foreach (var msg in step.messages)
                {
                    if (msg.type == LogType.Error || msg.type == LogType.Exception)
                        Debug.LogError($"  {msg.content}");
                }
            }
            EditorApplication.Exit(1);
        }
    }

    private static string GetEnvOrDefault(string key, string defaultValue)
    {
        string value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}
