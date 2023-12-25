using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int id;
    private CharacterPlayerProgress characterPlayerProgress = null;
    public int Id { get => id; set => id = value; }

    // Start is called before the first frame update
    void Start()
    {
        if(characterPlayerProgress == null)
        {
            CharacterPlayerProgress c = this.GetComponentInChildren<CharacterPlayerProgress>();
            if(c != null)
            {
                characterPlayerProgress = c;
                characterPlayerProgress.print();
            }            
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
