using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneratePlayers : MonoBehaviour
{
    public GameObject playerCardPrefab; // Le prefab représentant une carte de joueur
    public int numberOfPlayersToGenerate = 4; // Nombre de cartes de joueur à générer
    public Canvas canvas;

    public List<GameObject> playerCards;


    void Start()
    {
        playerCards = new List<GameObject>();
        GeneratePlayerCards();
    }

    void GeneratePlayerCards()
    {
        Vector3[] spawnPoints = new Vector3[]
        {
            new Vector3(0f, -425f, 0f),
            new Vector3(-687f, 0f, 0f),
            new Vector3(0f, 425f, 0f),
            new Vector3(687f, 0f, 0f)
        };

        for (int i = 0; i < numberOfPlayersToGenerate; i++)
        {
            playerCards.Add(Instantiate(playerCardPrefab, canvas.transform));
            playerCards[i].transform.position = spawnPoints[i];
            TextMeshProUGUI textMeshPro = playerCards[i].GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = "Player " + (i + 1);

            Vector3 currentScale = playerCards[i].transform.localScale;
            float scaleFactor = 0.9f; 
            Vector3 newScale = new Vector3(currentScale.x * scaleFactor, currentScale.y * scaleFactor, currentScale.z * scaleFactor);
            playerCards[i].transform.localScale = newScale;
        }
    }

    public PlayerCardManager[] GetPlayerCardManagers(int playerIndex)
    {
        PlayerCards playerCards = GetPlayerCards(playerIndex);
        if (playerCards != null)
        {
            Image[] cardImages = playerCards.cardImages;
            PlayerCardManager[] managers = new PlayerCardManager[cardImages.Length];

            for (int i = 0; i < cardImages.Length; i++)
            {
                managers[i] = cardImages[i].GetComponent<PlayerCardManager>();
            }

            return managers;
        }
        else
        {
            Debug.LogError("PlayerCards component not found for playerIndex: " + playerIndex);
            return new PlayerCardManager[0];
        }
    }

    public PlayerCards GetPlayerCards(int playerIndex)
    {
        return playerCards[playerIndex].GetComponent<PlayerCards>();
    }
}
