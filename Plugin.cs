using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;

namespace ClassicSuitRestoration
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "butterystancakes.lethalcompany.classicsuitrestoration", PLUGIN_NAME = "Classic Suit Restoration", PLUGIN_VERSION = "1.2.1";
        public static ConfigEntry<bool> configUnlockable;

        internal static new ManualLogSource Logger;

        void Awake()
        {
            configUnlockable = Config.Bind(
                "Misc",
                "Unlockable",
                false,
                "Instead of making the suit always available, unlock it once you complete a quota with all other suits on the rack.");

            new Harmony(PLUGIN_GUID).PatchAll();

            Logger = base.Logger;
            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }
    }
}