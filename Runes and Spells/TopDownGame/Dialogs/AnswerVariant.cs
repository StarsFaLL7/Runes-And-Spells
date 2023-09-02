using System;
using Runes_and_Spells.TopDownGame.Objects;
using Runes_and_Spells.UtilityClasses;

namespace Runes_and_Spells.TopDownGame.Dialogs;

public class AnswerVariant
{
    private Action<NPC> ActionAfterAnswer { get; }
    public string Text { get; set; }
    public DialogPhrase NextPhrase { get; set; }

    private readonly bool _closeAfterAnswer;

    public AnswerVariant(string text, DialogPhrase nextPhrase, bool closeDialogAfterAnswer, Action<NPC> actionAfterAnswer = null)
    {
        Text = text;
        NextPhrase = nextPhrase;
        ActionAfterAnswer = actionAfterAnswer;
        _closeAfterAnswer = closeDialogAfterAnswer;
    }

    public void Select(NPC npc)
    {
        if (NextPhrase is null || _closeAfterAnswer)
        {
            npc.CloseDialog();
        }
        if (NextPhrase is not null)
        {
            npc.SetCurrentPhrase(NextPhrase);
            if (NextPhrase.AlertText is not null && NextPhrase.AlertText != "")
            {
                AllGameItems.AlertSound.Play();
            }
        }

        if (ActionAfterAnswer is not null)
        {
            ActionAfterAnswer(npc);
        }
    }
}