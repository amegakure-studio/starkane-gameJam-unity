using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class MapCC : ModelInstance
{
    private UInt32 id;
    private UInt64 token_id;
    private byte size;
    private dojo.FieldElement obstacles_1;
    private BigInteger obstacles_1Int;
    private dojo.FieldElement obstacles_2;
    private BigInteger obstacles_2Int;
    private dojo.FieldElement obstacles_3;
    private BigInteger obstacles_3Int;
    private dojo.FieldElement owner;
    private UInt64 width;
    private UInt64 height;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.u32;
        token_id = model.members["token_id"].ty.ty_primitive.u64;
        size = model.members["size"].ty.ty_primitive.u8;
        
        obstacles_1 = model.members["obstacles_1"].ty.ty_primitive.felt252;
        var obstacles_1String = BitConverter.ToString(obstacles_1.data.ToArray()).Replace("-", "").ToLower();
        obstacles_1Int = BigInteger.Parse( obstacles_1String, NumberStyles.AllowHexSpecifier );

        obstacles_2 = model.members["obstacles_2"].ty.ty_primitive.felt252;
        var obstacles_2String = BitConverter.ToString(obstacles_2.data.ToArray()).Replace("-", "").ToLower();
        obstacles_2Int = BigInteger.Parse( obstacles_2String, NumberStyles.AllowHexSpecifier );
        
        obstacles_3 = model.members["obstacles_3"].ty.ty_primitive.felt252;
        var obstacles_3String = BitConverter.ToString(obstacles_3.data.ToArray()).Replace("-", "").ToLower();
        obstacles_3Int = BigInteger.Parse( obstacles_3String, NumberStyles.AllowHexSpecifier );
        
        owner = model.members["owner"].ty.ty_primitive.felt252;
        width = model.members["width"].ty.ty_primitive.u64;
        height = model.members["height"].ty.ty_primitive.u64;

        // Debug.Log("MapCC: \n id: " + id
        //           + "\n token_id: " + token_id
        //           + "\n size: " + size
        //           + "\n obstacles_1: " + obstacles_1
        //           + "\n obstacles_2: " + obstacles_2
        //           + "\n obstacles_3: " + obstacles_3
        //           + "\n owner: " + owner
        //           + "\n width: " + width
        //           + "\n height: " + height + "\n");
        
        // PrintWalkableStatus();
    }

    void PrintWalkableStatus()
    {
        StringBuilder resultBuilder = new StringBuilder();

        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 25; y++)
            {
                bool isWalkable = IsWalkable(new UnityEngine.Vector2(x, y));
                resultBuilder.AppendLine($"----WALKABLE???? {isWalkable} | X: {x} | Y: {y}");
            }
        }

        string resultString = resultBuilder.ToString();
        Debug.Log(resultString);
    }

   
    public bool IsWalkable(UnityEngine.Vector2 position)
    {
        ulong x = (ulong)position.x;
        ulong y = (ulong)position.y;
        return GetBit((ulong) y * 25 + x );
    }

    private bool GetBit(ulong position)
    {
        if (position >= 625)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Posición no válida");
        }

        if (position < 248)
        {
            return (obstacles_1Int & GetPow(247 - position)) == 0;
        }
        else if (position < 496)
        {
            return (obstacles_2Int & GetPow(247 - position % 248)) == 0;
        }
        else
        {
            return (obstacles_3Int & GetPow(247 - position % 248)) == 0;
        }
    }

    private BigInteger GetPow(ulong exponent)
{
    return BigInteger.Pow(2, (int)exponent);
}
}
