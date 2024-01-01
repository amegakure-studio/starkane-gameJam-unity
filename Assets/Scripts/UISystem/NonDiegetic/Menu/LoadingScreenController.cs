using Amegakure.Starkane.PubSub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreenController : MonoBehaviour
{
    private VisualElement container;

    private void Start()
    {
        VisualElement root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;
        container = root.Q<VisualElement>("LoadingScreen");
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEvent.GAME_LOADING_START, HandleGameLoadingStart);
        EventManager.Instance.Subscribe(GameEvent.GAME_LOADING_END, HandleGameLoadingEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEvent.GAME_LOADING_START, HandleGameLoadingStart);
        EventManager.Instance.Unsubscribe(GameEvent.GAME_LOADING_END, HandleGameLoadingEnd);
    }

    private void HandleGameLoadingStart(Dictionary<string, object> context)
    {
        container?.RemoveFromClassList("hidden");
    }

    private void HandleGameLoadingEnd(Dictionary<string, object> context)
    {
        container?.AddToClassList("hidden");
    }
}
