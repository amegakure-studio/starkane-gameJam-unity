using System;
using System.Globalization;
using System.Numerics;
using Dojo;
using Dojo.Starknet;
using Dojo.Torii;

public class MapCC : ModelInstance
{

    [ModelField("id")]
    public UInt32 id;

    [ModelField("token_id")]
    public UInt64 token_id;

    [ModelField("size")]
    public byte size;
    
    // Check this:
    [ModelField("obstacles_1")]
    public FieldElement obstacles_1;

    [ModelField("obstacles_2")]
    public FieldElement obstacles_2;

    [ModelField("obstacles_3")]
    public FieldElement obstacles_3;

    [ModelField("owner")]
    public FieldElement owner;

    [ModelField("width")]
    public UInt64 width;

    [ModelField("height")]
    public UInt64 height;

    private BigInteger obstacles_1Int;
    private BigInteger obstacles_2Int;
    private BigInteger obstacles_3Int;

    public override void Initialize(Model model)
    {   
        base.Initialize(model);

        //var obstacles_1String = BitConverter.ToString(obstacles_1.data.ToArray()).Replace("-", "").ToLower();
        //obstacles_1Int = BigInteger.Parse( obstacles_1String, NumberStyles.AllowHexSpecifier );

        //var obstacles_2String = BitConverter.ToString(obstacles_2.data.ToArray()).Replace("-", "").ToLower();
        //obstacles_2Int = BigInteger.Parse( obstacles_2String, NumberStyles.AllowHexSpecifier );
        
        //var obstacles_3String = BitConverter.ToString(obstacles_3.data.ToArray()).Replace("-", "").ToLower();
        //obstacles_3Int = BigInteger.Parse( obstacles_3String, NumberStyles.AllowHexSpecifier );
    }
   
    public bool IsWalkable(UnityEngine.Vector2 position)
    {
        ulong x = (ulong)position.x;
        ulong y = (ulong)position.y;
        return GetBit((ulong) x * 25 + y );
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
