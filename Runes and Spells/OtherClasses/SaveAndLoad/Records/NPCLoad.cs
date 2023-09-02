using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Runes_and_Spells.TopDownGame;

namespace Runes_and_Spells.OtherClasses.SaveAndLoad.Records;

public record NPCLoad(string VisibleName, bool IsQuestFinishedGood, bool IsQuestActive, bool IsQuestFinished, 
    int QuestEndDayCount, NPCType NPCType, int GivenScrollPower, bool IsFirstFinalQuestActive, 
    bool IsSecondFinalQuestActive, Vector2 PositionInPixelsLeftBottom, List<string> MageGivenScrollsIds, string Name);