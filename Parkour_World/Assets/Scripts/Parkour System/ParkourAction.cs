using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Parkour System/New Parkour Action")]
public class ParkourAction : ScriptableObject
{
    EnvironmentScanner scanner;
    [SerializeField] string animName;
    [SerializeField] string obstacleTag;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] bool rotateToObstacle;
    [SerializeField] float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 matchPosWeight = new Vector3(0, 1, 0);

    public Quaternion targetRotation { get; set; }
    public Vector3 matchPos { get; set; }
    public bool mirror { get; set; }

    public virtual bool checkIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;

        if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
            return false;

        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
        {
            targetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }

        if (enableTargetMatching)
            matchPos = hitData.heightHit.point;

        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart; 
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public float PostActionDelay => postActionDelay;
    public Vector3 MatchPosWeight => matchPosWeight;
}
