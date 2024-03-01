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
            if (!Plugin.configUnlockable.Value || RestoreClassicSuit.HasAllOtherSuits())
                RestoreClassicSuit.SpawnClassicSuit();
        }

        [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
        [HarmonyPostfix]
        public static void PostResetShip()
        {
            if (!Plugin.configUnlockable.Value)
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

        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        [HarmonyPostfix]
        public static void PostSetNewProfitQuota()
        {
            if (Plugin.configUnlockable.Value && RestoreClassicSuit.HasAllOtherSuits())
                RestoreClassicSuit.SpawnClassicSuit();
        }
    }
}