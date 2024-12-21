using UnityEngine;

public class ChuanTouTest : MonoBehaviour
{
    public float speed = 10;
    public float realHeigth = 1.5f;
    public float bodyRadius = 0.5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }
    }

    private void FixedUpdate()
    {
        // �����ƶ�����
        Move(speed);
    }

    #region // �����ƶ����

    // �жϵ�ǰλ�õĿ��ƶ���
    private Vector3 AntiPenetrationPos(Vector3 _aimPos)
    {
        if (transform.position == _aimPos)
        {
            return transform.position;
        }

        // �ж����ݵĿ��ƶ���
        bool couldHmove = true;
        bool couldVmove = true;

        // ��ȡ��ɫ����ƫ������
        Vector3 dirH = (new Vector3(_aimPos.x - transform.position.x, 0, 0)).normalized;
        Vector3 dirV = (new Vector3(0, 0, _aimPos.z - transform.position.z)).normalized;

        Vector3 bodyPos = transform.position;
        float tempPercent = 3;
        bodyPos.y += realHeigth / tempPercent;

        // �ж�����
        if (dirH != Vector3.zero && Physics.Raycast(new Ray(bodyPos, dirH), bodyRadius))
        {
            couldHmove = false;
        }
        else if (Physics.Raycast(new Ray(bodyPos + dirH * bodyRadius, Vector3.up), realHeigth / tempPercent))
        {
            couldHmove = false;
        }

        // �ж�����
        if (dirV != Vector3.zero && Physics.Raycast(new Ray(bodyPos, dirV), bodyRadius))
        {
            couldVmove = false;
        }
        else if (Physics.Raycast(new Ray(bodyPos + dirV * bodyRadius, Vector3.up), realHeigth / tempPercent))
        {
            couldVmove = false;
        }

        // ��������
        Debug.DrawLine(bodyPos, dirH * bodyRadius + bodyPos, Color.red);
        Debug.DrawLine(bodyPos, dirV * bodyRadius + bodyPos, Color.red);

        Vector3 temp = bodyPos + dirH * bodyRadius;
        Debug.DrawLine(temp, temp + Vector3.up * realHeigth, Color.red);
        temp = bodyPos + dirV * bodyRadius;
        Debug.DrawLine(temp, temp + Vector3.up * realHeigth, Color.red);

        // ���ݼ��������Ŀ��λ��
        Vector3 aimPos = _aimPos;
        if (!couldHmove) aimPos.x = transform.position.x;
        if (!couldVmove) aimPos.z = transform.position.z;

        return aimPos;
    }

    // �ƶ��߼�
    // �ƶ��߼�
    protected void Move(float speed)
    {
        Vector3 moveValue = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // ���������
        if (moveValue.magnitude > 0)
        {
            // �����ƶ�����
            Vector3 movement = moveValue.normalized * speed * Time.fixedDeltaTime;
            Vector3 aimPos = rb.position + movement;

            // ���� AntiPenetrationPos ���Ŀ��λ��
            Vector3 validPos = AntiPenetrationPos(aimPos);

            // ʹ�ø����ƶ�
            rb.MovePosition(validPos);

            // ��ģ�������ƶ�����
            Quaternion targetRotation = Quaternion.LookRotation(moveValue);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
        else
        {
            // û������ʱ����ֹͣ�����˶�
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // ȷ����תҲֹͣ
        }
    }


    #endregion
}
