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

namespace GupRankings
{
    public enum StatRankings
    {
        TotalOverallDamageDealt,
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
        public static BasePlugin instance;

        // We need our item definition to persist through our functions, and therefore make it a class field.

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            instance = this;
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { }; // COMMENT THIS OUT ON FINAL RELEASE
            rankDisplay = new RankingDisplay();
            NetworkingAPI.RegisterMessageType<SortStats>();

            statEnum = Config.Bind("Options", "Leaderboard Stat", StatRankings.TotalOverallDamageDealt, "Select the statistic that the leaderboard will be based on.");
            ModSettingsManager.AddOption(new ChoiceOption(statEnum));

            Log.Init(Logger);
        }

        // The Update() method is run on every frame of the game.
        public void Update()
        {
               
        }
    }
}