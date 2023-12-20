using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemyInteractable : InteractableBehaviour
{
    public override bool CanInteract()
    {
        return true;
    }

    public override void Interact()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentIndex < SceneManager.sceneCount)
            SceneManager.LoadScene(currentIndex + 1);
    }

    public override void Uninteract()
    {
        
    }
}
