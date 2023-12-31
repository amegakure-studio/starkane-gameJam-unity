using Amegakure.Starkane.EntitiesWrapper;
using bottlenoselabs.C2CS.Runtime;
using Dojo.Starknet;
using dojo_bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    private Button loginBtn;
    private TextField usernameTxtField;
    private TextField passwordTxtField;

    private void Start()
    {
        VisualElement root = GameObject.FindAnyObjectByType<UIDocument>().rootVisualElement;

        usernameTxtField = root.Q<TextField>("Username");
        passwordTxtField = root.Q<TextField>("Password");

        loginBtn = root.Q<Button>("LoginBtn");
        loginBtn.clicked += LoginBtn_clicked;
    }

    private void LoginBtn_clicked()
    {
        CallLoginTx();
        CreateSessionObj();
    }

    private void CreateSessionObj()
    {
        GameObject sessionGo = Instantiate(new GameObject);   
        Session session = sessionGo.AddComponent<Session>();
        session.Player = FindSessionPlayer();

        DontDestroyOnLoad(sessionGo);
        DontDestroyOnLoad(session.Player);
    }

    private Player FindSessionPlayer()
    {
        throw new NotImplementedException();
    }

    private void CallLoginTx()
    {
        string rpcUrl = "http://localhost:5050";

        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
        string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

        var account = new Account(provider, signer, playerAddress);
        string actionsAddress = "0x57a6556e89380b76465e525c725d8ed065a03b47fb9a4c9b676a1afea8177c5";


        var hash = new Hash128();
        hash.Append("Santi");
        hash.Append("Hello");
        string hashString = hash.ToString();
        Debug.Log(hashString);

        var character_id = dojo.felt_from_hex_be(new CString("0x02")).ok;
        var player_id = dojo.felt_from_hex_be(new CString(hashString)).ok;

        Debug.Log(player_id);
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
