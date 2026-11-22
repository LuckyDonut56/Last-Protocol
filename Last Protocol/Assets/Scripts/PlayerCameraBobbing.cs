using UnityEngine;

public class PlayerCameraBobbing : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float amplitude = 0.015f;
    [SerializeField] private float frequency = 10f;
    [SerializeField] private float runMultiplier = 1.4f;
    [SerializeField] private float toggleSpeed = 1.5f;
    [SerializeField] private float runThreshold = 6f;
    [SerializeField] private float returnSpeed = 8f;

    [SerializeField] private Transform playerCamera;

    private CharacterController characterController;
    private PlayerController playerController;
    private Vector3 originalPos;
    private Vector3 lastPos;
    private float currentSpeed;
    private float phase;
    private float currentFreq;
    private float currentAmp;
    private float targetFreq;
    private float targetAmp;
    private bool isGrounded;

    private void Awake()
    {
        originalPos = playerCamera.localPosition;
        lastPos = transform.position;
        currentFreq = frequency;
        currentAmp = 0f;    
        targetFreq = frequency;
        targetAmp = 0f;

        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Vector3 delta = transform.position - lastPos;
        currentSpeed = delta.magnitude / Time.deltaTime;
        lastPos = transform.position;
    }

    private void LateUpdate()
    {
        isGrounded = GetGroundedState();

        if (isGrounded && currentSpeed > toggleSpeed)
        {
            bool running = currentSpeed > runThreshold;
            targetAmp = amplitude * (running ? runMultiplier : 1f);
            targetFreq = frequency * (running ? runMultiplier : 1f);
        }
        else
        {
            targetAmp = 0f;
            targetFreq = frequency;  
        }

        currentAmp = Mathf.Lerp(currentAmp, targetAmp, 5f * Time.deltaTime);
        currentFreq = Mathf.Lerp(currentFreq, targetFreq, 5f * Time.deltaTime);

        phase += currentFreq * Time.deltaTime;

        Vector3 motion = Vector3.zero;
        motion.y = Mathf.Sin(phase) * currentAmp;
        motion.x = Mathf.Cos(phase * 0.5f) * currentAmp * 1.5f;  

        playerCamera.localPosition = originalPos + motion;
    }

    private bool GetGroundedState()
    {
        if (playerController != null)
            return playerController.IsGrounded;
        if (characterController != null)
            return characterController.isGrounded;
        return false;
    }
}