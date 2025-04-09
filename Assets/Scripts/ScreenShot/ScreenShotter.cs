using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotter : MonoBehaviour
{
   int count = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) /*|| Input.GetMouseButtonDown(0)*/)
            ScreenCapture.CaptureScreenshot($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)}/ScreenShot-{count++}.png", 2);
        
    }
}
