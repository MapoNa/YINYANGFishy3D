using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishController : MonoBehaviour
{
    public int CollectedItemCount = 0; // The count of collected items
    public float maxDistance = 1.44f; // Maximum distance for pulling interaction
    public float pullForce = 1f; // Force applied to the parent object
    private GameObject grabbedBall; // The currently grabbed ball
    public float obstacleDetectionDistance = 1f; // Distance to detect obstacles
    public string obstacleTag = "Wall"; // Tag for obstacles that block movement
    public bool isBlocked = false; // Whether the player is currently blocked
    public Slider yinYangSlider; // Slider to display the Yin-Yang value

    public float speed = 5f; // Movement speed
    public float flashSpeed = 2f; // Flash speed for the fish material
    public float rotationSpeed = 100f; // Rotation speed for the fish
    private float originalSpeed; // Backup of the original movement speed

    public Light mainLight; // Main light source in the scene
    public LayerMask shadowCastingLayers; // Layers used to detect shadows
    public bool isInShadow = false; // Whether the fish is currently in shadow
    public bool KeepY = true; // Keeps the fish's Y position constant
    public float yinYangValue = 0f; // Yin-Yang value of the fish, range -1 to 1
    public float size = 1f; // Size of the fish
    public float shrinkRate = 0.1f; // Rate at which the fish shrinks

    public Material fishMaterial; // Material applied to the fish

    // Colors representing Yin and Yang
    public Color yinColor = Color.black; // Color for Yin (shadow)
    public Color yangColor = Color.white; // Color for Yang (light)

    private Rigidbody rb; // Rigidbody for controlling the fish's physics
    public ParticleSystem splashEffect; // Particle effect for water splash
    public Camera interactionCam; // Camera for interaction view
    public GameObject FishNotEating; // Model for the fish when not eating
    public GameObject FishEating; // Model for the fish when eating
    public GameObject EatPoint; // Point where food is grabbed
    public GameObject Door1; // Reference to the interactable door
    public GameObject Door1Point; // Reference to the door's interaction point

    public float defaultYPosition = -1f; // Default Y position of the fish
    public float diveYPosition = -1.5f; // Target Y position when diving
    public float jumpYPosition = 3f; // Target Y position when jumping
    public float diveDuration = 0.5f; // Duration of the dive
    public float initialJumpSpeed = 15f; // Initial speed of the jump
    public float gravity = 5f; // Gravity effect during jump and fall
    public float maxFallSpeed = 12f; // Maximum speed when falling

    private bool isDiving = false; // Whether the fish is currently diving
    private bool isJumping = false; // Whether the fish is currently jumping
    private float currentYPosition; // Current Y position of the fish
    private float diveStartTime; // Time when the dive started
    private float currentJumpSpeed; // Current speed during the jump
    private float currentFallSpeed; // Current speed during the fall

    public float growthFactor = 0.15f; // How much the fish grows when eating
    public float foodShrinkFactor = 0.5f; // How much the food shrinks when eaten
    public float eatCooldown = 0.3f; // Cooldown between eating actions
    public string foodTag = "FishFood"; // Tag for food objects

    public float shrinkSpeed = 0.001f; // Rate at which the fish shrinks
    public float minScale = 0.5f; // Minimum scale of the fish

    public bool isEating = false; // Whether the fish is currently eating
    private bool canEat = true; // Whether the fish can eat (used for cooldown)

    public AudioSource EatAudioSource; // Audio source for eating sounds

    private void FixedUpdate()
    {
        HandleSplashEffect();
        HandleMovement();
        CheckAndMoveRedBall();
        HandleFishJump();
        //rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed);
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed;
    }


    /// </summary>
    public void UpdateFishColor()
    {
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            Color currentBaseColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);


            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            fishMaterial.color = Color.Lerp(currentBaseColor, Color.red, t);
        }
        else
        {
            Color currentColor = Color.Lerp(yangColor, yinColor, (yinYangValue + 1f) / 2f);
            fishMaterial.color = currentColor;
        }
    }


    void Start()
    {
        originalSpeed = speed;
        if (mainLight == null)
        {
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
    /// </summary>
    private void EnterIsEatingState()
    {
        if (canEat)
        {
            isEating = true;
            StartCoroutine(EatCooldown());
        }
    }


    void DetectShadow()
    {
        Vector3 lightDirection = -mainLight.transform.forward;
        Ray ray = new Ray(transform.position, lightDirection);


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
    /// </summary>
    private void TriggerEatFood()
    {
        if (isEating)
        {
            isEating = false;
            var coliders = Physics.OverlapSphere(transform.position, 1f);

            GameObject closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (Collider obj in coliders)
            {
                if (obj.CompareTag(foodTag) || obj.CompareTag("Collectible"))
                {
                    Debug.Log("Eating " + obj.name);
                    float distance = Vector3.Distance(transform.position, obj.transform.position);


                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = obj.gameObject;
                    }
                }
            }


            if (closestItem != null)
            {
                if (closestItem.CompareTag("Collectible"))
                {
                    Destroy(closestItem);
                    CollectedItemCount++;
                    Debug.Log($"CollectedItemCount: {CollectedItemCount}");
                }
                else if (closestItem.CompareTag(foodTag))
                {
                    Seaweed seaweed = closestItem.GetComponent<Seaweed>();
                    if (seaweed != null)
                    {
                        if (seaweed.isRegrowing)
                        {
                            return;
                        }


                        seaweed.OnEaten();

                        UpdateYinYang(seaweed.yinYangEffect);

                        transform.localScale += Vector3.one * seaweed.nutritionValue;
                    }
                    else
                    {
                        closestItem.transform.localScale *= foodShrinkFactor;


                        transform.localScale += Vector3.one * growthFactor;


                        if (closestItem.transform.localScale.x < 0.1f)
                        {
                            Destroy(closestItem);
                        }
                    }
                }
            }
        }
    }


    private void UpdateYinYang(float effect)
    {
        yinYangValue += effect;
        yinYangValue = Mathf.Clamp(yinYangValue, -1f, 1f);
    }

    private void CheckAndMoveRedBall()
    {
        if (isEating && transform.localScale.x > 0.75f)
        {
            GameObject[] redBalls = GameObject.FindGameObjectsWithTag("RedBall");

            foreach (GameObject redBall in redBalls)
            {
                float distance = Vector3.Distance(transform.position, redBall.transform.position);


                if (distance < 0.5f)
                {
                    Collider redBallCollider = redBall.GetComponent<Collider>();
                    if (redBallCollider != null)
                    {
                        redBallCollider.enabled = false;
                    }


                    redBall.transform.position = EatPoint.transform.position;


                    if (Door1 != null)
                    {
                        float parentDistance = Vector3.Distance(Door1.transform.position, redBall.transform.position);

                        if (parentDistance > maxDistance)
                        {
                            Rigidbody doorRb = Door1.GetComponent<Rigidbody>();
                            if (doorRb != null)
                            {
                                Vector3 pullDirection =
                                    (EatPoint.transform.position - Door1.transform.position).normalized;
                                doorRb.AddForce(pullDirection * 30f, ForceMode.Force);
                            }
                            else
                            {
                            }
                        }


                        float playerDistance = Vector3.Distance(Door1Point.transform.position, transform.position);
                        if (playerDistance > maxDistance)
                        {
                            Vector3 restrictedPosition = Door1Point.transform.position +
                                                         (transform.position - Door1Point.transform.position)
                                                        .normalized *
                                                         (maxDistance);

                            transform.position = Vector3.MoveTowards(transform.position, restrictedPosition,
                                                                     playerDistance - (maxDistance));
                        }
                    }
                }
            }
        }
        else
        {
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
    /// </summary>
    private IEnumerator EatCooldown()
    {
        canEat = false;
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
    }

    void HandleShrink()
    {
        if (Mathf.Abs(yinYangValue) >= 1f)
        {
            Vector3 currentScale = transform.localScale;


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
        //if (KeepY == false)
        //{
        //    speed = originalSpeed / 3f; 
        //}
        //else
        //{
        //    speed = originalSpeed; 
        //}

        GameObject redBall = GameObject.FindGameObjectWithTag("RedBall");

        HandleShrink();
        float normalizedValue = Mathf.InverseLerp(-1f, 1f, yinYangValue);
        yinYangSlider.value = normalizedValue;
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


        if (Input.GetMouseButton(0))
        {
            EnterIsEatingState();
        }

        if (Input.GetMouseButtonUp(0))
        {
            TriggerEatFood();
            if (Time.timeScale > 0.1f)
            {
                EatAudioSource.PlayOneShot(EatAudioSource.clip);
            }
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
            Vector3 position = transform.position;
            position.y = -1f;
            transform.position = position;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveInput.magnitude > 0)
        {
            Vector3 moveDirection = moveInput.normalized;

            if (!isBlocked)
            {
                rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }


            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }
    }


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


    private float previousYPosition;

    private void HandleSplashEffect()
    {
        if (splashEffect == null) return;


        splashEffect.transform.position = transform.position;


        if (isJumping && transform.position.y > -1)
        {
            if (!splashEffect.isPlaying)
            {
                Debug.Log("Fish above water during jump, splash starts!");
                splashEffect.Play();
            }
        }

        else if (transform.position.y <= -1 || isDiving)
        {
            if (splashEffect.isPlaying)
            {
                Debug.Log("Fish at or below water level, splash stops!");
                splashEffect.Stop();
            }
        }

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

        while (transform.position.y < targetHeight && currentJumpSpeed > 0)
        {
            currentYPosition += currentJumpSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
            currentJumpSpeed -= gravity * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

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