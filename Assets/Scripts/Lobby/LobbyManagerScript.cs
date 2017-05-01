using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManagerScript : NetworkLobbyManager
{
    public static LobbyManagerScript Instance { get; private set; }

    public int NumPlayers { get; private set; }

    private int _readyPlayers = 0;

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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    //{
    //    _readyPlayers++;

    //    Debug.Log("Player ready! numReadyPlayers=" + _readyPlayers);

    //    if (_readyPlayers == NumPlayers)
    //    {
    //        Debug.Log("Starting game!");
    //        GameController.Instance.StartGame();
    //    }

    //    return true;
    //}

    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();

        Debug.Log("Server started!");

        _readyPlayers = 0;
    }

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        base.OnLobbyServerConnect(conn);

        NumPlayers++;

        Debug.Log("Lobby: player connected! numPlayers=" + NumPlayers);
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerConnect(conn);

        NumPlayers--;

        Debug.Log("Lobby: player disconnected! numPlayers=" + NumPlayers);
    }

    public void OnPlayerReady()
    {
        _readyPlayers++;

        Debug.Log("Player ready! numReadyPlayers=" + _readyPlayers);

        if (_readyPlayers == NumPlayers)
        {
            Debug.Log("Starting game!");
            GameController.Instance.StartGame();
        }
    }
}
