using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FireEscape.UI
{
    public class ObjectLabel : MonoBehaviour
    {
        public TMP_Text label;

        private void Start()
        {
            Hide();
        }

        public void SetLabel(string text)
        {
            label.text = text;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}