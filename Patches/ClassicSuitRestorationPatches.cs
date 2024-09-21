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
            if (!Plugin.configUnlockable.Value || ES3.Load("ClassicSuitRestoration_Unlocked", GameNetworkManager.Instance.currentSaveFileName, false))
                RestoreClassicSuit.SpawnClassicSuit();
        }

        [HarmonyPatch(typeof(StartOfRound), "ResetShip")]
        [HarmonyPostfix]
        public static void PostResetShip(StartOfRound __instance)
        {
            if (__instance.IsServer && GameNetworkManager.Instance != null)
                ES3.DeleteKey("ClassicSuitRestoration_Unlocked", GameNetworkManager.Instance.currentSaveFileName);
            __instance.StartCoroutine(RestoreClassicSuit.CheckSuitsAfterDelay());
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
                RestoreClassicSuit.UnlockClassicSuit();
        }
    }
}