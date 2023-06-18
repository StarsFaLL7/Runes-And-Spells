using Microsoft.Xna.Framework;

namespace Runes_and_Spells.TopDownGame;

public record MapObjectInfo(Rectangle RectangleSpriteSheet, bool IsPassable, Vector2 TilesSize, int weight, string Name);