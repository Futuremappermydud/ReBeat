﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using ReBeat.HarmonyPatches.BeamapData;
using ReBeat.HarmonyPatches.Energy;

namespace ReBeat.HarmonyPatches.Score {
    [HarmonyPatch(typeof(global::ScoreController))]
    class ScoreController {
        internal static int TotalCutScore { get; private set; }
        internal static int TotalNotes { get; private set; }
        
        internal static int CurrentScore { get; private set; }
        internal static int CurrentMaxScore { get; private set; }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(global::ScoreController.Start))]
        static void OnStart(ScoreController __instance) {
            if (!Config.Instance.Enabled) return;
            TotalCutScore = 0;
            TotalNotes = 0;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(global::ScoreController.LateUpdate))]
        static void ScoreUpdate(ref int ____multipliedScore, ref int ____modifiedScore,
            ref int ____immediateMaxPossibleMultipliedScore, ref int ____immediateMaxPossibleModifiedScore,
            ref GameplayModifiersModelSO ____gameplayModifiersModel,
            ref List<GameplayModifierParamsSO> ____gameplayModifierParams,
            ref IGameEnergyCounter ____gameEnergyCounter, ref Action<int, int> ___scoreDidChangeEvent) {
            if (!Config.Instance.Enabled) return;

            double acc = ((double)TotalCutScore / ((double)TotalNotes*100d))*100d;
            int noteCount = NoteCount.Count;
            int misses = EnergyController.EnergyCounter.TotalMisses;
            int maxCombo = EnergyController.EnergyCounter.MaxCombo;

            double missCountCurve = noteCount / (50 * Math.Pow(misses, 2) + noteCount) * ((50d * noteCount + 1) / (50d * noteCount)) - 1 / (50d * noteCount);
            double maxComboCurve = Math.Pow(TotalNotes / ((1 - Math.Sqrt(0.5)) * maxCombo - TotalNotes), 2) - 1; 
            //const double j = 1d / 1020734678369717893d;
            double accCurve = (19.0444 * Math.Tan((Math.PI / 133d) * acc - 4.22) + 35.5) * 0.01; // rip j

            int score = TotalCutScore == 0 || TotalNotes == 0 ? 0 : (int)(1_000_000d * ((missCountCurve * 0.3) + (maxComboCurve * 0.3) + (accCurve * 0.4)) * ((double)TotalNotes / (double)noteCount));
            ____multipliedScore = score;
            ____immediateMaxPossibleMultipliedScore = (int)(1_000_000d * ((double)TotalNotes / (double)noteCount)); 

            float totalMultiplier = ____gameplayModifiersModel.GetTotalMultiplier(____gameplayModifierParams, ____gameEnergyCounter.energy);
            ____modifiedScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(____multipliedScore, totalMultiplier);
            ____immediateMaxPossibleModifiedScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(____immediateMaxPossibleMultipliedScore, totalMultiplier);

            CurrentScore = score;
            CurrentMaxScore = ____immediateMaxPossibleMultipliedScore;

            Action<int, int> action = ___scoreDidChangeEvent;
            if (action == null) return;
            action(____multipliedScore, ____modifiedScore);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(global::ScoreController.DespawnScoringElement))]
        static void HandleScoringElement(ScoringElement scoringElement) {
            if (!Config.Instance.Enabled) return;
            if (scoringElement.noteData.gameplayType == NoteData.GameplayType.Bomb) return;

            TotalCutScore += scoringElement.cutScore;
            TotalNotes++;
        }
    }
}