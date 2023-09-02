using Microsoft.Xna.Framework;

namespace Runes_and_Spells.OtherClasses.SaveAndLoad.Records;

public record ChestLoad(bool IsOpened, Vector2 PositionInPixelsLeftBottom, string Name);