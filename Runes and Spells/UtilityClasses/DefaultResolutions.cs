using System.Collections.Generic;
using System.Linq;
using Runes_and_Spells.UiClasses;

namespace Runes_and_Spells.UtilityClasses;

public class DefaultResolutions
{
    private Game1 _game;
    public List<UiDropdown.DdVariant> Variants;
    
    public DefaultResolutions(Game1 game, UiDropdown.DdVariant customVariant = null)
    {
        _game = game;
        Variants = new List<UiDropdown.DdVariant>()
        {
            new UiDropdown.DdVariant("1920x1080", () => {_game.SetResolution(1920, 1080);}),
            new UiDropdown.DdVariant("1600x900", () => {_game.SetResolution(1600, 900);}),
            new UiDropdown.DdVariant("1366x768", () => {_game.SetResolution(1366, 768);}),
            new UiDropdown.DdVariant("1280x720", () => {_game.SetResolution(1280, 720);}),
            new UiDropdown.DdVariant("1024x640", () => {_game.SetResolution(1024, 640);})
        };
        if (customVariant != null && Variants.All(v => v.VisibleText != customVariant.VisibleText))
        {
            Variants.Insert(0, customVariant);
        }
    }
}