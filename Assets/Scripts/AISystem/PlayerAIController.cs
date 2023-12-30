using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerAIController : MonoBehaviour
{
    private Player player;
    private Combat combat;

    public Player Player { get => player; set => player = value; }

    private void Start()
    {
        combat = FindAnyObjectByType<Combat>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
    }

    private void HandleCombatTurnChanged(Dictionary<string, object> context)
    {
        if (combat != null)
            combat = FindAnyObjectByType<Combat>();

        try
        {
            Player turn = (Player)context["Player"];

            if (turn.GetInstanceID() == player.GetInstanceID()) 
            {
                StartCoroutine(nameof(DecisionTakerCoroutine));
            }

            else
            {
                StopCoroutine(nameof(DecisionTakerCoroutine));
            }
        }
        catch { }
    }

    private IEnumerator DecisionTakerCoroutine()
    {
        yield return new WaitForSeconds(1f);
        
        List<Character> characters = combat.GetCharacters(player);

        foreach (Character character in characters)
        {
            TryDoSkill(character);
            yield return new WaitForSeconds(1f);
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
            //Move
            TryMove(character);

            while (character.IsMoving) yield return null;
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);

            TryDoSkill(character);
            yield return new WaitForSeconds(1f);
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);

        }

        combat.CallEndTurnTX(player);
    }

    private void TryMove(Character character)
    {
        MoveToTileNearPlayer(character);
    }

    private bool TryDoSkill(Character character)
    {
        foreach (Skill skill in character.Skills)
        {
            if (combat.CanDoSkill(player, character, skill))
            {
                Frontier skillFrontier = skill.GetFrontier(character.Location);

                foreach (Tile tile in skillFrontier.Tiles)
                {
                    GameObject occupyingObject = tile.OccupyingObject;

                    if (occupyingObject != null)
                    {
                        Character characterReceiver = occupyingObject.GetComponent<Character>();

                        if (characterReceiver != null)
                        {
                            Player playerReceiver = combat.GetPlayerByID(characterReceiver.GetPlayerId());
                            combat.DoSkill(player, character, skill, playerReceiver, characterReceiver);

                            return true;
                        }
                    }
                }
            }            
        }
        
        return false;
    }

    private void MoveToTileNearPlayer(Character character)
    {
            if(combat.CanMove(character, player))
            {
                List<Character> adversaryCharacters = combat.GetRivalCharacters(player);
                List<Tile> adversaryCoordinates = new();

                foreach (Character adversaryCharacter in adversaryCharacters)
                {
                    adversaryCoordinates.Add(adversaryCharacter.Location);
                }

                // Select nearest adversary character
                Character nearestAdversary = FindNearestCharacter(character.Location.coordinate);

                if (nearestAdversary != null) 
                {
                    // get the nearest tile
                    List<Tile> movementTiles = character.GetMovementFrontier().Tiles;
                    Tile nearestTileToAdversary = FindNearestTile(movementTiles, nearestAdversary.Location.coordinate);

                    combat.Move(character, player, nearestTileToAdversary);
                }            
            }           
    }

    private Character FindNearestCharacter(Vector2 location)
    {
        List<Character> adversaryCharacters = combat.GetRivalCharacters(player);
        Dictionary<Vector2, Character> adversaryCoordinates = new();

        if (adversaryCharacters.Count > 0)
        {
            foreach (Character adversaryCharacter in adversaryCharacters)
            {
                //adversaryCoordinates.Add(adversaryCharacter.GetLocation().coordinate, adversaryCharacter);
                adversaryCoordinates[adversaryCharacter.Location.coordinate] = adversaryCharacter;
            }

            // Use LINQ to find the location with the minimum distance
            Vector2 nearestLocation = adversaryCoordinates.Keys.Aggregate((min, loc) => Vector2.Distance(location, loc) < Vector2.Distance(location, min) ? loc : min);

            return adversaryCoordinates[nearestLocation];
        }

        return null;
    }

    private Tile FindNearestTile(List<Tile> locations, Vector2 adversarylocation)
    {
        Dictionary<Vector2, Tile> tileCoordinates = new();

        foreach (Tile tile in locations)
        {
            tileCoordinates.Add(tile.coordinate, tile);
        }

        // Use LINQ to find the location with the minimum distance
        Vector2 nearestLocation = tileCoordinates.Keys.Aggregate((min, loc) => Vector2.Distance(adversarylocation, loc) < Vector2.Distance(adversarylocation, min) ? loc : min);
        return tileCoordinates[nearestLocation];
    }
}
