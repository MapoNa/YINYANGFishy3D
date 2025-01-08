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
        // 调用移动方法
        Move(speed);
    }

    #region // 控制移动相关

    // 判断当前位置的可移动性
    private Vector3 AntiPenetrationPos(Vector3 _aimPos)
    {
        if (transform.position == _aimPos)
        {
            return transform.position;
        }

        // 判定横纵的可移动性
        bool couldHmove = true;
        bool couldVmove = true;

        // 获取角色横纵偏移向量
        Vector3 dirH = (new Vector3(_aimPos.x - transform.position.x, 0, 0)).normalized;
        Vector3 dirV = (new Vector3(0, 0, _aimPos.z - transform.position.z)).normalized;

        Vector3 bodyPos = transform.position;
        float tempPercent = 3;
        bodyPos.y += realHeigth / tempPercent;

        // 判定横向
        if (dirH != Vector3.zero && Physics.Raycast(new Ray(bodyPos, dirH), bodyRadius))
        {
            couldHmove = false;
        }
        else if (Physics.Raycast(new Ray(bodyPos + dirH * bodyRadius, Vector3.up), realHeigth / tempPercent))
        {
            couldHmove = false;
        }

        // 判定纵向
        if (dirV != Vector3.zero && Physics.Raycast(new Ray(bodyPos, dirV), bodyRadius))
        {
            couldVmove = false;
        }
        else if (Physics.Raycast(new Ray(bodyPos + dirV * bodyRadius, Vector3.up), realHeigth / tempPercent))
        {
            couldVmove = false;
        }

        // 调试射线
        Debug.DrawLine(bodyPos, dirH * bodyRadius + bodyPos, Color.red);
        Debug.DrawLine(bodyPos, dirV * bodyRadius + bodyPos, Color.red);

        Vector3 temp = bodyPos + dirH * bodyRadius;
        Debug.DrawLine(temp, temp + Vector3.up * realHeigth, Color.red);
        temp = bodyPos + dirV * bodyRadius;
        Debug.DrawLine(temp, temp + Vector3.up * realHeigth, Color.red);

        // 根据检测结果修正目标位置
        Vector3 aimPos = _aimPos;
        if (!couldHmove) aimPos.x = transform.position.x;
        if (!couldVmove) aimPos.z = transform.position.z;

        return aimPos;
    }

    // 移动逻辑
    // 移动逻辑
    protected void Move(float speed)
    {
        Vector3 moveValue = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // 如果有输入
        if (moveValue.magnitude > 0)
        {
            // 计算移动向量
            Vector3 movement = moveValue.normalized * speed * Time.fixedDeltaTime;
            Vector3 aimPos = rb.position + movement;

            // 调用 AntiPenetrationPos 检查目标位置
            Vector3 validPos = AntiPenetrationPos(aimPos);

            // 使用刚体移动
            rb.MovePosition(validPos);

            // 让模型面向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveValue);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
        else
        {
            // 没有输入时立刻停止刚体运动
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // 确保旋转也停止
        }
    }


    #endregion
}
