using System;
using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.PubSub;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDCombatController : MonoBehaviour
{
    private VisualElement root;
    private Combat combat;

    private Button endTurnBtn;
    private Label playerTurnLbl;
    private Player player;

    void Awake()
    {
        root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;

        endTurnBtn = root.Q<Button>("BtnEndTurn");
        endTurnBtn.clicked += BtnEndTurn_clicked;
        player = GameObject.FindAnyObjectByType<Session>().Player;

        playerTurnLbl = root.Q<VisualElement>("PlayerName").Q<Label>();
    }

    private void OnDestroy() { endTurnBtn.clicked -= BtnEndTurn_clicked; }

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

    private void HandleCutsceneCombatStart(Dictionary<string, object> context) {  HideElements(); }

    private void HandleCutsceneCombatEnd(Dictionary<string, object> context) { ShowElements(); }

    private void HideElements()
    {
        root.Q<VisualElement>("Footer").style.display = DisplayStyle.None;
        // root.style.visibility = Visibility.Hidden;
        // endTurnBtn.style.visibility = Visibility.Hidden;
        // playerTurnLbl.style.visibility = Visibility.Hidden;
    }

    private void ShowElements()
    {
        root.Q<VisualElement>("Footer").style.display = DisplayStyle.Flex;
        // root.style.visibility = Visibility.Visible;
        // endTurnBtn.style.visibility = Visibility.Visible;
        // playerTurnLbl.style.visibility = Visibility.Visible;
    }

    private void BtnEndTurn_clicked() 
    { 
        if(combat == null)
            combat = GameObject.FindAnyObjectByType<Combat>();
        
        combat.CallEndTurnTX(combat.GetActualTurnPlayer());
        
        EventManager.Instance.Publish(GameEvent.PATH_FRONTIERS_RESET);
    }

    private void HandleCombatTurnChanged(Dictionary<string, object> context)
    {
        try
        {
            Player playerTurn = (Player)context["Player"];
            bool isAdversary = !(playerTurn.Id == player.Id);

            playerTurnLbl.text = playerTurn.PlayerName;
            StyleEnum<Visibility> visibility = endTurnBtn.style.visibility;
            

            endTurnBtn.style.visibility = !isAdversary ? Visibility.Visible : Visibility.Hidden;
            
        } catch (Exception e) { Debug.LogException(e); }
    }    
}
