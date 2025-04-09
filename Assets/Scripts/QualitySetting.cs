using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum SceneType
{
    MainMenu,
    Gameplay,
    Custom
}
public class QualitySetting : MonoBehaviour
{
    public int lowRAMThreshold = 512; // in megabytes
    public int mediumRAMThreshold = 1024; // in megabytes

    public SceneType sceneType;
    void Start()
    {
        int systemRAM = SystemInfo.systemMemorySize; // in megabytes

        if(sceneType == SceneType.MainMenu)
        {
            SetLowQualitySettings();
        }
        else if(sceneType == SceneType.Gameplay)
        {
            SetMediumQualitySettings();
        }
        else if (sceneType == SceneType.Custom)
        {
            SetHighQualitySettings();
        }

        // Adjust quality settings based on available RAM
        //if (systemRAM < lowRAMThreshold)
        //{
        //    SetLowQualitySettings();
        //}
        //else if (systemRAM < mediumRAMThreshold)
        //{
        //    SetMediumQualitySettings();
        //}
        //else
        //{
        //    SetHighQualitySettings();
        //}
        //AdjustQuality();
    }

    void SetLowQualitySettings()
    {
        SetQualityLevel(0); // Set to the lowest quality level
        // Additional settings for low quality
    }

    void SetMediumQualitySettings()
    {
        SetQualityLevel(1); // Set to a medium quality level
        // Additional settings for medium quality
    }

    void SetHighQualitySettings()   
    {
        SetQualityLevel(QualitySettings.names.Length - 1); // Set to the highest quality level
        // Additional settings for high quality
    }

    void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);

        // Additionally, you can adjust URP settings here if needed
        UniversalRenderPipelineAsset urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
        if (urpAsset != null)
        {
            // Adjust URP settings based on quality level
            // For example:
            // urpAsset.renderScale = 0.5f; // Adjust render scale
        }
    }

    private void AdjustQuality()
    {
        Application.targetFrameRate = 30;

        float width1 = Screen.width * Screen.width;
        float height1 = Screen.height * Screen.height;
        float ypotinousa = width1 + height1;
        ypotinousa = Mathf.Sqrt(ypotinousa);
        float diagonalInches = ypotinousa / Screen.dpi;
        double width, height;

        if (SystemInfo.systemMemorySize >= 3072)//4096
        {
            width = Screen.width;
            height = Screen.height;
            Screen.SetResolution((int)width, (int)height, true);
        }
        else if (SystemInfo.systemMemorySize >= 2048)//3072
        {
            if (diagonalInches < 7)
            {
                width = Screen.width * 0.9;
                height = Screen.height * 0.9;
                Screen.SetResolution((int)width, (int)height, false, 60);
            }
            else
            {
                width = Screen.width * 0.9;
                height = Screen.height * 0.9;
                Screen.SetResolution((int)width, (int)height, false, 60);
            }

            //cheapGarage.SetActive(true);
            //expensiveGarage.SetActive(false);
        }
        else
        {
            if (diagonalInches < 7)
            {
                width = Screen.width * 0.8;
                height = Screen.height * 0.8;
                Screen.SetResolution((int)width, (int)height, false, 60);
            }
            else
            {
                width = Screen.width * 0.9;
                height = Screen.height * 0.9;
                Screen.SetResolution((int)width, (int)height, false, 60);
            }
        }
    }
}
