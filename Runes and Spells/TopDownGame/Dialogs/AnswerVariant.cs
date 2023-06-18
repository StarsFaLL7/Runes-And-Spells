using System;
using Runes_and_Spells.TopDownGame.Objects;

namespace Runes_and_Spells.TopDownGame.Dialogs;

public class AnswerVariant
{
    public Action<NPC> ActionAfterAnswer { get; }
    public string Text { get; set; }
    public DialogPhrase NextPhrase { get; set; }

    public bool CloseAfterAnswer;

    public AnswerVariant(string text, DialogPhrase nextPhrase, bool closeDialogAfterAnswer, Action<NPC> actionAfterAnswer = null)
    {
        Text = text;
        NextPhrase = nextPhrase;
        ActionAfterAnswer = actionAfterAnswer;
        CloseAfterAnswer = closeDialogAfterAnswer;
    }

    public void Select(NPC npc)
    {
        if (NextPhrase is null || CloseAfterAnswer)
        {
            npc.CloseDialog();
        }
        if (NextPhrase is not null)
        {
            npc.SetCurrentPhrase(NextPhrase);
        }

        if (ActionAfterAnswer is not null)
        {
            ActionAfterAnswer(npc);
        }
    }
}