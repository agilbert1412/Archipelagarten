using KG2;
using System.Collections.Generic;

namespace Archipelagarten2.Constants
{
    public static class Missions
    {
        public const string TALE_OF_JANITORS = "A Tale Of Two Janitors";
        public const string FLOWERS_FOR_DIANA = "Flowers For Diana";
        public const string HITMAN_GUARD = "The Hitman's Potty Guard";
        public const string CAIN_NOT_ABLE = "Cain's Not Able";
        public const string OPPOSITES_ATTRACT = "Opposites Attract";
        public const string DODGE_A_NUGGET = "If You Can Dodge A Nugget";
        public const string THINGS_GO_BOOM = "Things That Go Boom";
        public const string BREAKING_SAD = "Breaking Sad";
        public const string CREATURE_FEATURE = "Creature Feature";
        public const string SECRET_ENDING = "Secret Ending";

        public static readonly string[] ALL_MISSIONS = new[]
        {
            TALE_OF_JANITORS, FLOWERS_FOR_DIANA, HITMAN_GUARD,
            CAIN_NOT_ABLE, OPPOSITES_ATTRACT, DODGE_A_NUGGET,
            THINGS_GO_BOOM, BREAKING_SAD, CREATURE_FEATURE,
        };

        public static readonly Dictionary<Item, string> ItemToMissionMap = new()
        {
            {Item.APlus, FLOWERS_FOR_DIANA},
            {Item.ApplesoftPin, HITMAN_GUARD},
            {Item.Chemical, CAIN_NOT_ABLE},
            {Item.LaserCutter, OPPOSITES_ATTRACT},
            {Item.MonstermonPlushie, DODGE_A_NUGGET},
            {Item.PennyController, BREAKING_SAD},
            {Item.ToolBelt, TALE_OF_JANITORS},
            {Item.UltraBomb, THINGS_GO_BOOM},
        };
    }
}
