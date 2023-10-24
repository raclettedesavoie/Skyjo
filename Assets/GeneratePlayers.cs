using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GeneratePlayers : MonoBehaviour
{
    public GameObject playerCardPrefab; // Le prefab représentant une carte de joueur
    public List<GameObject> playerCards;
    public Vector3 spawnPoint; // L'endroit où les cartes de joueur seront instanciées
    public int numberOfPlayersToGenerate = 4; // Nombre de cartes de joueur à générer
    public Canvas canvas;

    void Start()
    {
        playerCards = new List<GameObject>();
        spawnPoint = new Vector3(-26.07838f, -376.6505f, 0f);
        GeneratePlayerCards();
    }

    void GeneratePlayerCards()
    {
        Vector3[] spawnPoints = new Vector3[]
        {
            new Vector3(0f, -373f, 0f),
            new Vector3(0f, 435f, 0f),
            new Vector3(-657f, 0f, 0f),
            new Vector3(657f, 0f, 0f)
        };

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Vector3 screenPos = new Vector3(spawnPoints[i][0], spawnPoints[i][1], 0);
            playerCards.Add(Instantiate(playerCardPrefab, canvas.transform));
            playerCards[i].transform.position = screenPos;
            // Vous pouvez personnaliser chaque carte de joueur ici en fonction de vos besoins.
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
