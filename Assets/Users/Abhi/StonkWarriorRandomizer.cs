using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StonkWarriorRandomizer : MonoBehaviour
{
    public Sprite[] sprites;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Randomize), 7, 7);
    }

    private void Randomize()
    {
        StartCoroutine(RandomizeFace());
    }

    private IEnumerator RandomizeFace()
    {
        Sprite s = sprites[Random.Range(0, sprites.Length)];
        Color temp = Color.white;
        temp = Color.Lerp(Color.white, Color.black, .1f);
        image.color = temp;
        yield return null;
        image.sprite = s;
        temp = Color.Lerp(Color.black, Color.white, .1f);
        yield return null;
    }
}
