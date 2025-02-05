using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class GameRoomPlayerCounter : NetworkBehaviour
{
    [SyncVar]
    private int minPlayer;
    [SyncVar]
    private int maxPlayer;

    [SerializeField]
    private TextMeshProUGUI playerCountText;

    public void UpdatePlayerCount()
    {
        var players = FindObjectsOfType<FPSRoomPlayer>();
        bool isStartable = players.Length >= maxPlayer;
        playerCountText.color = isStartable ? Color.white : Color.red;
        playerCountText.text = string.Format("{0}/{1}", players.Length, maxPlayer);
        LobbyUIManager.Instance.SetInteractableStartButton(isStartable);
    }

    private void Start()
    {
        if (isServer)
        {
            var manager = NetworkManager.singleton as FPSRoomManager;
            minPlayer = manager.minPlayerCount;
            maxPlayer = manager.maxConnections;
        }
    }
}
