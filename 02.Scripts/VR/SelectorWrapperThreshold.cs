using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FireEscape.VR
{
    public class SelectorWrapperThreshold : MonoBehaviour
    {
        [SerializeField]
        private SelectorUnityEventWrapper selector;

        [SerializeField]
        private float selectThresholdTime = 2.0f; // Time in seconds

        //public UnityEvent<float> OnSelectProgress; // Event to notify progress
        public UnityEvent<float> OnSelectStart; // Event to notify the start of selection
        public UnityEvent OnSelectThresholdReached;
        public UnityEvent OnSelectThresholdFailed;

        private bool isSelected;
        private Coroutine selectionTimerCoroutine;

        private void OnEnable()
        {
            selector.WhenSelected.AddListener(OnSelected);
            selector.WhenUnselected.AddListener(OnUnselected);
        }

        private void OnDisable()
        {
            selector.WhenSelected.RemoveListener(OnSelected);
            selector.WhenUnselected.RemoveListener(OnUnselected);
        }

        private void OnSelected()
        {
            isSelected = true;
            //OnSelectProgress?.Invoke(0f); // Notify to reset the progress
            OnSelectStart?.Invoke(selectThresholdTime); // Notify the start of selection

            if (selectionTimerCoroutine != null)
            {
                StopCoroutine(selectionTimerCoroutine);
            }

            selectionTimerCoroutine = StartCoroutine(SelectionTimer());
        }

        private void OnUnselected()
        {
            isSelected = false;
            //OnSelectProgress?.Invoke(0f); // Notify to reset the progress

            if (selectionTimerCoroutine != null)
            {
                StopCoroutine(selectionTimerCoroutine);
                OnSelectThresholdFailed?.Invoke();
            }
        }

        private IEnumerator SelectionTimer()
        {
            float elapsedTime = 0f;

            while (elapsedTime < selectThresholdTime && isSelected)
            {
                elapsedTime += Time.unscaledDeltaTime;
                //float normalizedProgress = Mathf.Clamp01(elapsedTime / selectThresholdTime);
                //OnSelectProgress?.Invoke(normalizedProgress); // Notify the normalized progress
                yield return null;
            }

            if (isSelected)
            {
                OnSelectThresholdReached?.Invoke();
            }
        }
    }
}