using FireEscape.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireEscape.Object
{
    public class ObjectUI : MonoBehaviour
    {
        public MeshRenderer meshRenderer;

        public ObjectLabel objectLabel;
        public DescriptionUI descriptionUI;

        private Material _defaultMaterial;
        public Material highlightMaterial;

        [HideInInspector]
        public bool isHighlight = false;

        public bool haveLabel = true;
        public bool haveSummary = true;

        public string objectName;

        [TextArea]
        public string summary;

        [TextArea]
        public string description;

        private void Awake()
        {
            _defaultMaterial = meshRenderer.material;

            if (highlightMaterial == null)
            {
                highlightMaterial = ObjectUIManager.Instance.highlightMaterial;
            }

            if (objectLabel != null)
            {
                objectLabel.SetLabel(objectName);
            }
            if (descriptionUI != null)
            {
                descriptionUI.SetDescription(objectName, summary);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerViewArea"))
            {
                Show();
            }
        }

        public void OnTriggerStay(Collider other)
        {
            //Debug.Log($"OnTriggerStay {other}");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerViewArea"))
            {
                Hide();
            }
        }

        public void Show()
        {
            if (objectLabel != null)
            {
                objectLabel.Show();
            }
            if (descriptionUI != null)
            {
                descriptionUI.Show();
            }
        }

        public void Hide()
        {
            if (objectLabel != null)
            {
                objectLabel.Hide();
            }
            if (descriptionUI != null)
            {
                descriptionUI.Hide();
            }
        }

        public void OnHover()
        {
            isHighlight = true;
            HighlightOn();
        }

        public void OnUnhover()
        {
            isHighlight = false;
            HighlightOff();
        }

        public void HighlightOn()
        {
            // Add Highlight Material
            List<Material> materials = new List<Material>();
            meshRenderer.GetSharedMaterials(materials);
            if (!materials.Contains(highlightMaterial))
            {
                materials.Add(highlightMaterial);
                meshRenderer.sharedMaterials = materials.ToArray();
            }
        }

        public void HighlightOff()
        {
            // Remove Highlight Material
            List<Material> materials = new List<Material>();
            meshRenderer.GetSharedMaterials(materials);
            if (materials.Contains(highlightMaterial))
            {
                materials.Remove(highlightMaterial);
                meshRenderer.sharedMaterials = materials.ToArray();
            }
        }
    }
}
