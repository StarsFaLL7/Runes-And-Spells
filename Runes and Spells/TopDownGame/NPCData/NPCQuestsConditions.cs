using System;
using System.Collections.Generic;
using System.Linq;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses;

namespace Runes_and_Spells.TopDownGame.NPCData;

public static class NPCQuestsConditions
{
    public static Dictionary<NPCType, Func<Item, bool>> Conditions = new()
    {
        { NPCType.Woodman, item => item.ID.Contains("sun")},
        { NPCType.FisherMan, item => item.ID.Contains("ocean") && !item.ID.Contains("blood") && !item.ID.Contains("distorted")},
        { NPCType.Hunter, item =>
        {
            var restricted = new string[]
            {
                "scroll_grass_ice_2",
                "scroll_grass_ice_1",
                "scroll_fire_ice_2",
                "blood",
                "black"
            };
            return item.ID.Contains("ice") && !restricted.Any(id => item.ID.Contains(id));
        }},
        {
            NPCType.Bard, item => item.ID.Contains("scroll_water_moon_2") || item.ID.Contains("scroll_water_blood_1") || item.ID.Contains("scroll_grass_moon_2") ||
                                  item.ID.Contains("scroll_moon_moon_1")
        },
        { NPCType.Witch, item => item.ID.Contains("life") && item.ID.Contains("blood") && !item.ID.Contains("scroll_blood_blood_1") 
                                 && !item.ID.Contains("scroll_ice_blood_1")},
        { NPCType.Trader, item => item.ID.Contains("scroll_water_grass_1") || item.ID.Contains("scroll_water_grass_2")},
        { NPCType.Thief, item => item.ID.Contains("scroll_air_moon_1")}
    };
}