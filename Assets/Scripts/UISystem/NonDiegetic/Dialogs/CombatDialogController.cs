using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.UI.NonDiegetic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Dialog))]
public class CombatDialogController : MonoBehaviour
{
    [SerializeField] string interactorName;
    [SerializeField] string dialogText;

    private Dialog dialog;
    private Dictionary<Button, ICommand> dialogOptions;

    void Start()
    {
        dialog = GetComponent<Dialog>();
        
        dialog.SetName(interactorName);
        dialog.SetText(dialogText);

        dialogOptions = new Dictionary<Button, ICommand>
        {
            { dialog.Root.Q<Button>("Btn1"), new CombatStart(dialog) },
            { dialog.Root.Q<Button>("Btn2"), new CombatCancel(dialog) }
        };

        dialog.RegisterCommands(dialogOptions);
    }
   
}
