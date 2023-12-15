using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dojo_bindings;
using UnityEditor.PackageManager;
using UnityEngine;
using Dojo.Torii;
using Dojo.Starknet;

namespace Dojo
{
    public class WorldManager : MonoBehaviour
    {
        [Header("RPC")]
        public string toriiUrl = "http://localhost:8080";
        public string rpcUrl = "http://localhost:5050";
        [Header("World")]
        public string worldAddress;
        public SynchronizationMaster synchronizationMaster;
        public ToriiClient toriiClient;

        // Start is called before the first frame update
        void Awake()
        {
            // create the torii client and start subscription service
            toriiClient = new ToriiClient(toriiUrl, rpcUrl, worldAddress, new dojo.KeysClause[] { });
            // start subscription service
            toriiClient.StartSubscription();

            // fetch entities from the world
            // TODO: maybe do in the start function of the SynchronizationMaster?
            // problem is when to start the subscription service
            synchronizationMaster.SynchronizeEntities();

            // listen for entity updates
            synchronizationMaster.RegisterEntityCallbacks();

            var provider = new JsonRpcClient(rpcUrl);

            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Starknet.Account(provider, signer, playerAddress);
            string actionsAddress = "0x152dcff993befafe5001975149d2c50bd9621da7cbaed74f68e7d5e54e65abc";
            
            dojo.Call call = new dojo.Call()
            {
                to = actionsAddress,
                selector = "spawn"
            };
            
            account.ExecuteRaw(new[] { call });
        }

        // Update is called once per frame
        void Update()
        {
        }


        public GameObject Entity(string name)
        {
            var entity = transform.Find(name);
            if (entity == null)
            {
                Debug.LogError($"Entity {name} not found");
                return null;
            }

            return entity.gameObject;
        }

        // return all children
        public GameObject[] Entities()
        {
            return transform.Cast<Transform>()
                .Select(t => t.gameObject)
                .ToArray();
        }

        public GameObject AddEntity(string key)
        {
            // check if entity already exists
            var entity = transform.Find(key)?.gameObject;
            if (entity != null)
            {
                Debug.LogWarning($"Entity {key} already exists");
                return entity.gameObject;
            }

            entity = new GameObject(key);
            entity.transform.parent = transform;

            return entity;
        }

        public void RemoveEntity(string key)
        {
            var entity = transform.Find(key);
            if (entity != null)
            {
                Destroy(entity.gameObject);
            }
        }
    }
}
