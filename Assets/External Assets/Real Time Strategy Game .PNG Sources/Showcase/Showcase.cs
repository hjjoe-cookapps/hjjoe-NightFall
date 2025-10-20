using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Showcase : MonoBehaviour
{
    public List<Sprite> Sprites;
    public Image Image;
    private int index;

    private void Start()
    {
        index = -1;
        Next();
    }

    public void Next()
    {
        index++;
        if (index >= Sprites.Count) index = 0;
        Image.sprite = Sprites[index];
    }
}
