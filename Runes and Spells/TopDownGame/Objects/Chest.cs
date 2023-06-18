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

    public Chest(Vector2 positionInPixelsLeftBottom, AllMapDynamicObjects.DynamicObjectInfo FromObjectInfo,TopDownCore core, ChestType type)
        : base(positionInPixelsLeftBottom, FromObjectInfo.Spritesheet, FromObjectInfo.SpriteSheetRectangle, FromObjectInfo.CollisionRectangle, 
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
        var key = _gameCore.Game.Inventory.Items.FirstOrDefault(i => i.ID == $"key_{Type.ToString().ToLower()}");
        if (key is not null)
        {
            IsOpened = true;
            var allRunesToGiveTier3 = ItemsDataHolder.Runes.FinishedRunes
                .Select(r => r.Value)
                .Where(r => r.ID.Split('_')[3] == "3")
                .ToArray();
            var allRunesToGiveTier12 = ItemsDataHolder.Runes.FinishedRunes
                .Select(r => r.Value)
                .Where(r => r.ID.Split('_')[3] != "3")
                .ToArray();

            ItemInfo runeToGiveInfo;
            switch (Type)
            {
                case ChestType.Silver:
                    var allUnknownRunesId = ItemsDataHolder.Runes.RunesCreateRecipes.Values
                        .Where(id => !AllGameItems.KnownRunesCraftRecipes.ContainsKey(id))
                        .ToArray();
                    
                    if (allUnknownRunesId.Length == 0)
                    {
                        runeToGiveInfo = allRunesToGiveTier12[Random.Shared.Next(allRunesToGiveTier12.Length)];
                        _gameCore.Game.Inventory.AddItem(new Item(runeToGiveInfo));
                        _givenItemTextures.Add(ItemsDataHolder.Runes.RecipeX64RunesTextures[runeToGiveInfo.ID.Replace("finished_", "")]);
                        break;
                    }
                    
                    var runeRecipeId = allUnknownRunesId[Random.Shared.Next(allUnknownRunesId.Length)];
                    AllGameItems.UnlockRuneCraftRecipe(runeRecipeId);
                    _givenItemTextures.Add(ItemsDataHolder.OtherItems.RuneCraftRecipePaperTexture);
                    
                    _gameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.Paper), Random.Shared.Next(1,2));
                    _givenItemTextures.Add(ItemsDataHolder.OtherItems.PaperTextureX64);
                    
                    break;
                
                case ChestType.Gold:
                    
                    runeToGiveInfo = allRunesToGiveTier3[Random.Shared.Next(allRunesToGiveTier3.Length)];
                    _gameCore.Game.Inventory.AddItem(new Item(runeToGiveInfo));
                    _givenItemTextures.Add(ItemsDataHolder.Runes.RecipeX64RunesTextures[runeToGiveInfo.ID.Replace("finished_", "")]);

                    if (AllGameItems.KnownScrollsCraftRecipes.Count == ScrollsRecipes.Recipes.Values.Count)
                    {
                        break;
                    }
                    var allScrollRecipesToUnlock =
                        ScrollsRecipes.Recipes.Values
                            .Where(i => !AllGameItems.KnownScrollsCraftRecipes.Contains(i))
                            .ToArray();
                    var scrollInfoToUnlock =
                            allScrollRecipesToUnlock[Random.Shared.Next(allScrollRecipesToUnlock.Length)];
                    AllGameItems.TryToUnlockScrollRecipe(scrollInfoToUnlock);
                    _givenItemTextures.Add(ItemsDataHolder.OtherItems.ScrollCraftRecipePaperTexture);
                    
                    break;
                case ChestType.Emerald:
                    for (var i = 0; i < 5; i++)
                    {
                        runeToGiveInfo = allRunesToGiveTier12[Random.Shared.Next(allRunesToGiveTier12.Length)];
                        _gameCore.Game.Inventory.AddItem(new Item(runeToGiveInfo));
                        _givenItemTextures.Add(ItemsDataHolder.Runes.RecipeX64RunesTextures[runeToGiveInfo.ID.Replace("finished_", "")]);
                    }
                    runeToGiveInfo = allRunesToGiveTier3[Random.Shared.Next(allRunesToGiveTier3.Length)];
                    _gameCore.Game.Inventory.AddItem(new Item(runeToGiveInfo));
                    _givenItemTextures.Add(ItemsDataHolder.Runes.RecipeX64RunesTextures[runeToGiveInfo.ID.Replace("finished_", "")]);
                    
                    _gameCore.Game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.Paper), Random.Shared.Next(3, 5));
                    _givenItemTextures.Add(ItemsDataHolder.OtherItems.PaperTextureX64);
                    
                    break;
                
            }
            _animationTimer.Start();
            key.SubtractCount(1);
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