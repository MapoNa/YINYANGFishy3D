using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishController : MonoBehaviour
{
    public int CollectedItemCount = 0; // 收集物品计数
    [SerializeField] float MoveSpeed;
    public float maxDistance = 1.44f; // 绳索的最大距离
    public float pullForce = 1f; // 给父物体施加的力大小
    private GameObject grabbedBall; // 当前咬住的球
    public float obstacleDetectionDistance = 1f; // 障碍物检测距离
    public string obstacleTag = "Wall"; // 阻止移动的障碍物标签
    public bool isBlocked = false; // 表示玩家是否被阻挡
    public Slider yinYangSlider;

    public float speed = 5f;
    public float rotationSpeed = 100f;
    private float originalSpeed; // 保存默认速度值

    public Light mainLight; // 场景中的主光源
    public LayerMask shadowCastingLayers; // 用于检测阴影的图层
    public bool isInShadow = false; // 当前是否在阴影中
    public bool KeepY = true;// 保持y的位置
    public float yinYangValue = 0f;  // 阴阳值：范围 -1 到 1
    public float size = 1f;         // 鱼的体型
    public float shrinkRate = 0.1f; // 溢出时缩小的速率

    public Material fishMaterial;  // 玩家鱼的材质

    // 阴阳颜色
    public Color yinColor = Color.black; // 阴影代表的颜色（黑色）
    public Color yangColor = Color.white;  // 阳光代表的颜色（红色）

    private Rigidbody rb;
    public ParticleSystem splashEffect;
    public Camera interactionCam;
    public GameObject FishNotEating;
    public GameObject FishEating;
    public GameObject EatPoint;
    public GameObject Door1;
    public GameObject Door1Point;
    //public ParticleManager particleManager; 
    // Fish jump variables
    public float defaultYPosition = -1f;
    public float diveYPosition = -1.5f;
    public float jumpYPosition = 3f;
    public float diveDuration = 0.5f;
    public float initialJumpSpeed = 15f;
    public float gravity = 5f;
    public float maxFallSpeed = 12f;

    private bool isDiving = false;
    private bool isJumping = false;
    private float currentYPosition;
    private float diveStartTime;
    private float currentJumpSpeed;
    private float currentFallSpeed;

    //Eat
    public float growthFactor = 0.1f;        // 鱼每次变大的量
    public float foodShrinkFactor = 0.5f;   // 食物每次变小的量
    public float eatCooldown = 0.3f;        // 每次进入 IsEating 状态的间隔时间
    public string foodTag = "FishFood";     // 食物的标签

    public float shrinkSpeed = 0.001f; // 减小体型的速度
    public float minScale = 0.5f; // 最小体型限制

    private bool isEating = false;          // 是否在吃食物
    private bool canEat = true;             // 是否可以进入 IsEating 状态
    private void FixedUpdate()
    {
        HandleSplashEffect();
        HandleMovement();
        CheckAndMoveRedBall();
        HandleFishJump();
        //rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed);
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed;
        // 找到 "RedBall"

    }





    /// 根据阴阳值动态更新鱼的颜色
    /// </summary>
    public float flashSpeed = 2f; // 闪烁速度

    public void UpdateFishColor()
    {
        // 判断阴阳值是否过高或过低
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // 保存当前颜色
            Color currentBaseColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);

            // 使用PingPong让颜色在当前颜色与红色之间闪烁
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            fishMaterial.color = Color.Lerp(currentBaseColor, Color.red, t);
        }
        else
        {
            // 正常更新颜色（不闪烁）
            Color currentColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);
            fishMaterial.color = currentColor;
        }
    }





    void Start()
    {
        originalSpeed = speed;
        if (mainLight == null)
        {
            // 自动查找主光源
            mainLight = FindObjectOfType<Light>();
            if (mainLight == null)
            {
                Debug.LogError("No Light found in the scene!");
                return;
            }
        }
        rb = GetComponent<Rigidbody>();


        currentYPosition = defaultYPosition;
        transform.position = new Vector3(transform.position.x, defaultYPosition, transform.position.z);
    }
    /// <summary>
    /// 进入 IsEating 状态
    /// </summary>
    private void EnterIsEatingState()
    {
        if (canEat)
        {
            isEating = true;
            StartCoroutine(EatCooldown()); // 开始冷却计时
        }
    }
    // 定义一个 LayerMask，排除 FishModel 图层



    void DetectShadow()
    {
        Vector3 lightDirection = -mainLight.transform.forward;
        Ray ray = new Ray(transform.position, lightDirection);

        // 排除 FishModel 层进行射线检测
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, shadowCastingLayers))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("FishModel"))
            {
                isInShadow = true;
            }
        }
        else
        {
            isInShadow = false;
        }
    }


    /// <summary>
    /// 松开左键时触发吃食物逻辑
    /// </summary>
    private void TriggerEatFood()
    {
        if (isEating)
        {
            isEating = false; // 停止吃的状态

            // 查找附近的物体
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 1f); // 1f 为检测半径，可调整

            GameObject closestItem = null; // 最近的食物或收集物品
            float closestDistance = float.MaxValue; // 初始化为一个很大的值

            foreach (Collider obj in nearbyObjects)
            {
                if (obj.CompareTag(foodTag) || obj.CompareTag("Collectible")) // 检查是否是食物或收集物品
                {
                    float distance = Vector3.Distance(transform.position, obj.transform.position);

                    // 如果当前物体比记录的更近，更新最近的物体
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = obj.gameObject;
                    }
                }
            }

            // 如果找到了最近的物体，进行交互
            if (closestItem != null)
            {
                if (closestItem.CompareTag("Collectible"))
                {
                    // 如果是收集物品，销毁并增加计数
                    Destroy(closestItem);
                    CollectedItemCount++;
                    Debug.Log($"CollectedItemCount: {CollectedItemCount}");
                }
                else if (closestItem.CompareTag(foodTag))
                {
                    Seaweed seaweed = closestItem.GetComponent<Seaweed>();
                    if (seaweed != null)
                    {
                        // 如果水草正在重生，跳过处理
                        if (seaweed.isRegrowing)
                        {
                            Debug.Log($"Skipped {closestItem.name} because it's regrowing.");
                            return;
                        }

                        // 调用水草的 OnEaten 方法，处理水草的体积缩小和生长逻辑
                        seaweed.OnEaten();

                        // 根据水草的阴阳效果更新玩家的阴阳值
                        UpdateYinYang(seaweed.yinYangEffect);

                        // 根据水草的营养值增加玩家体型
                        transform.localScale += Vector3.one * seaweed.nutritionValue;
                    }
                    else
                    {
                        // 如果不是水草，按照普通食物逻辑处理
                        closestItem.transform.localScale *= foodShrinkFactor; // 食物变小

                        // 鱼变大
                        transform.localScale += Vector3.one * growthFactor;

                        // 如果食物太小，则销毁它
                        if (closestItem.transform.localScale.x < 0.1f)
                        {
                            Destroy(closestItem);
                        }
                    }
                }
            }
        }
    }



    // 更新玩家阴阳值的方法
    private void UpdateYinYang(float effect)
    {
        yinYangValue += effect; // 根据水草的效果调整阴阳值
        print("吃！");
        yinYangValue = Mathf.Clamp(yinYangValue, -1f, 1f); // 确保阴阳值在 -1 到 1 之间
    }

    private void CheckAndMoveRedBall()
    {
        if (isEating && transform.localScale.x > 0.8f) // 确保当前处于进食状态
        {
            // 查找所有具有 "RedBall" 标签的对象
            GameObject[] redBalls = GameObject.FindGameObjectsWithTag("RedBall");

            foreach (GameObject redBall in redBalls)
            {
                float distance = Vector3.Distance(transform.position, redBall.transform.position);

                // 如果红球在附近，将其移动到鱼的位置
                if (distance < 1f) // 可根据需求调整距离范围
                {

                    // 禁用碰撞器
                    Collider redBallCollider = redBall.GetComponent<Collider>();
                    if (redBallCollider != null)
                    {
                        redBallCollider.enabled = false;
                    }

                    // 将红球的位置设置为鱼的位置
                    redBall.transform.position = EatPoint.transform.position;

                    // 检查红球与门的距离
                    if (Door1 != null) // 确保 Door1 已分配
                    {
                        float parentDistance = Vector3.Distance(Door1.transform.position, redBall.transform.position);

                        if (parentDistance > maxDistance) // 超过最大距离
                        {
                            Debug.Log("施加拉力");

                            Rigidbody doorRb = Door1.GetComponent<Rigidbody>();
                            if (doorRb != null)
                            {
                                // 计算正确的拉回方向并施加力
                                Vector3 pullDirection = (EatPoint.transform.position - Door1.transform.position).normalized;
                                doorRb.AddForce(pullDirection * 15f, ForceMode.Force); // 10.0f 可调整为所需的力
                            }
                            else
                            {
                                Debug.LogWarning("Door1 没有 Rigidbody 组件！");
                            }
                        }

                        // 限制玩家位置
                        float playerDistance = Vector3.Distance(Door1Point.transform.position, transform.position);
                        if (playerDistance > maxDistance)
                        {
                            Debug.Log("限制玩家位置");

                            Vector3 restrictedPosition = Door1Point.transform.position +
                                                          (transform.position - Door1Point.transform.position).normalized *
                                                          (maxDistance);

                            // 只在超过范围时修正位置
                            transform.position = Vector3.MoveTowards(transform.position, restrictedPosition,
                                                                     playerDistance - (maxDistance));

                        }

                    }
                }
            }
        }
        else
        {
            // 如果不在进食状态，重新启用碰撞器
            GameObject[] redBalls = GameObject.FindGameObjectsWithTag("RedBall");

            foreach (GameObject redBall in redBalls)
            {
                Collider redBallCollider = redBall.GetComponent<Collider>();
                if (redBallCollider != null)
                {
                    redBallCollider.enabled = true;
                }
            }
        }
    }


    /// <summary>
    /// 吃的冷却时间
    /// </summary>
    private IEnumerator EatCooldown()
    {
        canEat = false;
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
    }
    void HandleShrink()
    {
        // 检查阴阳值是否达到阈值
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // 获取当前缩放
            Vector3 currentScale = transform.localScale;

            // 如果体型大于最小限制，则逐渐缩小体型
            if (currentScale.x > minScale || currentScale.y > minScale || currentScale.z > minScale)
            {
                transform.localScale = Vector3.Lerp(
                    currentScale,
                    new Vector3(minScale, minScale, minScale),
                    shrinkSpeed * Time.deltaTime
                );
            }
        }
    }
    void Update()
    {
        if (KeepY == false)
        {
            speed = originalSpeed / 3f; // 将速度减少到原来的三分之一
        }
        else
        {
            speed = originalSpeed; // 恢复默认速度
        }
        GameObject redBall = GameObject.FindGameObjectWithTag("RedBall");
        
        HandleShrink();
        float normalizedValue = Mathf.InverseLerp(-1f, 1f, yinYangValue);
        yinYangSlider.value = normalizedValue; // 更新滑动条的值
        UpdateFishColor();
        //FixedUpdate();      
        //HandleMovement();
       
        DetectShadow();
        if (isInShadow == true && yinYangValue < 1)
        {
            yinYangValue += 0.0004f;
        }
        if (isInShadow == false && yinYangValue > -1)
        {
            yinYangValue -= 0.0004f;
        }






        if (Input.GetMouseButton(0)) // 按住左键
        {
            EnterIsEatingState();
        }

        if (Input.GetMouseButtonUp(0)) // 松开左键
        {
            TriggerEatFood();
        }
        if (isEating == true)
        {
            FishEating.SetActive(true);
            FishNotEating.SetActive(false);
        }
        else
        {
            FishEating.SetActive(false);
            FishNotEating.SetActive(true);
        }

        if (KeepY == true)
        {
            // 强制玩家 Y 坐标固定在 1
            Vector3 position = transform.position;
            position.y = -1f; // 固定到 y = 1
            transform.position = position;
        }
    }
    private void HandleMovement()
    {
        // 获取玩家输入
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveInput.magnitude > 0)
        {
            // 归一化方向向量
            Vector3 moveDirection = moveInput.normalized;

            if (!isBlocked)
            {
                // 直接设置刚体的速度
                rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            }
            else
            {
                // 如果被阻挡，仅停止水平移动，保留垂直速度
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            // 无论是否阻挡，都允许转向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // 停止输入时立即停止水平速度，但保持垂直速度
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero; // 停止角速度
        }
    }






    // 检测 Test 与 Door 或 Wall 的接触
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            isBlocked = true;
            Debug.Log($"Blocked by: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            isBlocked = false;
            Debug.Log($"Unblocked: {other.name}");
        }
    }



    private float previousYPosition; // 记录上一帧的 Y 位置

    private void HandleSplashEffect()
    {
        if (splashEffect == null) return;

        // 更新粒子系统位置为鱼的位置
        splashEffect.transform.position = transform.position;

        // 检查是否在跳跃状态并且鱼的 Y 坐标大于 -1
        if (isJumping && transform.position.y > -1)
        {
            if (!splashEffect.isPlaying) // 确保粒子效果没有重复播放
            {
                Debug.Log("Fish above water during jump, splash starts!");
                splashEffect.Play();
            }
        }
        // 如果鱼的 Y 坐标等于 -1 或处于下潜状态，停止粒子效果
        else if (transform.position.y <= -1 || isDiving)
        {
            if (splashEffect.isPlaying) // 确保粒子效果正在播放时停止
            {
                Debug.Log("Fish at or below water level, splash stops!");
                splashEffect.Stop();
            }
        }

        // 更新上一帧的 Y 位置
        previousYPosition = transform.position.y;
    }



    void HandleFishJump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isDiving && !isJumping)
            {
                isDiving = true;
                diveStartTime = Time.time;
                StartCoroutine(Dive());

                // 触发下潜粒子效果
                //particleManager?.PlayParticleEffect();
            }
        }
        else if (isDiving)
        {
            isDiving = false;
            isJumping = true;

            float diveProgress = Mathf.Clamp01((Time.time - diveStartTime) / diveDuration);
            float jumpHeight = Mathf.Lerp(diveYPosition, jumpYPosition, diveProgress);

            currentJumpSpeed = initialJumpSpeed;
            StartCoroutine(Jump(jumpHeight));

            // 触发出水粒子效果
            //particleManager?.PlayParticleEffect();
        }
    }

    private IEnumerator Dive()
    {
        KeepY = false;
        float startY = transform.position.y;

        while (isDiving && Time.time - diveStartTime < diveDuration)
        {
            float t = (Time.time - diveStartTime) / diveDuration;
            currentYPosition = Mathf.Lerp(startY, diveYPosition, t);
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
            yield return null;
        }

        if (isDiving)
        {
            currentYPosition = diveYPosition;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
        }
    }

    private IEnumerator Jump(float targetHeight)
    {
        float startY = transform.position.y;

        // 上升阶段
        while (transform.position.y < targetHeight && currentJumpSpeed > 0)
        {
            currentYPosition += currentJumpSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
            currentJumpSpeed -= gravity * Time.deltaTime;
            yield return null;
        }

        // 确保到达 targetHeight
        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

        // 下落阶段
        float currentFallSpeed = 0f;
        while (transform.position.y > defaultYPosition)
        {
            currentFallSpeed += gravity * Time.deltaTime;
            currentFallSpeed = Mathf.Clamp(currentFallSpeed, 0, maxFallSpeed);
            currentYPosition -= currentFallSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);

            if (transform.position.y <= defaultYPosition)
            {
                transform.position = new Vector3(transform.position.x, defaultYPosition, transform.position.z);

                // 停止播放粒子效果
                //if (splashEffect != null && splashEffect.isPlaying)
                //{
                //    splashEffect.Stop();
                //}

                break;
            }

            yield return null;
        }

        currentYPosition = defaultYPosition;
        isJumping = false;
        KeepY = true;
    }
}