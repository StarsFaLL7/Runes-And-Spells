using System.Collections.Generic;

namespace Runes_and_Spells.TopDownGame.Dialogs;

public class DialogPhrase
{
    public string Text;
    public AnswerVariant[] AnswerVariants;
    public AnswerVariant SelectedAnswerVariant;
    public string AlertText;

    public DialogPhrase(string text, string? alertText, params AnswerVariant[] answersVariants)
    {
        Text = text;
        AnswerVariants = answersVariants;
        SelectedAnswerVariant = answersVariants[0];
        AlertText = alertText;
    }
    
}