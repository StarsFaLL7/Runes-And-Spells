using System.Collections.Generic;

namespace Runes_and_Spells;

public static class RuneRecipes
{
    public static List<(List<int>, string)> Recipes;

    public static void Initialize()
    {
        Recipes = new List<(List<int>, string)>();
        Recipes.Add((new List<int>()
        {
            0,0,0
            
        }), "");
    }
}