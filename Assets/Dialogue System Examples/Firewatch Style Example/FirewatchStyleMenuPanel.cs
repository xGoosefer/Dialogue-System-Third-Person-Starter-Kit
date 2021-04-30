using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;

public class FirewatchStyleMenuPanel : StandardUIMenuPanel
{

    // When pressed, this key shows the response menu. 
    // When released, it clicks the selected button.
    public KeyCode actionKey = KeyCode.LeftShift;

    // Remember the response menu info so we can show it when 
    // the player presses the actionKey:
    private Subtitle subtitle = null;
    private Response[] responses = null;
    private Transform target = null;

    // When we need to open, we wait until we're closed to allow 
    // the Hide animation to finish:
    private bool needToOpen = false;

    public override void ShowResponses(Subtitle subtitle, Response[] responses, Transform target)
    {
        // Instead of showing the menu, just save the menu info and prepare the
        // menu to show when the player pressed the actionKey:
        this.subtitle = subtitle;
        this.responses = responses;
        this.target = target;
        gameObject.SetActive(true);
        panelState = PanelState.Closed;
    }

    protected override void Update()
    {
        if (Input.GetKey(actionKey))
        {
            if (panelState == PanelState.Closed || panelState == PanelState.Closing || panelState == PanelState.Uninitialized)
            {
                needToOpen = true;
            }
        }
        else
        {
            if (panelState == PanelState.Open || panelState == PanelState.Opening)
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    var button = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
                    if (button != null) button.onClick.Invoke();
                }
            }
        }
        if (needToOpen && panelState == PanelState.Closed)
        {
            base.ShowResponses(subtitle, responses, target);
            UITools.Select(instantiatedButtons[0].GetComponent<UnityEngine.UI.Button>());
            needToOpen = false;
        }
    }

}
