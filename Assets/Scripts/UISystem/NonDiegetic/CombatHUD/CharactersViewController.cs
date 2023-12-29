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

    void Awake()
    {
        root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        charactersContainer = root.Q<VisualElement>("CharactersGroup");
        characterBtns = new();
        characterIconDict = new();
        characterVeDict = new();
        charactersTurn = new();
        player = GameObject.FindAnyObjectByType<Session>().Player;
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
            Debug.Log("!!!!!!!!!!!!!!!CVC Updated!!!!!!!!!!!!1");
            Player playerTurn = (Player)context["Player"];
            
            if(combat == null)
                    combat = GameObject.FindAnyObjectByType<Combat>();

            charactersTurn = combat.GetCharacters(playerTurn);

            ShowCharacters(charactersTurn, playerTurn);
            SelectCharacter(charactersTurn[0]);

            charactersContainer.style.visibility = Visibility.Visible;
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private void ShowCharacters(List<Character> characters, Player playerTurn)
    {
        //charactersContainer?.Clear();
        characterVeDict.Clear();
        UnregisterBtns(characterBtns);

        List<VisualElement> characterVeContainers = charactersContainer.Children().ToList();
        ClearCharacterContainers(characterVeContainers);

        for (int i = 0; i < Math.Min(characters.Count, characterVeContainers.Count); i++)
        {
            VisualElement characterVe = charactersContainer[i];
            characterVe.Q<VisualElement>("Icon").style.backgroundImage = FindCharacterIcon(characters[i]);
            characterVe.Q<VisualElement>("Hp").Q<VisualElement>("Overlay").style.width = Length.Percent(characters[i].GetHpNormalized() * 100);
            characterVe.Q<VisualElement>("Mp").Q<VisualElement>("Overlay").style.width = Length.Percent(characters[i].GetMpNormalized() * 100);
            characterVe.RemoveFromClassList("invisible");

            if (player == playerTurn)
            {
                Button characterBtn = characterVe.Q<Button>();
                characterBtn.SetEnabled(true);
                Character selected = characters[i];
                characterBtn.clicked += () =>  SelectCharacter(selected);
                characterBtns.Add(characterBtn);
            }

            characterVeDict.Add(characters[i], characterVe);
            charactersContainer.Add(characterVe);
        }

        //foreach (Character character in characters)
        //{
        //    VisualElement characterVe = characterUIAsset.Instantiate();
        //    characterVe.AddToClassList("character");         
        //    characterVe.Q<VisualElement>("Icon").style.backgroundImage = FindCharacterIcon(character);
        //    characterVe.Q<VisualElement>("Hp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetHpNormalized() * 100);
        //    characterVe.Q<VisualElement>("Mp").Q<VisualElement>("Overlay").style.width = Length.Percent(character.GetMpNormalized() * 100);
            
        //    if (player != playerTurn) 
        //    {
        //        Button characterBtn = characterVe.Q<Button>();
        //        characterBtn.clicked += () => SelectCharacter(character);
        //        characterBtns.Add(characterBtn);
        //    }
            
        //    characterVeDict.Add(character, characterVe);
        //    charactersContainer.Add(characterVe);
        //}
    }

    private void ClearCharacterContainers(List<VisualElement> characterVeContainers)
    {
        characterVeContainers.ForEach(characterVe => characterVe.AddToClassList("invisible"));
        characterVeContainers.ForEach(characterVe => characterVe.Q<Button>().SetEnabled(false));
    }

    private StyleBackground FindCharacterIcon(Character character) 
    {
        if (!characterIconDict.ContainsKey(character.CharacterName))
        {         
            Sprite sprite = Resources.Load<Sprite>(characterIconsFolder + character.CharacterName);
           
            StyleBackground styleBackground = new (sprite);
            characterIconDict.Add(character.CharacterName, styleBackground);

            return styleBackground;
        }
        
        else return characterIconDict[character.CharacterName];        
    }

    private void SelectCharacter(Character character)
    {
        
        if (combat.CanMove(character, player))
        {
            Debug.Log("character selected: " + character.CharacterName);
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
