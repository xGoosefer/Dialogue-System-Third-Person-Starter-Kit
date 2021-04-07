using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[SelectionBase]
public class ThirdPersonCharacterController : MonoBehaviour
{
    [SerializeField]
    private float cameraRotationSpeed = 1;

    [SerializeField]
    private float cameraVerticalLookMax = 340, cameraVerticalLookMin = 40;

    private float yMoveInput, xMoveInput, yLookInput, xLookInput;
    private bool sprintInputButtonDown;
    private int sprintAnimParameter = Animator.StringToHash("IsSprinting");
    private int forwardAnimParameter = Animator.StringToHash("Forward");
    private int horizontalAnimParameter = Animator.StringToHash("Horizontal");
    private Animator animator;
    private GameObject cameraFollowTarget;
    private Vector3 cameraFollowOffset;
    private bool isSprintActivated;
    private bool inConversation;
    private CinemachineVirtualCamera walkingCamera, sprintingCamera;

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
        animator = GetComponent<Animator>();
        walkingCamera = GameObject.Find("Walking Camera").GetComponent<CinemachineVirtualCamera>();
        sprintingCamera = GameObject.Find("Sprinting Camera").GetComponent<CinemachineVirtualCamera>();
        cameraFollowTarget = GameObject.Find("Camera Follow Target");
        cameraFollowTarget.transform.SetParent(null);
        cameraFollowOffset = cameraFollowTarget.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        UpdateCameraFollowTargetPosition();
        UpdateAnimParameters();
        UpdateRotation();
        UpdateCameraPriority();
    }

    private void UpdateCameraPriority()
    {
        int offPriority = 0, onPriority = 10;
        walkingCamera.Priority = IsSprinting ? offPriority : onPriority;
        sprintingCamera.Priority = IsSprinting ? onPriority : offPriority;
    }

    private void UpdateCameraFollowTargetPosition()
    {
        cameraFollowTarget.transform.position = transform.position + cameraFollowOffset;
    }

    /// <summary>
    /// Rotates camera and player based on mouse input.
    /// </summary>
    private void UpdateRotation()
    {
        cameraFollowTarget.transform.rotation *= Quaternion.AngleAxis(xLookInput * cameraRotationSpeed, Vector3.up);
        cameraFollowTarget.transform.rotation *= Quaternion.AngleAxis(yLookInput * cameraRotationSpeed, Vector3.right);

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

        //If the player is movinig, set the player rotation based on the look transform
        if (Mathf.Abs(xMoveInput) > 0 || Mathf.Abs(yMoveInput) > 0)
        {
            if (IsSprinting)
            {
                var moveInput = new Vector3(xMoveInput, 0, yMoveInput);
                var camForward = cameraFollowTarget.transform.forward;
                camForward.y = 0;
                var camRotationFlattened = Quaternion.LookRotation(camForward);
                var cameraRelativeInput = camRotationFlattened * moveInput;

                 transform.rotation = Quaternion.LookRotation(cameraRelativeInput);
                // cameraFollowTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
                Debug.DrawRay(transform.position, cameraRelativeInput);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, cameraFollowTarget.transform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
               // cameraFollowTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
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
