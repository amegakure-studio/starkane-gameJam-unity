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

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.COMBAT_VICTORY, HandleCombatVictory);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.COMBAT_VICTORY, HandleCombatVictory);

        if (btnGoToWorld != null )
            btnGoToWorld.clicked -= goToWorldCommand.Do;
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
        try
        {
            Player winner = (Player)context["Player"];

            VisualTreeAsset menuAsset = winner == sessionPlayer ? victoryMenuAsset : defeatMenuAsset;
            VisualElement menu = menuAsset.Instantiate();

            btnGoToWorld = menu.Q<Button>("ReturnBtn");
            btnGoToWorld.clicked += goToWorldCommand.Do;

            container.Add(menu);

        } catch { }
       
    }
}
