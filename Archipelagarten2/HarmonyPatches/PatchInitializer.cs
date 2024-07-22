using Archipelagarten2.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Death;
using Archipelagarten2.Items;
using BepInEx.Logging;
using HarmonyLib;
using Archipelagarten2.HarmonyPatches.GenericPatches;
using Archipelagarten2.HarmonyPatches.NPCPatches;
using Archipelagarten2.HarmonyPatches.DebugPatches;
using Archipelagarten2.HarmonyPatches.MoneyPatches;

namespace Archipelagarten2.HarmonyPatches
{
    public class PatchInitializer
    {
        public PatchInitializer()
        {
        }

        public void InitializeAllPatches(ManualLogSource logger, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            InitializeDebugPatches(logger);
            InitializeGenericPatches(logger, archipelago, locationChecker);
            InitializeNPCPatches(logger, archipelago, locationChecker);
            InitializeMoneyPatches(logger, archipelago, locationChecker);
            InitializeDeathPatches(logger, archipelago, locationChecker);
        }

        private static void InitializeDebugPatches(ManualLogSource logger)
        {
            ChangeRoomPatch.Initialize(logger);
            GoToNextAreaPatch.Initialize(logger);
        }

        private static void InitializeGenericPatches(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            var gameStateWriter = new GameStateWriter(archipelago);
            CreateSavePatch.Initialize(logger, archipelago, gameStateWriter);
            LoadKG2Patch.Initialize(logger, archipelago);
            EndDayPanelPatch.Initialize(logger, archipelago, locationChecker);
            EnterSanctumPatch.Initialize(logger, archipelago, locationChecker);
            UnlockMonstermonPatch.Initialize(logger, archipelago, locationChecker);
            UnlockFullOutfitPatch.Initialize(logger, archipelago, locationChecker);
            AddFlagPatch.Initialize(logger, archipelago, locationChecker);
        }

        private static void InitializeNPCPatches(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            BobCheckStatusPatch.Initialize(logger, archipelago, locationChecker);
            BobKissingPatch.Initialize(logger, archipelago, locationChecker);
            PrincipalSendToRecessPatch.Initialize(logger, archipelago, locationChecker);
            ScienceTeacherReturnToStudyHallPatch.Initialize(logger, archipelago, locationChecker);
            TalkToAgnesPatch.Initialize(logger, archipelago, locationChecker);
            CindyTissuePatch.Initialize(logger, archipelago, locationChecker);
            CarlaPoppedFirecrackerPatch.Initialize(logger, archipelago, locationChecker);
            SpareTheCatPatch.Initialize(logger, archipelago, locationChecker);
            CheckItemUnlockPatch.Initialize(logger, archipelago, locationChecker);
        }

        private static void InitializeMoneyPatches(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            FelixGetFivePatch.Initialize(logger, archipelago, locationChecker);
            FelixGetNickelPatch.Initialize(logger, archipelago, locationChecker);
            GetNuggetCaveMoneyPatch.Initialize(logger, archipelago, locationChecker);
            MontySellInhalerPatch.Initialize(logger, archipelago, locationChecker);
            MontySellWeedMonstermonPatch.Initialize(logger, archipelago, locationChecker);
            MontySellWeedPatch.Initialize(logger, archipelago, locationChecker);
            TakeTedCubbyMoneyPatch.Initialize(logger, archipelago, locationChecker);
        }

        private static void InitializeDeathPatches(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            CallDeathPatch.Initialize(logger, archipelago, locationChecker);
            DeathMessagePatch.Initialize(logger, archipelago, locationChecker);
        }
    }
}
