using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource stepsAudioSource;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.25f;

    [Header("Components")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float stepTimer = 0f;
    private bool wasMoving = false;
    private float xRotation = 0f;

    private float inputHorizontal;
    private float inputVertical;
    private bool inputRun;
    private bool inputJumpPressed;

    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ReadInput();
        UpdateGroundedState();

        HandleMouseLook();
        HandleJump();
        HandleMovement();
        HandleStepSounds();
    }

    void ReadInput()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        inputRun = Input.GetKey(KeyCode.LeftShift);
        inputJumpPressed = Input.GetButtonDown("Jump");
    }

    void UpdateGroundedState()
    {
        isGrounded = characterController.isGrounded;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 moveDirection = transform.right * inputHorizontal + transform.forward * inputVertical;

        float currentSpeed = inputRun && isGrounded ? runSpeed : walkSpeed;

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (inputJumpPressed && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    void HandleStepSounds()
    {
        bool isMoving = (Mathf.Abs(inputHorizontal) > 0.1f || Mathf.Abs(inputVertical) > 0.1f) && isGrounded;
        bool isRunning = inputRun && isGrounded;

        float currentInterval = isRunning ? runStepInterval : stepInterval;
        if (isMoving && !wasMoving)
        {
            PlayStepSound();
            stepTimer = 0f;
        }
        else if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= currentInterval)
            {
                PlayStepSound();
                stepTimer = 0f;
            }
        }
        else
            stepTimer = 0f;

        wasMoving = isMoving;
    }


    void PlayStepSound()
    {
        if (stepSounds == null || stepSounds.Length == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, stepSounds.Length);
        AudioClip selectedClip = stepSounds[randomIndex];

        if (selectedClip != null && stepsAudioSource != null)
        {
            stepsAudioSource.clip = selectedClip;
            stepsAudioSource.time = 0f;
            stepsAudioSource.Play();
        }
    }
}