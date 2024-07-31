using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements.UIR;

namespace GupRankings.SortStats
{
    public class SyncStats : INetMessage
    {
        NetworkInstanceId netId;
        string stats;
        public static string statsStatic;

        public SyncStats()
        {
        }

        public SyncStats(NetworkInstanceId netId, string stats)
        {
            this.netId = netId;
            this.stats = stats;
            statsStatic = stats;
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            stats = reader.ReadString();
            statsStatic = stats;
        }

        public void OnReceived()
        {
            Debug.Log(stats);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(stats);
        }
    }


    //internal class SortStats : NetworkBehaviour
    //{
    //    private static SortStats _instance;

    //    string stats;

    //    private void Awake()
    //    {
    //        _instance = this;
    //    }
    //    public void SendStats(string statsSerialized)
    //    {
    //        // This function should only be run by the server (host) so safety we check if the network server is active
    //        if (NetworkServer.active)
    //        {
    //            stats = statsSerialized; // For now leaving like this might have server do stuff with sorting
    //            RpcTest(statsSerialized);
    //        }
    //        else
    //        {
    //            CmdTest(statsSerialized);
    //        }
    //    }

    //    public static void Invoke(string msg)
    //    {
    //        _instance.RpcTest(msg);
    //    }

    //    public void GetStats(ref string stats)
    //    {
    //        stats = this.stats;
    //    }

    //    [Command]
    //    private void CmdTest(string msg)
    //    {
    //        RpcTest(msg);
    //    }

    //    [TargetRpc]
    //    private void RpcTest(string msg)
    //    {
    //        LogDebug(msg);
    //    }

    //    private void LogDebug(string msg)
    //    {
    //        Debug.Log(msg);
    //    }

    //    // Functions
    //    // SendStats 
    //    // DeserializeStats (Helper function)
    //    // SortStats (Helper function, only sorts when different)
    //    // GetSerializedStats
    //    // GetStatsDisplayString
    //}
}
