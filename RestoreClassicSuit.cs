using System.Reflection;

namespace ClassicSuitRestoration
{
    internal class RestoreClassicSuit
    {
        public static int classicSuitIndex = -1;

        public static void SpawnClassicSuit()
        {
            if (classicSuitIndex > 0 && StartOfRound.Instance.IsServer)
                typeof(StartOfRound).GetMethod("SpawnUnlockable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(StartOfRound.Instance, new object[]{classicSuitIndex});
        }
    }
}
