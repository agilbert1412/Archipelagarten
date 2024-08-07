﻿using Archipelagarten2.Archipelago;
using Archipelagarten2.Death;
using Archipelagarten2.Items;
using HarmonyLib;
using Archipelagarten2.HarmonyPatches.GenericPatches;
using Archipelagarten2.HarmonyPatches.NPCPatches;
using Archipelagarten2.HarmonyPatches.DebugPatches;
using Archipelagarten2.HarmonyPatches.MoneyPatches;
using Archipelagarten2.UnityObjects;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;

namespace Archipelagarten2.HarmonyPatches
{
    public class PatchInitializer
    {
        public PatchInitializer()
        {
        }

        public void InitializeAllPatches(ILogger logger, Harmony harmony, KindergartenArchipelagoClient archipelago, LocationChecker locationChecker, GameObjectFactory gameObjectFactory, UnityActions unityActions, TrapManager trapManager)
        {
            InitializeDebugPatches(logger, unityActions, trapManager);
            InitializeGenericPatches(logger, archipelago, locationChecker, gameObjectFactory);
            InitializeNPCPatches(logger, archipelago, locationChecker);
            InitializeMoneyPatches(logger, archipelago, locationChecker);
            InitializeDeathPatches(logger, archipelago, locationChecker, gameObjectFactory);
        }

        private static void InitializeDebugPatches(ILogger logger, UnityActions unityActions, TrapManager trapManager)
        {
            ChangeRoomPatch.Initialize(logger);
            GoToNextAreaPatch.Initialize(logger);
            WalkToPointPatch.Initialize(logger);
            HandleInputPatch.Initialize(logger, unityActions, trapManager);
        }

        private static void InitializeGenericPatches(ILogger logger, KindergartenArchipelagoClient archipelago, LocationChecker locationChecker, GameObjectFactory gameObjectFactory)
        {
            var gameStateWriter = new GameStateWriter(archipelago);
            CreateSavePatch.Initialize(logger, archipelago, gameStateWriter);
            LoadKG2Patch.Initialize(logger, archipelago, gameObjectFactory);
            EndDayPanelPatch.Initialize(logger, archipelago, locationChecker);
            EnterSanctumPatch.Initialize(logger, archipelago, locationChecker);
            UnlockMonstermonPatch.Initialize(logger, archipelago, locationChecker);
            UnlockFullOutfitPatch.Initialize(logger, archipelago, locationChecker);
            AddFlagPatch.Initialize(logger, archipelago, locationChecker);
        }

        private static void InitializeNPCPatches(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
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

        private static void InitializeMoneyPatches(ILogger logger, KindergartenArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            FelixGetFivePatch.Initialize(logger, archipelago, locationChecker);
            FelixGetNickelPatch.Initialize(logger, archipelago, locationChecker);
            GetNuggetCaveMoneyPatch.Initialize(logger, archipelago, locationChecker);
            MontySellInhalerPatch.Initialize(logger, archipelago, locationChecker);
            MontySellWeedMonstermonPatch.Initialize(logger, archipelago, locationChecker);
            MontySellWeedPatch.Initialize(logger, archipelago, locationChecker);
            TakeTedCubbyMoneyPatch.Initialize(logger, archipelago, locationChecker);
        }

        private static void InitializeDeathPatches(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker, GameObjectFactory gameObjectFactory)
        {
            CallDeathPatch.Initialize(logger, archipelago, locationChecker);
            DeathMessagePatch.Initialize(logger, archipelago, locationChecker);
            BedroomStartPatch.Initialize(logger, archipelago, locationChecker, gameObjectFactory);
            SetFacialExpressionPatch.Initialize(logger);
        }
    }
}
