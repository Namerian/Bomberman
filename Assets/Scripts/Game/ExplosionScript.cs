using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionScript : NetworkBehaviour
{
    [SerializeField]
    private float _lifetime;

    [SerializeField]
    private List<DropListElement> _drops;

    private bool _initialized = false;
    private PlayerController _player;
    private float _delay;

    public void Initialize(PlayerController player, float delay)
    {
        _initialized = true;
        _player = player;
        _delay = delay;
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
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.enabled = false;

        yield return new WaitForSeconds(_delay);

        renderer.enabled = true;

        Vector2 worldPos = this.transform.position;
        Vector2 tilePos = new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y));

        Collider2D[] hits = Physics2D.OverlapBoxAll(tilePos, Vector2.one, 0);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector2 playerWorldPos = hit.transform.position;
                Vector2 playerTilePos = new Vector2(Mathf.Round(playerWorldPos.x), Mathf.Round(playerWorldPos.y));

                if(tilePos == playerTilePos)
                {
                    hit.GetComponent<PlayerController>().CmdDie();
                }
            }
            else if(hit.CompareTag("DestructibleWall"))
            {
                Destroy(hit.gameObject);

                int dice = UnityEngine.Random.Range(1, 100);
                int cumu = 0;

                foreach(DropListElement element in _drops)
                {
                    cumu += element.chance;

                    if(dice <= cumu)
                    {
                        //GameObject drop = Instantiate(element.prefab, this.transform.position, element.prefab.transform.rotation);
                        //NetworkServer.Spawn(drop);
                        //SpawnerScript.Instance.CmdSpawnDrop(element.prefab, this.transform.position);
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(_lifetime);

        Destroy(this.gameObject);
    }

    [Serializable]
    private class DropListElement
    {
        public GameObject prefab;
        public int chance;
    }
}