using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

namespace Valve.VR
{
    public class SteamVR_CopyExampleInputFiles : Editor
    {
        public const string steamVRInputExampleJSONCopiedKey = "SteamVR_Input_CopiedExamples";

        public const string exampleJSONFolderName = "ExampleJSON";

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnReloadScripts()
        {
            CopyFiles();
        }

        public static void CopyFiles(bool force = false)
        {
            var hasCopied = EditorPrefs.GetBool(steamVRInputExampleJSONCopiedKey, false);
            if (hasCopied == false || force == true)
            {
                var actionsFilePath = SteamVR_Settings.instance.actionsFilePath;
                var exists = File.Exists(actionsFilePath);
                if (exists == false)
                {
                    var monoScripts = MonoImporter.GetAllRuntimeMonoScripts();

                    var steamVRInputType = typeof(SteamVR_Input);
                    var monoScript = monoScripts.FirstOrDefault(script => script.GetClass() == steamVRInputType);
                    var path = AssetDatabase.GetAssetPath(monoScript);

                    var lastIndex = path.LastIndexOf("/");
                    path = path.Substring(0, lastIndex + 1);
                    path += exampleJSONFolderName;

                    var dataPath = Application.dataPath;
                    lastIndex = dataPath.LastIndexOf("/Assets");
                    dataPath = dataPath.Substring(0, lastIndex + 1);

                    path = dataPath + path;

                    var files = Directory.GetFiles(path, "*.json");
                    foreach (var file in files)
                    {
                        lastIndex = file.LastIndexOf("\\");
                        var filename = file.Substring(lastIndex + 1);

                        var newPath = Path.Combine(dataPath, filename);

                        try
                        {
                            File.Copy(file, newPath, false);
                            Debug.Log("[SteamVR Input] Copied example input JSON to path: " + newPath);
                        }
                        catch
                        {
                            Debug.LogError("[SteamVR Input] Could not copy file: " + file + " to path: " + newPath);
                        }
                    }

                    EditorPrefs.SetBool(steamVRInputExampleJSONCopiedKey, true);
                }
            }
        }
    }
}