using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehaviour : MonoBehaviour
{
    public abstract bool CanInteract();
    public abstract void Interact();
    public abstract void Uninteract();
}
