using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField]
    private int _mapSize;

    [SerializeField]
    private Transform _wallHolder;

    [SerializeField]
    private GameObject _destructibleWallPrefab;

    [SyncVar]
    private int _seed;

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

    // Use this for initialization
    [ServerCallback]
    void Start()
    {
        _seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit ();
#endif
        }
    }

    public void StartGame()
    {
        RpcGenerateDestructibleWalls();
    }

    [ClientRpc]
    private void RpcGenerateDestructibleWalls()
    {
        int halfSize = _mapSize / 2;
        System.Random random = new System.Random(_seed);

        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                if (!CheckForWall(new Vector2(x, y))
                    && (Mathf.Abs(x) != halfSize || Mathf.Abs(y) != halfSize)
                    && (random.Next(1, 11) > 6)
                    && !(Mathf.Abs(y) == halfSize && (Mathf.Abs(x) == halfSize - 1))
                    && !(Mathf.Abs(x) == halfSize && (Mathf.Abs(y) == halfSize - 1)))
                {
                    GameObject wall = Instantiate(_destructibleWallPrefab, new Vector2(x, y), _destructibleWallPrefab.transform.rotation);
                    wall.transform.parent = _wallHolder;
                    NetworkServer.Spawn(wall);
                }
            }
        }
    }

    private bool CheckForWall(Vector2 tilePos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(tilePos, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("FixedWall") || hit.collider.CompareTag("DestructibleWall"))
            {
                return true;
            }
        }

        return false;
    }
}
