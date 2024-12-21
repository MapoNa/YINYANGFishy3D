using System.Collections;
using UnityEngine;

public class Seaweed : MonoBehaviour
{
    public float growthRate = 0.1f;         // ÿ�������ı���
    public float growthDuration = 10f;     // ˮ�ݴ� 0 ������ʼ��������ʱ��
    public float eatShrinkFactor = 0.2f;   // ÿ�α��Ե�����С�ı���
    public float minScale = 0.1f;          // ��С��������ڴ�ֵ�ᱻ����
    public float nutritionValue = 0.05f;   // �Ե�ˮ��ʱ���ӵ�Ӫ��ֵ
    public float yinYangEffect = 0.1f;     // ����ֵ����������������������������������

    private Vector3 initialScale;          // ��ʼ���
    public bool isRegrowing = false;      // ˮ���Ƿ���������

    private void Start()
    {
        initialScale = transform.localScale; // ��¼ˮ�ݳ�ʼ���
    }

    private void Update()
    {
        if (isRegrowing)
        {
            Regrow(); // ������ʱ����
        }
    }

    public void OnEaten()
    {
        if (isRegrowing) return; // �������������������

        // ��Сˮ�����
        transform.localScale -= initialScale * eatShrinkFactor;

        // ���ˮ�������С����������
        if (transform.localScale.x < initialScale.x * minScale)
        {
            StartCoroutine(StartRegrowth());
        }
    }

    private IEnumerator StartRegrowth()
    {
        isRegrowing = true;
        transform.localScale = Vector3.zero; // ���������Ϊ 0
        yield return new WaitForSeconds(growthDuration); // �ȴ�����ʱ��
        isRegrowing = false;
        transform.localScale = initialScale; // �ָ���ʼ���
    }

    private void Regrow()
    {
        // ������������ֱ���ָ���ʼ���
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale, growthRate * Time.deltaTime);
    }
}
