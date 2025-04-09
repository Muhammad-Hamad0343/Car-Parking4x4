using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Loading
{
    Splash,
    Game
}
public class SliderTween : MonoBehaviour
{
    public Loading Loading;
    void Start()
    {
        if(Loading == Loading.Game)
            GetComponent<Slider>().DOValue(1, 5.05f).SetEase(Ease.Linear);
    }
    public void RestartTween()
    {
        GetComponent<Slider>().value = 0;
        GetComponent<Slider>().DOValue(1, 2.15f).SetEase(Ease.Linear);
    }
    
}
