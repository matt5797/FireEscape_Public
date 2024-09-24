using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FireEscape.VR
{
    public class Minimap : MonoBehaviour
    {
        public Transform pointOfInterest;
        public Transform stage;

        private float targetStageScale;
        private float targetStageOffsetY;
        private Vector3 targetStagePosition;

        public float transitionSpeed = 5f; // You can adjust this value to change the transition speed

        private Vector3 lastPOIScale;
        private Vector3 lastPOIPosition;

        public float stageMaxScale = 1;
        public float stageMinScale = 0.25f;

        public float stageMaxOffsetY = 0.32f;
        public float stageMinOffsetY = 0.1f;

        public Vector2 stageMaxOffset = Vector2.one;
        public Vector2 stageMinOffset = -Vector2.one;

        public float poiMaxScale = 1;
        public float poiMinScale = 0.25f;

        public Vector2 poiMaxOffset = Vector2.one;
        public Vector2 poiMinOffset = -Vector2.one;

        public HologramController hologramController;

        private float lastHologramScale;

        public float hologramMaxScale = 10;
        public float hologramMinScale = 1;

        // private bool isChanging;

        private void OnEnable()
        {
            UpdatePointOfInterest();

            lastPOIScale = pointOfInterest.localScale;
            lastPOIPosition = pointOfInterest.localPosition;
            lastHologramScale = hologramController.CutoutDistanceValue;

            targetStageScale = stage.localScale.x;
            targetStageOffsetY = stage.localPosition.y;
            targetStagePosition = stage.localPosition;
        }

        private void Start()
        {
            OnOff(false);

            UpdateValue();
        }

        private void LateUpdate()
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            if (lastPOIScale != pointOfInterest.localScale)
            {
                UpdateStageScale();
                lastPOIScale = pointOfInterest.localScale;
            }
            if (lastPOIPosition != pointOfInterest.localPosition)
            {
                UpdateStagePosition();
                lastPOIPosition = pointOfInterest.localPosition;
            }

            Vector3 targetScale = new Vector3(targetStageScale, targetStageScale, targetStageScale);
            Vector3 targetPosition = new Vector3(targetStagePosition.x, targetStageOffsetY, targetStagePosition.z);

            // Smooth transition of poi scale and position
            stage.localScale = Vector3.Lerp(stage.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
            stage.localPosition = Vector3.Lerp(stage.localPosition, targetPosition, Time.unscaledDeltaTime * transitionSpeed);
            hologramController.CutoutDistanceValue = Mathf.Lerp(hologramController.CutoutDistanceValue, lastHologramScale, Time.unscaledDeltaTime * transitionSpeed);

            /*// 허용 오차 정의 (이 값은 적절히 조정 필요)
            float tolerance = 0.01f;

            // 스케일과 위치의 차이를 계산
            float scaleDifference = Vector3.Distance(stage.localScale, targetScale);
            float positionDifference = Vector3.Distance(stage.localPosition, targetPosition);

            // 오차 범위 내에 있는지 확인
            if (scaleDifference > tolerance || positionDifference > tolerance)
                isChanging = true;
            else
                isChanging = false;*/
        }

        public void OnOff(bool value)
        {
            gameObject.SetActive(value);
        }

        private void UpdateStageScale()
        {
            float poiScale = pointOfInterest.localScale.x;
            float percentY = Mathf.InverseLerp(poiMinScale, poiMaxScale, poiScale);

            float stageScale = Mathf.Lerp(stageMinScale, stageMaxScale, percentY);
            //stage.localScale = new Vector3(stageScale, stageScale, stageScale);
            targetStageScale = stageScale;

            float poiOffsetY = Mathf.Lerp(stageMinOffsetY, stageMaxOffsetY, percentY);
            //stage.localPosition = new Vector3(stage.localPosition.x, poiOffsetY, stage.localPosition.z);
            targetStageOffsetY = poiOffsetY;

            lastHologramScale = Mathf.Lerp(hologramMinScale, hologramMaxScale, percentY);
        }

        private void UpdateStagePosition()
        {
            Vector3 poiPosition = pointOfInterest.localPosition;

            // Calculate the percentage of movement within the bounds for x and z
            float percentX = Mathf.InverseLerp(poiMinOffset.x, poiMaxOffset.x, poiPosition.x);
            float percentZ = Mathf.InverseLerp(poiMinOffset.y, poiMaxOffset.y, poiPosition.z);

            float stageX = Mathf.Lerp(stageMinOffset.x, stageMaxOffset.x, percentX);
            float stageZ = Mathf.Lerp(stageMinOffset.y, stageMaxOffset.y, percentZ);

            Vector3 stagePosition = new Vector3(stageX, stage.localPosition.y, stageZ);
            //stage.localPosition = stagePosition;
            targetStagePosition = stagePosition;
        }

        public void UpdatePointOfInterest()
        {
            // Update Point of Interest From Stage Scale and Position
            float stageScale = stage.localScale.x;
            float percentScale = Mathf.InverseLerp(stageMinScale, stageMaxScale, stageScale);
            float poiScale = Mathf.Lerp(poiMinScale, poiMaxScale, percentScale);
            pointOfInterest.localScale = new Vector3(poiScale, poiScale, poiScale);

            Vector3 poiPosition = pointOfInterest.localPosition;

            // Calculate the percentage of movement
            float percentX = Mathf.InverseLerp(stageMinOffset.x, stageMaxOffset.x, stage.localPosition.x);
            float percentZ = Mathf.InverseLerp(stageMinOffset.y, stageMaxOffset.y, stage.localPosition.z);

            // Apply this percentage to the stage's bounds
            float poiX = Mathf.Lerp(poiMinOffset.x, poiMaxOffset.x, percentX);
            float poiZ = Mathf.Lerp(poiMinOffset.y, poiMaxOffset.y, percentZ);

            pointOfInterest.localPosition = new Vector3(poiX, pointOfInterest.localPosition.y, poiZ);
        }
    }
}