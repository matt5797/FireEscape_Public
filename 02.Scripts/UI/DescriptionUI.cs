using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FireEscape.UI
{
    public class DescriptionUI : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text summary;

        private void Start()
        {
            Hide();
        }

        public void SetDescription(string name, string summary)
        {
            nameText.text = name;
            this.summary.text = summary;
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