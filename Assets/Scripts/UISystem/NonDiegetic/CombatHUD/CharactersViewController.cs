using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CharactersViewController : MonoBehaviour
{
    [SerializeField] VisualTreeAsset characterUIAsset;
    
    private readonly string characterIconsFolder = "UI/Characters/";
    private VisualElement root;
    private List<Button> characterBtns;
    private VisualElement charactersContainer;
    private List<Character> charactersTurn;
    private Combat combat;

    private Dictionary<string, StyleBackground> characterIconDict;
    private Dictionary<Character, VisualElement> characterVeDict;
    private Player player;

    void Start()
    {
        root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        charactersContainer = root.Q<VisualElement>("CharactersGroup");
        characterBtns = new();
        characterIconDict = new();
        characterVeDict = new();
        charactersTurn = new();
        player = GameObject.FindAnyObjectByType<Session>().Player;
        combat = GameObject.FindAnyObjectByType<Combat>();
    }

    private void OnDestroy()
    {
        UnregisterBtns(characterBtns);
    }
    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Subscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void HandleSkillDone(Dictionary<string, object> context)
    {
        if (player != null)
        {
            foreach(Character character in player.Characters)
            {
                VisualElement characterVe = characterVeDict[character];
                characterVe.Q<VisualElement>("Hp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetHpNormalized() * 100);
                characterVe.Q<VisualElement>("Mp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetMpNormalized() * 100);
            }
        }
    }

    private void HandleCombatTurnChanged(Dictionary<string, object> context)
    {
        try
        {
            Player playerTurn = (Player)context["Player"];
            charactersTurn = combat.GetCharacters(player);

            ShowCharacters(charactersTurn, playerTurn);
            SelectCharacter(charactersTurn[0]);

            charactersContainer.style.visibility = Visibility.Visible;
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private void ShowCharacters(List<Character> characters, Player playerTurn)
    {
        charactersContainer?.Clear();
        characterVeDict.Clear();
        UnregisterBtns(characterBtns);

        foreach (Character character in characters)
        {
            VisualElement characterVe = characterUIAsset.Instantiate();
            characterVe.AddToClassList("character");         
            characterVe.Q<VisualElement>("Icon").style.backgroundImage = FindCharacterIcon(character);
            characterVe.Q<VisualElement>("Hp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetHpNormalized() * 100);
            characterVe.Q<VisualElement>("Mp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetMpNormalized() * 100);
            
            if (player != playerTurn) 
            {
                Button characterBtn = characterVe.Q<Button>();
                characterBtn.clicked += () => SelectCharacter(character);
                characterBtns.Add(characterBtn);
            }
            
            characterVeDict.Add(character, characterVe);
            charactersContainer.Add(characterVe);
        }
    }

    private StyleBackground FindCharacterIcon(Character character) 
    {
        if (!characterIconDict.ContainsKey(character.GetCharacterName()))
        {         
            Sprite sprite = Resources.Load<Sprite>(characterIconsFolder + character.GetCharacterName());
           
            StyleBackground styleBackground = new (sprite);
            characterIconDict.Add(character.GetCharacterName(), styleBackground);

            return styleBackground;
        }
        
        else return characterIconDict[character.GetCharacterName()];        
    }

    private void SelectCharacter(Character character)
    {
        if (combat.CanMove(character, player))
        {
            EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED,
                    new Dictionary<string, object>() { { "Character", character } });

            //character.GetMovementFrontier();
        }        
    }

    private void UnregisterBtns(List<Button> btns)
    {
        btns.ForEach(btn => btn.clicked -= () => { });
        btns.Clear();
    }

}
