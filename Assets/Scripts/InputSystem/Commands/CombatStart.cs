using System.Collections.Generic;
using Amegakure.Starkane.PubSub;
using Amegakure.Starkane.UI.NonDiegetic;

public class CombatStart : ICommand
{
    private Dialog dialog;
    public string Name => "Attack";

    public CombatStart(Dialog dialog)
    { this.dialog = dialog; }

    public void Do()
    {
        Dictionary<string, object> eventData = new();
        EventManager.Instance.Publish(GameEvent.INPUT_COMBAT_ACCEPT, eventData);
        dialog.Hide();
    }
}
