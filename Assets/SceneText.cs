using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneText : MonoBehaviour
    {
        private TextMeshPro _text;

		private void Awake()
		{
			_text = GetComponent<TextMeshPro>();

			_text.text = RunManager.CurrentRoomScene;
		}
	}
}
