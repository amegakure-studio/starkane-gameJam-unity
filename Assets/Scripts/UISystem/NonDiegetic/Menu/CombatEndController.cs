using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatEndController : MonoBehaviour
{
    [SerializeField] VisualTreeAsset victoryMenuAsset;
    [SerializeField] VisualTreeAsset defeatMenuAsset;

    private Player sessionPlayer;
    private VisualElement container;

    private ICommand goToWorldCommand;
    private Button btnGoToWorld;

    private bool isCutsceneRunning = false;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_VICTORY, HandleCombatVictory);
        EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        EventManager.Instance.Subscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_VICTORY, HandleCombatVictory);
        EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_START, HandleCutsceneCombatStart);
        EventManager.Instance.Unsubscribe(GameEvent.CUTSCENE_COMBAT_END, HandleCutsceneCombatEnd);

        if (btnGoToWorld != null )
            btnGoToWorld.clicked -= goToWorldCommand.Do;
    }

    private void HandleCutsceneCombatEnd(Dictionary<string, object> dictionary)
    {
        isCutsceneRunning = false;
    }

    private void HandleCutsceneCombatStart(Dictionary<string, object> dictionary)
    {
        isCutsceneRunning = true;
    }

    private void Awake()
    {
        ICommand goToWorldCommand = new GoToWorld();
    }

    private void Start()
    {
        VisualElement root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("Middle");
    }

    private void HandleCombatVictory(Dictionary<string, object> context)
    {
        StartCoroutine(nameof(VictoryCoroutine), context);
    }

    private IEnumerator VictoryCoroutine(Dictionary<string, object> context)
    {
        while(isCutsceneRunning) { yield return null; }

        try
        {
            Player winner = (Player)context["Player"];
            Player sessionPlayer = GameObject.FindObjectOfType<Session>().Player;

            VisualTreeAsset menuAsset = winner.GetInstanceID() == sessionPlayer.GetInstanceID() ? victoryMenuAsset : defeatMenuAsset;
            VisualElement menu = menuAsset.Instantiate();

            btnGoToWorld = menu.Q<Button>("ReturnBtn");
            btnGoToWorld.clicked += ChangeScene;

            container.Add(menu);

        } catch (Exception e) { Debug.LogError(e); }
    }

    private void ChangeScene()
    {
        if(goToWorldCommand == null)
            goToWorldCommand = new GoToWorld();
        goToWorldCommand.Do();
    }
}
