using System;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject vfxInteractPrefab;
    [SerializeField] AudioClip sfxInteract;
    [SerializeField] AudioClip sfxUninteract;
    [SerializeField] InteractableBehaviour interactableBehaviour;

    private bool isInteracting = false;
    private GameObject vfxInteract;
    private AudioSource audioSource;

    private void Awake() { audioSource = GetComponent<AudioSource>(); }

    private void OnEnable() { EventManager.Instance.Subscribe(GameEvent.INTERACT, HandleInteract); }

    private void OnDisable() { EventManager.Instance.Unsubscribe(GameEvent.INTERACT, HandleInteract); }

    private void HandleInteract(Dictionary<string, object> context)
    {
        try 
        {
            int goId = (int)context["Id"];

            if (gameObject.GetInstanceID() != goId)
                isInteracting = false;
        }
        catch(Exception e) { Debug.LogError(e); }
    }

    private void OnMouseOver()
    {
        if (interactableBehaviour.CanInteract())
        {
            if (vfxInteractPrefab != null && vfxInteract == null)
            {
                Vector3 vfxPosition = new Vector3(transform.position.x, 0, transform.position.z);          
                vfxInteract = Instantiate(vfxInteractPrefab, vfxPosition, Quaternion.identity);
                vfxInteract.transform.localScale = new Vector3(.75f, .25f, .75f);
            }
        }
    }

    private void OnMouseExit()
    {
        if (vfxInteract != null && !isInteracting)
            Destroy(vfxInteract);
    }

    private void OnMouseDown()
    {
        if (interactableBehaviour.CanInteract())
        {
            if (!isInteracting)
            {
                isInteracting = true;
                interactableBehaviour.Interact();
         
                Dictionary<string, object> context = new()
                {
                    { "Id", this.gameObject.GetInstanceID() }
                };

                EventManager.Instance.Publish(GameEvent.INTERACT, context);

                if (sfxInteract != null)
                    audioSource.PlayOneShot(sfxInteract);
            }
            else
            {
                isInteracting = false; 
                interactableBehaviour.Uninteract();

                if (sfxUninteract != null)
                    audioSource.PlayOneShot(sfxUninteract);
            }
        }       
    }

    public bool IsInteracting() { return isInteracting; }

    public void EndInteraction() 
    { 
        isInteracting = false;

        if (vfxInteract != null)
            Destroy(vfxInteract);
    }

    public void SetInteractableBehaviour(InteractableBehaviour behaviour) { this.interactableBehaviour = behaviour; }
}
