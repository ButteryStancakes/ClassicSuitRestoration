using BepInEx;
using HarmonyLib;

namespace ClassicSuitRestoration
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "butterystancakes.lethalcompany.classicsuitrestoration", PLUGIN_NAME = "Classic Suit Restoration", PLUGIN_VERSION = "1.0.0";

        void Awake()
        {
            new Harmony(PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }
    }
}