using UnityEngine;

public class SpriteRandomHelper
{
    public static Sprite[] sprites = Resources.LoadAll<Sprite>("LogoProfile");
    public static Sprite GetSprite(int spriteIndex)
    {
        spriteIndex = spriteIndex % sprites.Length;
        return sprites[spriteIndex];
    }
}