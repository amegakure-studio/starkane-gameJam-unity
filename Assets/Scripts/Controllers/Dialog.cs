using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Dialog : MonoBehaviour
{
    private VisualElement root;
    private Dictionary<Button, ICommand> buttonMap;
    private Label dialogTxt;
    private Label interactorName;

    private void Start()
    {
        root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement.Q<VisualElement>("Dialog");
        dialogTxt = root.Q<Label>("Text");
        interactorName = root.Q<Label>("Name");

        buttonMap = new Dictionary<Button, ICommand>
        {
            { root.Q<Button>("Btn1"), new CombatStart(this) },
            { root.Q<Button>("Btn2"), new CombatCancel(this) }
        };

        RegisterCommands(buttonMap);
    }

    private void RegisterCommands(Dictionary<Button, ICommand> map)
    {
        foreach (Button btn in map.Keys)
        {
            btn.clicked += map[btn].Do;
            btn.text = map[btn].Name;
        }
    }

    public void Show() 
    {
        root.visible = true;
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide() 
    { 
        root.visible = false;
        root.style.display = DisplayStyle.None;
    }

    public void SetText(string text) { dialogTxt.text = text;}

    public void SetName(string name) { interactorName.text = name; }
}
