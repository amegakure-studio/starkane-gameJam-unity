using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;

public class SkillsViewController : MonoBehaviour
{
    [SerializeField] VisualTreeAsset skillUIAsset;

    private VisualElement root;
    private VisualElement skillContainer;
    private Dictionary<Skill, VisualElement> skillsDict;
    private Dictionary<Button, Skill> skillBtns;

    private readonly string skillIconsFolder = "UI/Skills/";
    private Dictionary<string, StyleBackground> skillIconDict;
    private Skill skillSelected;
    private Amegakure.Starkane.EntitiesWrapper.Character characterSelected;
    private Player player;
    private Combat combat;

    private void Awake()
    {
        player = GameObject.FindAnyObjectByType<Session>().Player;
    }

    void Start()
    {
        skillBtns = new();
        skillsDict = new();
        skillIconDict = new();

        root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        skillContainer = root.Q<VisualElement>("SkillGroup");
    }

    private void OnDestroy() { UnregisterBtns(skillBtns.Keys.ToList()); }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
        EventManager.Instance.Subscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Subscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
        EventManager.Instance.Subscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_TURN_CHANGED, HandleCombatTurnChanged);
        EventManager.Instance.Unsubscribe(GameEvent.TILE_SELECTED, HandleTileSelected);
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_SKILL_DONE, HandleSkillDone);
    }

    private void HandleSkillDone(Dictionary<string, object> context)
    {
        foreach (Skill skill in skillsDict.Keys)
        {
            VisualElement skillVe = skillsDict[skill];
            skillVe.Q<VisualElement>("Icon").AddToClassList("disabled");
            skillVe.Q<Button>().SetEnabled(false);
        }
    }

    private void HandleTileSelected(Dictionary<string, object> context)
    {
        try
        {
            Tile target = (Tile)context["Tile"];

            if (!target.IsMovementTile)
            {
                GameObject occupyingObjectGo = target.OccupyingObject;

                if (occupyingObjectGo != null)
                {
                    Character characterReceiver = occupyingObjectGo.GetComponent<Character>();
                    if (characterReceiver != null)
                    {
                        string playerIdReceiver = characterReceiver.GetPlayerId();
                        combat = GetCombat();

                        Player playerReceiver = combat.GetPlayerByID(playerIdReceiver);
                        combat.DoSkill(player, characterSelected, skillSelected, playerReceiver, characterReceiver);

                        List<Tile> tilesToReset = skillSelected.GetFrontier(characterSelected.Location).Tiles;
                        EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET, new() { { "Tiles", tilesToReset } });
                    }
                }
            }
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    private void HandleCombatTurnChanged(Dictionary<string, object> context)
    {
        try
        {
            Player playerTurn = (Player)context["Player"];

            combat = GetCombat();

            List<Character> playerCharacters = combat.GetCharacters(player);
            Character character = playerCharacters.Find(pc => pc.IsAlive());
            if (character != null)
            {
                characterSelected = character;
                ShowSkills(character.Skills, character);

                skillContainer.style.visibility = playerTurn == player ? Visibility.Visible : Visibility.Hidden;
            }

        }
        catch (Exception e) { Debug.LogException(e); }
    }

    private void ShowSkills(List<Skill> skills, Character character)
    {
        UnregisterBtns(skillBtns.Keys.ToList());
        skillBtns.Clear();
        skillsDict.Clear();

        List<VisualElement> skillVeContainers = skillContainer.Children().ToList();

        ClearSkillContainers(skillVeContainers);

        for (int i = 0; i < Math.Min(characterSelected.Skills.Count, skillVeContainers.Count); i++)
        {
            Skill skillSelected = characterSelected.Skills[i];

            VisualElement skillVe = skillContainer[i];
            skillVe.Q<VisualElement>("Icon").style.backgroundImage = FindSkillIcon(characterSelected.Skills[i]);
            skillVe.RemoveFromClassList("invisible");

            Button skillBtn = skillVe.Q<Button>();
            skillVe.Q<VisualElement>("Icon").RemoveFromClassList("disabled");
            skillBtn.SetEnabled(true);
            skillBtn.clicked += () => SelectSkill(skillSelected);

            skillBtns.Add(skillBtn, skillSelected);

            combat = GetCombat();

            if (!combat.CanDoSkill(player, character, skillSelected))
            {
                skillVe.Q<VisualElement>("Icon").AddToClassList("disabled");
                skillVe.Q<Button>().SetEnabled(false);
            }

            skillsDict.Add(skillSelected, skillVe);
        }
    }

    private Combat GetCombat()
    {
        return combat != null ? combat : FindAnyObjectByType<Combat>();
    }

    private void ClearSkillContainers(List<VisualElement> skillVeContainers)
    {
        skillVeContainers.ForEach(characterVe => characterVe.AddToClassList("invisible"));
        skillVeContainers.ForEach(characterVe => characterVe.Q<Button>().SetEnabled(false));
    }

    private void SelectSkill(Skill skill)
    {
        List<Tile> tilesToReset = characterSelected.GetMovementFrontier().Tiles;

        if (skillSelected != null)
            tilesToReset.AddRange(skillSelected.GetFrontier(characterSelected.Location).Tiles);

        EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET, new() { { "Tiles", tilesToReset } });

        skillSelected = skill;

        EventManager.Instance.Publish(GameEvent.FRONTIER_UPDATED, new() { { "Frontier", skillSelected.GetFrontier(characterSelected.Location) } });
    }

    private StyleBackground FindSkillIcon(Skill skill)
    {
        string skillName = skill.Skill_type.ToString();
        if (!skillIconDict.ContainsKey(skillName))
        {
            Sprite sprite = Resources.Load<Sprite>(skillIconsFolder + skillName);

            StyleBackground styleBackground = new(sprite);
            skillIconDict.Add(skillName, styleBackground);

            return styleBackground;
        }

        else return skillIconDict[skillName];
    }


    private void HandleCharacterSelected(Dictionary<string, object> context)
    {
        characterSelected = (Character)context["Character"];
        ShowSkills(characterSelected.Skills, characterSelected);
    }

    private void UnregisterBtns(List<Button> btns)
    {
        btns.ForEach(btn => btn.clicked -= () => { });
        btns.Clear();
    }

}
