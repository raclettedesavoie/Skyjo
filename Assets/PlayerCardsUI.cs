using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCards : MonoBehaviour
{
    public Image[] cardImages ;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cardImages.Length; i++)
        {
            cardImages[i].sprite = SetBackCardImage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Sprite SetBackCardImage()
    {
        return Resources.Load<Sprite>("BackCard");
    }

    public void SetCardImage(int cardNumber,int deckValue)
    {
        cardImages[cardNumber-1].sprite = Resources.Load<Sprite>($"{deckValue}");
    }
}
