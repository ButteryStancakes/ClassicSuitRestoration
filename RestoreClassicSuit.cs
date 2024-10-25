using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassicSuitRestoration
{
    internal class RestoreClassicSuit
    {
        internal static int classicSuitIndex = -1;

        internal static void InitClassicSuit()
        {
            UnlockablesList unlockablesList = null;

            if (StartOfRound.Instance != null)
                unlockablesList = StartOfRound.Instance.unlockablesList;

            if (unlockablesList == null)
            {
                Plugin.Logger.LogWarning("Trying to edit unlockables when StartOfRound is not initialized yet, this shouldn't happen normally");
                UnlockablesList[] unlockablesLists = Resources.FindObjectsOfTypeAll<UnlockablesList>();
                if (unlockablesLists == null || unlockablesLists.Length < 1)
                {
                    Plugin.Logger.LogError("Can't find unlockables list");
                    return;
                }
                unlockablesList = unlockablesLists[0];
            }

            UnlockableItem classicSuit = unlockablesList.unlockables.FirstOrDefault(unlockable => unlockable.suitMaterial?.name == "OldSuit");
            if (classicSuit == null)
            {
                classicSuit = new()
                {
                    suitMaterial = Object.Instantiate(unlockablesList.unlockables[0].suitMaterial),
                    headCostumeObject = null
                };
                classicSuit.suitMaterial.name = "OldSuit";
                try
                {
                    AssetBundle classicSuitBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "classicsuitrestoration"));
                    Texture suitTexture = null;
                    if (Plugin.configOfficial.Value)
                    {
                        suitTexture = classicSuitBundle.LoadAsset<Texture>("ScavengerPlayerModel");
                        if (Plugin.configBirthday.Value || System.DateTime.Today.DayOfYear == new System.DateTime(2023, 10, 23).DayOfYear)
                        {
                            classicSuit.unlockableName = "Birthday suit";
                            classicSuit.headCostumeObject = classicSuitBundle.LoadAsset<GameObject>("PartyHatContainer");
                        }
                        else
                            classicSuit.unlockableName = "Brown suit";
                    }
                    else
                    {
                        suitTexture = classicSuitBundle.LoadAsset<Texture>("OldSuitMockUp");
                        classicSuit.suitMaterial.SetColor("_BaseColor", Color.white);
                        classicSuit.suitMaterial.SetTexture("_NormalMap", classicSuitBundle.LoadAsset<Texture>("ScavengerPlayerModel 1"));
                        classicSuit.unlockableName = "Classic suit";
                    }
                    classicSuit.suitMaterial.SetTexture("_MainTex", suitTexture);
                    classicSuit.suitMaterial.SetTexture("_BaseColorMap", suitTexture);
                    classicSuitBundle.Unload(false);
                }
                catch (System.Exception e)
                {
                    Plugin.Logger.LogError("Failed to acquire suit texture from asset bundle");
                    Plugin.Logger.LogError(e.Message);
                }
                classicSuitIndex = unlockablesList.unlockables.Count;
                unlockablesList.unlockables.Add(classicSuit);
            }
            else
            {
                Plugin.Logger.LogWarning("Tried to add classic suit to unlockables list twice");
                classicSuitIndex = unlockablesList.unlockables.IndexOf(classicSuit);
            }
        }

        internal static void SpawnClassicSuit()
        {
            InitClassicSuit();
            if (classicSuitIndex > 0 && StartOfRound.Instance.IsServer && !StartOfRound.Instance.isChallengeFile && !StartOfRound.Instance.SpawnedShipUnlockables.ContainsKey(classicSuitIndex))
            {
                foreach (UnlockableSuit unlockableSuit in Object.FindObjectsOfType<UnlockableSuit>())
                {
                    if (unlockableSuit.syncedSuitID.Value == classicSuitIndex)
                    {
                        Plugin.Logger.LogWarning("Tried to spawn another classic suit on the rack while one already existed");
                        return;
                    }
                }

                typeof(StartOfRound).GetMethod("SpawnUnlockable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(StartOfRound.Instance, [classicSuitIndex]);
            }
        }

        internal static void UnlockClassicSuit()
        {
            if (Plugin.configUnlockable.Value && StartOfRound.Instance.IsServer && !StartOfRound.Instance.isChallengeFile)
                ES3.Save("ClassicSuitRestoration_Unlocked", true, GameNetworkManager.Instance.currentSaveFileName);
            SpawnClassicSuit();
        }

        internal static bool HasAllOtherSuits()
        {
            List<int> ownedSuits = new();
            foreach (UnlockableSuit unlockableSuit in Object.FindObjectsOfType<UnlockableSuit>())
            {
                ownedSuits.Add(unlockableSuit.syncedSuitID.Value);
                Plugin.Logger.LogInfo($"Player owns suit #{unlockableSuit.syncedSuitID.Value} - \"{StartOfRound.Instance.unlockablesList.unlockables[unlockableSuit.syncedSuitID.Value].unlockableName}\"");
            }

            for (int i = 1; i < classicSuitIndex; i++)
            {
                if (StartOfRound.Instance.unlockablesList.unlockables[i].unlockableType == 0 && !ownedSuits.Contains(i))
                {
                    Plugin.Logger.LogInfo($"Player doesn't have #{i} - \"{StartOfRound.Instance.unlockablesList.unlockables[i].unlockableName}\"");
                    return false;
                }
            }

            Plugin.Logger.LogInfo($"Player will unlock classic suit");
            return true;
        }

        internal static IEnumerator CheckSuitsAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            if (!Plugin.configUnlockable.Value)
                SpawnClassicSuit();
            else if (HasAllOtherSuits())
                UnlockClassicSuit();
        }
    }
}
