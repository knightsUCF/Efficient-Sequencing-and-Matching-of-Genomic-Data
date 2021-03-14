using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    
    Buttons buttons;


    void Awake()
    {
        buttons = FindObjectOfType<Buttons>();
    }


    void Show()
    {
        gameObject.SetActive(true);
    }


    void Hide()
    {
        gameObject.SetActive(false);
    }


    void Start()
    {
        // yes, can use a for loop here

        Invoke("Hide", 0.1f);
        Invoke("Show", 0.2f);
        Invoke("Hide", 0.3f);
        Invoke("Show", 0.4f);
        Invoke("Hide", 0.5f);
        Invoke("Show", 0.6f);
        Invoke("Hide", 0.7f);
        Invoke("Show", 0.8f);
        Invoke("Hide", 0.9f);
        Invoke("Show", 1.0f);
        Invoke("Hide", 1.1f);
        Invoke("Show", 1.2f);
        Invoke("Hide", 1.3f);
        Invoke("Show", 1.4f);
        Invoke("ShowAdvanceArrow", 1.5f);

    }


    void ShowAdvanceArrow()
    {
        buttons.ShowAdvanceArrow();
    }

}
