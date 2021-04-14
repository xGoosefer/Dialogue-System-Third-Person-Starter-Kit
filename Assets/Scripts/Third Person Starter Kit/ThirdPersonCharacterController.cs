using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// This is designed to be added as a child to any model from Mixamo and "just work."
/// We are doing a lot of workarounds to move things around to support ease of use for the user.
/// </summary>
public class ThirdPersonCharacterController : MonoBehaviour
{
    [SerializeField]
    private bool invertCameraY = true;

    [SerializeField]
    private float cameraRotationSpeed = 0.5f;

    [SerializeField]
    private float cameraVerticalLookMax = 300, cameraVerticalLookMin = 60;

    [SerializeField]
    private RuntimeAnimatorController animatorController;

    private float yMoveInput, xMoveInput, yLookInput, xLookInput;
    private bool sprintInputButtonDown;
    private int sprintAnimParameter = Animator.StringToHash("IsSprinting");
    private int forwardAnimParameter = Animator.StringToHash("Forward");
    private int horizontalAnimParameter = Animator.StringToHash("Horizontal");
    private Animator animator;
    /// <summary>
    /// This object is moved in code to support camera rotation.
    /// </summary>
    private GameObject cameraFollowTarget;
    private Vector3 cameraFollowOffset;
    private bool isSprintActivated;
    private bool inConversation;
    private CinemachineVirtualCamera walkingCamera, sprintingCamera;
    private CharacterController characterControllerOriginal;
    /// <summary>
    /// GameObject that holds the player cameras so they can all be detached at once easily.
    /// They need to be children of the PlayerCharacter for easy set up, 
    /// but need to be detached at runtime to function properly.
    /// </summary>
    private Transform playerCameras;

    private bool IsSprinting
    {
        get
        {
            if (isSprintActivated)
            {
                if ((Mathf.Abs(yMoveInput) == 0 && Mathf.Abs(xMoveInput) == 0))
                    isSprintActivated = false;
            }
            else
                isSprintActivated = (Mathf.Abs(yMoveInput) > 0 || Mathf.Abs(xMoveInput) > 0) && sprintInputButtonDown;

            return isSprintActivated;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MoveCharacterControllerComponentToParent();
        InitializeAnimator();
        InitializePlayerCameras();
        IntializeCameraFollowTarget();

        // Set parent tag to player so cameras can properly ignore, etc.
        transform.parent.tag = "Player";
    }

    private void InitializeAnimator()
    {
        animator = GetComponentInParent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void InitializePlayerCameras()
    {
        playerCameras = GameObject.Find("Player Cameras").transform;
        playerCameras.SetParent(null);
        walkingCamera = GameObject.Find("Walking Camera").GetComponent<CinemachineVirtualCamera>();
        sprintingCamera = GameObject.Find("Sprinting Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void IntializeCameraFollowTarget()
    {
        cameraFollowTarget = GameObject.Find("Camera Follow Target");
        cameraFollowOffset = cameraFollowTarget.transform.position - transform.position;
    }

    /// <summary>
    /// For some reason, collision only works if the character controller component is on the parent.
    /// To simplify setup for students, this is my workaround.
    /// </summary>
    private void MoveCharacterControllerComponentToParent()
    {
        characterControllerOriginal = GetComponent<CharacterController>();
        var characterControllerNew = gameObject.transform.parent.gameObject.AddComponent<CharacterController>();
        characterControllerNew.slopeLimit = characterControllerOriginal.slopeLimit;
        characterControllerNew.stepOffset = characterControllerOriginal.stepOffset;
        characterControllerOriginal.skinWidth = characterControllerOriginal.skinWidth;
        characterControllerNew.minMoveDistance = characterControllerOriginal.minMoveDistance;
        characterControllerNew.center = characterControllerOriginal.center;
        characterControllerNew.radius = characterControllerOriginal.radius;
        characterControllerNew.height = characterControllerOriginal.height;
        Destroy(characterControllerOriginal);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateCameraFollowTargetPosition();
        UpdateAnimParameters();
        UpdateCameraRotation();
        UpdatePlayerRotation();
        UpdateCameraPriority();
    }

    /// <summary>
    /// Switch to a special camera for sprinting.
    /// </summary>
    private void UpdateCameraPriority()
    {
        int offPriority = 0, onPriority = 10;
        walkingCamera.Priority = IsSprinting ? offPriority : onPriority;
        sprintingCamera.Priority = IsSprinting ? onPriority : offPriority;
    }

    /// <summary>
    /// Moves the cameraFollowTarget with the player.
    /// The cameraFollowTarget cannot be attached to the player because
    /// we don't want it to rotate with the player.
    /// </summary>
    private void UpdateCameraFollowTargetPosition()
    {
        cameraFollowTarget.transform.position = transform.position + cameraFollowOffset;
    }

    /// <summary>
    /// Rotates camera based on mouse input.
    /// </summary>
    private void UpdateCameraRotation()
    {
        var invertYMultiplier = invertCameraY ? -1 : 1;
        cameraFollowTarget.transform.rotation *= Quaternion.AngleAxis(xLookInput * cameraRotationSpeed, Vector3.up);
        cameraFollowTarget.transform.rotation *= Quaternion.AngleAxis(yLookInput * -cameraRotationSpeed * invertYMultiplier, Vector3.right);

        var angles = cameraFollowTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = cameraFollowTarget.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < cameraVerticalLookMax)
        {
            angles.x = cameraVerticalLookMax;
        }
        else if (angle < 180 && angle > cameraVerticalLookMin)
        {
            angles.x = cameraVerticalLookMin;
        }

        cameraFollowTarget.transform.localEulerAngles = angles;
    }

    /// <summary>
    /// If the player is standing still, the camera should orbit them.
    /// If the player is walking, set the player rotation based on the look transform
    /// If the player is sprinting, rotate the player based on camera-relative input.
    /// </summary>
    private void UpdatePlayerRotation()
    {
        if (Mathf.Abs(xMoveInput) > 0 || Mathf.Abs(yMoveInput) > 0)
        {
            if (IsSprinting)
            {
                var moveInput = new Vector3(xMoveInput, 0, yMoveInput);
                var camForward = cameraFollowTarget.transform.forward;
                camForward.y = 0;
                var camRotationFlattened = Quaternion.LookRotation(camForward);
                var cameraRelativeInput = camRotationFlattened * moveInput;

                transform.parent.rotation = Quaternion.LookRotation(cameraRelativeInput);
                // Debug.DrawRay(transform.position, cameraRelativeInput);
            }
            else
            {
                transform.parent.rotation = Quaternion.Euler(0, cameraFollowTarget.transform.rotation.eulerAngles.y, 0);
            }
        }
    }

    private void UpdateAnimParameters()
    {
        animator.SetBool(sprintAnimParameter, IsSprinting);
        animator.SetFloat(forwardAnimParameter, yMoveInput);
        animator.SetFloat(horizontalAnimParameter, xMoveInput);
    }

    private void GetInput()
    {
        xMoveInput = inConversation ? 0 : Input.GetAxis("Horizontal");
        yMoveInput = inConversation ? 0 : Input.GetAxis("Vertical");
        xLookInput = inConversation ? 0 : Input.GetAxis("Mouse X");
        yLookInput = inConversation ? 0 : Input.GetAxis("Mouse Y");
        sprintInputButtonDown = inConversation ? false : Input.GetButton("Fire3");
    }

    private void OnEnable()
    {
        DialogueManager.instance.conversationStarted += OnConversationStarted;
        DialogueManager.instance.conversationEnded += OnConversationEnded;
    }
    private void OnDisable()
    {
        DialogueManager.instance.conversationStarted -= OnConversationStarted;
        DialogueManager.instance.conversationEnded -= OnConversationEnded;
    }
    private void OnConversationStarted(Transform t)
    {
        inConversation = true;
    }
    private void OnConversationEnded(Transform t)
    {
        inConversation = false;
    }
}
