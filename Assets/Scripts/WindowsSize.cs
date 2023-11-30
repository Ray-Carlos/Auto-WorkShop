using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
    }

    // Update is called once per frame
    void Update()
    {
        // float standard_width = Screen.currentResolution.width;
        // float standard_height = standard_width * 9 / 16;

        // Screen.SetResolution((int)standard_width, (int)standard_height, false);
    }
}
