using System.Collections;
using UnityEngine;

public class Seaweed : MonoBehaviour
{
    public float growthRate = 0.1f;         // Growth rate per second
    public float growthDuration = 10f;     // Time required for the seaweed to fully regrow
    public float eatShrinkFactor = 0.2f;   // Shrink factor applied when eaten
    public float minScale = 0.1f;          // Minimum scale before the seaweed is destroyed
    public float nutritionValue = 0.05f;   // Nutrition value added when the seaweed is eaten
    public float yinYangEffect = 0.1f;     // Effect on YinYang value (positive increases Yang, negative increases Yin)

    private Vector3 initialScale;          // Initial scale of the seaweed
    public bool isRegrowing = false;       // Whether the seaweed is currently regrowing

    private void Start()
    {
        initialScale = transform.localScale; // Record the initial scale of the seaweed
    }

    private void Update()
    {
        if (isRegrowing)
        {
            Regrow(); // Regrow the seaweed during the regrowth phase
        }
    }

    public void OnEaten()
    {
        if (isRegrowing) return; // Skip if the seaweed is already regrowing

        // Shrink the seaweed's scale
        transform.localScale -= initialScale * eatShrinkFactor;

        // Trigger regrowth if the seaweed's scale becomes too small
        if (transform.localScale.x < initialScale.x * minScale)
        {
            StartCoroutine(StartRegrowth());
        }
    }

    private IEnumerator StartRegrowth()
    {
        isRegrowing = true;
        transform.localScale = Vector3.zero; // Reset scale to zero
        yield return new WaitForSeconds(growthDuration); // Wait for the regrowth duration
        isRegrowing = false;
        transform.localScale = initialScale; // Restore the initial scale
    }

    private void Regrow()
    {
        // Gradually regrow the seaweed towards its initial scale
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale, growthRate * Time.deltaTime);
    }
}
