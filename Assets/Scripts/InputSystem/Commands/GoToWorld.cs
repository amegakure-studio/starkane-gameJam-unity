
using UnityEngine.SceneManagement;

public class GoToWorld : ICommand
{
    public string Name => "Go to world";

    public void Do()
    {
        SceneManager.LoadScene(1);
    }
}
