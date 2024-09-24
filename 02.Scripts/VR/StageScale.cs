using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace FireEscape.VR
{
    public class StageScale : MonoBehaviour
    {
        public float duration = 0.5f;

        [Serializable]
        public struct ScaleLevel
        {
            public float scale;
            public float yOffset;
        }

        public List<ScaleLevel> ScaleLevelList = new List<ScaleLevel>();
        private int ScaleIndex = 0;

        private bool isScaling = false;

        public UnityEvent OnScaleChanged;

        private void Start()
        {
            float scale = ScaleLevelList[ScaleIndex].scale;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ScaleUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ScaleDown();
            }
#endif
        }

        public void ScaleUp()
        {
            if (isScaling)
                return;

            if (ScaleIndex < ScaleLevelList.Count - 1)
            {
                StartCoroutine(ScaleChange(ScaleIndex + 1));
            }
        }

        public void ScaleDown()
        {
            if (isScaling)
                return;

            if (ScaleIndex > 0)
            {
                StartCoroutine(ScaleChange(ScaleIndex - 1));
            }
        }

        public IEnumerator ScaleChange(int newIndex)
        {
            float time = 0f;
            float startScale = transform.localScale.x;
            float endScale = ScaleLevelList[newIndex].scale;

            float startOffsetY = transform.localPosition.y;
            float endOffsetY = ScaleLevelList[newIndex].yOffset;

            isScaling = true;

            while (time < duration)
            {
                float scale = Mathf.Lerp(startScale, endScale, time / duration);
                transform.localScale = new Vector3(scale, scale, scale);

                float offsetY = Mathf.Lerp(startOffsetY, endOffsetY, time / duration);
                transform.localPosition = new Vector3(transform.localPosition.x, offsetY, transform.localPosition.z);

                time += Time.unscaledDeltaTime;
                yield return null;
            }
            transform.localScale = new Vector3(endScale, endScale, endScale);
            transform.localPosition = new Vector3(transform.localPosition.x, endOffsetY, transform.localPosition.z);
            ScaleIndex = newIndex;

            isScaling = false;

            OnScaleChanged.Invoke();
        }
    }
}