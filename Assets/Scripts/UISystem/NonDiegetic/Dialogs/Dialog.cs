using Amegakure.Starkane.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Amegakure.Starkane.UI.NonDiegetic
{
    public class Dialog : MonoBehaviour
    {
        private VisualElement root;
        private Label dialogTxt;
        private Label interactorName;

        private void Start()
        {
            root = FindRoot();
            dialogTxt = root.Q<Label>("Text");
            interactorName = root.Q<Label>("Name");
        }

        public void RegisterCommands(Dictionary<Button, ICommand> dialogOptions)
        {
            foreach (Button btn in dialogOptions.Keys)
            {
                btn.clicked += dialogOptions[btn].Do;
                btn.text = dialogOptions[btn].Name;
            }
        }

        private VisualElement FindRoot()
        { return GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement.Q<VisualElement>("Dialog"); }

        public void Show() 
        {
            root ??= FindRoot();

            root.visible = true;
            root.style.display = DisplayStyle.Flex;
        }

        public void Hide() 
        {
            root ??= FindRoot();

            root.visible = false;
            root.style.display = DisplayStyle.None;
        }

        public void SetText(string text)
        {
            root ??= FindRoot();
            dialogTxt ??= root.Q<Label>("Text");

            dialogTxt.text = text;
        }

        public void SetName(string name)
        {
            root ??= FindRoot();
            interactorName ??= root.Q<Label>("Name");

            interactorName.text = name;
        }

        public VisualElement Root 
        { 
            get 
            { 
                root ??= FindRoot();
                return root; 
            }
        }
    }
}
