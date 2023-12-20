using System;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] GameObject vfxInteractPrefab;
    [SerializeField] AudioClip sfxInteract;
    [SerializeField] AudioClip sfxUninteract;

    private GameObject vfxInteract;
    private bool isInteracting = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() 
    { 
        EventManager.Instance.Subscribe(GameEvent.INTERACT, HandleInteract);
        //Implementacion
        EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void OnDisable() 
    { 
        EventManager.Instance.Unsubscribe(GameEvent.INTERACT, HandleInteract);
        //Implementacion
        EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
    }

    private void HandleTileSelected(Dictionary<string, object> context)
    {
        //Implementacion

        if (isInteracting) 
        { 
            try
            {
                Tile target = (Tile)context["Tile"];
                CharacterMovement characterMovement = GetComponent<CharacterMovement>();

                characterMovement.GoTo(target);
                isInteracting = false;

            } catch (Exception e) { Debug.LogError(e); }
        }
    }

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
        if (vfxInteractPrefab != null && vfxInteract == null)
        {
            Vector3 vfxPosition = new Vector3(transform.position.x, 0, transform.position.z);          
            vfxInteract = Instantiate(vfxInteractPrefab, vfxPosition, Quaternion.identity);
            vfxInteract.transform.localScale = new Vector3(.75f, .25f, .75f);
        }
    }

    private void OnMouseExit()
    {
        Destroy(vfxInteract);
    }

    private void OnMouseDown()
    {
        Interact();

        Dictionary<string, object> context = new()
        {
            { "Id", this.gameObject.GetInstanceID() }
        };

        EventManager.Instance.Publish(GameEvent.INTERACT, context);
    }

    private void Interact()
    {
        CharacterMovement characterMovement = GetComponent<CharacterMovement>();

        if (!characterMovement.Moving)
        {
            if (!isInteracting)
            {
                isInteracting = true;
                characterMovement.FindPaths(characterMovement.Location);
                audioSource.PlayOneShot(sfxInteract);
            }
            else
            {
                isInteracting = false;
                characterMovement.ClearFrontier();
                audioSource.PlayOneShot(sfxUninteract);
            }
        }       
    }
}
