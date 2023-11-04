using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Button kickPlayerButton;
    public Image logoPlayer;
    public TextMeshProUGUI playerName;

    public CloudService cloudService;
    public Unity.Services.Lobbies.Models.Player player;

    public void AwakeFunction()
    {
        kickPlayerButton.onClick.AddListener(KickPlayer);

        if (player != null && player.Id == AuthenticationService.Instance.PlayerId)
        {
            var btnChangeLogo = GetComponentInChildren<Button>();
            btnChangeLogo.onClick.AddListener(() =>
            {
                Debug.Log("clicked");
                cloudService.UpdatePlayerData("nextLogo");
            });
        }
    }

    public void SetKickPlayerButtonVisible(bool visible)
    {
        kickPlayerButton.gameObject.SetActive(visible);
    }

    public void UpdatePlayer(Unity.Services.Lobbies.Models.Player player)
    {
        this.player = player;
        if (this.player.Data != null)
        {
            playerName.text = player.Data["PlayerName"].Value;
            logoPlayer.sprite = SpriteRandomHelper.GetSprite(int.Parse(player.Data["PlayerLogo"].Value));
        }
    }

    private void KickPlayer()
    {
        if (player != null)
        {
            cloudService.KickPlayer(player.Id);
        }
    }
}
