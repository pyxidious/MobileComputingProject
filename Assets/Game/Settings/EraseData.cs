using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseData : MonoBehaviour
{
    void Start()
    {
        
    }

    public void eraseData() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    void Update()
    {
        
    }
}
