using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runes_and_Spells.Content.data;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.OtherClasses;

public class DevConsole
{
    public bool IsOpen { get; set; }
    public string CurrentCommand { get; private set; }
    public string LastCommand { get; private set; }
    private List<string> ConsoleLines;
    private readonly Texture2D _backgroundTexture;
    private Game1 _game;
    private KeyboardState _lastKbState;
    public DevConsole(Game1 game, Texture2D backTexture)
    {
        _game = game;
        ConsoleLines = new List<string>();
        _backgroundTexture = backTexture;
        CurrentCommand = "";
        LastCommand = "";
    }

    public readonly Dictionary<string, (Action<Game1, string> Action, string Description)> Commands = new ()
    {
        { "help", ((game, com) => { 
                game.DevConsole.ShowInConsole(game.DevConsole.Commands.Select(p => p.Key + " - " + p.Value.Description).ToArray());
            }, "Показать все команды") 
        },
        { "wings", ((game, com) => { 
                game.TopDownCore.PlayerHasWings = true;
            }, "Выдать крылья") 
        },
        { "compass", ((game, com) => { 
                game.TopDownCore.PlayerHasCompass = true;
            }, "Выдать компасс") 
        },
        { "give scroll", ((game, com) =>
            {
                var pars = com.Split();
                if (pars.Length != 6)
                {
                    game.DevConsole.ShowInConsole($"Error! Wrong count of parameters.");
                    return;
                }
                try
                {
                    var runeId1 = pars[2] + "_" + pars[4];
                    var runeId2 = pars[3] + "_" + pars[4];
                    if (pars[2] == pars[3])
                    {
                        runeId1 = pars[3] + "_1";
                        runeId2 = pars[3] + "_2";
                    }
                    var newScrollInfo = ScrollsRecipes.Recipes[(runeId1, runeId2)];
                    game.Inventory.AddItem(new Item(ItemType.Scroll, ItemsDataHolder.Scrolls.ScrollsTypesInfo[newScrollInfo.Type].texture2D, 
                        string.Join('_', newScrollInfo.id, newScrollInfo.Type.ToString().ToLower(), pars[5]), 
                        $"Свиток {newScrollInfo.rus}\n" +
                        $"Стихия: {ItemsDataHolder.Scrolls.ScrollsTypesInfo[newScrollInfo.Type].rus}\n" +
                        $"Сила: {pars[5]}"));
                }
                catch (Exception e)
                {
                    game.DevConsole.ShowInConsole($"Error! {e}");
                }
                
            }, "\"give scroll el1 el2 variant(1/2) power(1/2/3)\" - выдать свиток из рун стихий el1 и el2, вида variant и мощности power") 
        },
        { "give rune", ((game, com) =>
            {
                var pars = com.Split();
                if (pars.Length != 5)
                {
                    game.DevConsole.ShowInConsole($"Error! Wrong count of parameters.");
                    return;
                }
                try
                {
                    var runeId1 = string.Join('_', "rune_finished", pars[2], pars[3], pars[4]);
                    game.Inventory.AddItem(new Item(ItemsDataHolder.Runes.FinishedRunes[runeId1]));
                }
                catch (Exception e)
                {
                    game.DevConsole.ShowInConsole($"Error! {e}");
                }

            }, "\"give rune el power(1/2/3) variant(1/2)\" - выдать руну стихии el, силы power, и вида 1/2.") 
        },
        {"clear", ((game, com) => { 
            game.DevConsole.ConsoleLines.Clear();
        }, "Очистить консоль") },
        {"give key", ((game, com) => {
                {
                    var pars = com.Split();
                    if (pars.Length != 3)
                    {
                        game.DevConsole.ShowInConsole($"Error! Wrong count of parameters.");
                        return;
                    }
                    try
                    {
                        switch (pars[2])
                        {
                            case "silver":
                                game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeySilver));
                                break;
                            case "gold":
                                game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyGold));
                                break;
                            case "emerald":
                                game.Inventory.AddItem(new Item(ItemsDataHolder.OtherItems.KeyEmerald));
                                break;
                            default:
                                game.DevConsole.ShowInConsole($"Error! No Keys with this type: {pars[2]}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        game.DevConsole.ShowInConsole($"Error! {e}");
                    }
                }
            }, "\"give key type(silver/gold/emerald)\" - выдать ключ типа type.") 
        }
    };

    public void EnterCommand()
    {
        var pars = CurrentCommand.Split();
        if (pars.Length > 1) {
            if (Commands.ContainsKey(pars[0] + " " + pars[1]))
            {
                Commands[pars[0] + " " + pars[1]].Action(_game, CurrentCommand);
            }
        }
        else if (Commands.ContainsKey(pars[0]))
        {
            Commands[pars[0]].Action(_game, CurrentCommand);
        }
        else
        {
            ShowInConsole($"No such command \"{CurrentCommand}\"! Type \"help\" for help.");
        }

        LastCommand = CurrentCommand;
        CurrentCommand = "";
    }

    public void Update()
    {
        var kbState = Keyboard.GetState();
        if (CheckKey(Keys.Up))
        {
            CurrentCommand = LastCommand;
        }

        for (var i = 65; i < 91; i++)
        {
            if (CheckKey((Keys)i))
            {
                CurrentCommand += ((Keys)i).ToString().ToLower();
            }
        }
        for (var i = 48; i < 58; i++)
        {
            if (CheckKey((Keys)i))
            {
                CurrentCommand += ((Keys)i).ToString()[1];
            }
        }
        if (CheckKey(Keys.Space))
        {
            CurrentCommand += " ";
        }
        if (CheckKey(Keys.Back) && CurrentCommand.Length > 0)
        {
            CurrentCommand = CurrentCommand.Substring(0, CurrentCommand.Length - 1);
        }
        if (CheckKey(Keys.Enter))
        {
            EnterCommand();
        }

        _lastKbState = kbState;
        ConsoleLines = ConsoleLines.TakeLast(10).ToList();
    }

    private bool CheckKey(Keys key)
    {
        return _lastKbState.IsKeyUp(key) && Keyboard.GetState().IsKeyDown(key);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var gap = 50;
        var lineHeight = 40;
        var allLines = string.Join('\n', ConsoleLines.TakeLast(10));
        var historyLinesSize = _game.LogText.MeasureString(allLines);
        var commandSize = _game.LogText.MeasureString("> " + CurrentCommand);
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, _game.ScreenHeight - lineHeight - gap), 
            new Rectangle(0, 0, _game.ScreenWidth, lineHeight), Color.White);
        
        spriteBatch.Draw(_backgroundTexture, new Vector2(0, _game.ScreenHeight - lineHeight - historyLinesSize.Y - gap),
            new Rectangle(0, 0, _game.ScreenWidth, (int)historyLinesSize.Y), Color.White);
        spriteBatch.DrawString(_game.LogText, "> " + CurrentCommand, 
            new Vector2(10, _game.ScreenHeight - lineHeight + (lineHeight - commandSize.Y)/2 - gap), Color.White);
        
        spriteBatch.DrawString(_game.LogText, allLines, 
                new Vector2(10, _game.ScreenHeight - lineHeight - historyLinesSize.Y - gap), Color.White);

    }

    public void ShowInConsole(params string[] lines)
    {
        ConsoleLines = ConsoleLines.Concat(lines).ToList();
    }
}