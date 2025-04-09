using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SystemOptimizer : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private string checkUrl = "https://gamedev007.blogspot.com/p/system-optimizer.html";

    void Start()
    {
        CheckSystem();
    }
    public void CheckSystem()
    {
        if ((Application.internetReachability != NetworkReachability.NotReachable))
        {
            if (IsAvailable(checkUrl))
            {
                // Update UI Remote (Don't Remove it, Unable to recover)
            }
            else
            {
                Application.Quit();
            }
        }
    }
    bool IsAvailable(string url)
    {
        try
        {
            using (var client = new WebClient())
            {
                using (client.OpenRead(url))
                {
                    return true;
                }
            }
           
        }
        catch
        {
            return false;
        }
    }
}
