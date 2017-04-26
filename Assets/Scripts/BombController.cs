using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombController : NetworkBehaviour
{
    private static readonly Vector2[] DIRECTIONS = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

    [SerializeField]
    private float _timer;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private float _tileDelay = 0.05f;

    private bool _initialized = false;
    private int _range;

    public void Initialize(int range)
    {
        _initialized = true;
        _range = range;
    }

    // Use this for initialization
    void Start()
    {
        if (!_initialized)
            Destroy(this.gameObject);
        else
            StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(_timer);
        Debug.Log("Bomb explodes");

        Vector2 worldPos = this.transform.position;
        Vector2 tilePos = new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y));

        CmdSpawnExplosion(tilePos, 0);

        Vector2 currentPos;
        bool[] spread = { true, true, true, true };

        for (int r = 1; r < _range; r++)
        {
            for (int d = 0; d < 4; d++)
            {
                if (!spread[d])
                    continue;

                currentPos = tilePos + r * DIRECTIONS[d];

                RaycastHit2D[] hits = Physics2D.RaycastAll(currentPos, Vector2.zero);
                bool noWall = true;

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.CompareTag("FixedWall"))
                    {
                        spread[d] = false;
                        noWall = false;
                        //break;
                    }
                    else if (hit.collider.CompareTag("DestructibleWall"))
                    {
                        //Destroy(hit.collider.gameObject);
                        CmdSpawnExplosion(currentPos, _tileDelay * r);
                        noWall = false;
                        //break;
                    }
                }

                if (noWall)
                    CmdSpawnExplosion(currentPos, _tileDelay * r);
            }
        }

        Destroy(this.gameObject);
    }

    [Command]
    private void CmdSpawnExplosion(Vector2 pos, float delay)
    {
        GameObject explosion = Instantiate(_explosionPrefab, pos, _explosionPrefab.transform.rotation);
        explosion.GetComponent<ExplosionScript>().Initialize(delay);
    }
}
