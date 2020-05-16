using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Souls")]

public class Souls : ScriptableObject
{
    public string Type;
    public Sprite icon;
    public Sprite mask;
    public AudioClip sound;
}
