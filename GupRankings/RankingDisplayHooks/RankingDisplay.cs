using GupRankings.Base;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using RoR2.Stats;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using R2API.Networking.Interfaces;
using R2API.Networking;
using GupRankings.SortStatsNetMessage;
using System.Collections.Generic;

namespace GupRankings.RankingDisplayHooks
{
    public class RankingDisplay : IBase
    {
        VerticalLayoutGroup layoutGroup;
        LayoutElement layoutElement;
        private static Hook updateHook;
        TextMeshProUGUI text;
        GameObject textObj = null;
        Transform track;
        public Dictionary<StatRankings, StatDef> StatsMap;
        float originalFontSize;

        public RankingDisplay()
        {
            StatsMap = new Dictionary<StatRankings, StatDef>
            {
                { StatRankings.TotalDamageDealt, StatDef.totalDamageDealt },
                { StatRankings.TotalMinionDamageDealt, StatDef.totalMinionDamageDealt },
                { StatRankings.HighestDamageDealt, StatDef.highestDamageDealt },
                { StatRankings.TotalGoldCollected, StatDef.goldCollected },
                { StatRankings.TotalGoldSpent, StatDef.totalGoldPurchases },
                { StatRankings.LeastDamageTaken, StatDef.totalDamageTaken },
                { StatRankings.TotalKills, StatDef.totalKills },
                { StatRankings.TotalEliteKills, StatDef.totalEliteKills }
            };
            Hooks();
        }

        public void Hooks()
        {
            var targetMethod = typeof(ObjectivePanelController).GetMethod(nameof(ObjectivePanelController.Update), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(RankingDisplay).GetMethod(nameof(RankingDisplay.Update), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateHook = new Hook(targetMethod, destMethod, this);
        }

        void Update(Action<ObjectivePanelController> orig, ObjectivePanelController self)
        {
            rankDisplay();
            orig(self);
        }

        public void rankDisplay()
        {
            foreach (var item in RoR2.Run.instance.uiInstance.GetComponentsInChildren<VerticalLayoutGroup>())
            {
                if (item.gameObject.name == "RightInfoBar" && textObj == null)
                {
                    Transform objectivePanel = item.transform.Find("ObjectivePanel");
                    GameObject obj = GameObject.Instantiate(objectivePanel.gameObject);
                    obj.transform.parent = objectivePanel.parent.transform;
                    obj.name = "RankingDisplay";

                    RectTransform r = obj.GetComponent<RectTransform>();
                    layoutGroup = obj.GetComponent<VerticalLayoutGroup>();
                    text = obj.GetComponentInChildren<TextMeshProUGUI>();
                    layoutElement = obj.GetComponentInChildren<LayoutElement>();

                    r.localPosition = Vector3.zero;
                    r.localEulerAngles = Vector3.zero;
                    r.localScale = Vector3.one;
                    layoutGroup.enabled = false;
                    layoutGroup.enabled = true;

                    if (textObj == null)
                    {
                        text.alignment = TMPro.TextAlignmentOptions.TopLeft;
                        text.color = Color.white;
                        textObj = text.gameObject;
                        track = obj.transform;
                        originalFontSize = text.fontSize;
                    }
                }
            }

            if (NetworkServer.active)
            {
                int players = 1;
                string statString = "";
                foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
                {
                    try
                    {
                        // The nullplayer thing is for debug will be removed on release
                        StatSheet stats = playerCharacterMaster.master.playerStatsComponent.currentStats;
                        if (playerCharacterMaster.GetDisplayName().Equals("") || playerCharacterMaster.GetDisplayName().Equals(" "))
                        {
                            statString += $"nullplayer{players}" + "\t" + stats.GetStatValueString(StatsMap[BasePlugin.instance.statEnum.Value]) + "\n";
                            players++;
                        }
                        else
                        { 
                            statString += playerCharacterMaster.GetDisplayName() + "\t" + stats.GetStatValueString(StatsMap[BasePlugin.instance.statEnum.Value]) + "\n";
                        }
                    } 
                    catch
                    {
                        Debug.Log(playerCharacterMaster.GetDisplayName() + " has thrown an exception...");
                    }
                }
                new SortStats(LocalUserManager.GetFirstLocalUser().currentNetworkUser.netId, statString.Substring(0, statString.Length - 1), BasePlugin.instance.statEnum.Value, BasePlugin.instance.statEnum.BoxedValue.ToString()).Send(NetworkDestination.Clients);
                SortStats.HostSync(statString.Substring(0, statString.Length - 1), BasePlugin.instance.statEnum.Value, BasePlugin.instance.statEnum.BoxedValue.ToString());
            }

            text.fontSize = BasePlugin.instance.fontSize.Value == 0 ? originalFontSize : BasePlugin.instance.fontSize.Value;
            if (text)
            {
                layoutElement.preferredHeight = text.fontSize * SortStats.sortedStatDisplay.Split('\n').Length;
                text.SetText(SortStats.sortedStatDisplay);
            }
            if (!textObj) textObj.SetActive(true);
        }
    }
}
