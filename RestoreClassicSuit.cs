using System.IO;
using System.Linq;
using System.Reflection;
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
                if (unlockablesLists == null || unlockablesLists.Length > 0)
                {
                    Plugin.Logger.LogError("Can't find unlockables list");
                    return;
                }
                unlockablesList = unlockablesLists[0];
            }

            UnlockableItem classicSuit = unlockablesList.unlockables.FirstOrDefault(unlockable => unlockable.unlockableName == "Veteran suit");
            if (classicSuit == null)
            {
                classicSuit = new UnlockableItem();
                classicSuit.unlockableName = "Veteran suit";
                classicSuit.suitMaterial = Object.Instantiate(unlockablesList.unlockables[0].suitMaterial);
                classicSuit.suitMaterial.name = "OldSuit";
                try
                {
                    AssetBundle classicSuitBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "classicsuitrestoration"));
                    classicSuit.suitMaterial.mainTexture = classicSuitBundle.LoadAsset<Texture2D>("OldSuitMockUp");
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
                typeof(StartOfRound).GetMethod("SpawnUnlockable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(StartOfRound.Instance, new object[]{classicSuitIndex});
        }
    }
}
