using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Story
{
    static class SaveGame
    {
        public static PlayerSaveData PlayerSaveData = new PlayerSaveData(new List<Tuple<int, bool[]>>());
        public static List<LevelData> LevelSaveData = new List<LevelData>();
        private const string PlayerSaveFile = "SaveFile.dat";
        private const string LevelDataFile = "LevelData.dat";

        public static void LoadContent()
        {
            ReadLevelFile();
        }

        public static void SaveLevelData(LevelData LevelData)
        {
            // Load in old data
            ReadLevelFile();

            // Overwrite a level if it has the same level id
            bool OverWritten = false;
            for (int index = 0; index < LevelSaveData.Count; index++)
            {
                if (LevelSaveData[index].LevelID == LevelData.LevelID)
                {
                    LevelSaveData[index] = LevelData;
                    OverWritten = true;
                    break;
                }
            }

            // If not overwritten, add it to the list
            if (!OverWritten)
                LevelSaveData.Add(LevelData);

            // Save new file
            SaveLevelDataToFile();
        }

        public static LevelData LoadLevel(int LevelID)
        {
            ReadLevelFile();

            for (int index = 0; index < LevelSaveData.Count; index++)
            {
                if (LevelSaveData[index].LevelID == LevelID)
                {
                    LevelData LevelData = LevelSaveData[index];
                    return LevelData;
                }
            }

            throw new Exception("Unable to load level -- level ID not found");
        }

        private static void SaveLevelDataToFile()
        {
            // Open stream to the file
            Stream SaveStream = File.Open(LevelDataFile, FileMode.Create);

            // Formatting is in binary (smaller files this way)
            BinaryFormatter BFormatter = new BinaryFormatter();

            // Save our level data
            foreach (LevelData LevelData in LevelSaveData)
                BFormatter.Serialize(SaveStream, LevelData);

            // Close our file stream
            SaveStream.Close();
        }

        private static void ReadLevelFile()
        {
            LevelSaveData.Clear();

            if (!File.Exists(LevelDataFile))
            {
                return;
            }

            // Open stream to the file
            Stream OpenStream = File.OpenRead(LevelDataFile);

            // Create our formatter to deserialize the objects
            BinaryFormatter BFormatter = new BinaryFormatter();

            // Read in level data
            while (OpenStream.Position != OpenStream.Length)
            {
                LevelSaveData.Add((LevelData)BFormatter.Deserialize(OpenStream));
            }

            //Close our file stream
            OpenStream.Close();
        }

        public static void SaveLevelCompletion(int LevelID, bool AppleCollected, bool BananaCollected, bool OrangeCollected)
        {
            for (int i = 0; i < PlayerSaveData.LevelProgress.Count; i++)
            {
                if (PlayerSaveData.LevelProgress[i].Item1 == LevelID)
                {
                    PlayerSaveData.LevelProgress[i].Item2[0] = PlayerSaveData.LevelProgress[i].Item2[0] | AppleCollected;
                    PlayerSaveData.LevelProgress[i].Item2[1] = PlayerSaveData.LevelProgress[i].Item2[1] | BananaCollected;
                    PlayerSaveData.LevelProgress[i].Item2[2] = PlayerSaveData.LevelProgress[i].Item2[2] | OrangeCollected;
                    SavePlayerDataToFile();
                    return;
                }
            }

            // No save data found for this level, insert it.
            PlayerSaveData.LevelProgress.Add(new Tuple<int, bool[]>(
                LevelID, new bool[] { AppleCollected, BananaCollected, OrangeCollected }));
            SavePlayerDataToFile();
        }

        public static PlayerSaveData LoadPlayerData()
        {
            ReadPlayerSaveFile();

            return PlayerSaveData;
        }

        private static void SavePlayerDataToFile()
        {
            // Open stream to the file
            Stream SaveStream = File.Open(PlayerSaveFile, FileMode.Create);

            // Formatting is in binary (smaller files this way)
            BinaryFormatter BFormatter = new BinaryFormatter();

            // Save player data
            BFormatter.Serialize(SaveStream, PlayerSaveData);

            // Close our file stream
            SaveStream.Close();
        }

        private static void ReadPlayerSaveFile()
        {
            if (!File.Exists(PlayerSaveFile))
            {
                return;
            }

            // Open stream to the file
            Stream OpenStream = File.OpenRead(PlayerSaveFile);

            // Create our formatter to deserialize the objects
            BinaryFormatter BFormatter = new BinaryFormatter();

            
            // Read in player data
            PlayerSaveData = (PlayerSaveData)BFormatter.Deserialize(OpenStream);

            //Close our file stream
            OpenStream.Close();
        }
    }

    [Serializable]
    struct PlayerSaveData
    {
        public List<Tuple<int, bool[]>> LevelProgress;
        public PlayerSaveData(List<Tuple<int, bool[]>> LevelProgress)
        {
            this.LevelProgress = LevelProgress;
        }
    }
}
