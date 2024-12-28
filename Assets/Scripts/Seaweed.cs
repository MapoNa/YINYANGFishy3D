using System.Collections;
using UnityEngine;

public class Seaweed : MonoBehaviour
{
    public float growthRate = 0.1f;         // Ã¿ÃëÔö³¤µÄ±ÈÀý
    public float growthDuration = 10f;     // Ë®²Ý´Ó 0 ³¤µ½³õÊ¼Ìå»ýËùÐèµÄÊ±¼ä
    public float eatShrinkFactor = 0.2f;   // Ã¿´Î±»³ÔµôºóËõÐ¡µÄ±ÈÀý
    public float minScale = 0.1f;          // ×îÐ¡Ìå»ý£¬µÍÓÚ´ËÖµ»á±»Ïú»Ù
    public float nutritionValue = 0.05f;   // ³ÔµôË®²ÝÊ±Ôö¼ÓµÄÓªÑøÖµ
    public float yinYangEffect = 0.1f;     // ÒõÑôÖµµÄÔö¼õÁ¿£¨ÕýÊýÔö¼ÓÑô£¬¸ºÊýÔö¼ÓÒõ£©

    private Vector3 initialScale;          // ³õÊ¼Ìå»ý
    public bool isRegrowing = false;      // Ë®²ÝÊÇ·ñÕýÔÚÖØÉú

    private void Start()
    {
        initialScale = transform.localScale; // ¼ÇÂ¼Ë®²Ý³õÊ¼Ìå»ý
    }

    private void Update()
    {
        if (isRegrowing)
        {
            Regrow(); // ÔÚÖØÉúÊ±Éú³¤
        }
    }

    public void OnEaten()
    {
        if (isRegrowing) return; // Èç¹ûÕýÔÚÖØÉú£¬²»´¦Àí

        // ËõÐ¡Ë®²ÝÌå»ý
        transform.localScale -= initialScale * eatShrinkFactor;

        // Èç¹ûË®²ÝÌå»ý¹ýÐ¡£¬´¥·¢ÖØÉú
        if (transform.localScale.x < initialScale.x * minScale)
        {
            StartCoroutine(StartRegrowth());
        }
    }

    private IEnumerator StartRegrowth()
    {
        isRegrowing = true;
        transform.localScale = Vector3.zero; // ½«Ìå»ýÖØÖÃÎª 0
        yield return new WaitForSeconds(growthDuration); // µÈ´ýÖØÉúÊ±¼ä
        isRegrowing = false;
        transform.localScale = initialScale; // »Ö¸´³õÊ¼Ìå»ý
    }

    private void Regrow()
    {
        // °´±ÈÀýÉú³¤£¬Ö±µ½»Ö¸´³õÊ¼Ìå»ý
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale, growthRate * Time.deltaTime);
    }
}
