using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 35f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isGrounded;

    private float standingCameraHeight;
    private float crouchCameraHeightOffset;
    public float crouchHeightRatio = 0.7f;
    public float crouchMoveSpeedMultiplier = 0.5f;
    private float currentMoveSpeed;
    private bool isCrouching = false;
    public float crouchTransitionSpeed = 5f;

    private float targetCameraHeight;
    private float verticalRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null) enabled = false;
        if (cameraTransform == null) enabled = false;
        standingCameraHeight = cameraTransform.localPosition.y;
        crouchCameraHeightOffset = standingCameraHeight * (1f - crouchHeightRatio);
        targetCameraHeight = standingCameraHeight;
        currentMoveSpeed = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        QuitToMainMenu();

        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalVelocity < 0) verticalVelocity = -2f;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * verticalInput + right * horizontalInput).normalized;
        moveDirection *= currentMoveSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded) verticalVelocity = jumpForce;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            targetCameraHeight = isCrouching ? standingCameraHeight - crouchCameraHeightOffset : standingCameraHeight;
            UpdateCharacterControllerHeight();
        }

        float currentCameraY = cameraTransform.localPosition.y;
        float newCameraY = Mathf.Lerp(currentCameraY, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newCameraY, cameraTransform.localPosition.z);

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        
        verticalRotation -= mouseY * rotationSpeed * Time.deltaTime;
        
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        
        cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    void UpdateCharacterControllerHeight()
    {
        if (isCrouching)
        {
            currentMoveSpeed = moveSpeed * crouchMoveSpeedMultiplier;
            characterController.height = 2f * crouchHeightRatio;
            characterController.center = new Vector3(0, characterController.height / 2f, 0);
        }
        else
        {
            currentMoveSpeed = moveSpeed;
            characterController.height = 2f;
            characterController.center = Vector3.zero;
        }
    }

    private void QuitToMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            SceneManager.LoadScene("MenuScene");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
       
    }
}
