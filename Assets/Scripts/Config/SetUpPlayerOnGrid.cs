using System.Collections;
using System.Collections.Generic;
using Amegakure.Starkane.EntitiesWrapper;
using UnityEngine;

public class SetUpPlayerOnGrid : MonoBehaviour
{
    void Start()
    {
        Character sessionCharacter = GameObject.FindAnyObjectByType<Character>();
        if(sessionCharacter)
        {
            Context context = GameObject.FindObjectOfType<Context>();
            sessionCharacter.Location = context.GetInitialLocation();
        }        
        
    }

}
