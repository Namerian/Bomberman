using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnerScript : NetworkBehaviour
{
    public static SpawnerScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    [Command]
    public void CmdSpawnDrop(GameObject prefab, Vector3 position)
    {
        RpcSpawnDrop(prefab, position);
    }

    [ClientRpc]
    private void RpcSpawnDrop(GameObject prefab, Vector3 position)
    {
        GameObject drop = Instantiate(prefab, position, prefab.transform.rotation);
        NetworkServer.Spawn(drop);
    }
}
