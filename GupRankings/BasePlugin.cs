using BepInEx;
using UnityEngine;
using R2API.Networking;
using GupRankings.RankingDisplayHooks;
using GupRankings.SortStatsNetMessage;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using System.IO;
using R2API.Utils;

namespace GupRankings
{
    public enum StatRankings
    {
        TotalDamageDealt,
        TotalMinionDamageDealt,
        HighestDamageDealt,
        TotalGoldCollected,
        TotalGoldSpent,
        LeastDamageTaken,
        TotalKills,
        TotalEliteKills,
    }

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class BasePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "KarmaReplicant";
        public const string PluginName = "GupRankings";
        public const string PluginVersion = "1.0.0";
        internal RankingDisplay rankDisplay;
        public ConfigEntry<StatRankings> statEnum;
        public ConfigEntry<Color> headerColor;
        public ConfigEntry<Color> firstColor;
        public ConfigEntry<Color> secondColor;
        public ConfigEntry<Color> thirdColor;
        public ConfigEntry<float> fontSize;
        public ConfigEntry<float> userNameLength;
        public byte[] imgData;
        public Sprite logo;
      
        public static BasePlugin instance;

        public void Awake()
        {
            string directory = System.IO.Path.GetDirectoryName(Info.Location);
            imgData = File.ReadAllBytes(directory + "/icon.png");
            Texture2D t = new Texture2D(256, 256);
            t.LoadImage(imgData);
            logo = Sprite.Create(t, new Rect(0f, 0f, t.width, t.height), new Vector2(0, 0));

            instance = this;
            // On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { }; // Allows for hosting game on localhost (for testing)
            rankDisplay = new RankingDisplay();
            NetworkingAPI.RegisterMessageType<SortStats>();

            statEnum = Config.Bind("Options", "Leaderboard Stat", StatRankings.TotalDamageDealt, "Select the statistic that the leaderboard will be based on. Whatever the host has it set to will be displayed in game.");
            ModSettingsManager.AddOption(new ChoiceOption(statEnum));

            fontSize = Config.Bind("Options", "Font Size", 0f, "Select the font size for the leaderboard text. Setting it to 0 will set it to the default size of the panel. The percentage equals the size.");
            ModSettingsManager.AddOption(new SliderOption(fontSize, new SliderConfig { min = 0f, max = 64f }));

            userNameLength = Config.Bind("Options", "Username Length", 8f, "Select the maximum amount of characters of player's usernames that will be shown to help make everything fit. Setting it to 1 will have the full name displayed regardless. The percentage equals the size.");
            ModSettingsManager.AddOption(new SliderOption(userNameLength, new SliderConfig { min = 1f, max = 32f }));

            headerColor = Config.Bind("Options", "Header Color", new Color(1, 132f/255f, 0), "Select the header color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(headerColor));

            firstColor = Config.Bind("Options", "First Color", new Color(217f/255f, 184f/255f, 7f/255f), "Select the first place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(firstColor));

            secondColor = Config.Bind("Options", "Second Color", new Color(192f/255f, 192f/255f, 192f/255f), "Select the second place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(secondColor));

            thirdColor = Config.Bind("Options", "Third Color", new Color(191f/255f, 120f/255f, 57f/255f), "Select the third place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(thirdColor));

            ModSettingsManager.SetModIcon(logo);

            Log.Init(Logger);
        }

        // public void Update()
        // {
        // }
    }
}