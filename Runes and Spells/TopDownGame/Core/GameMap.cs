using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Runes_and_Spells.TopDownGame.Core.Utility;
using Runes_and_Spells.TopDownGame.NPCData;
using Runes_and_Spells.TopDownGame.Objects;

namespace Runes_and_Spells.TopDownGame.Core;

public class GameMap
{
    public Tile[,] Tiles { get; private set; }
    public List<Tile[,]> Chunks { get; private set; }
    public const int MapWidth = 160;
    public const int MapHeight = 64;
    public const int TileSize = 64;
    public const int ChunkSize = 8;

    private List<Rectangle> mudPuddlesRectangles = new List<Rectangle>()
    {
        new Rectangle(7424, 512, TileSize*2, TileSize*2)
    };

    private List<Rectangle> NoGenerationAreas = new List<Rectangle>()
    {
        new Rectangle(MapWidth*64-1024, 0, 1024, 1024), //start
        new Rectangle((MapWidth-50)*TileSize-160, 604-160, 320, 320), //woodman
        new Rectangle(MapWidth/2*TileSize-160, 672-160, 320, 320), //Hunter
        new Rectangle(MapWidth*3*TileSize/10 - 160, 1300-160, 320, 320), //Bard
        new Rectangle(1500 - 160, 900-160, 320, 320), //Witch
        new Rectangle(MapWidth*TileSize*3/10-160, 3200-160, 320, 320), //Trader
        new Rectangle(MapWidth*TileSize/2-320, 3700-320, 640, 640), //Mage
        new Rectangle(MapWidth*TileSize*9/10-160, 3000-160, 320, 320), // Thief
    };

    private readonly TopDownCore _gameCore;
    private Texture2D _gameMapSpriteSheet;
    
    public List<MapObject> FrontObjects;
    public List<MudPuddle> MudPuddles;
    public List<NPC> NPCList = new List<NPC>();

    public enum TileType
    {
        Grass,
        Road,
        Water
    }
    
    public GameMap(TopDownCore core)
    {
        _gameCore = core;
        _gameMapSpriteSheet = core.Game.Content.Load<Texture2D>("top-down/spritesheets/map_objects");
    }

    public void GenerateMap(Vector2 cameraPos)
    {
        Tiles = new Tile[MapWidth, MapHeight];
        Chunks = new List<Tile[,]>();
        for (int i = 0; i < MapHeight*MapWidth/ChunkSize/ChunkSize; i++)
        {   
            Chunks.Add(new Tile[ChunkSize,ChunkSize]);
        }

        var ChunkIndexY = 0;
        for (int x = 0; x < MapWidth; x++)
        {
            var chunkIndexX = -1;
            if (x % ChunkSize == 0)
            {
                ChunkIndexY = MapHeight / ChunkSize * (x / 8);
            }
            for (int y = 0; y < MapHeight; y++)
            {
                var type = (TileType)Random.Shared.Next(Enum.GetValues(typeof(TileType)).Length);
                Tiles[x, y] = new Tile(TileType.Grass, new Vector2(
                    x*TileSize - cameraPos.X, y*TileSize - cameraPos.Y));
                
                if (y % ChunkSize == 0)
                {
                    chunkIndexX++;
                }

                Chunks[chunkIndexX + ChunkIndexY][x % ChunkSize, y % ChunkSize] = Tiles[x, y];
            }
        }
        GenerateLake();
        GenerateRiver();
        AddDecorationObjects();
        GenerateSolidObjects();
        GenerateChests();
        AddPreGeneratedSolidObjects();
        
    }

    public void GenerateLake()
    {
        var center = new Vector2(MapWidth / 2 + Random.Shared.Next(-1, 1),MapHeight/2 + Random.Shared.Next(-1,1) - 2);
        var moves = new List<Vector2>()
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1),
        };
        
        var fillArea = new List<Vector2>()
        {
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(2, 0),
            new Vector2(2, 1),
            new Vector2(0, 2),
            new Vector2(1, 2),
            new Vector2(-1,0),
            new Vector2(-1, 1),
            new Vector2(0,-1),
            new Vector2(1,-1)
        };
        var currentTile = Tiles[(int)center.X, (int)center.Y];
        for (int i = 0; i < 28; i++)
        {
            var lastCoordinates = new Vector2((int)center.X, (int)center.Y);
            while (currentTile.Type == TileType.Water)
            {
                var move = moves[Random.Shared.Next(moves.Count)];
                if ((int)(center.X + move.X) < 0 ||
                    (int)(center.X + move.X) > MapWidth ||
                    (int)(center.Y + move.Y) < 0 ||
                    (int)(center.Y + move.Y) > MapHeight)
                {
                    Console.WriteLine($"Outside = {(int)(center.X + move.X)}, {(int)(center.Y + move.Y)}");
                    continue;
                }

                lastCoordinates.X += move.X;
                lastCoordinates.Y += move.Y;
                currentTile = Tiles[(int)lastCoordinates.X, (int)lastCoordinates.Y];
            }

            foreach (var move in fillArea)
            {
                Tiles[(int)(lastCoordinates.X + move.X), (int)(lastCoordinates.Y + move.Y)].Type = TileType.Water;
                Tiles[(int)(lastCoordinates.X + move.X), (int)(lastCoordinates.Y + move.Y)].IsPassable = false;
            }
        }
    }

    public void GenerateRiver()
    {
        var startPosY = Tiles.GetLength(1) / 2;
        var posY = startPosY;
        var maxMove = 10;
        for (var i = 0; i < MapWidth; i += 3)
        {
            for (int j = -3; j < 4; j++)
            {
                Tiles[i, posY + j].Type = TileType.Water;
                Tiles[i, posY + j].IsPassable = false;
                if (i + 1 < Tiles.GetLength(0))
                {
                    Tiles[i+1, posY + j].Type = TileType.Water;
                    Tiles[i+1, posY + j].IsPassable = false;
                }
                if (i + 2 < Tiles.GetLength(0))
                {
                    Tiles[i+2, posY + j].Type = TileType.Water;
                    Tiles[i+2, posY + j].IsPassable = false;
                }
            }

            posY += Random.Shared.Next(-1, 2);
            posY = Math.Min(Math.Max(startPosY - maxMove, posY), startPosY + maxMove);
        }
    }
    
    private void AddDecorationObjects()
    {
        foreach (var chunk in Chunks)
        {
            var generatedObjects = new List<MapObjectInfo>();
            var tries = 10;
            for (var i = 0; i < tries; i++)
            {
                var tilePos = new Vector2(Random.Shared.Next(ChunkSize), Random.Shared.Next(ChunkSize));
                var tile = chunk[(int)tilePos.X, (int)tilePos.Y];
                if (tile.Type != TileType.Water && tile.DecorationObject is null && 
                    !mudPuddlesRectangles.Any(r => r.Contains(tile.PositionInPixels)))
                {
                    var variantsToGenerate = new List<MapObjectInfo>();
                    for (int j = 0; j < 4; j++)
                        variantsToGenerate.Add(AllMapStaticObjectsInfo.AllFloorObjects[Random.Shared.Next(AllMapStaticObjectsInfo.AllFloorObjects.Count)]);
                    
                    var newObject = variantsToGenerate.MaxBy(obj => obj.weight);
                    if (generatedObjects.Count(o => o.Name == newObject.Name) >= newObject.weight ||
                        tilePos.X + newObject.TilesSize.X >= ChunkSize ||
                        tilePos.Y + newObject.TilesSize.Y >= ChunkSize)
                    {
                        continue;
                    }
                    tile.DecorationObject = newObject;
                    generatedObjects.Add(newObject);
                }
            }
        }
        
        
    }

    private void GenerateSolidObjects()
    {
        FrontObjects = new List<MapObject>();
        MudPuddles = new List<MudPuddle>();
        
        
        //FrontObjects.Add(new Chest(new Vector2(1224, 1224), silverChestInfo, _gameCore, Chest.ChestType.Silver));

        //FrontObjects.Add(new Chest(new Vector2(1124, 1124), goldenChestInfo, _gameCore, Chest.ChestType.Gold));
        
        //FrontObjects.Add(new Chest(new Vector2(1324, 1324), emeraldChestInfo, _gameCore, Chest.ChestType.Emerald));
        
        foreach (var chunk in Chunks)
        {
            var generatedObjects = new List<MapObject>();
            var tries = 4;
           

            if (chunk[0,0].PositionInPixels.X >= 3300 && chunk[0,0].PositionInPixels.X < 6144 &&
                chunk[0,0].PositionInPixels.Y < 320)
            {
                tries = 15;
            }
            for (var i = 0; i < tries; i++)
            {
                var tile = chunk[Random.Shared.Next(ChunkSize), Random.Shared.Next(ChunkSize)];
                var failed = false;
                if (tile.Type != TileType.Water && tile.DecorationObject is null)
                {
                    var newObjectInfo = SelectRandomSolidObjectInfo();
                    if (generatedObjects.Count(o => o.Name == newObjectInfo.Name) >= newObjectInfo.GenerationWeight ||
                        IsThereSolidObject(
                            tile.PositionInPixels.X + newObjectInfo.CollisionRectangle.X + newObjectInfo.CollisionRectangle.Width, 
                            tile.PositionInPixels.Y + TileSize - newObjectInfo.CollisionRectangle.Y - newObjectInfo.CollisionRectangle.Height) ||
                        tile.PositionInPixels.X + newObjectInfo.SpriteSheetRectangle.Width > MapWidth * TileSize ||
                        tile.PositionInPixels.Y + TileSize - newObjectInfo.SpriteSheetRectangle.Height < 0 ||
                        NoGenerationAreas
                            .Any(r => r.Contains(tile.PositionInPixels) || 
                                                   r.Contains(tile.PositionInPixels.X - newObjectInfo.SpriteSheetRectangle.Width, 
                                                       tile.PositionInPixels.Y + newObjectInfo.SpriteSheetRectangle.Height)))
                    {
                        continue;
                    }

                    for (var j = 0; j < newObjectInfo.SpriteSheetRectangle.Width; j += TileSize)
                    {
                        if (GetTileByPixelCoordinates(tile.PositionInPixels.X + j, tile.PositionInPixels.Y).Type == TileType.Water)
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (failed)
                        continue;

                    var newObject = new MapObject(
                        new Vector2(tile.PositionInPixels.X, tile.PositionInPixels.Y + TileSize),
                        _gameMapSpriteSheet,
                        newObjectInfo.SpriteSheetRectangle,
                        newObjectInfo.CollisionRectangle,
                        newObjectInfo.Name, _gameCore);
                    if (!newObject.Name.Contains("tree"))
                    {
                        newObject.PositionInPixelsLeftBottom.Y -= 1;
                    }

                    FrontObjects.Add(newObject);

                    generatedObjects.Add(newObject);
                }
            }
        }
    }

    private void GenerateChests()
    {
        var silverChestInfo = AllMapDynamicObjects.AllObjects["chest_silver"];
        var goldenChestInfo = AllMapDynamicObjects.AllObjects["chest_golden"];
        var emeraldChestInfo = AllMapDynamicObjects.AllObjects["chest_emerald"];
        var selectedChunks = new List<Tile[,]>();
        for (int i = 0; i < 50; i++)
        {
            var tries = 0;
            while (true)
            {
                var chunk = Chunks[Random.Shared.Next(Chunks.Count)];
                if (!selectedChunks.Contains(chunk))
                {
                    selectedChunks.Add(chunk);
                    break;
                }
                tries++;
                if (tries>20)
                {
                    break;
                }
            }
        }
        
        foreach (var tiles in selectedChunks)
        {
            var tilesTemp = tiles;
            var SilverPlacedhere = false;
            var indexX = Random.Shared.Next(0, tiles.GetLength(0));
            var indexY = Random.Shared.Next(0, tiles.GetLength(1));
            var tile = tiles[indexX, indexY];
            var tries = 0;
            while (!SilverPlacedhere)
            {
                tries++;
                tile = tilesTemp[Random.Shared.Next(3, tilesTemp.GetLength(0) - 3),
                        Random.Shared.Next(3, tilesTemp.GetLength(1) - 3)];
                if (!IsThereSolidObject(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height) && tile.Type == TileType.Grass)
                {
                    SilverPlacedhere = true;
                    FrontObjects.Add(new Chest(
                        new Vector2(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height), 
                        silverChestInfo, _gameCore, Chest.ChestType.Silver));
                    break;
                }

                if (tries > 5)
                {
                    tilesTemp = Chunks[Random.Shared.Next(Chunks.Count)];
                    tries = 0;
                }
            }
        }

        var GoldPlaced = 0;
        while (GoldPlaced < 20)
        {
            var tile = Tiles[Random.Shared.Next(Tiles.GetLength(0)), Random.Shared.Next(Tiles.GetLength(1))];
            if (!IsThereSolidObject(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height) && tile.Type == TileType.Grass)
            {
                FrontObjects.Add(new Chest(
                    new Vector2(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height), 
                    goldenChestInfo, _gameCore, Chest.ChestType.Gold));
                GoldPlaced++;
            }
        }
        var EmeraldPlaced = 0;
        while (EmeraldPlaced < 10)
        {
            var tile = Tiles[Random.Shared.Next(Tiles.GetLength(0)), Random.Shared.Next(Tiles.GetLength(1))];
            if (!IsThereSolidObject(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height) && tile.Type == TileType.Grass)
            {
                FrontObjects.Add(new Chest(
                    new Vector2(tile.PositionInPixels.X, tile.PositionInPixels.Y + silverChestInfo.SpriteSheetRectangle.Height), 
                    emeraldChestInfo, _gameCore, Chest.ChestType.Emerald));
                EmeraldPlaced++;
            }
        }
    }
    

    private void AddPreGeneratedSolidObjects()
    {
        AddNPCs();

        MudPuddles.Add(new MudPuddle(new Vector2((MapWidth-12)*TileSize, 640), 
            AllMapDynamicObjects.AllObjects["mud_puddle"], AllMapDynamicObjects.PlusTextureForChests, _gameCore));
        
        var fenceHousePositions = new List<Vector2>()
        {
            new Vector2(MapWidth*TileSize,384),
            new Vector2((MapWidth-1)*TileSize,384),
            new Vector2((MapWidth-2)*TileSize,384),
            new Vector2(MapWidth*TileSize,704),
            new Vector2((MapWidth-1)*TileSize,704),
            new Vector2((MapWidth-2)*TileSize,704)
        };
        var fenceInfo = AllMapStaticObjectsInfo.AllOtherSolidObjects["fence_horizontal_1"];
        var fenceLeftEnd = AllMapStaticObjectsInfo.AllOtherSolidObjects["fence_horizontal_end_left_1"];
        foreach (var fencePos in fenceHousePositions)
        {
            FrontObjects.Add(new MapObject(
                fencePos, _gameMapSpriteSheet, fenceInfo.SpriteSheetRectangle, fenceInfo.CollisionRectangle, fenceInfo.Name, _gameCore));
        }
        FrontObjects.Add(new MapObject(
            new Vector2((MapWidth-3)*TileSize, 704), _gameMapSpriteSheet, fenceLeftEnd.SpriteSheetRectangle, 
            fenceLeftEnd.CollisionRectangle, fenceLeftEnd.Name, _gameCore));
        FrontObjects.Add(new MapObject(
            new Vector2((MapWidth-3)*TileSize, 384), _gameMapSpriteSheet, fenceLeftEnd.SpriteSheetRectangle, 
            fenceLeftEnd.CollisionRectangle, fenceLeftEnd.Name, _gameCore));
        var flowersMarketPositions = new List<Vector2>()
        {
            new Vector2((MapWidth-9)*TileSize, 192),
            new Vector2((MapWidth-9)*TileSize, 64),
            new Vector2((MapWidth-9)*TileSize, 128),
            new Vector2((MapWidth-3)*TileSize, 192),
            new Vector2((MapWidth-3)*TileSize, 64),
            new Vector2((MapWidth-3)*TileSize, 128)
        };
        var flowersVariants = new List<(Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle, string Name)>()
        {
            AllMapStaticObjectsInfo.AllOtherSolidObjects["flower_bed_yellow"],
            AllMapStaticObjectsInfo.AllOtherSolidObjects["flower_bed_white"],
            AllMapStaticObjectsInfo.AllOtherSolidObjects["flower_bed_blue"]

        };
        foreach (var flowersPos in flowersMarketPositions)
        {
            var flower = flowersVariants[Random.Shared.Next(flowersVariants.Count)];
            FrontObjects.Add(new MapObject(
                flowersPos, _gameMapSpriteSheet, flower.SpriteSheetRectangle, 
                flower.CollisionRectangle, flower.Name, _gameCore));
        }
    }

    private void AddNPCs()
    {
        NPCList.Add(
            new NPC(new Vector2((MapWidth-50)*TileSize, 604), AllMapDynamicObjects.AllObjects["npc_woodman"], "npc_name", "Дровосек Иван",
                NPCType.Woodman, AllDialogs.DialogInfo[NPCType.Woodman].FirstPhrase, _gameCore)
        );
        
        var x = MapWidth - 16;
        var lastTile = Tiles[MapWidth-16, 0];
        Vector2 fishermanPos = default;
        for (var y = 0; y < MapHeight; y++)
        {
            if (Tiles[x, y].Type == TileType.Water && lastTile.Type == TileType.Grass)
            {
                fishermanPos = new Vector2(lastTile.PositionInPixels.X, lastTile.PositionInPixels.Y);
                break;
            }
            lastTile = Tiles[x, y];
        }
        NPCList.Add(
            new NPC(fishermanPos, AllMapDynamicObjects.AllObjects["npc_fisherman"], "npc_fisherman", "Рыбак Финли",
                NPCType.FisherMan, AllDialogs.DialogInfo[NPCType.FisherMan].FirstPhrase, _gameCore));
        NoGenerationAreas.Add(new Rectangle((int)fishermanPos.X - 160, (int)fishermanPos.Y - 160, 320, 320));
        
        NPCList.Add(
            new NPC(new Vector2(MapWidth/2*TileSize, 672), AllMapDynamicObjects.AllObjects["npc_hunter"], 
                "npc_hunter", "Охотник Тормунд", NPCType.Hunter,AllDialogs.DialogInfo[NPCType.Hunter].FirstPhrase,_gameCore));
        
        NPCList.Add(
            new NPC(new Vector2(MapWidth*3*TileSize/10, 1300), AllMapDynamicObjects.AllObjects["npc_bard"], 
                "npc_bard", "Бард Василек", NPCType.Bard,AllDialogs.DialogInfo[NPCType.Bard].FirstPhrase,_gameCore));
        NPCList.Add(
            new NPC(new Vector2(1500, 900), AllMapDynamicObjects.AllObjects["npc_witch"], 
                "npc_witch", "Целительница Вивиана", NPCType.Witch,AllDialogs.DialogInfo[NPCType.Witch].FirstPhrase,_gameCore));
        NPCList.Add(new NPC(new Vector2(MapWidth*TileSize*3/10, 3200), AllMapDynamicObjects.AllObjects["npc_trader"], 
            "npc_trader", "Торговец Альфред", NPCType.Trader, AllDialogs.DialogInfo[NPCType.Trader].FirstPhrase,_gameCore));
        NPCList.Add(new NPC(new Vector2(MapWidth*TileSize/2, 3700), AllMapDynamicObjects.AllObjects["npc_mage"], 
            "npc_mage", "Волшебник Малфиус", NPCType.Trader, AllDialogs.MageDialog.FirstPhrase,_gameCore));
        NPCList.Add(new NPC(new Vector2(MapWidth*TileSize*9/10, 3000), AllMapDynamicObjects.AllObjects["npc_thief"], 
            "npc_thief", "Вор", NPCType.Trader, AllDialogs.DialogInfo[NPCType.Thief].FirstPhrase,_gameCore));
    }

    public Tile GetTileByPixelCoordinates(float x, float y)
    {
        return Tiles[(int)x / TileSize, (int)y / TileSize];
    }

    public (Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle, bool GroupGeneration, int GenerationWeight, string Name) SelectRandomSolidObjectInfo()
    {
        var variantsToGenerate = new List<(Rectangle SpriteSheetRectangle, Rectangle CollisionRectangle,
            bool GroupGeneration, int GenerationWeight, string Name)>();
                    
        for (int j = 0; j < 5; j++)
            variantsToGenerate.Add(
                AllMapStaticObjectsInfo.AllGeneratedSolidObjects[
                    Random.Shared.Next(AllMapStaticObjectsInfo.AllGeneratedSolidObjects.Count)]);

        return variantsToGenerate.MaxBy(obj => obj.GenerationWeight);
    }
    
    public bool IsThereSolidObject(float x, float y)
    {
        return FrontObjects
            .Concat(NPCList)
            .Where(o => o.IsVisible)
            .Any(o =>
                new Rectangle(
                        (int)o.PositionInPixelsLeftBottom.X + o.CollisionRectangle.X,
                        (int)o.PositionInPixelsLeftBottom.Y - o.CollisionRectangle.Y - o.CollisionRectangle.Height,
                        o.CollisionRectangle.Width, 
                        o.CollisionRectangle.Height)
                    .Contains(x, y)
            );
    }
}