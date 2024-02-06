using HarmonyLib;
using Unity.Netcode;

namespace ClassicSuitRestoration.Patches
{
    [HarmonyPatch]
    class ClassicSuitRestorationPatches
    {
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        public static void StartOfRoundPostAwake()
        {
            RestoreClassicSuit.InitClassicSuit();
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void StartOfRoundPostStart()
        {
            RestoreClassicSuit.SpawnClassicSuit();
        }

        [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
        [HarmonyPostfix]
        public static void PostResetShip()
        {
            RestoreClassicSuit.SpawnClassicSuit();
        }

        [HarmonyPatch(typeof(UnlockableSuit), "Update")]
        [HarmonyPrefix]
        public static void UnlockableSuitPreUpdate(UnlockableSuit __instance)
        {
            if (GameNetworkManager.Instance != null && NetworkManager.Singleton != null && !NetworkManager.Singleton.ShutdownInProgress && __instance.suitID != __instance.syncedSuitID.Value && __instance.syncedSuitID.Value >= StartOfRound.Instance.unlockablesList.unlockables.Count)
                RestoreClassicSuit.InitClassicSuit();
        }

        [HarmonyPatch(typeof(UnlockableSuit), "SwitchSuitForPlayer")]
        [HarmonyPrefix]
        public static void PreSwitchSuitForPlayer(ref int suitID)
        {
            if (suitID >= StartOfRound.Instance.unlockablesList.unlockables.Count)
            {
                RestoreClassicSuit.InitClassicSuit();
                suitID = RestoreClassicSuit.classicSuitIndex;
            }
        }
    }
}