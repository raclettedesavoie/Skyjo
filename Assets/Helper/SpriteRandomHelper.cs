using UnityEngine;

public class SpriteRandomHelper
{
    public static Sprite GetRandomSprite() 
    { 
        Sprite[] sprites = Resources.LoadAll<Sprite>("LogoProfile");

        return sprites[Random.Range(0, sprites.Length)];
    }
}