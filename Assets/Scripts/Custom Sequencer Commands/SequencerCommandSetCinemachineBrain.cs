using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using Cinemachine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandSetCinemachineBrain : SequencerCommand
    {
        private CinemachineBrain cinemachineBrain;

        public void Awake()
        {
            cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = GetParameterAsBool(0);
            Stop();
        }
    }
}
