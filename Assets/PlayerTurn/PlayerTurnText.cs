using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTurnText : MonoBehaviour
{
    public TextMeshProUGUI playerTurnTextMesh;
    public void SetText(string playerTurnText)
    {
        playerTurnTextMesh.text = playerTurnText;
    }
}
