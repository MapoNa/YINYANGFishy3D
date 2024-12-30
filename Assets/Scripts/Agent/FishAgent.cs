using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishAgent : Agent
{
    public Vector2 SizeRange = new Vector2() { x = 0.5f, y = 1.5f };
    private Material fishMaterial;
    public Color[] FishColors = new Color[2];
    public MeshRenderer[] FishRenderers; //public

    private void Start()
    {
        InitFishData();
    }

    private void InitFishData()
    {
        if (rb != null)
        {
            var scaleValue = RandomSize(SizeRange);
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            rb.mass = scaleValue;
        }

        var shader = Shader.Find("Unlit/Color");
        var mat = new Material(shader);
        mat.SetColor("_Color", FishColors[Random.Range(0, FishColors.Length)]);
        foreach (var renderer in FishRenderers)
        {
            renderer.material = mat;
        }
    }

    private float RandomSize(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
}