﻿#region Packages

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Trainer;
using UnityEngine;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime.Files
{
    public static class FileManager
    {
        #region Values

        private static readonly BinaryFormatter formatter = new();

        #endregion

        #region In

        public static void SaveData<T>(string fileName, T data)
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

        #region Internal

        private static T LoadFromFile<T>(string fileName) where T : class
        {
            string path = Application.persistentDataPath + "/" + fileName;

            Logger.AddLog(typeof(FileManager).ToString(), "Loading from path:\n" + path);

            if (!File.Exists(path)) return null;

            FileStream stream = new(
                path,
                FileMode.Open);

            T result = formatter.Deserialize(stream) as T;

            stream.Close();

            return result;
        }

        private static void SaveToFile<T>(string fileName, T toSave)
        {
            string path = Application.persistentDataPath + "/" + fileName;

            Logger.AddLog(typeof(FileManager).ToString(), "Saving to path:\n" + path);

            FileStream stream = new(
                path,
                FileMode.Create);

            formatter.Serialize(stream, toSave);

            stream.Close();
        }

        #endregion
    }

    public class PlayerData
    {
        public readonly int badgeCount;
        public readonly string[] pronouns;
        public readonly Pokemon[] inTeam = new Pokemon[6], inBox = new Pokemon[0];

        public PlayerData(PlayerManager manager)
        {
            CharacterSheet characterSheet = manager.GetCharacterSheet();
            Team team = manager.GetTeam();

            badgeCount = characterSheet.badgeCount;
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

    public class StoryTriggers
    {
        private readonly string[] keys;
        private readonly string[] values;

        public StoryTriggers(string[] keys, string[] values)
        {
            this.keys = keys;
            this.values = values;
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        public Dictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> result = GetDictionary();
            for (int i = 0;
                 i < keys.Length;
                 i++)
                result.Add(keys[i], values[i]);
            return result;
        }
    }
}