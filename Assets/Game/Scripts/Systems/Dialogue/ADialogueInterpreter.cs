using Game.Systems.Dialogue.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ADialogueInterpreter : MonoBehaviour
    {
        protected DialogueData _dialogue;

        public void OpenAndProcessDialogue(DialogueData dialogue)
        {
            _dialogue = dialogue;

            // TODO: open dialogue menu

        }

        public abstract void OnEvent(string functionName);
    }
}
