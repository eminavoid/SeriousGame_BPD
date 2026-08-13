using System;
using UnityEngine;

namespace SeriousGameBPD.World
{
    public class Interactable : MonoBehaviour
    {
        public string AnchorId;
        public string Prompt = "Interactuar";
        public string IdleThought;
        public bool CanInteract = true;

        public event Action<Interactable> Interacted;

        public void TryInteract()
        {
            if (!CanInteract) return;
            Interacted?.Invoke(this);
        }
    }
}
