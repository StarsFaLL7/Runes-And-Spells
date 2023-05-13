using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.classes;

namespace Runes_and_Spells;

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

    public RuneCraftingMiniGame(Vector2 position, Mode mode, ContentManager content)
    {
        _position = position;
        _mode = mode;
        _cellOnTexture = content.Load<Texture2D>("textures/rune_crafting_table/cell_clicked");
        _cellOffTexture = content.Load<Texture2D>("textures/rune_crafting_table/cell");
        _backTexture = content.Load<Texture2D>("textures/rune_crafting_table/UI_bg");
        _rectangles = new List<Rectangle>();
        if (_mode == Mode.X3)
        {
            _currentScheme = new List<bool>(9);
            for (var y = 0; y < 3; y++)
            for (var x = 0; x < 3; x++)
            {
                _rectangles.Add(new Rectangle(
                    (int)_position.X + 18 + x*(_cellOffTexture.Width + 6), 
                    (int)_position.Y + 18 + y*(_cellOffTexture.Height + 6), _cellOffTexture.Width, _cellOffTexture.Height));
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
        
        var newId = AllGameItems.GetIdByRecipe(_currentScheme);
        var newItem = AllGameItems.UnknownRunes[newId];
        outputSlot.SetItem(new Item(newItem));
        
        _currentScheme = new List<bool>(9);
        for (var i = 0; i < 9; i++) _currentScheme.Add(false);
    }

    public void Update()
    {
        if (!IsActive) return;
        lastMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        var hoveredRectangle = _rectangles.FirstOrDefault(r => r.Contains(currentMouseState.Position));
        var index = _rectangles.IndexOf(hoveredRectangle);
        if (hoveredRectangle != Rectangle.Empty &&
            hoveredRectangle.Contains(lastMouseState.Position) && 
            lastMouseState.LeftButton == ButtonState.Released && 
            currentMouseState.LeftButton == ButtonState.Pressed)
        {
            _currentScheme[index] = !_currentScheme[index];
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_mode == Mode.X3)
        {
            spriteBatch.Draw(_backTexture, _position, Color.White);
            if (!IsActive) return;
            
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
            {
                spriteBatch.Draw(_currentScheme[y * 3 + x] ? _cellOnTexture : _cellOffTexture,
                    new Vector2(_rectangles[y * 3 + x].X, _rectangles[y * 3 + x].Y), Color.White);
            }
        }
    }
}