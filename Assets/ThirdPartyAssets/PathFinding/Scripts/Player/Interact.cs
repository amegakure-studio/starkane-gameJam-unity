using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Interact : MonoBehaviour
{
    #region member fields
    [SerializeField]
    AudioClip click;
    [SerializeField]
    AudioClip pop;
    [SerializeField]
    LayerMask interactMask;

    Camera mainCam;
    Tile currentTile;
    CharacterMovement selectedCharacter;
    private List<CharacterMovement> characters; 
    private bool hasColorChangedStarted = false;
    private List<Tile> changedColorTiles = new();
    #endregion

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();    
        characters = GameObject.FindObjectsOfType<CharacterMovement>().ToList();
        characters ??= new List<CharacterMovement>();
    }

    private void Update()
    {
        Clear();
        MouseUpdate();
    }

    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            return;

        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
    }

    private void InspectTile()
    {
        if (currentTile.Occupied)
            InspectCharacter();
        else
            NavigateToTile();
    }

    private void InspectCharacter()
    {
        foreach(CharacterMovement character in characters)
        {
            if (character.Moving)
                return;
        }

        CharacterMovement c = currentTile.occupyingCharacter;
        if( c != null)
        {
            currentTile.SetColor(TileColor.Highlighted);

            if (Input.GetMouseButtonDown(0))
                SelectCharacter();
        }
    }

    private void Clear()
    {
        if(!hasColorChangedStarted && currentTile != null && currentTile.CanBeReached)
        {
            foreach(Tile tile in changedColorTiles)
            {
                if(tile.Occupied)
                {
                    tile.ClearColor();
                }  
                else
                {
                    tile.SetColor(TileColor.Green);
                }
            }
            changedColorTiles.Clear();
        }

        if (currentTile == null  || !currentTile.Occupied)
            return;

        currentTile.ClearColor();
        currentTile = null;
    }

    private void SelectCharacter()
    {
        selectedCharacter = currentTile.occupyingCharacter;
        selectedCharacter.FindPaths(currentTile);
        GetComponent<AudioSource>().PlayOneShot(pop);
    }

    private void NavigateToTile()
    {
        if (selectedCharacter == null)
            return;

        if (selectedCharacter.Moving || !currentTile.CanBeReached)
            return;

        if (selectedCharacter.CanReachTile(currentTile) && !hasColorChangedStarted)
            StartCoroutine(MouseOnHover());
        
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(click);
            selectedCharacter.GoTo(currentTile);          
            selectedCharacter = null;
        }
    }
    
    private IEnumerator MouseOnHover()
    {
            hasColorChangedStarted = true;
            currentTile.SetColor(TileColor.Highlighted);
            
            changedColorTiles.Add(currentTile);

            yield return null;
            
            hasColorChangedStarted = false;
            
    }
}