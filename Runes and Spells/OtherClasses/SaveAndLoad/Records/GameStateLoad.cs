namespace Runes_and_Spells.OtherClasses.SaveAndLoad.Records;

public record GameStateLoad(int Balance, int DayCount, float Energy, bool IsFinalQuestIsPlaying, bool PlayerHasWings, 
    bool PlayerHasCompass, string FinalScroll, GameScreen LastScreen, int RunesUnlocked, int ScrollsUnlocked);