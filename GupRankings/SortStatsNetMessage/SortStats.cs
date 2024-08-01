using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using R2API.Networking.Interfaces;
using Rewired.Integration.UnityUI;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements.UIR;

namespace GupRankings.SortStatsNetMessage
{
    public class SortStats : INetMessage
    {
        private NetworkInstanceId netId;
        private string rawStats;
        private List<KeyValuePair<string, ulong>> statsData; // key is the players username and value is the stat value
        public static string sortedStatDisplay;
        private bool changed;

        public SortStats()
        {
            rawStats = "";
            statsData = new List<KeyValuePair<string, ulong>>();
            sortedStatDisplay = "<color=#ff8300>Leaderboard (Total Damage Dealt):</color>\nIF YOU SEE THIS THERE IS A PROBLEM!!!\nPLEASE REPORT IT TO THE GITHUB ISSUES PAGE!";
            changed = false;
        }

        public SortStats(NetworkInstanceId netId, string stats)
        {
            this.netId = netId;
            this.rawStats = stats;
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            string temp = reader.ReadString();
            if (!temp.Equals(rawStats))
            {
                rawStats = temp;
                changed = true;
            }
            else
            {
                changed = false;
            }
        }

        public void OnReceived()
        {
            if (changed)
            {
                statsData.Clear();
                // Parse the raw string
                foreach (string playerRaw in rawStats.Split('\n'))
                {
                    string[] playerData = playerRaw.Split('\t');
                    statsData.Add(new KeyValuePair<string, ulong>(playerData[0], ulong.Parse(playerData[1])));
                }
                // Sort the stats 
                statsData.Sort(PlayerStatsComparisonLess); // Get player with highest at front for now
                // Set display string
                SetDisplayString(statsData);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(rawStats);
        }

        private static void SetDisplayString(List<KeyValuePair<string, ulong>> data)
        {
            sortedStatDisplay = "<color=#ff8300>Leaderboard (Total Damage Dealt):</color>\n";
            int position = 1;
            foreach (var player in data)
            {
                switch (position)
                {
                    case 1:
                        sortedStatDisplay += $"<color=#d9b807>1. {player.Key}: {player.Value}</color>\n";
                        break;
                    case 2:
                        sortedStatDisplay += $"<color=#c0c0c0>2. {player.Key}: {player.Value}</color>\n";
                        break;
                    case 3:
                        sortedStatDisplay += $"<color=#bf7739>3. {player.Key}: {player.Value}</color>\n";
                        break;
                    default:
                        sortedStatDisplay += $"{position}. {player.Key}: {player.Value}\n";
                        break;
                }
                position++;
            }
        }

        public static void HostSync(string s)
        {
            List<KeyValuePair<string, ulong>> statsDataTemp = new List<KeyValuePair<string, ulong>>();
            foreach (string playerRaw in s.Split('\n'))
            {
                string[] playerData = playerRaw.Split('\t');
                statsDataTemp.Add(new KeyValuePair<string, ulong>(playerData[0], ulong.Parse(playerData[1])));
            }
            // Sort the stats 
            statsDataTemp.Sort(PlayerStatsComparisonLess); // Get player with highest at front for now
            // Set display string
            SetDisplayString(statsDataTemp);
        }

        private static int PlayerStatsComparisonGreater(KeyValuePair<string, ulong> x, KeyValuePair<string, ulong> y)
        {
            return Convert.ToInt32(x.Value > y.Value);
        }

        private static int PlayerStatsComparisonLess(KeyValuePair<string, ulong> x, KeyValuePair<string, ulong> y)
        {
            return Convert.ToInt32(x.Value < y.Value);
        }
    }
}