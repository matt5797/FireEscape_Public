using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmUI : MonoBehaviour
{
    public void Start()
    {
        Hide();
    }
    
    public void OnOff(bool onOff)
    {
        if (onOff)
        {
            Show();
        }
        else
        {
            Hide();
        }
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
