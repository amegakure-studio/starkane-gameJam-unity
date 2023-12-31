using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class Recommendation : ModelInstance
{
    private dojo.FieldElement from;
    private dojo.FieldElement to;
    private bool recommended;

    public override void Initialize(Model model)
    {
        from = model.members["from"].ty.ty_primitive.felt252;
        to = model.members["to"].ty.ty_primitive.felt252;
        recommended = model.members["recommended"].ty.ty_primitive.p_bool;

        var fromString = BitConverter.ToString(from.data.ToArray()).Replace("-", "").ToLower();
        // Assuming the 'from' field is a hexadecimal representation of felt252 data
        // Convert it to an integer or process it as needed

        var toString = BitConverter.ToString(to.data.ToArray()).Replace("-", "").ToLower();
        // Assuming the 'to' field is a hexadecimal representation of felt252 data
        // Convert it to an integer or process it as needed

        // Debug.Log("Recommendation: \n from: " + fromString
        //           + "\n to: " + toString
        //           + "\n recommended: " + recommended + "\n");
    }
}
