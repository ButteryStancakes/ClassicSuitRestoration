using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;

namespace ClassicSuitRestoration
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("butterystancakes.lethalcompany.keepunlocks", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "butterystancakes.lethalcompany.classicsuitrestoration", PLUGIN_NAME = "Classic Suit Restoration", PLUGIN_VERSION = "2.1.1";
        public static ConfigEntry<bool> configUnlockable, configOfficial, configBirthday;

        internal static new ManualLogSource Logger;

        void Awake()
        {
            configOfficial = Config.Bind(
                "Suit",
                "Official",
                true,
                "Uses the official brown suit texture from v65.\nNOTE: This texture is official, but it is a recreation in the final game's visual style, and not the actual suit texture from earlier versions of the game.");

            configBirthday = Config.Bind(
                "Suit",
                "Birthday",
                false,
                "If using the anniversary suit (from the \"Official\" setting), this will complete the costume by adding back the party hat as well.");

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