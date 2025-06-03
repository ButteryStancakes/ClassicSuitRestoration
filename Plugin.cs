using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ClassicSuitRestoration
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(GUID_LOBBY_COMPATIBILITY, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(GUID_KEEP_UNLOCKS, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal const string PLUGIN_GUID = "butterystancakes.lethalcompany.classicsuitrestoration", PLUGIN_NAME = "Classic Suit Restoration", PLUGIN_VERSION = "2.1.3";
        public static ConfigEntry<bool> configUnlockable, configOfficial, configBirthday;

        const string GUID_LOBBY_COMPATIBILITY = "BMX.LobbyCompatibility", GUID_KEEP_UNLOCKS = "butterystancakes.lethalcompany.keepunlocks";

        internal static new ManualLogSource Logger;

        void Awake()
        {
            Logger = base.Logger;

            if (Chainloader.PluginInfos.ContainsKey(GUID_LOBBY_COMPATIBILITY))
            {
                Logger.LogInfo("CROSS-COMPATIBILITY - Lobby Compatibility detected");
                LobbyCompatibility.Init();
            }

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

            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }
    }
}