using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCards : MonoBehaviour
{
    public Image[] cardImages ;
    public List<Card> cardsValue ;
    public int numberPlayer;
    public TextMeshProUGUI playerName;
    public Image logoProfile;
    // Start is called before the first frame update
    void Start()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("LogoProfile");

        if (sprites.Length > 0)
        {
            Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];

            if (logoProfile != null)
            {
                logoProfile.sprite = randomSprite;
            }
        }
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

    public void RemoveCardFromScene(int numberColumn)
    {
        cardImages[numberColumn].color = new Color(0,0,0,0);
        cardImages[numberColumn+4].color = new Color(0,0,0,0);
        cardImages[numberColumn+8].color = new Color(0,0,0,0);
    }
}
