using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

namespace Valve.VR
{
    public class SteamVR_Input_PostProcessBuild
    {
        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            SteamVR_Input.InitializeFile();

            var fileInfo = new FileInfo(pathToBuiltProject);
            var buildPath = fileInfo.Directory.FullName;

            var files = SteamVR_Input.actionFile.GetFilesToCopy();

            var overwrite = EditorPrefs.GetBool(SteamVR_Input_Generator.steamVRInputOverwriteBuildKey);

            foreach (var file in files)
            {
                var bindingInfo = new FileInfo(file);
                var newFilePath = Path.Combine(buildPath, bindingInfo.Name);

                var exists = false;
                if (File.Exists(newFilePath))
                    exists = true;

                if (exists)
                {
                    if (overwrite)
                    {
                        var existingFile = new FileInfo(newFilePath);
                        existingFile.IsReadOnly = false;
                        existingFile.Delete();

                        File.Copy(file, newFilePath);

                        //UpdateAppKey(newFilePath, fileInfo.Name);
                        RemoveAppKey(newFilePath, fileInfo.Name);

                        Debug.Log("[SteamVR] Copied (overwrote) SteamVR Input file at build path: " + newFilePath);
                    }
                    else
                    {
                        Debug.Log("[SteamVR] Skipped writing existing file at build path: " + newFilePath);
                    }
                }
                else
                {
                    File.Copy(file, newFilePath);
                    //UpdateAppKey(newFilePath, fileInfo.Name);
                    RemoveAppKey(newFilePath, fileInfo.Name);

                    Debug.Log("[SteamVR] Copied SteamVR Input file to build folder: " + newFilePath);
                }

            }
        }

        private static void UpdateAppKey(string newFilePath, string executableName)
        {
            if (File.Exists(newFilePath))
            {
                var jsonText = System.IO.File.ReadAllText(newFilePath);

                var findString = "\"app_key\" : \"";
                var stringStart = jsonText.IndexOf(findString);

                if (stringStart == -1)
                {
                    findString = findString.Replace(" ", "");
                    stringStart = jsonText.IndexOf(findString);

                    if (stringStart == -1)
                        return; //no app key
                }

                stringStart += findString.Length;
                var stringEnd = jsonText.IndexOf("\"", stringStart);

                var stringLength = stringEnd - stringStart;

                var currentAppKey = jsonText.Substring(stringStart, stringLength);

                if (string.Equals(currentAppKey, SteamVR_Settings.instance.editorAppKey, System.StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    jsonText = jsonText.Replace(currentAppKey, SteamVR_Settings.instance.editorAppKey);

                    var file = new FileInfo(newFilePath);
                    file.IsReadOnly = false;

                    File.WriteAllText(newFilePath, jsonText);
                }
            }
        }

        private const string findString_appKeyStart = "\"app_key\"";
        private const string findString_appKeyEnd = "\",";
        private static void RemoveAppKey(string newFilePath, string executableName)
        {
            if (File.Exists(newFilePath))
            {
                var jsonText = System.IO.File.ReadAllText(newFilePath);

                var findString = "\"app_key\"";
                var stringStart = jsonText.IndexOf(findString);

                if (stringStart == -1)
                    return; //no app key

                var stringEnd = jsonText.IndexOf("\",", stringStart);

                if (stringEnd == -1)
                    return; //no end?

                stringEnd += findString_appKeyEnd.Length;

                var stringLength = stringEnd - stringStart;

                var newJsonText = jsonText.Remove(stringStart, stringLength);

                var file = new FileInfo(newFilePath);
                file.IsReadOnly = false;

                File.WriteAllText(newFilePath, newJsonText);
            }
        }
    }
}