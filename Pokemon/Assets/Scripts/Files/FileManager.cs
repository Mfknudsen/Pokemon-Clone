#region Packages

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen.Files
{
    public static class FileManager
    {
        #region Values

        private static readonly Dictionary<Type, string> fileNames = new Dictionary<Type, string>()
        {
            { typeof(PlayerData), "PlayerData" },
            { typeof(StoryTriggers), "PlayerData" }
        };

        private static bool checkedForTriggers = false;
        private static Dictionary<string, bool> storyTriggers = new Dictionary<string, bool>();

        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        #endregion

        #region In

        public static void SaveData<T>(T data)
        {
            try
            {
                SaveToFile(
                    fileNames[typeof(T)],
                    data);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static T LoadData<T>() where T : class
        {
            try
            {
                return LoadFromFile<T>(
                    fileNames[typeof(T)]);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        #endregion

        #region Out

        public static bool? GetStoryTrigger(string key)
        {
            if (!checkedForTriggers)
            {
                storyTriggers = LoadFromFile<StoryTriggers>(fileNames[typeof(StoryTriggers)]).GetDictionary();
                checkedForTriggers = true;
            }

            if (storyTriggers.ContainsKey(key))
                return storyTriggers[key];

            return null;
        }

        #endregion

        #region Internal

        private static T LoadFromFile<T>(string fileName) where T : class
        {
            string path = Application.persistentDataPath + "/" + fileName;

            if (!File.Exists(path)) return null;

            FileStream stream = new FileStream(
                path,
                FileMode.Open);

            T result = formatter.Deserialize(stream) as T;

            stream.Close();

            return result;
        }

        private static void SaveToFile<T>(string fileName, T toSave)
        {
            string path = Application.persistentDataPath + "/" + fileName;

            FileStream stream = new FileStream(
                path,
                FileMode.Create);

            formatter.Serialize(stream, toSave);

            stream.Close();
        }

        #endregion
    }

    internal class PlayerData
    {
        public PlayerData(PlayerManager manager)
        {
        }
    }

    internal class StoryTriggers
    {
        private string[] keys;
        private bool[] values;

        public Dictionary<string, bool> GetDictionary()
        {
            Dictionary<string, bool> result = GetDictionary();

            for (int i = 0; i < keys.Length; i++)
                result.Add(keys[i], values[i]);

            return result;
        }
    }
}