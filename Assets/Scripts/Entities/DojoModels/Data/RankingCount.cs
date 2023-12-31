using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class RankingCount : ModelInstance
{
    private dojo.FieldElement id;
    private int idValue;
    private UInt32 index;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.felt252;
        index = model.members["index"].ty.ty_primitive.u32;

        var idString = BitConverter.ToString(id.data.ToArray()).Replace("-", "").ToLower();
        idValue = System.Int32.Parse(idString, System.Globalization.NumberStyles.AllowHexSpecifier);

        // Debug.Log("RankingCount: \n id: " + idValue
        //           + "\n index: " + index + "\n");
    }
}
