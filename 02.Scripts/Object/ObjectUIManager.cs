using FireEscape.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FireEscape.Object
{
    public class ObjectUIManager : MonoBehaviour
    {
        public static ObjectUIManager Instance { get; private set; }

        public Material highlightMaterial;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }


    }
}