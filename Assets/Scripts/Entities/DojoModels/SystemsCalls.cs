using bottlenoselabs.C2CS.Runtime;
using Dojo.Starknet;
using dojo_bindings;
using UnityEngine;

public class SystemsCalls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x217d22689e0ca2c8f8c57171016704b6e2436a54e26a44367d16da9d87fa75b";
            
            var character_id = dojo.felt_from_hex_be(new CString("0x02")).ok;
            var player_id = dojo.felt_from_hex_be(new CString("0x01")).ok;

            dojo.Call call = new dojo.Call()
            {
                calldata = new dojo.FieldElement[]
                {
                        character_id,
                        player_id,
                        dojo.felt_from_hex_be(new CString("0x01")).ok
                },
                to = actionsAddress,
                selector = "mint"
            };

            account.ExecuteRaw(new[] { call });
        }
    }
}
