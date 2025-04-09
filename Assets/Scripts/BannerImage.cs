using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BannerImage : MonoBehaviour
{
    [SerializeField] private Image banner_PlaceHolder;
    private void Start()
    {
        SetImageSize();
    }
    //private float CalculateCanvasBannerSize()
    //{
    //    float bannerSizePixels = Screen.height <= 400 ? 32 : Screen.height < 720 ? 50 : 90;
    //    var percent = (100f / Screen.height) * bannerSizePixels;
    //    var bannerSize = canvasScaler.referenceResolution.y * (percent / 100f);
    //    return bannerSize;
    //}
    void SetImageSize()
    {
        //if (AdsManager.Instance.bannerView != null)
        //{
        //    banner_PlaceHolder.rectTransform.sizeDelta = new Vector2(AdsManager.Instance.bannerView.GetWidthInPixels() + 10, AdsManager.Instance.bannerView.GetHeightInPixels() + 10);
        //}
        //else
        //{
        //    banner_PlaceHolder.gameObject.SetActive(false);
        //}
    }
}
