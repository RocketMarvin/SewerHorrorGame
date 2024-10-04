using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SimplePlayerControllerRework : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;
    [Space]
    public Camera playerCamera;
    public float standingHeight_Cam = 0.2f, crouchingHeight_Cam = -0.85f;
    public float walkSpeed = 1.15f;
    public float runSpeed = 4.0f;
    public static float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float crouchSpeed = 0.66f;
    public float gravity = 1500f;
    public float crouchSmooth_Speed = 0.5f;

    // Voeg een extra referentie toe voor de trigger collider
    public CapsuleCollider playerCollider;
    public CapsuleCollider triggerCollider;

    Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    public CharacterController characterController;
    public static bool canMove = true;
    [SerializeField] private bool standingStil = true;
    private bool isRunning = false;
    [Space]
    [SerializeField] private float stamina = 5;
    [SerializeField] private Image staminaBar, staminaBar2;
    [SerializeField] private GameObject StaminaBar, StaminaBar2;
    [SerializeField] private GameObject StaminaBackGround;
    [Space]
    [SerializeField] private bool isOutOfStamina = false, isCrouching = false;
    private bool canRun = false;
    private bool StaminaRunningCheck = false, playSound = false, smoothCrouch = false;
    public bool inMenu = false;
    public AudioSource outOfBreath;

    void Start()
    {
        staminaBar.color = Color.white;
        staminaBar2.color = Color.white;
        playSound = false;
        cameraAnimator.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isOutOfStamina = false;
        isCrouching = false;
        smoothCrouch = false;

        // Controleer of de trigger collider is toegewezen
        if (triggerCollider == null)
        {
            Debug.LogError("Trigger collider is niet toegewezen!");
        }
    }

    void Update()
    {
        if (canMove)
        {
            PlayerMovement();
            if (runSpeed <= 2.08f) CrouchBehaviour();
        }
        StaminaBehaviour();
        CameraBehaviour();
    }

    private void PlayerMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        if (!isOutOfStamina && !isCrouching) isRunning = Input.GetKey(KeyCode.LeftShift);
        else if (isOutOfStamina && isCrouching) isRunning = false;
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (isOutOfStamina == true && canRun) playSound = true;
        else if (!isOutOfStamina) outOfBreath.Stop();

        if (playSound)
        {
            staminaBar.color = Color.red;
            staminaBar2.color = Color.red;
            outOfBreath.Play();
            canRun = false;
            playSound = false;
        }

        if (!standingStil && !isCrouching)
        {
            if (StaminaRunningCheck)
            {
                cameraAnimator.SetBool("isWalking", false);
                cameraAnimator.SetBool("isRunning", true);
            }
            else
            {
                if (runSpeed < 2.08f)
                {
                    cameraAnimator.SetBool("isWalking", true);
                    cameraAnimator.SetBool("isRunning", false);
                }
            }
        }

        if (Input.GetAxis("Vertical") <= 0 && -Input.GetAxis("Vertical") <= 0)
        {
            if (Input.GetAxis("Horizontal") <= 0 && -Input.GetAxis("Horizontal") <= 0)
            {
                cameraAnimator.SetBool("isWalking", false);
                cameraAnimator.SetBool("isRunning", false);
                standingStil = true;
                canRun = false;
                if (!isCrouching) runSpeed = 1.66f;
            }
            else
            {
                standingStil = false;
                if (!isOutOfStamina && !isCrouching)
                {
                    canRun = true;
                }
            }
        }
        else
        {
            standingStil = false;
            if (!isOutOfStamina && !isCrouching)
            {
                canRun = true;
            }
        }

        characterController.Move(Vector3.ClampMagnitude(moveDirection, runSpeed) * Time.deltaTime);
    }

    private void StaminaBehaviour()
    {
        staminaBar.fillAmount = stamina / 10;
        staminaBar2.fillAmount = stamina / 10;
        if (isRunning && canRun)
        {
            StaminaRunningCheck = true;

            stamina -= Time.deltaTime * 0.45f;

            runSpeed += Time.deltaTime * 3f;

            if (runSpeed >= 4.16f)
            {
                stamina -= Time.deltaTime;
                runSpeed = 4.16f;
            }

            StaminaBackGround.SetActive(true);
            StaminaBar.SetActive(true);
            StaminaBar2.SetActive(true);
            cameraAnimator.SetBool("isWalking", false);
            cameraAnimator.SetBool("isRunning", true);
            if (stamina <= 0)
            {
                isOutOfStamina = true;
                stamina = 0;
            }
        }
        else
        {
            StaminaRunningCheck = false;
        }

        if (!StaminaRunningCheck)
        {
            if (!isOutOfStamina)
            {
                if (runSpeed > 2.08f) runSpeed -= Time.deltaTime * 2.5f;

                if (runSpeed < 2.08f) runSpeed = 1.66f;

                stamina += Time.deltaTime;
            }
            if (stamina >= 10)
            {
                stamina = 10;
                StaminaBackGround.SetActive(false);
                StaminaBar.SetActive(false);
                StaminaBar2.SetActive(false);
            }
        }

        if (isOutOfStamina)
        {
            stamina += Time.deltaTime;
            if (runSpeed > 2.08f) runSpeed -= Time.deltaTime * 2.5f;

            if (runSpeed < 2.08f && !isCrouching)
            {
                cameraAnimator.SetBool("isRunning", false);
                runSpeed = 1.66f;
            }

            if (stamina > 10)
            {
                stamina = 10;
                StaminaBackGround.SetActive(false);
                StaminaBar.SetActive(false);
                StaminaBar2.SetActive(false);
            }

            if (stamina > 3.5f)
            {
                staminaBar.color = Color.white;
                staminaBar2.color = Color.white;
                canRun = true;
                isOutOfStamina = false;
            }
        }
    }

    private void CrouchBehaviour()
    {
        if (Input.GetKeyDown(KeyCode.C)) isCrouching = !isCrouching;

        if (isCrouching)
        {
            smoothCrouch = false;

            if (runSpeed >= 2.08f) runSpeed -= Time.deltaTime * 4;
            else if (runSpeed < 2.08f) runSpeed -= Time.deltaTime * 2;

            if (runSpeed <= crouchSpeed) runSpeed = crouchSpeed;

            // Verander de hoogte van de colliders
            characterController.height = Mathf.Lerp(characterController.height, 0.85f, Time.deltaTime * crouchSmooth_Speed);
            playerCollider.height = Mathf.Lerp(playerCollider.height, 0.85f, Time.deltaTime * crouchSmooth_Speed);
            triggerCollider.height = Mathf.Lerp(triggerCollider.height, 0.85f, Time.deltaTime * crouchSmooth_Speed);

            // Verander alleen de y-positie van de camera (hou x en z op 0) en smooth de beweging
            Vector3 cameraPosition = playerCamera.transform.localPosition;
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, crouchingHeight_Cam, Time.deltaTime * crouchSmooth_Speed);
            playerCamera.transform.localPosition = cameraPosition;

            // Center blijft op 1f
            playerCollider.center = new Vector3(0, 1f, 0);
            triggerCollider.center = new Vector3(0, 1f, 0);
        }
        else if (!isCrouching && !smoothCrouch)
        {
            // Verander de hoogte van de colliders geleidelijk
            characterController.height = Mathf.Lerp(characterController.height, 1.5f, Time.deltaTime * crouchSmooth_Speed);
            playerCollider.height = Mathf.Lerp(playerCollider.height, 1.5f, Time.deltaTime * crouchSmooth_Speed);
            triggerCollider.height = Mathf.Lerp(triggerCollider.height, 1.5f, Time.deltaTime * crouchSmooth_Speed);

            // Smooth de camera terug naar de originele hoogte (hou x en z op 0)
            Vector3 cameraPosition = playerCamera.transform.localPosition;
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, standingHeight_Cam, Time.deltaTime * crouchSmooth_Speed);
            playerCamera.transform.localPosition = cameraPosition;

            // Zorg ervoor dat de center op 1f blijft
            playerCollider.center = new Vector3(0, 1f, 0);
            triggerCollider.center = new Vector3(0, 1f, 0);

            if (characterController.height >= 1.5f && playerCollider.height >= 1.5f && triggerCollider.height >= 1.5f)
            {
                characterController.height = 1.5f;
                playerCollider.height = 1.5f;
                triggerCollider.height = 1.5f;

                // Center blijft op 1f
                playerCollider.center = new Vector3(0, 1f, 0);
                triggerCollider.center = new Vector3(0, 1f, 0);

                smoothCrouch = true;
            }
        }
    }

    private void CameraBehaviour()
    {
        if (canMove)
        {
            cameraAnimator.enabled = true;
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else cameraAnimator.enabled = false;
    }
}
