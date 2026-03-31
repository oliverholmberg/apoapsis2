using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
#pragma warning disable CS0618 // Suppress obsolete warnings for BuildTargetGroup API

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
        // iOS 17 minimum for both simulator and device
        PlayerSettings.iOS.targetOSVersionString = "17.0";

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

        // Set app icons from Assets/Icons/
        SetiOSIcons();

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

    private static void SetiOSIcons()
    {
        // Get the icon sizes Unity expects for iOS
        var iconSizes = PlayerSettings.GetIconSizes(BuildTargetGroup.iOS);
        var icons = new Texture2D[iconSizes.Length];

        for (int i = 0; i < iconSizes.Length; i++)
        {
            int size = iconSizes[i];
            string path = $"Assets/Icons/icon_{size}.png";
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
            {
                icons[i] = tex;
            }
            else
            {
                Debug.LogWarning($"Icon not found: {path} (size {size})");
            }
        }

        PlayerSettings.SetIcons(BuildTargetGroup.iOS, icons);

        // Set default icon (used as fallback and App Store 1024x1024)
        var defaultIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Icons/icon_1024.png");
        if (defaultIcon != null)
        {
            PlayerSettings.SetIcons(BuildTargetGroup.Unknown, new Texture2D[] { defaultIcon });
        }

        Debug.Log($"iOS icons configured from Assets/Icons/ ({icons.Count(i => i != null)}/{iconSizes.Length} found)");
    }

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Skip encryption compliance prompt on TestFlight
        plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        plist.WriteToFile(plistPath);
    }
}
