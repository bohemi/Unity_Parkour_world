using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Action/New Vault Action")]

public class VaultAction : ParkourAction
{
    public override bool checkIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
    {
        if (!base.checkIfPossible(hitData, player))
            return false;

        var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);
            
        if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
            mirror = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        else
            mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        
        return true;
    }
}
