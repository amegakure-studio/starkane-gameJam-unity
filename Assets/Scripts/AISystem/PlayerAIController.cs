using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerAIController : MonoBehaviour
{
    private Player player;
    private Combat combat;

    public Player Player { get => player; set => player = value; }

    private bool isCutSceneRunning = false;
    private bool isCoroutineRunning = false;

    private void Start()
    {
        combat = FindAnyObjectByType<Combat>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
    }


    private void HandleCutsceneCombatEnd(Dictionary<string, object> dictionary)
    {
        isCutSceneRunning = false;
    }

    private void HandleCutsceneCombatStart(Dictionary<string, object> dictionary)
    {
        isCutSceneRunning = true;
    }

    private void HandleCombatTurnChanged(Dictionary<string, object> context)
    {
        if (combat == null)
            combat = FindAnyObjectByType<Combat>();

        try
        {
            Player turn = (Player)context["Player"];
            if (turn.HexID.Equals(player.HexID))
            {
                if(!isCoroutineRunning)
                {
                    isCoroutineRunning = true;
                    StartCoroutine(nameof(DecisionTakerCoroutine));
                }
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

        List<Character> characters = combat.GetCharacters(player).FindAll(cc => cc.IsAlive());
        foreach (Character character in characters)
        {
            // Debug.Log("!!!!-- Character AI: " + character.name);
            TryDoSkill(character);
            
            yield return new WaitForSeconds(1f);
            while (isCutSceneRunning) yield return null;
            
            yield return new WaitForSeconds(2f);
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
            //Move
            
            TryMove(character);
            
            yield return new WaitForSeconds(2f);
            while (character.IsMoving) yield return null;
            
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);

            TryDoSkill(character);

            yield return new WaitForSeconds(1f);
            EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
            
            yield return new WaitForSeconds(1f);
            while (isCutSceneRunning) yield return null;
            
            yield return new WaitForSeconds(1f);
        }

        combat.CallEndTurnTX(player);
        isCoroutineRunning = false;
    }

    private void TryMove(Character character)
    {
        MoveToTileNearPlayer(character);
    }

    private bool TryDoSkill(Character character)
    {
        EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
        foreach (Skill skill in character.Skills)
        {
            if (combat.CanDoSkill(player, character, skill))
            {
                Debug.Log("Can do skill with character: " + character.CharacterName);
                Frontier skillFrontier = skill.GetFrontier(character.Location);
                EventManager.Instance.Publish(GameEvent.FRONTIER_UPDATED, new() { { "Frontier", skillFrontier } });

                foreach (Tile tile in skillFrontier.Tiles)
                {
                    GameObject occupyingObject = tile.OccupyingObject;

                    if (occupyingObject != null)
                    {
                        Character characterReceiver = occupyingObject.GetComponent<Character>();

                        if (characterReceiver != null)
                        {
                            Player playerReceiver = combat.GetPlayerByID(characterReceiver.GetPlayerId());
                            if(playerReceiver.GetInstanceID() != player.GetInstanceID())
                            {
                                combat.DoSkill(player, character, skill, playerReceiver, characterReceiver);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private void MoveToTileNearPlayer(Character character)
    {
        EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
        if (combat.CanMove(character, player))
        {
            List<Character> adversaryCharacters = combat.GetRivalCharacters(player).FindAll(rc => rc.IsAlive());
            List<Tile> adversaryCoordinates = new();

            foreach (Character adversaryCharacter in adversaryCharacters)
            {
                adversaryCoordinates.Add(adversaryCharacter.Location);
            }

            // Select nearest adversary character
            Character nearestAdversary = FindNearestCharacter(adversaryCharacters, character.Location.coordinate);

            if (nearestAdversary != null)
            {
                // get the nearest tile
                Frontier characterFrontier = character.GetMovementFrontier();
                List<Tile> movementTiles = characterFrontier.Tiles;
                EventManager.Instance.Publish(GameEvent.FRONTIER_UPDATED, new() { { "Frontier", characterFrontier } });

                Tile nearestTileToAdversary = FindNearestTile(movementTiles, nearestAdversary.Location.coordinate);
                
                if (nearestTileToAdversary != null)
                    combat.Move(character, player, nearestTileToAdversary);
            }
        }
    }

    private Character FindNearestCharacter(List<Character> adversaryCharacters, Vector2 location)
    {
        // List<Character> adversaryCharacters = combat.GetRivalCharacters(player);
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
            if (!tile.Occupied())
                tileCoordinates.Add(tile.coordinate, tile);
        }

        if (tileCoordinates.Count > 0)
        {
            // Use LINQ to find the location with the minimum distance
            Vector2 nearestLocation = tileCoordinates.Keys.Aggregate((min, loc) => Vector2.Distance(adversarylocation, loc) < Vector2.Distance(adversarylocation, min) ? loc : min);
            
            return tileCoordinates[nearestLocation];
        }

        return null;
    }
}
