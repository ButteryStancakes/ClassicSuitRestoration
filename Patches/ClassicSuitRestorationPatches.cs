using System.IO;
using UnityEngine;
using HarmonyLib;
using BepInEx;
using System.Reflection;

namespace ClassicSuitRestoration.Patches
{
    [HarmonyPatch]
    class ClassicSuitRestorationPatches
    {
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        public static void PostAwake(StartOfRound __instance)
        {
            try
            {
                UnlockableItem classicSuit = new UnlockableItem();
                classicSuit.unlockableName = "Classic suit";
                classicSuit.suitMaterial = Object.Instantiate(__instance.unlockablesList.unlockables[0].suitMaterial);
                classicSuit.suitMaterial.name = "OldSuit";
                AssetBundle classicSuitBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "classicsuitrestoration"));
                classicSuit.suitMaterial.mainTexture = classicSuitBundle.LoadAsset<Texture2D>("OldSuitMockUp");
                classicSuitBundle.Unload(false);
                RestoreClassicSuit.classicSuitIndex = __instance.unlockablesList.unlockables.Count;
                __instance.unlockablesList.unlockables.Add(classicSuit);
            }
            catch
            {
                RestoreClassicSuit.classicSuitIndex = -1;
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void PostStart()
        {
            RestoreClassicSuit.SpawnClassicSuit();
        }

        [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
        [HarmonyPostfix]
        public static void PostResetShip()
        {
            RestoreClassicSuit.SpawnClassicSuit();
        }
    }
}