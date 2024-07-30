using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isAttacking;

    private const string Hit1Name = "Hit1";
    private const string Hit2Name = "Hit2";
    private const string Hit3Name = "Hit3";

    private const string Return1Name = "Return1";
    private const string Return2Name = "Return2";
    private const string Return3Name = "Return3";

    private const string Return1ToHitName = "Return1ToHit";
    private const string Return2ToHitName = "Return2ToHit";
    private const string Return3ToHitName = "Return3ToHit";

    private const string Transition1Name = "Transition1";
    private const string Transition2Name = "Transition2";
    private const string Transition3Name = "Transition3";

    private const string IdleName = "Idle";

    private const string TriggerName = "Hit";

    private bool IsPlaying(string name)
    {
        var answer = animator.GetCurrentAnimatorStateInfo(0);
        return answer.IsName(name);
    }

    private bool IsInTransition() => animator.IsInTransition(0);

    private bool IsHitting()
    {
        var list = new List<string>() { Hit1Name, Hit2Name, Hit3Name };
        var state = animator.GetCurrentAnimatorStateInfo(0);
        foreach (var s in list)
        {
            if (state.IsName(s))
            {
                return true;
            }
        }
        return false;
    }

    private bool CanHit()
    {
        var transition = animator.GetAnimatorTransitionInfo(0);
        if (IsInTransition())
        {
            print("transition");
            return false;
        }
        
        var tlist = new List<string>() { Transition1Name, Transition2Name, Transition3Name };
        foreach (var s in tlist)
        {
            if (transition.IsName(s))
            {
                print(s);
                return false;
            }
        }
        var state = animator.GetCurrentAnimatorStateInfo(0);
        var list = new List<string>()
        {
            Hit1Name, Hit2Name, Hit3Name, Return1ToHitName, Return2ToHitName, Return3ToHitName
        };
        foreach (var s in list)
        {
            if (state.IsName(s))
            {
                print(s);
                return false;
            }
        }
        return true;
    }

    public void Click()
    {
        if (!CanHit())
        {
            if (IsHitting())
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                print(state.normalizedTime);
                if (state.normalizedTime > 0.7f)
                {
                    animator.SetTrigger(TriggerName);
                }
            }
            return;
        }
        /*if (IsPlaying(Return1Name) && !IsPlaying(Return1ToHitName))
        {
            animator.SetTrigger(TriggerName);
        }*/
        animator.SetTrigger(TriggerName);
    }
}
