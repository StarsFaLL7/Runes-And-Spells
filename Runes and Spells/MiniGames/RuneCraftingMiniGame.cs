using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.OtherClasses;
using Runes_and_Spells.UiClasses;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.MiniGames;

public class RuneCraftingMiniGame
{
    public enum Mode
    {
        X3,
        X4
    }

    public bool IsActive { get; private set; }
    private Mode _mode;
    private List<bool> _currentScheme;
    private List<Rectangle> _rectangles;
    private Texture2D _cellOffTexture;
    private Texture2D _cellOnTexture;
    private Texture2D _backTexture;
    private Vector2 _position;
    private MouseState lastMouseState;
    private MouseState currentMouseState;
    private Game1 _game;
    private SoundEffect _soundFailed;
    private SoundEffect[] _soundsCrafted;
    
    public RuneCraftingMiniGame(Vector2 position, Mode mode, ContentManager content, Game1 game)
    {
        _game = game;
        _position = position;
        _mode = mode;
        _cellOnTexture = content.Load<Texture2D>("textures/rune_crafting_table/cell_clicked");
        _cellOffTexture = content.Load<Texture2D>("textures/rune_crafting_table/cell");
        _backTexture = content.Load<Texture2D>("textures/rune_crafting_table/UI_bg");
        _soundFailed = content.Load<SoundEffect>("sounds/rune_craft_failed");
        _soundsCrafted = new[]
        {
            content.Load<SoundEffect>("sounds/rune_craft1"),
            content.Load<SoundEffect>("sounds/rune_craft2"),
            content.Load<SoundEffect>("sounds/rune_craft3"),
        };

        _rectangles = new List<Rectangle>();
        if (_mode == Mode.X3)
        {
            _currentScheme = new List<bool>(9);
            for (var y = 0; y < 3; y++)
            for (var x = 0; x < 3; x++)
            {
                _rectangles.Add(new Rectangle(
                    (int)((_position.X + 18 + x*(_cellOffTexture.Width + 6))*Game1.ResolutionScale.X), 
                    (int)((_position.Y + 18 + y*(_cellOffTexture.Height + 6))*Game1.ResolutionScale.Y), 
                    (int)(_cellOffTexture.Width*Game1.ResolutionScale.X), 
                    (int)(_cellOffTexture.Height*Game1.ResolutionScale.Y)));
                _currentScheme.Add(false);
            }
        }
    }

    public void Start(UiSlot inputSlot)
    {
        inputSlot.Lock();
        IsActive = true;
    }

    public void Stop(UiSlot inputSlot, UiSlot outputSlot)
    {
        inputSlot.Clear();
        inputSlot.Unlock();
        IsActive = false;
        
        var newId = AllGameItems.GetRuneIdByRecipe(_currentScheme);
        ItemInfo newItem;
        
        if (newId == "clay_small")
        {
            newItem = ItemsDataHolder.OtherItems.ClaySmall;
            _soundFailed.Play();
        }
        else
        {
            newItem = AllGameItems.UnknownRunes[newId];
            if (AllGameItems.KnownRunesCraftRecipes.ContainsKey(newItem.ID) &&
                AllGameItems.KnownRunesCraftRecipes[newItem.ID])
            {
                newItem = newItem with
                {
                    ToolTip = "Слепок руны\n" + ItemsDataHolder.Runes
                        .FinishedRunes[newItem.ID.Replace("unknown", "finished")].ToolTip
                };
            }

            var sound = _soundsCrafted[Random.Shared.Next(_soundsCrafted.Length)];
            sound.Play();
        }
        _game.SubtractEnergy(2f);
        outputSlot.SetItem(new Item(newItem));
        
        _currentScheme = new List<bool>(9);
        for (var i = 0; i < 9; i++) _currentScheme.Add(false);
    }

    public List<bool> GetCurrentScheme()
    {
        return _currentScheme;
    }

    public void Update()
    {
        if (!IsActive) return;
        lastMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        
        for (var y = 0; y < 3; y++)
        for (var x = 0; x < 3; x++)
        {
            _rectangles[y*3+x] = new Rectangle(
                (int)((_position.X + 18 + x*(_cellOffTexture.Width + 6))*Game1.ResolutionScale.X), 
                (int)((_position.Y + 18 + y*(_cellOffTexture.Height + 6))*Game1.ResolutionScale.Y), 
                (int)(_cellOffTexture.Width*Game1.ResolutionScale.X), 
                (int)(_cellOffTexture.Height*Game1.ResolutionScale.Y));
        }
        
        var hoveredRectangle = _rectangles.FirstOrDefault(r => r.Contains(currentMouseState.Position));
        var index = _rectangles.IndexOf(hoveredRectangle);
        if (hoveredRectangle != Rectangle.Empty &&
            hoveredRectangle.Contains(lastMouseState.Position) && 
            lastMouseState.LeftButton == ButtonState.Released && 
            currentMouseState.LeftButton == ButtonState.Pressed)
        {
            if (_game.Introduction.IsPlaying && index == 4)
                _currentScheme[index] = true;
            else if (!_game.Introduction.IsPlaying)
                _currentScheme[index] = !_currentScheme[index];
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_mode == Mode.X3)
        {
            spriteBatch.Draw(_backTexture, _position*Game1.ResolutionScale, null, 
                Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            if (!IsActive) return;
            
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
            {
                spriteBatch.Draw(_currentScheme[y * 3 + x] ? _cellOnTexture : _cellOffTexture,
                    new Vector2(_rectangles[y * 3 + x].X, _rectangles[y * 3 + x].Y), null, 
                    Color.White, 0f, Vector2.Zero, Game1.ResolutionScale, SpriteEffects.None, 1f);
            }
        }
    }
}