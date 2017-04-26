using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{
    [SerializeField]
    private int _mapSize;

    [SerializeField]
    private Transform _wallHolder;

    [SerializeField]
    private GameObject _destructibleWallPrefab;

    // Use this for initialization
    [ServerCallback]
    void Start()
    {
            RpcGenerateDestructibleWalls();
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

    [ClientRpc]
    private void RpcGenerateDestructibleWalls()
    {
        Debug.LogError("error");

        int halfSize = _mapSize / 2;
        System.Random random = new System.Random();

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
                }
            }
        }
    }

    /*[ClientRpc]
    private void RpcGenerateDestructibleWall(Vector2 position)
    {
        GameObject wall = Instantiate(_destructibleWallPrefab, position, _destructibleWallPrefab.transform.rotation);
        wall.transform.parent = _wallHolder;
    }*/

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
