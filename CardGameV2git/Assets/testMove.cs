using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class testMove : MonoBehaviour
{
    public MMFeedbacks fb;
    public MMFeedbacks fb1;

    public void doThing()
    {
        fb?.PlayFeedbacks();
        fb1?.PlayFeedbacks();
    }
    
}
