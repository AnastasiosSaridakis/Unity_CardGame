using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;


public class MyFeedbacks : MonoBehaviour
{
    [SerializeField] private MMFeedbacks attackFeedback1;
    [SerializeField] private MMFeedbacks attackFeedback2;
    [SerializeField] private MMFeedbacks damageTakenFeedback1;
    [SerializeField] private MMFeedbacks damageTakenFeedback2;
    [SerializeField] private MMFeedbacks tauntFeedback;


    public void GetAttackedFeedback()
    {
        damageTakenFeedback1?.PlayFeedbacks();
        damageTakenFeedback2?.PlayFeedbacks();
    }
    
    public void AttackFeedback()
    {
        attackFeedback1?.PlayFeedbacks();
        attackFeedback2?.PlayFeedbacks();
    }

    public void TauntFeedback()
    {
        tauntFeedback?.StopFeedbacks();
        tauntFeedback?.PlayFeedbacks();
    }
}
