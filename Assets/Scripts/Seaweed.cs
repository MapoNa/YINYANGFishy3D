using System.Collections;
using UnityEngine;

public class Seaweed : MonoBehaviour
{
    public float growthRate = 0.1f;         // 每秒增长的比例
    public float growthDuration = 10f;     // 水草从 0 长到初始体积所需的时间
    public float eatShrinkFactor = 0.2f;   // 每次被吃掉后缩小的比例
    public float minScale = 0.1f;          // 最小体积，低于此值会被销毁
    public float nutritionValue = 0.05f;   // 吃掉水草时增加的营养值
    public float yinYangEffect = 0.1f;     // 阴阳值的增减量（正数增加阳，负数增加阴）

    private Vector3 initialScale;          // 初始体积
    public bool isRegrowing = false;      // 水草是否正在重生

    private void Start()
    {
        initialScale = transform.localScale; // 记录水草初始体积
    }

    private void Update()
    {
        if (isRegrowing)
        {
            Regrow(); // 在重生时生长
        }
    }

    public void OnEaten()
    {
        if (isRegrowing) return; // 如果正在重生，不处理

        // 缩小水草体积
        transform.localScale -= initialScale * eatShrinkFactor;

        // 如果水草体积过小，触发重生
        if (transform.localScale.x < initialScale.x * minScale)
        {
            StartCoroutine(StartRegrowth());
        }
    }

    private IEnumerator StartRegrowth()
    {
        isRegrowing = true;
        transform.localScale = Vector3.zero; // 将体积重置为 0
        yield return new WaitForSeconds(growthDuration); // 等待重生时间
        isRegrowing = false;
        transform.localScale = initialScale; // 恢复初始体积
    }

    private void Regrow()
    {
        // 按比例生长，直到恢复初始体积
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale, growthRate * Time.deltaTime);
    }
}
