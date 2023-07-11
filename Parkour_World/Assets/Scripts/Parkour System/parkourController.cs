using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parkourController : MonoBehaviour
{

    bool inAction;
    [SerializeField] List<ParkourAction> parkourActions;

    EnvironmentScanner environmentScanner;
    PlayerController playerController;
    Animator animator;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Jump") && !inAction)
        {
            var hitData = environmentScanner.obstacleCheck();
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.checkIfPossible(hitData, transform))
                    {
                        StartCoroutine(doParkourAction(action));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator doParkourAction(ParkourAction action)
    {
        inAction = true;
        playerController.setControl(false);

        animator.SetBool("mirrorAction", action.mirror);
        animator.CrossFade(action.AnimName, 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.AnimName))
            Debug.Log("Wrong animation name");

        float timer = 0;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            if (action.RotateToObstacle)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.targetRotation,
                playerController.RotationSpeed * Time.deltaTime);

            if (action.EnableTargetMatching)
                matchTarget(action);

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }
        yield return new WaitForSeconds(action.PostActionDelay);

        playerController.setControl(true);
        inAction = false;
    }

    void matchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.matchPos, transform.rotation, action.MatchBodyPart,
        new MatchTargetWeightMask(action.MatchPosWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
