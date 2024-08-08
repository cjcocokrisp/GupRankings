using BepInEx;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using R2API.Networking;
using GupRankings.RankingDisplayHooks;
using GupRankings.SortStatsNetMessage;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.Lib;
using RiskOfOptions.OptionConfigs;

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

    // This attribute is required, and lists metadata for your plugin.
    // Need to edit this at some point and get it to work right
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.rune580.riskofoptions")]
    public class BasePlugin : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
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
      
        public static BasePlugin instance;

        // We need our item definition to persist through our functions, and therefore make it a class field.

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            instance = this;
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { }; // COMMENT THIS OUT ON FINAL RELEASE
            rankDisplay = new RankingDisplay();
            NetworkingAPI.RegisterMessageType<SortStats>();

            statEnum = Config.Bind("Options", "Leaderboard Stat", StatRankings.TotalDamageDealt, "Select the statistic that the leaderboard will be based on.");
            ModSettingsManager.AddOption(new ChoiceOption(statEnum));

            fontSize = Config.Bind("Options", "Font Size", 0f, "Select the font size for the leaderboard text. Setting it to 0 will set it to the default size of the panel. The percentage equals the size.");
            ModSettingsManager.AddOption(new SliderOption(fontSize, new SliderConfig { min = 0f, max = 64f }));

            headerColor = Config.Bind("Options", "Header Color", new Color(255, 132, 0), "Select the header color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(headerColor));

            firstColor = Config.Bind("Options", "First Color", new Color(217, 184, 7), "Select the first place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(firstColor));

            secondColor = Config.Bind("Options", "Second Color", new Color(192, 192, 192), "Select the second place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(secondColor));

            thirdColor = Config.Bind("Options", "Third Color", new Color(191, 120, 57), "Select the third place color of the leaderboard.");
            ModSettingsManager.AddOption(new ColorOption(thirdColor));

            Log.Init(Logger);
        }

        // The Update() method is run on every frame of the game.
        public void Update()
        {
            Debug.Log(firstColor.Value.ToRGBHex().ToString());
        }
    }
}