#region Packages

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;
using Type = System.Type;

#endregion

namespace Mfknudsen.Files
{
    public static class FileManager
    {
        #region Values

        private static bool checkedForTriggers;
        private static Dictionary<string, bool> storyTriggers = new Dictionary<string, bool>();

        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        #endregion

        #region In

        public static void SaveData<T>(T data, string fileName)
        {
            try
            {
                SaveToFile(
                    fileName,
                    data);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static T LoadData<T>(string fileName) where T : class
        {
            try
            {
                return LoadFromFile<T>(
                    fileName);
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
                storyTriggers = LoadFromFile<StoryTriggers>("StoryTriggers").GetDictionary();
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
        public readonly int badge;
        public readonly string[] pronouns;
        public readonly Pokemon[] inTeam = new Pokemon[6], inBox;

        public PlayerData(PlayerManager manager)
        {
            CharacterSheet characterSheet = manager.GetCharacterSheet();
            Team team = manager.GetTeam();

            badge = characterSheet.badgeCount;
            pronouns = new[]
            {
                characterSheet.pronoun1,
                characterSheet.pronoun2,
                characterSheet.pronoun3
            };

            for (int i = 0; i < 6; i++)
                inTeam[i] = team.GetPokemonByIndex(i);
        }
    }

    internal class StoryTriggers
    {
        private string[] keys;
        private bool[] values;

        public StoryTriggers(string[] keys, bool[] values)
        {
            this.keys = keys;
            this.values = values;
        }

        public Dictionary<string, bool> GetDictionary()
        {
            Dictionary<string, bool> result = GetDictionary();

            for (int i = 0; i < keys.Length; i++)
                result.Add(keys[i], values[i]);

            return result;
        }
    }
}