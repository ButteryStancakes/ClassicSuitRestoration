using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ClassicSuitRestoration
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "butterystancakes.lethalcompany.classicsuitrestoration", PLUGIN_NAME = "Classic Suit Restoration", PLUGIN_VERSION = "1.0.3";

        internal static new ManualLogSource Logger;

        void Awake()
        {
            new Harmony(PLUGIN_GUID).PatchAll();

            Logger = base.Logger;
            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }
    }
}