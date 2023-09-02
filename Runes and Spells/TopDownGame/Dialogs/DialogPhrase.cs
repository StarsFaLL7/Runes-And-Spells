using System.Collections.Generic;

namespace Runes_and_Spells.TopDownGame.Dialogs;

public class DialogPhrase
{
    public string Text;
    public readonly AnswerVariant[] AnswerVariants;
    public AnswerVariant SelectedAnswerVariant;
    public readonly string AlertText;

    public DialogPhrase(string text, string? alertText, params AnswerVariant[] answersVariants)
    {
        Text = text;
        AnswerVariants = answersVariants;
        SelectedAnswerVariant = answersVariants[0];
        AlertText = alertText;
    }
    
}