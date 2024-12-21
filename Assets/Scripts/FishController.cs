using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishController : MonoBehaviour
{
    public int CollectedItemCount = 0; // �ռ���Ʒ����
    [SerializeField] float MoveSpeed;
    public float maxDistance = 1.44f; // ������������
    public float pullForce = 1f; // ��������ʩ�ӵ�����С
    private GameObject grabbedBall; // ��ǰҧס����
    public float obstacleDetectionDistance = 1f; // �ϰ��������
    public string obstacleTag = "Wall"; // ��ֹ�ƶ����ϰ����ǩ
    public bool isBlocked = false; // ��ʾ����Ƿ��赲
    public Slider yinYangSlider;

    public float speed = 5f;
    public float rotationSpeed = 100f;
    private float originalSpeed; // ����Ĭ���ٶ�ֵ

    public Light mainLight; // �����е�����Դ
    public LayerMask shadowCastingLayers; // ���ڼ����Ӱ��ͼ��
    public bool isInShadow = false; // ��ǰ�Ƿ�����Ӱ��
    public bool KeepY = true;// ����y��λ��
    public float yinYangValue = 0f;  // ����ֵ����Χ -1 �� 1
    public float size = 1f;         // �������
    public float shrinkRate = 0.1f; // ���ʱ��С������

    public Material fishMaterial;  // �����Ĳ���

    // ������ɫ
    public Color yinColor = Color.black; // ��Ӱ�������ɫ����ɫ��
    public Color yangColor = Color.white;  // ����������ɫ����ɫ��

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
    public float growthFactor = 0.1f;        // ��ÿ�α�����
    public float foodShrinkFactor = 0.5f;   // ʳ��ÿ�α�С����
    public float eatCooldown = 0.3f;        // ÿ�ν��� IsEating ״̬�ļ��ʱ��
    public string foodTag = "FishFood";     // ʳ��ı�ǩ

    public float shrinkSpeed = 0.001f; // ��С���͵��ٶ�
    public float minScale = 0.5f; // ��С��������

    private bool isEating = false;          // �Ƿ��ڳ�ʳ��
    private bool canEat = true;             // �Ƿ���Խ��� IsEating ״̬
    private void FixedUpdate()
    {
        HandleSplashEffect();
        HandleMovement();
        CheckAndMoveRedBall();
        HandleFishJump();
        //rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed);
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed;
        // �ҵ� "RedBall"

    }





    /// ��������ֵ��̬���������ɫ
    /// </summary>
    public float flashSpeed = 2f; // ��˸�ٶ�

    public void UpdateFishColor()
    {
        // �ж�����ֵ�Ƿ���߻����
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // ���浱ǰ��ɫ
            Color currentBaseColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);

            // ʹ��PingPong����ɫ�ڵ�ǰ��ɫ���ɫ֮����˸
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            fishMaterial.color = Color.Lerp(currentBaseColor, Color.red, t);
        }
        else
        {
            // ����������ɫ������˸��
            Color currentColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);
            fishMaterial.color = currentColor;
        }
    }





    void Start()
    {
        originalSpeed = speed;
        if (mainLight == null)
        {
            // �Զ���������Դ
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
    /// ���� IsEating ״̬
    /// </summary>
    private void EnterIsEatingState()
    {
        if (canEat)
        {
            isEating = true;
            StartCoroutine(EatCooldown()); // ��ʼ��ȴ��ʱ
        }
    }
    // ����һ�� LayerMask���ų� FishModel ͼ��



    void DetectShadow()
    {
        Vector3 lightDirection = -mainLight.transform.forward;
        Ray ray = new Ray(transform.position, lightDirection);

        // �ų� FishModel ��������߼��
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
    /// �ɿ����ʱ������ʳ���߼�
    /// </summary>
    private void TriggerEatFood()
    {
        if (isEating)
        {
            isEating = false; // ֹͣ�Ե�״̬

            // ���Ҹ���������
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 1f); // 1f Ϊ���뾶���ɵ���

            GameObject closestItem = null; // �����ʳ����ռ���Ʒ
            float closestDistance = float.MaxValue; // ��ʼ��Ϊһ���ܴ��ֵ

            foreach (Collider obj in nearbyObjects)
            {
                if (obj.CompareTag(foodTag) || obj.CompareTag("Collectible")) // ����Ƿ���ʳ����ռ���Ʒ
                {
                    float distance = Vector3.Distance(transform.position, obj.transform.position);

                    // �����ǰ����ȼ�¼�ĸ������������������
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = obj.gameObject;
                    }
                }
            }

            // ����ҵ�����������壬���н���
            if (closestItem != null)
            {
                if (closestItem.CompareTag("Collectible"))
                {
                    // ������ռ���Ʒ�����ٲ����Ӽ���
                    Destroy(closestItem);
                    CollectedItemCount++;
                    Debug.Log($"CollectedItemCount: {CollectedItemCount}");
                }
                else if (closestItem.CompareTag(foodTag))
                {
                    Seaweed seaweed = closestItem.GetComponent<Seaweed>();
                    if (seaweed != null)
                    {
                        // ���ˮ��������������������
                        if (seaweed.isRegrowing)
                        {
                            Debug.Log($"Skipped {closestItem.name} because it's regrowing.");
                            return;
                        }

                        // ����ˮ�ݵ� OnEaten ����������ˮ�ݵ������С�������߼�
                        seaweed.OnEaten();

                        // ����ˮ�ݵ�����Ч��������ҵ�����ֵ
                        UpdateYinYang(seaweed.yinYangEffect);

                        // ����ˮ�ݵ�Ӫ��ֵ�����������
                        transform.localScale += Vector3.one * seaweed.nutritionValue;
                    }
                    else
                    {
                        // �������ˮ�ݣ�������ͨʳ���߼�����
                        closestItem.transform.localScale *= foodShrinkFactor; // ʳ���С

                        // ����
                        transform.localScale += Vector3.one * growthFactor;

                        // ���ʳ��̫С����������
                        if (closestItem.transform.localScale.x < 0.1f)
                        {
                            Destroy(closestItem);
                        }
                    }
                }
            }
        }
    }



    // �����������ֵ�ķ���
    private void UpdateYinYang(float effect)
    {
        yinYangValue += effect; // ����ˮ�ݵ�Ч����������ֵ
        print("�ԣ�");
        yinYangValue = Mathf.Clamp(yinYangValue, -1f, 1f); // ȷ������ֵ�� -1 �� 1 ֮��
    }

    private void CheckAndMoveRedBall()
    {
        if (isEating && transform.localScale.x > 0.8f) // ȷ����ǰ���ڽ�ʳ״̬
        {
            // �������о��� "RedBall" ��ǩ�Ķ���
            GameObject[] redBalls = GameObject.FindGameObjectsWithTag("RedBall");

            foreach (GameObject redBall in redBalls)
            {
                float distance = Vector3.Distance(transform.position, redBall.transform.position);

                // ��������ڸ����������ƶ������λ��
                if (distance < 1f) // �ɸ�������������뷶Χ
                {

                    // ������ײ��
                    Collider redBallCollider = redBall.GetComponent<Collider>();
                    if (redBallCollider != null)
                    {
                        redBallCollider.enabled = false;
                    }

                    // �������λ������Ϊ���λ��
                    redBall.transform.position = EatPoint.transform.position;

                    // ���������ŵľ���
                    if (Door1 != null) // ȷ�� Door1 �ѷ���
                    {
                        float parentDistance = Vector3.Distance(Door1.transform.position, redBall.transform.position);

                        if (parentDistance > maxDistance) // ����������
                        {
                            Debug.Log("ʩ������");

                            Rigidbody doorRb = Door1.GetComponent<Rigidbody>();
                            if (doorRb != null)
                            {
                                // ������ȷ�����ط���ʩ����
                                Vector3 pullDirection = (EatPoint.transform.position - Door1.transform.position).normalized;
                                doorRb.AddForce(pullDirection * 15f, ForceMode.Force); // 10.0f �ɵ���Ϊ�������
                            }
                            else
                            {
                                Debug.LogWarning("Door1 û�� Rigidbody �����");
                            }
                        }

                        // �������λ��
                        float playerDistance = Vector3.Distance(Door1Point.transform.position, transform.position);
                        if (playerDistance > maxDistance)
                        {
                            Debug.Log("�������λ��");

                            Vector3 restrictedPosition = Door1Point.transform.position +
                                                          (transform.position - Door1Point.transform.position).normalized *
                                                          (maxDistance);

                            // ֻ�ڳ�����Χʱ����λ��
                            transform.position = Vector3.MoveTowards(transform.position, restrictedPosition,
                                                                     playerDistance - (maxDistance));

                        }

                    }
                }
            }
        }
        else
        {
            // ������ڽ�ʳ״̬������������ײ��
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
    /// �Ե���ȴʱ��
    /// </summary>
    private IEnumerator EatCooldown()
    {
        canEat = false;
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
    }
    void HandleShrink()
    {
        // �������ֵ�Ƿ�ﵽ��ֵ
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // ��ȡ��ǰ����
            Vector3 currentScale = transform.localScale;

            // ������ʹ�����С���ƣ�������С����
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
            speed = originalSpeed / 3f; // ���ٶȼ��ٵ�ԭ��������֮һ
        }
        else
        {
            speed = originalSpeed; // �ָ�Ĭ���ٶ�
        }
        GameObject redBall = GameObject.FindGameObjectWithTag("RedBall");
        
        HandleShrink();
        float normalizedValue = Mathf.InverseLerp(-1f, 1f, yinYangValue);
        yinYangSlider.value = normalizedValue; // ���»�������ֵ
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






        if (Input.GetMouseButton(0)) // ��ס���
        {
            EnterIsEatingState();
        }

        if (Input.GetMouseButtonUp(0)) // �ɿ����
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
            // ǿ����� Y ����̶��� 1
            Vector3 position = transform.position;
            position.y = -1f; // �̶��� y = 1
            transform.position = position;
        }
    }
    private void HandleMovement()
    {
        // ��ȡ�������
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveInput.magnitude > 0)
        {
            // ��һ����������
            Vector3 moveDirection = moveInput.normalized;

            if (!isBlocked)
            {
                // ֱ�����ø�����ٶ�
                rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            }
            else
            {
                // ������赲����ֹͣˮƽ�ƶ���������ֱ�ٶ�
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            // �����Ƿ��赲��������ת��
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // ֹͣ����ʱ����ֹͣˮƽ�ٶȣ������ִ�ֱ�ٶ�
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero; // ֹͣ���ٶ�
        }
    }






    // ��� Test �� Door �� Wall �ĽӴ�
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



    private float previousYPosition; // ��¼��һ֡�� Y λ��

    private void HandleSplashEffect()
    {
        if (splashEffect == null) return;

        // ��������ϵͳλ��Ϊ���λ��
        splashEffect.transform.position = transform.position;

        // ����Ƿ�����Ծ״̬������� Y ������� -1
        if (isJumping && transform.position.y > -1)
        {
            if (!splashEffect.isPlaying) // ȷ������Ч��û���ظ�����
            {
                Debug.Log("Fish above water during jump, splash starts!");
                splashEffect.Play();
            }
        }
        // ������ Y ������� -1 ������Ǳ״̬��ֹͣ����Ч��
        else if (transform.position.y <= -1 || isDiving)
        {
            if (splashEffect.isPlaying) // ȷ������Ч�����ڲ���ʱֹͣ
            {
                Debug.Log("Fish at or below water level, splash stops!");
                splashEffect.Stop();
            }
        }

        // ������һ֡�� Y λ��
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

                // ������Ǳ����Ч��
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

            // ������ˮ����Ч��
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

        // �����׶�
        while (transform.position.y < targetHeight && currentJumpSpeed > 0)
        {
            currentYPosition += currentJumpSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
            currentJumpSpeed -= gravity * Time.deltaTime;
            yield return null;
        }

        // ȷ������ targetHeight
        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

        // ����׶�
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

                // ֹͣ��������Ч��
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