using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameReviewManager : MonoBehaviour
{

    public static InGameReviewManager instance;
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;


    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


        StartCoroutine(RequestReview());
    }


    IEnumerator RequestReview()
    {
        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
    }


    public void Launch()
    {
        StartCoroutine(LaunchReview());
    }


    // Update is called once per frame

    IEnumerator LaunchReview()
    {
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}
