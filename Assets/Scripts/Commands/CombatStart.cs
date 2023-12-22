public class CombatStart : ICommand
{
    private Dialog dialog;
    public string Name => "Atacar";

    public CombatStart(Dialog dialog)
    { this.dialog = dialog; }

    public void Do()
    {
        EventManager.Instance.Publish(GameEvent.BATTLE_INTERACTION, null);
        dialog.Hide();
    }
}
