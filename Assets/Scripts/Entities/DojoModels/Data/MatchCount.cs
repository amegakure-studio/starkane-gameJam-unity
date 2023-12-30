using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MatchCount : ModelInstance
{
    private dojo.FieldElement id;
    private UInt32 index;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.felt252;
        index = model.members["index"].ty.ty_primitive.u32;

        var idString = BitConverter.ToString(id.data.ToArray()).Replace("-", "").ToLower();
        // Assuming the 'id' field is a hexadecimal representation of felt252 data
        // Convert it to an integer or process it as needed

        // Debug.Log("MatchCount: \n id: " + idString
        //           + "\n index: " + index + "\n");
    }
}
