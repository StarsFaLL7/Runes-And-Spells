using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.TopDownGame.Core;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Objects;

public class Chest : MapObject
{
    public enum ChestType
    {
        Silver,
        Gold,
        Emerald
    }

    private ChestType Type;
    
    private readonly TopDownCore _gameCore;

    private Timer _animationTimer;
    private int _animationFrame;
    
    private List<Texture2D> _givenItemTextures;
    private Texture2D _plusTexture = AllMapDynamicObjects.PlusTextureForChests;
    private Vector2 _givenItemPosition;
    private float _givenItemDeltaY;
    private Timer _giveItemTimer;
    private float _givenItemAlpha = 1;
    private int _giveItemIndex;
    public bool IsOpened;
    
    public Chest(Vector2 positionInPixelsLeftBottom, Rectangle spriteSheetRectangle, 
        Rectangle collisionRectangle, TopDownCore core, ChestType type, Texture2D spritesheet) 
        : base(positionInPixelsLeftBottom, spritesheet, spriteSheetRectangle, collisionRectangle, $"chest_{type.ToString().ToLower()}", core)
    {
        //PositionInPixelsLeftBottom = positionInPixelsLeftBottom;
        //SpriteSheetRectangle = spriteSheetRectangle;
        //CollisionRectangle = collisionRectangle;
        _gameCore = core;
        Type = type;
        //_spritesheet = spritesheet;
        _givenItemTextures = new List<Texture2D>();
        _animationTimer = new Timer(400, () =>
        {
            if (SpriteSheet.Width <= GameMap.TileSize * (_animationFrame + 1))
            {
                AnimationFinished();
            }
            else
            {
                _animationFrame++;
                _animationTimer.StartAgain();
            }
        });
    }

    public void SetOpenedFrame() => _animationFrame = SpriteSheet.Width / GameMap.TileSize - 1;

    public Chest(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo fromObjectInfo,TopDownCore core, ChestType type)
        : base(positionInPixelsLeftBottom, fromObjectInfo.Spritesheet, fromObjectInfo.SpriteSheetRectangle, fromObjectInfo.CollisionRectangle, 
            $"chest_{type.ToString().ToLower()}", core)
    {
        _gameCore = core;
        Type = type;
        _givenItemTextures = new List<Texture2D>();
        _animationTimer = new Timer(400, () =>
        {
            if (SpriteSheet.Width <= GameMap.TileSize * (_animationFrame + 1))
            {
                AnimationFinished();
            }
            else
            {
                _animationFrame++;
                _animationTimer.StartAgain();
            }
        });
    }

    public void Update()
    {
        if (Keyboard.GetState()[Keys.E] == KeyState.Down &&
            new Rectangle((int)PositionInPixelsLeftBottom.X - GameMap.TileSize/2, 
                (int)(PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - GameMap.TileSize/2),
                SpriteSheetRectangle.Width + GameMap.TileSize, SpriteSheetRectangle.Height + GameMap.TileSize)
                .Contains(_gameCore.PlayerPosition.X, _gameCore.PlayerPosition.Y) &&
            !IsOpened)
        {
            Clicked();
        }

        if (_animationTimer.IsRunning) _animationTimer.Tick();
        if (_giveItemTimer is not null && _giveItemTimer.IsRunning)
        {
            _giveItemTimer.Tick();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var drawPos = new Vector2(PositionInPixelsLeftBottom.X - _gameCore.CameraPosition.X, 
            PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Width - _gameCore.CameraPosition.Y);
        _givenItemPosition = new Vector2(
            drawPos.X + (GameMap.TileSize - _plusTexture.Width - 64)/2, drawPos.Y - _plusTexture.Height - _givenItemDeltaY);
        
        if (!IsVisible) return;

        spriteBatch.Draw(SpriteSheet, drawPos, new Rectangle(_animationFrame * GameMap.TileSize, 0, GameMap.TileSize, GameMap.TileSize), Color.White);
        if (_giveItemTimer is not null && _giveItemTimer.IsRunning)
        {
            spriteBatch.Draw(_plusTexture, 
                new Vector2(_givenItemPosition.X, 
                    _givenItemPosition.Y + (_givenItemTextures[_giveItemIndex].Height - _plusTexture.Height) / 2), Color.White*_givenItemAlpha);
            spriteBatch.Draw(_givenItemTextures[_giveItemIndex], new Vector2(_givenItemPosition.X + _plusTexture.Width, _givenItemPosition.Y), Color.White*_givenItemAlpha);
        }
    }
    
    private void Clicked()
    {
        var key = _gameCore.Game.Inventory.Items.FirstOrDefault(i => i.ID == $"key_{Type.ToString().ToLower()}" && i.Count > 0);
        if (key is not null)
        {
            IsOpened = true;
            key.SubtractCount(1);
            GameCore.Game.SubtractEnergy(1f);
            switch (Type)
            {
                case ChestType.Silver:
                    var allUnknownRunesId = ItemsDataHolder.Runes.RunesCreateRecipes.Values
                        .Where(id => !AllGameItems.KnownRunesCraftRecipes.ContainsKey(id))
                        .ToArray();
                    var necessaryElements = new Dictionary<NPCType, string>()
                    {
                        { NPCType.FisherMan, "water"},
                        { NPCType.Woodman, "fire"},
                        { NPCType.Hunter, "ice"},
                        { NPCType.Bard, "moon"},
                        { NPCType.Witch, "blood"}
                    };
                    foreach (var necEl in necessaryElements)
                    {
                        if (!_gameCore.Map.NPCList.First(f => f.NPCType == necEl.Key).IsQuestFinished &&
                            AllGameItems.KnownRunesCraftRecipes.All(k => !k.Key.Contains(necEl.Value)))
                        {
                            allUnknownRunesId = allUnknownRunesId.Where(r => r.Contains(necEl.Value)).ToArray();
                            necessaryElements.Remove(necEl.Key);
                        }
                    }

                    if (allUnknownRunesId.Length == 0)
                    {
                        GiveRandomRunes(1, 1, 2, 3);
                    }
                    else
                    {
                        var runeRecipeId = allUnknownRunesId[Random.Shared.Next(allUnknownRunesId.Length)];
                        AllGameItems.UnlockRuneCraftRecipe(runeRecipeId);
                        _givenItemTextures.Add(ItemsDataHolder.OtherItems.RuneCraftRecipePaperTexture);
                    }

                    AllGameItems.ChestOpenDefault.Play();
                    _gameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.Paper));
                    _givenItemTextures.Add(ItemsDataHolder.OtherItems.PaperTextureX64);
                    break;
                case ChestType.Gold:
                    GiveRandomRunes(2, 3);
                    UnlockRandomScrollRecipes(1);
                    AllGameItems.ChestOpenDefault.Play();
                    break;
                case ChestType.Emerald:
                    GiveRandomRunes(2, 1);
                    GiveRandomRunes(2, 2);
                    GiveRandomRunes(1, 3);
                    UnlockRandomScrollRecipes(2);
                    AllGameItems.ChestOpenMagic.Play();
                    break;
            }
            _animationTimer.Start();
        }
    }

    private void GiveRandomRunes(int count, params int[] powerTiers)
    {
        var allRunesToGiveTier = ItemsDataHolder.Runes.FinishedRunes
            .Select(r => r.Value)
            .Where(r => powerTiers.Contains(int.Parse(r.ID.Split('_')[3])) )
            .ToArray();
        
        for (var i = 0; i < count; i++)
        {
            var runeToGiveInfo = allRunesToGiveTier[Random.Shared.Next(allRunesToGiveTier.Length)];
            _gameCore.Game.Inventory.AddItem(new Item(runeToGiveInfo));
            _givenItemTextures.Add(ItemsDataHolder.Runes.RecipeX64RunesTextures[runeToGiveInfo.ID.Replace("finished_", "")]);
        }
    }

    private void UnlockRandomScrollRecipes(int count)
    {
        if (AllGameItems.KnownScrollsCraftRecipes.Count == ScrollsRecipes.Recipes.Values.Count)
            return;
        var allScrollRecipesToUnlock =
            ScrollsRecipes.Recipes.Values
                .Where(scrollInfo => !AllGameItems.KnownScrollsCraftRecipes.Contains(scrollInfo))
                .ToArray();
        var givenItems = new List<ScrollsRecipes.ScrollInfo>();
        for (var i = 0; i < count; i++)
        {
            allScrollRecipesToUnlock = allScrollRecipesToUnlock.Where(inf => !givenItems.Contains(inf)).ToArray();
            if (allScrollRecipesToUnlock.Length == 0)
                break;
            var scrollInfoToUnlock =
                allScrollRecipesToUnlock[Random.Shared.Next(allScrollRecipesToUnlock.Length)];
            
            AllGameItems.TryToUnlockScrollRecipe(scrollInfoToUnlock);
            _givenItemTextures.Add(ItemsDataHolder.OtherItems.ScrollCraftRecipePaperTexture);
            givenItems.Add(scrollInfoToUnlock);
        }
    }
    
    private void AnimationFinished()
    {
        _giveItemTimer = new Timer(50, () =>
        {
            if (_givenItemAlpha > 0)
            {
                _givenItemAlpha -= 0.05f;
                _givenItemDeltaY += 1;
                _giveItemTimer.StartAgain();
                return;
            }

            _giveItemIndex++;
            if (_giveItemIndex >= _givenItemTextures.Count)
                return;
            _givenItemDeltaY = 0;
            _givenItemAlpha = 1f;
            _giveItemTimer.StartAgain();
            
        });
        _giveItemTimer.Start();
        _givenItemPosition = new Vector2(
            PositionInPixelsLeftBottom.X - _gameCore.CameraPosition.X,
            PositionInPixelsLeftBottom.Y - SpriteSheetRectangle.Height - _gameCore.CameraPosition.Y);
    }
}