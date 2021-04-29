using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

/// <summary>
/// Hides and locks cursor at start of game, shows cursor and unlocks during conversations.
/// </summary>
public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        DialogueManager.instance.GetComponent<PixelCrushers.InputDeviceManager>().controlCursorState = false;
        UpdateCursorVisibility(false);
    }
    private void UpdateCursorVisibility(bool shouldShowCursor)
    {
        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnConversationEnded(Transform t)
    {
        UpdateCursorVisibility(false);
    }

    private void OnConversationStarted(Transform t)
    {
        UpdateCursorVisibility(true);
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
}
