using FireEscape.Turn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FireEscape.UI
{
    public class ExitSearchPhaseToggle : MonoBehaviour
    {
        private Toggle toggle;

        private bool IsPhaseRunning => TurnManager.Instance.IsPhaseRunning;
        private bool IsSearchPhase => TurnManager.Instance.currentPhase == TurnManager.Phase.SearchPhase;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
            TurnManager.Instance.OnSearchPhaseStart.AddListener(OnSearchPhaseStart);
            TurnManager.Instance.OnSearchPhaseEnd.AddListener(OnSearchPhaseEnd);

            toggle.interactable = true;
        }

        private void Update()
        {
            toggle.interactable = IsSearchPhase && !IsPhaseRunning;
        }

        public void OnValueChanged(bool value)
        {
            if (value)
            {
                TurnManager.Instance.SeachPhaseExit();
            }
        }

        public void OnSearchPhaseStart()
        {
            toggle.interactable = true;
        }

        public void OnSearchPhaseEnd()
        {
            toggle.interactable = false;
        }
    }
}