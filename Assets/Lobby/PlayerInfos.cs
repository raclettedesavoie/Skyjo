using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Button kickPlayerButton;
    public Image logoPlayer;
    public TextMeshProUGUI playerName;
    public CloudService cloudService;

    private Unity.Services.Lobbies.Models.Player player;
    private void Awake()
    {
        kickPlayerButton.onClick.AddListener(KickPlayer);
    }

    public void SetKickPlayerButtonVisible(bool visible)
    {
        kickPlayerButton.gameObject.SetActive(visible);
    }

    public void UpdatePlayer(Unity.Services.Lobbies.Models.Player player)
    {
        this.player = player;
        playerName.text = player.Data["PlayerName"].Value;
        logoPlayer.sprite = Resources.Load<Sprite>("LogoProfile/"+player.Data["PlayerLogo"].Value);
    }

    private void KickPlayer()
    {
        if (player != null)
        {
            cloudService.KickPlayer(player.Id);
        }
    }
}
