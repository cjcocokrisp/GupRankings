using BepInEx;
using GupRankings.RankingDisplay;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using R2API.Networking;

namespace GupRankings
{
    // This attribute is required, and lists metadata for your plugin.
    // Need to edit this at some point and get it to work right
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
        internal GupRankings.RankingDisplay.RankingDisplay rankDisplay;
        internal static GameObject CentralNetworkObject;
        private static GameObject _centralNetworkObjectSpawned;

        // We need our item definition to persist through our functions, and therefore make it a class field.

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
            rankDisplay = new GupRankings.RankingDisplay.RankingDisplay();
            NetworkingAPI.RegisterMessageType<GupRankings.SortStats.SyncStats>();
            //GameObject tmpObj = new GameObject("tmpObj");
            //tmpObj.AddComponent<NetworkIdentity>();
            //CentralNetworkObject = tmpObj.InstantiateClone("NetworkObjUnique");
            //GameObject.Destroy(tmpObj);
            //CentralNetworkObject.AddComponent<SyncStats>();
            // Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            
            //Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/PlayerMaster.prefab").Completed += delegate (AsyncOperationHandle<GameObject> obj)
            //{
            //    if (obj.Result)
            //    {
            //        obj.Result.AddComponent<GupRankings.SortStats.SortStats>();
            //    }
            //};
        }

        //[ConCommand(commandName = "debuglog_on_all", flags = ConVarFlags.ExecuteOnServer, helpText = "Logs a network message to all connected people.")]
        //private static void CCNetworkLog(ConCommandArgs args)
        //{
        //    // Although here it's not relevant, you can ensure you are the server by checking if the NetworkServer is active.
        //    if (NetworkServer.active)
        //    {
        //        // Before we can Invoke our NetworkMessage, we need to make sure our centralized networkobject is spawned.
        //        // For doing that, we Instantiate the CentralNetworkObject, we obviously check
        //        // if we don't already have one that is already instantiated and activated in the current scene.
        //        // Note : Make sure you Instantiate the gameobject, and not spawn it directly,
        //        // it would get deleted otherwise on scene change, even with DontDestroyOnLoad.
        //        if (!_centralNetworkObjectSpawned)
        //        {
        //            _centralNetworkObjectSpawned = Object.Instantiate(CentralNetworkObject);
        //            NetworkServer.Spawn(_centralNetworkObjectSpawned);
        //        }
        //        // This readOnlyInstancesList is great for going over all players in general, 
        //        // so it might be worth commiting to memory.
        //        int x = 1;
        //        foreach (NetworkUser user in NetworkUser.readOnlyInstancesList)
        //        {
        //            // `args.userArgs` is a list of all words in the command arguments.
        //            SyncStats.Invoke("Connections: " + x);
        //            x++;
        //        }
        //    }
        //}

        // The Update() method is run on every frame of the game.
        private void Update()
        {

        }
    }
//    internal class SyncStats : NetworkBehaviour
//    {
//        private static SyncStats _instance;

//        string stats;

//        private void Awake()
//        {
//            _instance = this;
//        }
//        public void SendStats(string statsSerialized)
//        {
//            // This function should only be run by the server (host) so safety we check if the network server is active
//            if (NetworkServer.active)
//            {
//                stats = statsSerialized; // For now leaving like this might have server do stuff with sorting
//                RpcTest(statsSerialized);
//            }
//            else
//            {
//                CmdTest(statsSerialized);
//            }
//        }

//        public static void Invoke(string msg)
//        {
//            _instance.RpcTest(msg);
//        }

//        public void GetStats(ref string stats)
//        {
//            stats = this.stats;
//        }

//        [Command]
//        private void CmdTest(string msg)
//        {
//            RpcTest(msg);
//        }

//        [TargetRpc]
//        private void RpcTest(string msg)
//        {
//            LogDebug(msg);
//        }

//        private void LogDebug(string msg)
//        {
//            Debug.Log(msg);
//        }
//    }
}
