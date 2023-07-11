using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
    [SerializeField] float forwardRayLength = 0.8f;
    [SerializeField] float heightRayLength = 5;
    [SerializeField] float ledgeRayLength = 10;
    [SerializeField] float ledgeHeightThreshold = 0.75f;
    [SerializeField] LayerMask obstacleLayer;

    public ObstacleHitData obstacleCheck()
    {
        var hitData = new ObstacleHitData();

        var forwardOrigin = transform.position + forwardRayOffset;

        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward,
                                  out hitData.forwardHit, forwardRayLength, obstacleLayer);

        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength,
                     (hitData.forwardHitFound) ? Color.red : Color.blue);

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;

            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down * heightRayLength,
                                     out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength,
                         (hitData.heightHitFound) ? Color.red : Color.blue);
        }

        return hitData;
    }

    public bool ledgeCheck(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero)
            return false;

        float originOffSet = 0.5f;
        var origin = transform.position + moveDir * originOffSet + Vector3.up;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleLayer))
        {
            float height = transform.position.y - hit.point.y;

            if (height > ledgeHeightThreshold)
            {
                return true;
            }
        }

        return false;
    }


    public struct ObstacleHitData
    {
        public bool forwardHitFound;
        public bool heightHitFound;
        public RaycastHit forwardHit;
        public RaycastHit heightHit;
    }
}
