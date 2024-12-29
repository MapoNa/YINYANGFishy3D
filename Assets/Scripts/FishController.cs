using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishController : MonoBehaviour
{
    public int CollectedItemCount = 0; // ÊÕ¼¯ÎïÆ·¼ÆÊý
    public float maxDistance = 1.44f; // ÉþË÷µÄ×î´ó¾àÀë
    public float pullForce = 1f; // ¸ø¸¸ÎïÌåÊ©¼ÓµÄÁ¦´óÐ¡
    private GameObject grabbedBall; // µ±Ç°Ò§×¡µÄÇò
    public float obstacleDetectionDistance = 1f; // ÕÏ°­Îï¼ì²â¾àÀë
    public string obstacleTag = "Wall"; // ×èÖ¹ÒÆ¶¯µÄÕÏ°­Îï±êÇ©
    public bool isBlocked = false; // ±íÊ¾Íæ¼ÒÊÇ·ñ±»×èµ²
    public Slider yinYangSlider;

    public float speed = 5f;
    public float flashSpeed = 2f; // ÉÁË¸ËÙ¶È
    public float rotationSpeed = 100f;
    private float originalSpeed; // ±£´æÄ¬ÈÏËÙ¶ÈÖµ

    public Light mainLight; // ³¡¾°ÖÐµÄÖ÷¹âÔ´
    public LayerMask shadowCastingLayers; // ÓÃÓÚ¼ì²âÒõÓ°µÄÍ¼²ã
    public bool isInShadow = false; // µ±Ç°ÊÇ·ñÔÚÒõÓ°ÖÐ
    public bool KeepY = true; // ±£³ÖyµÄÎ»ÖÃ
    public float yinYangValue = 0f; // ÒõÑôÖµ£º·¶Î§ -1 µ½ 1
    public float size = 1f; // ÓãµÄÌåÐÍ
    public float shrinkRate = 0.1f; // Òç³öÊ±ËõÐ¡µÄËÙÂÊ

    public Material fishMaterial; // Íæ¼ÒÓãµÄ²ÄÖÊ

    // ÒõÑôÑÕÉ«
    public Color yinColor = Color.black; // ÒõÓ°´ú±íµÄÑÕÉ«£¨ºÚÉ«£©
    public Color yangColor = Color.white; // Ñô¹â´ú±íµÄÑÕÉ«£¨ºìÉ«£©

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
    public float growthFactor = 0.1f; // ÓãÃ¿´Î±ä´óµÄÁ¿
    public float foodShrinkFactor = 0.5f; // Ê³ÎïÃ¿´Î±äÐ¡µÄÁ¿
    public float eatCooldown = 0.3f; // Ã¿´Î½øÈë IsEating ×´Ì¬µÄ¼ä¸ôÊ±¼ä
    public string foodTag = "FishFood"; // Ê³ÎïµÄ±êÇ©

    public float shrinkSpeed = 0.001f; // ¼õÐ¡ÌåÐÍµÄËÙ¶È
    public float minScale = 0.5f; // ×îÐ¡ÌåÐÍÏÞÖÆ

    public bool isEating = false; // ÊÇ·ñÔÚ³ÔÊ³Îï

    private bool canEat = true;

    public AudioSource EatAudioSource;

    private void FixedUpdate()
    {
        HandleSplashEffect();
        HandleMovement();
        CheckAndMoveRedBall();
        HandleFishJump();
        //rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed);
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed;
        // ÕÒµ½ "RedBall"
    }


    /// ¸ù¾ÝÒõÑôÖµ¶¯Ì¬¸üÐÂÓãµÄÑÕÉ«
    /// </summary>
    public void UpdateFishColor()
    {
        // ÅÐ¶ÏÒõÑôÖµÊÇ·ñ¹ý¸ß»ò¹ýµÍ
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // ±£´æµ±Ç°ÑÕÉ«
            Color currentBaseColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);

            // Ê¹ÓÃPingPongÈÃÑÕÉ«ÔÚµ±Ç°ÑÕÉ«ÓëºìÉ«Ö®¼äÉÁË¸
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            fishMaterial.color = Color.Lerp(currentBaseColor, Color.red, t);
        }
        else
        {
            // Õý³£¸üÐÂÑÕÉ«£¨²»ÉÁË¸£©
            Color currentColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);
            fishMaterial.color = currentColor;
        }
    }


    void Start()
    {
        originalSpeed = speed;
        if (mainLight == null)
        {
            // ×Ô¶¯²éÕÒÖ÷¹âÔ´
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
    /// ½øÈë IsEating ×´Ì¬
    /// </summary>
    private void EnterIsEatingState()
    {
        if (canEat)
        {
            isEating = true;
            StartCoroutine(EatCooldown()); // ¿ªÊ¼ÀäÈ´¼ÆÊ±
        }
    }
    // ¶¨ÒåÒ»¸ö LayerMask£¬ÅÅ³ý FishModel Í¼²ã


    void DetectShadow()
    {
        Vector3 lightDirection = -mainLight.transform.forward;
        Ray ray = new Ray(transform.position, lightDirection);

        // ÅÅ³ý FishModel ²ã½øÐÐÉäÏß¼ì²â
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
    /// ËÉ¿ª×ó¼üÊ±´¥·¢³ÔÊ³ÎïÂß¼­
    /// </summary>
    private void TriggerEatFood()
    {
        if (isEating)
        {
            isEating = false; // Í£Ö¹³ÔµÄ×´Ì¬
            // ²éÕÒ¸½½üµÄÎïÌå
            var coliders = Physics.OverlapSphere(transform.position, 1f); // 1f Îª¼ì²â°ë¾¶£¬¿Éµ÷Õû

            GameObject closestItem = null; // ×î½üµÄÊ³Îï»òÊÕ¼¯ÎïÆ·
            float closestDistance = float.MaxValue; // ³õÊ¼»¯ÎªÒ»¸öºÜ´óµÄÖµ

            foreach (Collider obj in coliders)
            {
                if (obj.CompareTag(foodTag) || obj.CompareTag("Collectible")) // ¼ì²éÊÇ·ñÊÇÊ³Îï»òÊÕ¼¯ÎïÆ·
                {
                    Debug.Log("Eating " + obj.name);
                    float distance = Vector3.Distance(transform.position, obj.transform.position);

                    // Èç¹ûµ±Ç°ÎïÌå±È¼ÇÂ¼µÄ¸ü½ü£¬¸üÐÂ×î½üµÄÎïÌå
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = obj.gameObject;
                    }
                }
            }

            // Èç¹ûÕÒµ½ÁË×î½üµÄÎïÌå£¬½øÐÐ½»»¥
            if (closestItem != null)
            {
                if (closestItem.CompareTag("Collectible"))
                {
                    // Èç¹ûÊÇÊÕ¼¯ÎïÆ·£¬Ïú»Ù²¢Ôö¼Ó¼ÆÊý
                    Destroy(closestItem);
                    CollectedItemCount++;
                    Debug.Log($"CollectedItemCount: {CollectedItemCount}");
                }
                else if (closestItem.CompareTag(foodTag))
                {
                    Seaweed seaweed = closestItem.GetComponent<Seaweed>();
                    if (seaweed != null)
                    {
                        // Èç¹ûË®²ÝÕýÔÚÖØÉú£¬Ìø¹ý´¦Àí
                        if (seaweed.isRegrowing)
                        {
                            Debug.Log($"Skipped {closestItem.name} because it's regrowing.");
                            return;
                        }

                        // µ÷ÓÃË®²ÝµÄ OnEaten ·½·¨£¬´¦ÀíË®²ÝµÄÌå»ýËõÐ¡ºÍÉú³¤Âß¼­
                        seaweed.OnEaten();

                        // ¸ù¾ÝË®²ÝµÄÒõÑôÐ§¹û¸üÐÂÍæ¼ÒµÄÒõÑôÖµ
                        UpdateYinYang(seaweed.yinYangEffect);

                        // ¸ù¾ÝË®²ÝµÄÓªÑøÖµÔö¼ÓÍæ¼ÒÌåÐÍ
                        transform.localScale += Vector3.one * seaweed.nutritionValue;
                    }
                    else
                    {
                        // Èç¹û²»ÊÇË®²Ý£¬°´ÕÕÆÕÍ¨Ê³ÎïÂß¼­´¦Àí
                        closestItem.transform.localScale *= foodShrinkFactor; // Ê³Îï±äÐ¡

                        // Óã±ä´ó
                        transform.localScale += Vector3.one * growthFactor;

                        // Èç¹ûÊ³ÎïÌ«Ð¡£¬ÔòÏú»ÙËü
                        if (closestItem.transform.localScale.x < 0.1f)
                        {
                            Destroy(closestItem);
                        }
                    }
                }
            }
        }
    }


    // ¸üÐÂÍæ¼ÒÒõÑôÖµµÄ·½·¨
    private void UpdateYinYang(float effect)
    {
        yinYangValue += effect; // ¸ù¾ÝË®²ÝµÄÐ§¹ûµ÷ÕûÒõÑôÖµ
        print("³Ô£¡");
        yinYangValue = Mathf.Clamp(yinYangValue, -1f, 1f); // È·±£ÒõÑôÖµÔÚ -1 µ½ 1 Ö®¼ä
    }

    private void CheckAndMoveRedBall()
    {
        if (isEating && transform.localScale.x > 0.8f) // È·±£µ±Ç°´¦ÓÚ½øÊ³×´Ì¬
        {
            // ²éÕÒËùÓÐ¾ßÓÐ "RedBall" ±êÇ©µÄ¶ÔÏó
            GameObject[] redBalls = GameObject.FindGameObjectsWithTag("RedBall");

            foreach (GameObject redBall in redBalls)
            {
                float distance = Vector3.Distance(transform.position, redBall.transform.position);

                // Èç¹ûºìÇòÔÚ¸½½ü£¬½«ÆäÒÆ¶¯µ½ÓãµÄÎ»ÖÃ
                if (distance < 1f) // ¿É¸ù¾ÝÐèÇóµ÷Õû¾àÀë·¶Î§
                {
                    // ½ûÓÃÅö×²Æ÷
                    Collider redBallCollider = redBall.GetComponent<Collider>();
                    if (redBallCollider != null)
                    {
                        redBallCollider.enabled = false;
                    }

                    // ½«ºìÇòµÄÎ»ÖÃÉèÖÃÎªÓãµÄÎ»ÖÃ
                    redBall.transform.position = EatPoint.transform.position;

                    // ¼ì²éºìÇòÓëÃÅµÄ¾àÀë
                    if (Door1 != null) // È·±£ Door1 ÒÑ·ÖÅä
                    {
                        float parentDistance = Vector3.Distance(Door1.transform.position, redBall.transform.position);

                        if (parentDistance > maxDistance) // ³¬¹ý×î´ó¾àÀë
                        {
                            Debug.Log("Ê©¼ÓÀ­Á¦");

                            Rigidbody doorRb = Door1.GetComponent<Rigidbody>();
                            if (doorRb != null)
                            {
                                // ¼ÆËãÕýÈ·µÄÀ­»Ø·½Ïò²¢Ê©¼ÓÁ¦
                                Vector3 pullDirection =
                                    (EatPoint.transform.position - Door1.transform.position).normalized;
                                doorRb.AddForce(pullDirection * 15f, ForceMode.Force); // 10.0f ¿Éµ÷ÕûÎªËùÐèµÄÁ¦
                            }
                            else
                            {
                                Debug.LogWarning("Door1 Ã»ÓÐ Rigidbody ×é¼þ£¡");
                            }
                        }

                        // ÏÞÖÆÍæ¼ÒÎ»ÖÃ
                        float playerDistance = Vector3.Distance(Door1Point.transform.position, transform.position);
                        if (playerDistance > maxDistance)
                        {
                            Debug.Log("ÏÞÖÆÍæ¼ÒÎ»ÖÃ");

                            Vector3 restrictedPosition = Door1Point.transform.position +
                                                         (transform.position - Door1Point.transform.position)
                                                        .normalized *
                                                         (maxDistance);

                            // Ö»ÔÚ³¬¹ý·¶Î§Ê±ÐÞÕýÎ»ÖÃ
                            transform.position = Vector3.MoveTowards(transform.position, restrictedPosition,
                                                                     playerDistance - (maxDistance));
                        }
                    }
                }
            }
        }
        else
        {
            // Èç¹û²»ÔÚ½øÊ³×´Ì¬£¬ÖØÐÂÆôÓÃÅö×²Æ÷
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
    /// ³ÔµÄÀäÈ´Ê±¼ä
    /// </summary>
    private IEnumerator EatCooldown()
    {
        canEat = false;
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
    }

    void HandleShrink()
    {
        // ¼ì²éÒõÑôÖµÊÇ·ñ´ïµ½ãÐÖµ
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            // »ñÈ¡µ±Ç°Ëõ·Å
            Vector3 currentScale = transform.localScale;

            // Èç¹ûÌåÐÍ´óÓÚ×îÐ¡ÏÞÖÆ£¬ÔòÖð½¥ËõÐ¡ÌåÐÍ
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
            speed = originalSpeed / 3f; // ½«ËÙ¶È¼õÉÙµ½Ô­À´µÄÈý·ÖÖ®Ò»
        }
        else
        {
            speed = originalSpeed; // »Ö¸´Ä¬ÈÏËÙ¶È
        }

        GameObject redBall = GameObject.FindGameObjectWithTag("RedBall");

        HandleShrink();
        float normalizedValue = Mathf.InverseLerp(-1f, 1f, yinYangValue);
        yinYangSlider.value = normalizedValue; // ¸üÐÂ»¬¶¯ÌõµÄÖµ
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


        if (Input.GetMouseButton(0)) // °´×¡×ó¼ü
        {
            EnterIsEatingState();
        }

        if (Input.GetMouseButtonUp(0)) // ËÉ¿ª×ó¼ü
        {
            TriggerEatFood();
            EatAudioSource.PlayOneShot(EatAudioSource.clip);
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
            // Ç¿ÖÆÍæ¼Ò Y ×ø±ê¹Ì¶¨ÔÚ 1
            Vector3 position = transform.position;
            position.y = -1f; // ¹Ì¶¨µ½ y = 1
            transform.position = position;
        }
    }

    private void HandleMovement()
    {
        // »ñÈ¡Íæ¼ÒÊäÈë
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveInput.magnitude > 0)
        {
            // ¹éÒ»»¯·½ÏòÏòÁ¿
            Vector3 moveDirection = moveInput.normalized;

            if (!isBlocked)
            {
                // Ö±½ÓÉèÖÃ¸ÕÌåµÄËÙ¶È
                rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            }
            else
            {
                // Èç¹û±»×èµ²£¬½öÍ£Ö¹Ë®Æ½ÒÆ¶¯£¬±£Áô´¹Ö±ËÙ¶È
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            // ÎÞÂÛÊÇ·ñ×èµ²£¬¶¼ÔÊÐí×ªÏò
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Í£Ö¹ÊäÈëÊ±Á¢¼´Í£Ö¹Ë®Æ½ËÙ¶È£¬µ«±£³Ö´¹Ö±ËÙ¶È
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero; // Í£Ö¹½ÇËÙ¶È
        }
    }


    // ¼ì²â Test Óë Door »ò Wall µÄ½Ó´¥
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


    private float previousYPosition; // ¼ÇÂ¼ÉÏÒ»Ö¡µÄ Y Î»ÖÃ

    private void HandleSplashEffect()
    {
        if (splashEffect == null) return;

        // ¸üÐÂÁ£×ÓÏµÍ³Î»ÖÃÎªÓãµÄÎ»ÖÃ
        splashEffect.transform.position = transform.position;

        // ¼ì²éÊÇ·ñÔÚÌøÔ¾×´Ì¬²¢ÇÒÓãµÄ Y ×ø±ê´óÓÚ -1
        if (isJumping && transform.position.y > -1)
        {
            if (!splashEffect.isPlaying) // È·±£Á£×ÓÐ§¹ûÃ»ÓÐÖØ¸´²¥·Å
            {
                Debug.Log("Fish above water during jump, splash starts!");
                splashEffect.Play();
            }
        }
        // Èç¹ûÓãµÄ Y ×ø±êµÈÓÚ -1 »ò´¦ÓÚÏÂÇ±×´Ì¬£¬Í£Ö¹Á£×ÓÐ§¹û
        else if (transform.position.y <= -1 || isDiving)
        {
            if (splashEffect.isPlaying) // È·±£Á£×ÓÐ§¹ûÕýÔÚ²¥·ÅÊ±Í£Ö¹
            {
                Debug.Log("Fish at or below water level, splash stops!");
                splashEffect.Stop();
            }
        }

        // ¸üÐÂÉÏÒ»Ö¡µÄ Y Î»ÖÃ
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

                // ´¥·¢ÏÂÇ±Á£×ÓÐ§¹û
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

            // ´¥·¢³öË®Á£×ÓÐ§¹û
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

        // ÉÏÉý½×¶Î
        while (transform.position.y < targetHeight && currentJumpSpeed > 0)
        {
            currentYPosition += currentJumpSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
            currentJumpSpeed -= gravity * Time.deltaTime;
            yield return null;
        }

        // È·±£µ½´ï targetHeight
        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

        // ÏÂÂä½×¶Î
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

                // Í£Ö¹²¥·ÅÁ£×ÓÐ§¹û
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