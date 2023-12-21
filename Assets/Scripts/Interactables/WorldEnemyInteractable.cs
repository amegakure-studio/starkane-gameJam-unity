using UnityEngine;

public class WorldEnemyInteractable : InteractableBehaviour
{
    Dialog dialog;

    private void Start()
    {
        dialog = GameObject.FindFirstObjectByType<Dialog>();
    }

    public override bool CanInteract()
    {
        return true;
    }

    public override void Interact()
    {
        dialog.Show();      
    }

    public override void Uninteract()
    {
        dialog.Hide();
    }
}
