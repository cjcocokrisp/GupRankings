using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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
        private StatRankings selectedStat;
        public static string statLabel = "none";
        private List<KeyValuePair<string, ulong>> statsData; // key is the players username and value is the stat value
        public static string sortedStatDisplay;
        private bool changed;

        public SortStats()
        {
            rawStats = "";
            statsData = new List<KeyValuePair<string, ulong>>();
            sortedStatDisplay = $"<color=#ff8300>Leaderboard ({statLabel}):</color>\nIF YOU SEE THIS THERE IS A PROBLEM!!!\nPLEASE REPORT IT TO THE GITHUB ISSUES PAGE!";
            changed = false;
        }

        public SortStats(NetworkInstanceId netId, string stats, StatRankings selectedStat, string statName)
        {
            this.netId = netId;
            this.rawStats = stats;
            this.selectedStat = selectedStat;
            statLabel = statName;
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
            selectedStat = (StatRankings)reader.ReadInt32();
            statLabel = reader.ReadString();
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
                if (selectedStat == StatRankings.LeastDamageTaken) // There are less stats that take least so add to this conditional to add more, will refactor this at some point
                    statsData.Sort(PlayerStatsComparisonGreater); // Get player with least in the front
                else
                    statsData.Sort(PlayerStatsComparisonLess); // Get player with highest in the front
                // Set display string
                SetDisplayString(statsData);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(rawStats);
            writer.Write((int)selectedStat);
            writer.Write(statLabel);
        }

        private static void SetDisplayString(List<KeyValuePair<string, ulong>> data)
        {
            Color32 headerColor = BasePlugin.instance.headerColor.Value;
            Color32 firstColor = BasePlugin.instance.firstColor.Value;
            Color32 secondColor = BasePlugin.instance.secondColor.Value;
            Color32 thirdColor = BasePlugin.instance.thirdColor.Value;

            sortedStatDisplay = $"<color=#{ConvertColorToHexString(headerColor)}>Leaderboard ({statLabel}):</color>\n";
            int position = 1;
            foreach (var player in data)
            {
                string name = player.Key;
                if ((int)BasePlugin.instance.userNameLength.Value != 1 && player.Key.Length > (int)BasePlugin.instance.userNameLength.Value)
                {
                    name = name.Substring(0, (int)BasePlugin.instance.userNameLength.Value);
                }
                switch (position)
                {
                    case 1:
                        sortedStatDisplay += $"<color=#{ConvertColorToHexString(firstColor)}>1. {name}: {player.Value}</color>\n";
                        break;
                    case 2:
                        sortedStatDisplay += $"<color=#{ConvertColorToHexString(secondColor)}>2. {name}: {player.Value}</color>\n";
                        break;
                    case 3:
                        sortedStatDisplay += $"<color=#{ConvertColorToHexString(thirdColor)}>3. {name}: {player.Value}</color>\n";
                        break;
                    default:
                        sortedStatDisplay += $"{position}. {name}: {player.Value}\n";
                        break;
                }
                position++;
            }
        }

        public static void HostSync(string s, StatRankings selectedStat, string statName)
        {
            statLabel = statName;
            List<KeyValuePair<string, ulong>> statsDataTemp = new List<KeyValuePair<string, ulong>>();
            foreach (string playerRaw in s.Split('\n'))
            {
                string[] playerData = playerRaw.Split('\t');
                statsDataTemp.Add(new KeyValuePair<string, ulong>(playerData[0], ulong.Parse(playerData[1])));
            }
            // Sort the stats 
            if (selectedStat == StatRankings.LeastDamageTaken) 
                statsDataTemp.Sort(PlayerStatsComparisonGreater); 
            else
                statsDataTemp.Sort(PlayerStatsComparisonLess);
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

        private static string ConvertColorToHexString(Color32 color)
        {
            byte[] rgbValues = { color.r, color.g, color.b };
            string[] strBytes = BitConverter.ToString(rgbValues).Split('-');

            string hexCode = "";
            foreach (var strByte in strBytes){
                hexCode += strByte;
            }

            return hexCode;
        }
    }
}