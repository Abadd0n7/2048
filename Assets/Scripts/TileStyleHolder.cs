using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileStyle
{
    public int Number;
    public Color32 TileNumber;
    public Color32 TextColor;
}

public class TileStyleHolder : MonoBehaviour
{
    public static TileStyleHolder Instance;

    public TileStyle[] TileStyles;

    void Awake ()
	{
	    Instance = this;
	}
}
