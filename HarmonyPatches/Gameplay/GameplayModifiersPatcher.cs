﻿using BeatSaber5.HarmonyPatches.UI;
using HarmonyLib;

namespace BeatSaber5.HarmonyPatches.Gameplay {
    [HarmonyPatch(typeof(GameplayModifiers))]
    class GameplayModifiersPatcher {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.energyType), MethodType.Getter)]
        static void EnergyType(ref GameplayModifiers.EnergyType __result) {
            if (!Config.Instance.Enabled) return;
            // always battery, we handle insta fail/ one hp in the energy patch
            __result = GameplayModifiers.EnergyType.Battery; 
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.noFailOn0Energy), MethodType.Getter)]
        static void NoFailOn0Energy(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.NoFail;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.instaFail), MethodType.Getter)]
        static void InstaFail(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.OneLife;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.enabledObstacleType), MethodType.Getter)]
        static void EnabledObstacleType(ref GameplayModifiers.EnabledObstacleType __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.NoWalls
                ? GameplayModifiers.EnabledObstacleType.NoObstacles
                : GameplayModifiers.EnabledObstacleType.All;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.strictAngles), MethodType.Getter)]
        static void StrictAngles(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.StrictAngles;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.disappearingArrows), MethodType.Getter)]
        static void DisappearingArrows(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.DisappearingArrows;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.ghostNotes), MethodType.Getter)]
        static void GhostNotes(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            // TODO: Legacy modifier support
            // hidden currently just uses ghost notes, so when ghost notes is added back it will also be set here,
            // and then there will be a check in the mesh patches for if hidden is enabled
            __result = Modifiers.instance.Hidden;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.noBombs), MethodType.Getter)]
        static void NoBombs(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.NoBombs;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeed), MethodType.Getter)]
        static void SongSpeed(ref GameplayModifiers.SongSpeed __result) {
            if (!Config.Instance.Enabled) return;

            if (Modifiers.instance.SlowerSong) {__result = GameplayModifiers.SongSpeed.Slower; return;}
            if (Modifiers.instance.FasterSong) {__result = GameplayModifiers.SongSpeed.Faster; return;}
            if (Modifiers.instance.SuperFastSong) { __result = GameplayModifiers.SongSpeed.SuperFast; return; }
            __result = GameplayModifiers.SongSpeed.Normal;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.noArrows), MethodType.Getter)]
        static void NoArrows(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = false; // TODO: legacy modifier support
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.proMode), MethodType.Getter)]
        static void ProMode(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.ProMode;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.zenMode), MethodType.Getter)]
        static void ZenMode(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.ZenMode;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.smallCubes), MethodType.Getter)]
        static void SmallCubes(ref bool __result) {
            if (!Config.Instance.Enabled) return;
            __result = false; // TODO: legacy modifier support
        }
        
        internal static float SongSpeedMultiplier { get; private set; }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.songSpeedMul), MethodType.Getter)]
        static void SongSpeedMul(ref float __result) {
            if (!Config.Instance.Enabled) return;
            if (__result < 0.9f) __result = 0.75f;
            SongSpeedMultiplier = __result;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayModifiers.cutAngleTolerance), MethodType.Getter)]
        static void CutAngleTolerance(ref float __result) {
            if (!Config.Instance.Enabled) return;
            __result = Modifiers.instance.ProMode ? 37.5f : 
                Modifiers.instance.EasyMode ? 52.5f : 45f;
        }
    }
}