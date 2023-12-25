using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using System;
using System.Globalization;

public class PlayerStadistics : ModelInstance
{
    dojo.FieldElement owner;
    UInt64 matchs_won;
    UInt64 matchs_lost;
    UInt32 characters_owned;
    UInt64 total_score;
    private int m_IntID;

    public override void Initialize(Model model)
    {
        owner = model.members["owner"].ty.ty_primitive.felt252;
        matchs_won = model.members["matchs_won"].ty.ty_primitive.u64;
        matchs_lost = model.members["matchs_lost"].ty.ty_primitive.u64;
        characters_owned = model.members["characters_owned"].ty.ty_primitive.u32;
        total_score = model.members["total_score"].ty.ty_primitive.u64;
        
        var hexString = BitConverter.ToString(owner.data.ToArray()).Replace("-", "").ToLower();
        m_IntID = System.Int32.Parse( hexString, NumberStyles.AllowHexSpecifier );

        GameObject[] playerGoList = GameObject.FindGameObjectsWithTag("Player");
        
        foreach( GameObject playerGo in playerGoList)
        {
            try
            {
                playerGo.TryGetComponent<Player>(out Player player);
                if(player != null)
                {
                    if(player.Id == m_IntID)
                    {
                        gameObject.transform.parent = playerGo.transform;
                    }   
                }        
            }
            catch(Exception e){Debug.LogError(e);}
        }
       
    }

   public override string ToString()
   {
        return "owner: " + m_IntID + "\n"
                  + "matchs_won: " + matchs_won + "\n" + "matchs_lost: " + matchs_lost + "\n"
                  + "characters_owned: " + characters_owned
                  + "\n" + "total_score: " + total_score;
   }
}
