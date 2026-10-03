using System;
using System.IO;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using UnityEngine;

namespace Gacha.Game
{
    /// <summary>
    /// Unity glue for the rules in Core: loads Data/*.json, supplies the clock, and reads and writes the save file.
    /// One per scene; screens reach it through Instance.
    /// </summary>
    public sealed class GameService : MonoBehaviour
    {
        public static GameService Instance { get; private set; }

        public GameData Data { get; private set; }
        public CampaignState State { get; private set; }

        /// <summary>UTC unix seconds; the only clock the rules see.</summary>
        public long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        /// <summary>False when the save on disk came from a newer build: play continues but nothing overwrites it.</summary>
        public bool CanSave { get; private set; } = true;

        static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        /// <summary>Editor only for now; Android loading is out of scope for phase 1c.</summary>
        static string DataDir => Path.Combine(Application.dataPath, "_Game", "Data");

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Data = GameData.Load(DataDir);
            string text = File.Exists(SavePath) ? File.ReadAllText(SavePath) : null;
            try { State = SaveGame.Read(text, Now); }
            catch (InvalidDataException e)
            {
                Debug.LogError("Save not loaded: " + e.Message + ". Playing a fresh game without saving.");
                State = CampaignState.New((ulong)Now, Now);
                CanSave = false;
            }
        }

        public void Save()
        {
            if (!CanSave || State == null) return;
            string tmp = SavePath + ".tmp";
            File.WriteAllText(tmp, SaveGame.Write(State));
            if (File.Exists(SavePath)) File.Replace(tmp, SavePath, null);
            else File.Move(tmp, SavePath);
        }

        void OnApplicationPause(bool paused) { if (paused) Save(); }
        void OnApplicationQuit() => Save();
        void OnDestroy() { if (Instance == this) { Save(); Instance = null; } }
    }
}
