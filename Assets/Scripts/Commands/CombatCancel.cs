
public class CombatCancel : ICommand
{
    private Dialog dialog;

    public CombatCancel(Dialog dialog)
    { this.dialog = dialog; }

    public string Name => "Cancelar";

    public void Do()
    {
        dialog.Hide();
    }
}
