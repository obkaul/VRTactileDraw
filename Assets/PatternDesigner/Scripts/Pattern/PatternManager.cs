using Assets.PatternDesigner.Scripts.PaintSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.Pattern
{
    /// <summary>
    ///     Handles saving, loading, converting, deleting, creating, renaming patterns
    /// </summary>
    public static class PatternManager
    {
        //%userprofile%\AppData\Local\Packages\<productname>\LocalState
        private static readonly string DIRECTORY = Application.persistentDataPath + "/Patterns/";

        public static List<Pattern> patterns = new List<Pattern>();

        public static event Action<Pattern> Added;

        public static event Action<Pattern> Removed;

        public static event Action<Pattern> Saved;

        private static void Add(Pattern pattern)
        {
            patterns.Add(pattern);

            Added?.Invoke(pattern);
        }

        private static void Remove(Pattern pattern)
        {
            patterns.Remove(pattern);

            Removed?.Invoke(pattern);
        }

        private static Pattern Get(string name)
        {
            var query = from pattern in patterns
                        where pattern.name == name
                        select pattern;

            return query.SingleOrDefault();
        }

        public static PaintPattern CreatePaint(string name)
        {
            Debug.Log("Creating PaintPattern instance");
            var pattern = new PaintPattern(name);
            Add(pattern);
            return pattern;
        }

        private static void Load(string path)
        {
            if (!File.Exists(path)) return;
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            using (var reader = new StreamReader(stream))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var json = reader.ReadToEnd();
                var creationTime = File.GetCreationTime(path);
                var lastWriteTime = File.GetLastWriteTime(path);

                var patternType = JsonUtility.FromJson<PatternType>(json);

                Pattern pattern = null;

                switch (patternType.type)
                {
                    case 0:
                        break;

                    case 1:
                        pattern = new PaintPattern(name, creationTime, lastWriteTime);
                        break;
                }

                pattern?.FromJson(json);
                Add(pattern);
            }
        }

        public static void LoadAll()
        {
            if (!Directory.Exists(DIRECTORY))
                return;

            foreach (var file in Directory.GetFiles(DIRECTORY, "*.json"))
                Load(file);
        }

        private static void Save(Pattern pattern)
        {
            if (!Directory.Exists(DIRECTORY))
                Directory.CreateDirectory(DIRECTORY);

            var path = GetPath(pattern.name);

            if (File.Exists(path))
                File.Delete(path);

            using (var stream = new FileStream(path, FileMode.CreateNew))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(pattern.ToJson());
                }
            }

            pattern.UpdateLastWriteTime();

            Saved?.Invoke(pattern);
        }

        public static void SaveAll()
        {
            foreach (var pattern in patterns)
                Save(pattern);
        }

        public static void Rename(Pattern pattern, string name)
        {
            var oldPath = GetPath(pattern.name);

            if (File.Exists(oldPath))
                File.Move(oldPath, GetPath(name));

            pattern.name = name;
        }

        public static void Delete(Pattern pattern)
        {
            var path = GetPath(pattern.name);

            if (File.Exists(path))
                File.Delete(path);

            Remove(pattern);
        }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        public static bool DoesNameExist(string name)
        {
            return Get(name) != null;
        }

        private static string GetPath(string fileName)
        {
            return DIRECTORY + fileName + ".json";
        }

        [Serializable]
        private class PatternType
        {
            public int type = 0;
        }
    }
}