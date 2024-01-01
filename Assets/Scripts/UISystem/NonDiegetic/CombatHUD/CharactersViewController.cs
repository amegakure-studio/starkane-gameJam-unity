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
        // EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        // EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
        // EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        // EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
    }

    private void HandleCutsceneCombatStart(Dictionary<string, object> context) { charactersContainer.style.visibility = Visibility.Hidden; }

    private void HandleCutsceneCombatEnd(Dictionary<string, object> context) { charactersContainer.style.visibility = Visibility.Visible; }

    private void HandleSkillDone(Dictionary<string, object> context)
    {
        if (player != null)
        {
            foreach (Character character in player.Characters)
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

            if (combat == null)
                combat = GameObject.FindAnyObjectByType<Combat>();

            charactersTurn = combat.GetCharacters(playerTurn);

            Debug.Log("Character in battle for player: " + player.PlayerName + ", " + charactersTurn.Count);

            ShowCharacters(charactersTurn, playerTurn);
            SelectCharacter(charactersTurn[0]);
        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private void ShowCharacters(List<Character> characters, Player playerTurn)
    {
        characterVeDict.Clear();
        UnregisterBtns(characterBtns);

        List<VisualElement> characterVeContainers = charactersContainer.Children().ToList();
        ClearCharacterContainers(characterVeContainers);

        for (int i = 0; i < Math.Min(characters.Count, characterVeContainers.Count); i++)
        {
            Character selected = characters[i];
            VisualElement characterVe = charactersContainer[i];
            characterVe.Q<VisualElement>("Icon").style.backgroundImage = FindCharacterIcon(characters[i]);
            // characterVe.Q<VisualElement>("Hp").Q<VisualElement>("Overlay").style.width = Length.Percent(characters[i].GetHpNormalized() * 100);
            // characterVe.Q<VisualElement>("Mp").Q<VisualElement>("Overlay").style.width = Length.Percent(characters[i].GetMpNormalized() * 100);
            characterVe.RemoveFromClassList("invisible");

            Button characterBtn = characterVe.Q<Button>();
            characterBtn.SetEnabled(true);
            
            Debug.Log("Character name: " + selected.CharacterName);
            characterBtn.clicked += () => SelectCharacter(selected);
            characterBtns.Add(characterBtn);

            // if (player == playerTurn)
            // {
            //     Button characterBtn = characterVe.Q<Button>();
            //     characterBtn.SetEnabled(true);

            //     Character selected = characters[i];
            //     Debug.Log("Character name: " + selected.CharacterName);
            //     characterBtn.clicked += () =>  SelectCharacter(selected);
            //     characterBtns.Add(characterBtn);
            // }

            characterVeDict.Add(characters[i], characterVe);
            // charactersContainer.Add(characterVe);
        }
        Debug.Log("Container elements: " + characterVeDict.Keys.Count);
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

            StyleBackground styleBackground = new(sprite);
            characterIconDict.Add(character.CharacterName, styleBackground);

            return styleBackground;
        }

        else return characterIconDict[character.CharacterName];
    }

    private void SelectCharacter(Character character)
    {
        Debug.Log("Character Selected: " + character.CharacterName);

        if (combat.CanMove(character, player))
        {
            // Debug.Log("character selected: " + character.CharacterName);
            EventManager.Instance.Publish(GameEvent.INPUT_CHARACTER_SELECTED,
                    new Dictionary<string, object>() { { "Character", character } });
        }
    }

    private void UnregisterBtns(List<Button> btns)
    {
        btns.ForEach(btn => btn.clicked -= () => { });
        btns.Clear();
    }

}
