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

namespace GupRankings.RankingDisplay
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
            PlayerStatsComponent stats = LocalUserManager.GetFirstLocalUser().cachedStatsComponent;
            string name = LocalUserManager.GetFirstLocalUser().userProfile.name;

            if (text) text.SetText(name + "\n" + "Total Damage: " + stats.recordedStats.GetStatValueString(StatDef.totalDamageDealt) + "\n" + "Total Gold: " + stats.recordedStats.GetStatValueString(StatDef.goldCollected));
            if (!textObj) textObj.SetActive(true);
        }
    }
}
