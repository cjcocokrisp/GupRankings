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

        public RankingDisplay()
        {
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

                    text.alignment = TMPro.TextAlignmentOptions.TopLeft;
                    text.color = Color.white;
                    text.fontSize = 10;
                    textObj = text.gameObject;
                    track = obj.transform;
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
                            statString += $"nullplayer{players}" + "\t" + stats.GetStatValueString(StatDef.totalDamageDealt) + "\n";
                            players++;
                        }
                        else
                        { 
                            statString += playerCharacterMaster.GetDisplayName() + "\t" + stats.GetStatValueString(StatDef.totalDamageDealt) + "\n";
                        }
                    } 
                    catch
                    {
                        Debug.Log(playerCharacterMaster.GetDisplayName() + " has thrown an exception...");
                    }
                }
                new SortStats(LocalUserManager.GetFirstLocalUser().currentNetworkUser.netId, statString.Substring(0, statString.Length - 1)).Send(NetworkDestination.Clients);
                SortStats.HostSync(statString.Substring(0, statString.Length - 1));
            }

            if (text) text.SetText(SortStats.sortedStatDisplay);
            if (!textObj) textObj.SetActive(true);
        }
    }
}
