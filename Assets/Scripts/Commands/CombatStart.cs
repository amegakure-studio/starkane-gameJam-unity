
using UnityEngine.SceneManagement;

public class CombatStart : ICommand
{
    public string Name => "Atacar";

    public void Do()
    {
        SceneManager.LoadScene("Combat");
    }
}
