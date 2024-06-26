﻿using BibitesModsLoader;
using HarmonyLib;
using SimulationScripts.BibiteScripts;

namespace ExempleMod
{
    // ModInfo is generated by the .editorconfig,
    // You can copy it to include it in the project
    // or wait for me to release a nuggets (if I just want to release a nuggets)...
    [Mod(ModInfo.MOD_NAME, ModInfo.MOD_AUTHORS, ModInfo.MOD_VERSION,
        Description = "An exemple mod !")]
    public class ExempleMod : IMod
    {
        public Harmony Harmony { get; } = new Harmony(ModInfo.MOD_GUID);

        public void Enable()
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            Harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(VirusSpawner), nameof(VirusSpawner.tryInfectWithNewVirus))]
    public static class PatchTryInfectWithNewVirus
    {
        [HarmonyPrefix]
        public static void LogTryInfectWithNewVirus(VirusSpawner __instance)
        {
            var genes = string.Join(", ", __instance.virus.Genes);
            Loader.Log($"A new virus spawned ! genes: {genes}");

        }
    }
}
