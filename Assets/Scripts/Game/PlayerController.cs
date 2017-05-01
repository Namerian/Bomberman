using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _turnOffset;

    [SerializeField]
    private GameObject _bombPrefab;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private int _bombs = 3;

    [SerializeField]
    private int _range = 4;

    private int _bombUpgrades;
    private int _rangeUpgrades;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GetComponent<Renderer>().material.color = Color.blue;

        UiManagerScript.Instance.SetBombText(_bombs);
        UiManagerScript.Instance.SetRangeText(_range);

        CmdPlayerReady();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }


        Vector2 worldPos = this.transform.position;
        Vector2 tilePos = new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y));

        //if (Input.GetKeyDown(KeyCode.Space))
        if (Input.GetButtonDown("Jump") && _bombs > 0)
        {
            CmdSpawnBomb(tilePos);
            _bombs--;
            UiManagerScript.Instance.SetBombText(_bombs);
        }

        Vector2 direction = Vector2.zero;
        bool wallUp = CheckForWall(tilePos + new Vector2(0, 1));
        bool wallRight = CheckForWall(tilePos + new Vector2(1, 0));
        bool wallDown = CheckForWall(tilePos + new Vector2(0, -1));
        bool wallLeft = CheckForWall(tilePos + new Vector2(-1, 0));

        /*if (Input.GetKey(KeyCode.UpArrow) && worldPos.y < 4 && tilePos.x % 2 == 0 && worldPos.x > tilePos.x - _turnOffset && worldPos.x < tilePos.x + _turnOffset)
            direction.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow) && worldPos.y > -4 && tilePos.x % 2 == 0 && worldPos.x > tilePos.x - _turnOffset && worldPos.x < tilePos.x + _turnOffset)
            direction.y = -1;
        else if (Input.GetKey(KeyCode.LeftArrow) && worldPos.x > -4 && tilePos.y % 2 == 0 && worldPos.y > tilePos.y - _turnOffset && worldPos.y < tilePos.y + _turnOffset)
            direction.x = -1;
        else if (Input.GetKey(KeyCode.RightArrow) && worldPos.x < 4 && tilePos.y % 2 == 0 && worldPos.y > tilePos.y - _turnOffset && worldPos.y < tilePos.y + _turnOffset)
            direction.x = 1;*/

        if (Input.GetAxis("Vertical") == 1//Input.GetKey(KeyCode.UpArrow)
            && !(wallUp && worldPos.y >= tilePos.y)
            && (worldPos.x > tilePos.x - _turnOffset && worldPos.x < tilePos.x + _turnOffset))
        {
            direction.y = 1;
        }
        else if (Input.GetAxis("Vertical") == -1 //Input.GetKey(KeyCode.DownArrow)
            && !(wallDown && worldPos.y <= tilePos.y)
            && (worldPos.x > tilePos.x - _turnOffset && worldPos.x < tilePos.x + _turnOffset))
        {
            direction.y = -1;
        }
        else if (Input.GetAxis("Horizontal") == -1 //Input.GetKey(KeyCode.LeftArrow)
            && !(wallLeft && worldPos.x <= tilePos.x)
            && (worldPos.y > tilePos.y - _turnOffset && worldPos.y < tilePos.y + _turnOffset))
        {
            direction.x = -1;
        }
        else if (Input.GetAxis("Horizontal") == 1 //Input.GetKey(KeyCode.RightArrow)
            && !(wallRight && worldPos.x >= tilePos.x)
            && (worldPos.y > tilePos.y - _turnOffset && worldPos.y < tilePos.y + _turnOffset))
        {
            direction.x = 1;
        }

        Vector2 newWorldPos = worldPos + (direction.normalized * _moveSpeed * Time.deltaTime);

        if (direction.x != 0 && direction.y == 0)
        {
            newWorldPos.y = Mathf.Round(newWorldPos.y);
        }

        if (direction.y != 0 && direction.x == 0)
        {
            newWorldPos.x = Mathf.Round(newWorldPos.x);
        }

        this.transform.position = newWorldPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (collision.CompareTag("UpgradeBomb"))
        {
            _bombUpgrades++;
            _bombs++;
            UiManagerScript.Instance.SetBombText(_bombs);

            Destroy(collision.gameObject);
        }
        else if(collision.CompareTag("UpgradeRange"))
        {
            _rangeUpgrades++;
            _range++;
            UiManagerScript.Instance.SetRangeText(_range);

            Destroy(collision.gameObject);
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

    

    //=============================================================
    // Commands

    [Command]
    private void CmdPlayerReady()
    {
        LobbyManagerScript.Instance.OnPlayerReady();
    }

    [Command]
    public void CmdDie()
    {
        Destroy(this.gameObject);
    }

    //=============================================================
    // Spawn Bomb

    [Command]
    private void CmdSpawnBomb(Vector2 pos)
    {
        RpcSpawnBomb(pos);
    }

    [ClientRpc]
    private void RpcSpawnBomb(Vector2 pos)
    {
        GameObject bomb = Instantiate(_bombPrefab, pos, _bombPrefab.transform.rotation);
        bomb.GetComponent<BombController>().Initialize(this, _range);
    }

    //=============================================================
    // Spawn Explosion

    [Command]
    public void CmdSpawnExplosion(Vector2 pos, float delay)
    {
        RpcSpawnExplosion(pos, delay);
    }

    [ClientRpc]
    public void RpcSpawnExplosion(Vector2 pos, float delay)
    {
        GameObject explosion = Instantiate(_explosionPrefab, pos, _explosionPrefab.transform.rotation);
        explosion.GetComponent<ExplosionScript>().Initialize(this, delay);
    }
}
