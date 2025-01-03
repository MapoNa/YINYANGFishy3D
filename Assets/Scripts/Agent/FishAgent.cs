using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishAgent : Agent
{
    // Define the size range of the fish
    public Vector2 SizeRange = new Vector2() { x = 0.5f, y = 1.5f };

    // Private field to store the fish's material
    private Material fishMaterial;

    // Array to store the fish's colors
    public Color[] FishColors = new Color[2];

    // Array to store the fish's renderers
    public MeshRenderer[] FishRenderers;

    // Method called when the game starts
    private void Start()
    {
        // Initialize the fish's data
        InitFishData();
    }

    // Method to initialize the fish's data
    private void InitFishData()
    {
        // Check if the rigidbody is not null
        if (rb != null)
        {
            // Generate a random size value and apply it to the fish's scale and mass
            var scaleValue = RandomSize(SizeRange);
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            rb.mass = scaleValue;
        }

        // Find the unlit/color shader
        var shader = Shader.Find("Unlit/Color");

        // Create a new material with the unlit/color shader
        var mat = new Material(shader);

        // Set the material's color to a random color from the FishColors array
        mat.SetColor("_Color", FishColors[Random.Range(0, FishColors.Length)]);

        // Apply the material to each renderer in the FishRenderers array
        foreach (var renderer in FishRenderers)
        {
            renderer.material = mat;
        }
    }

    // Method to generate a random size value within the given range
    private float RandomSize(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
}