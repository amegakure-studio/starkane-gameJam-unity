using UnityEngine;
using System.Collections.Generic;

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
    Character selectedCharacter;
    Pathfinder pathfinder;
    private List<Character> characters; 
    #endregion

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();
        characters = new();
        
        GameObject[] charactersGo = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject go in charactersGo)
        {
            try
            {
                Character character = go.GetComponent<Character>();
                characters.Add(character);
            }
            catch {}
        }

        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
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
        foreach(Character character in characters)
        {
            if (character.Moving)
                return;
        }

        Character c = currentTile.occupyingCharacter;
        if( c != null)
        {
            currentTile.SetColor(TileColor.Highlighted);

            if (Input.GetMouseButtonDown(0))
                SelectCharacter();
        }
    }

    private void Clear()
    {
        if (currentTile == null  || currentTile.Occupied == false)
            return;

        currentTile.ClearColor();
        currentTile = null;
    }

    private void SelectCharacter()
    {
        selectedCharacter = currentTile.occupyingCharacter;
        pathfinder.FindPaths(selectedCharacter);
        GetComponent<AudioSource>().PlayOneShot(pop);
    }

    private void NavigateToTile()
    {
        if (selectedCharacter == null)
            return;

        if (selectedCharacter.Moving == true || currentTile.CanBeReached == false)
            return;

        Path currentPath = pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(click);
            selectedCharacter.Move(currentPath);
            pathfinder.ResetPathfinder();
            selectedCharacter = null;
        }
    }
}
