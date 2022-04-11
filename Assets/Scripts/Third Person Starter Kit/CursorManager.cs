using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

/// <summary>
/// Hides and locks cursor at start of game. Use DialogueSystemTrigge's "Show Cursor During Conversation"
/// to control cursor visibility during conversations.
/// </summary>
public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        UpdateCursorVisibility(false);
    }
    private void UpdateCursorVisibility(bool shouldShowCursor)
    {
        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
