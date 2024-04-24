using UnityEngine;
using System.Collections;

[System.Serializable]
public class MoodBoxData : object
{
    public float noiseAmount;
    public float colorMixBlend;
    public Color colorMix;
    public float fogY;
    public Color fogColor;
    public string adventureString;
    public bool outside;
    public MoodBoxData()
    {
        this.noiseAmount = 0.375f;
        this.colorMix = Color.green;
        this.fogY = -10f;
        this.fogColor = Color.black;
        this.adventureString = "";
    }

}