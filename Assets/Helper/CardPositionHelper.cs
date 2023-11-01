using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPositionHelper
{
    public static Vector3[] spawnPoints = new Vector3[]
       {
            new Vector3(0f, -425f, 0f),
            new Vector3(-687f, 0f, 0f),
            new Vector3(0f, 425f, 0f),
            new Vector3(687f, 0f, 0f)
       };
    public static Vector3[] spawnPointsFor5Players = new Vector3[]
    {
            new Vector3(500f, -425f, 0f),
            new Vector3(-500, -425f, 0f),
            new Vector3(-717f, 425f, 0f),
            new Vector3(0, 425f, 0f),
            new Vector3(717f, 425f, 0f),
    };
    public static Vector3[] spawnPointsFor6Players = new Vector3[]
    {
            new Vector3(0f, -425f, 0f),
            new Vector3(-717f, -425f, 0f),
            new Vector3(-717f, 425f, 0f),
            new Vector3(0, 425f, 0f),
            new Vector3(717f, 425f, 0f),
            new Vector3(717f, -425f, 0f)
    };
    public static Vector3[] spawnPointsFor7Players = new Vector3[]
    {
            new Vector3(0f, -455f, 0f),
            new Vector3(-687f, -455f, 0f),
            new Vector3(-687f, 0, 0f),
            new Vector3(-687f, 455f, 0f),
            new Vector3(0, 455f, 0f),
            new Vector3(687f, 455f, 0f),
            new Vector3(687f, -455f, 0f)
    };
    public static Vector3[] spawnPointsFor8Players = new Vector3[]
    {
            new Vector3(0f, -455f, 0f),
            new Vector3(-687f, -455f, 0f),
            new Vector3(-687f, 0, 0f),
            new Vector3(-687f, 455f, 0f),
            new Vector3(0, 455f, 0f),
            new Vector3(687f, 455f, 0f),
            new Vector3(687f, 0, 0f),
            new Vector3(687f, -455f, 0f)
    };

    public static void GetPositionHelper(int numberOfPlayersToGenerate, out Vector3[] playerPosition, out float scaleFactor)
    {
        playerPosition = null; // Initialisez les paramètres de sortie pour éviter les erreurs.
        scaleFactor = 0f;

        if (numberOfPlayersToGenerate < 5)
        {
            playerPosition = CardPositionHelper.spawnPoints;
            scaleFactor = 0.9f;
        }
        else if (numberOfPlayersToGenerate == 5)
        {
            scaleFactor = 0.9f;
            playerPosition = CardPositionHelper.spawnPointsFor5Players;
        }
        else if (numberOfPlayersToGenerate == 6)
        {
            scaleFactor = 0.9f;
            playerPosition = CardPositionHelper.spawnPointsFor6Players;
        }
        else if (numberOfPlayersToGenerate == 7)
        {
            scaleFactor = 0.7f;
            playerPosition = spawnPointsFor7Players; // Assurez-vous d'avoir une définition correcte de spawnPointsFor7Players.
        }
        else if (numberOfPlayersToGenerate == 8)
        {
            scaleFactor = 0.7f;
            playerPosition = CardPositionHelper.spawnPointsFor8Players;
        }
    }
}
