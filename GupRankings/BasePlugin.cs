using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GupRankings
{
    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class BasePlugin : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "AuthorName";
        public const string PluginName = "ExamplePlugin";
        public const string PluginVersion = "1.0.0";

        // We need our item definition to persist through our functions, and therefore make it a class field.
        private static ItemDef myItemDef;

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            // Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
        }

        // The Update() method is run on every frame of the game.
        private void Update()
        {

        }
    }
}
