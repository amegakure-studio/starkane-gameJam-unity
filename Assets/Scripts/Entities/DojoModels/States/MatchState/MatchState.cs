using System;
using System.Globalization;
using System.Numerics;
using Dojo;
using Dojo.Starknet;
using Dojo.Torii;

public class MatchState : ModelInstance
{
    [ModelField("id")]
    public UInt32 id;

    [ModelField("turn")]
    public UInt32 turn;

    [ModelField("player_turn")]
    public FieldElement player_turn;

    [ModelField("map_id")]
    public UInt32 map_id;

    [ModelField("winner")]
    public FieldElement winner;

    public event Action<string> playerTurnIdChanged;
    public event Action<string> winnerChanged;

    public uint Id { get => id; set => id = value; }


    //public override void Initialize(Model model)
    //{
    //    base.Initialize(model);

    //    //var player_turn_string = BitConverter.ToString(player_turn.data.ToArray()).Replace("-", "").ToLower();
    //    //PlayerTurnId = BigInteger.Parse( player_turn_string, NumberStyles.AllowHexSpecifier );

    //    //var winner_id_string = BitConverter.ToString(winner.data.ToArray()).Replace("-", "").ToLower();
    //    //WinnerId = BigInteger.Parse( winner_id_string, NumberStyles.AllowHexSpecifier );
    //}

    public override void OnUpdate(Model model)
    {
        FieldElement oldPlayerTurn = player_turn;
        FieldElement oldWinnerID = winner;

        base.OnUpdate(model);

        FieldElement newPlayerTurn = player_turn;
        FieldElement newWinnerID = winner;

        if (oldPlayerTurn != null && !oldPlayerTurn.Hex().Equals(newPlayerTurn.Hex()) )
        {
            playerTurnIdChanged?.Invoke(player_turn.Hex());
        }

        if (oldWinnerID != null && !oldWinnerID.Hex().Equals(newWinnerID.Hex()))
        {
            winnerChanged?.Invoke(winner.Hex());
        }

    }
}
