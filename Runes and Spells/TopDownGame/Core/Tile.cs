using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.TopDownGame.Core.Utility;

namespace Runes_and_Spells.TopDownGame.Core;

public class Tile
{
    public bool IsPassable { get; set; }
    public Vector2 PositionInPixels { get; private set; }
    public bool IsVisible { get; set; }
    public GameMap.TileType Type { get; set; }

    public MapObjectInfo DecorationObject { get; set; }
    
    public Tile(GameMap.TileType type, Vector2 positionInPixels, MapObjectInfo mapObject = null)
    {
        PositionInPixels = positionInPixels;
        IsPassable = type != GameMap.TileType.Water;
        Type = type;
        DecorationObject = mapObject;
    }
}