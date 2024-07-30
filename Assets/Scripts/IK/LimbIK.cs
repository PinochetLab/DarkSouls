using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LimbIK : MonoBehaviour
{
    [SerializeField] private Transform shoulder;
    [SerializeField] private Transform forearm;
    [SerializeField] private Transform hand;

    [SerializeField] private Vector3 forwardVector = -Vector3.up;

    [SerializeField] private Transform target;

    [SerializeField] private bool fixedAngle;
    [SerializeField] private float angle;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float shoulderAngle;
    [SerializeField] private float forearmAngle;
    [SerializeField] private bool alignRotation = true;


    private void Update()
    {
        var realTarget = target.position + hand.right * offset.x + hand.forward * offset.z + hand.up * offset.y;

        var dir = (realTarget - shoulder.position).normalized;
        var targetRight = target.right;
        var normal = Vector3.Cross(dir, targetRight).normalized;
        var bendDir = Vector3.Cross(dir, normal);

        if (fixedAngle)
        {
            var cross = Vector3.Cross(dir, Vector3.down).normalized;
            if (cross == Vector3.zero)
            {
                cross = Vector3.Cross(dir, Vector3.right).normalized;
            }
            bendDir = -Vector3.Cross(dir, cross);
        }

        bendDir = Quaternion.AngleAxis(angle, dir) * bendDir;

        var c = Vector3.Distance(shoulder.position, realTarget);
        var a = Vector3.Distance(shoulder.position, forearm.position);
        var b = Vector3.Distance(forearm.position, hand.position);
        if (c > a + b)
        {
            var rotOffset = Quaternion.FromToRotation(Vector3.forward, -forwardVector);

            shoulder.rotation = Quaternion.LookRotation(dir, shoulder.up) * rotOffset;
            forearm.rotation = Quaternion.LookRotation(dir, forearm.up) * rotOffset;

            if (alignRotation)
            {
                hand.rotation = Quaternion.LookRotation(target.right, target.forward) * rotOffset;
            }
            else
            {
                hand.rotation = forearm.rotation;
            }
            return;
        }
        var y = (c * c + a * a - b * b) / 2 / c;
        var x = Mathf.Sqrt(a * a - y * y);

        var shoulderDir = (dir * y + bendDir * x).normalized;
        var forearmDir = (dir * (c - y) - bendDir * x).normalized;

        var rotationOffset = Quaternion.FromToRotation(Vector3.forward, -forwardVector);

        var v1 = normal;
        var v2 = -Vector3.Cross(shoulderDir, normal);
        var v = v2 * Mathf.Cos(shoulderAngle * Mathf.Deg2Rad) + v1 * Mathf.Sin(shoulderAngle * Mathf.Deg2Rad);
        shoulder.rotation = Quaternion.LookRotation(shoulderDir, v) * rotationOffset;

        v2 = -Vector3.Cross(forearmDir, normal);
        v = v2 * Mathf.Cos(forearmAngle * Mathf.Deg2Rad) + v1 * Mathf.Sin(forearmAngle * Mathf.Deg2Rad);
        forearm.rotation = Quaternion.LookRotation(forearmDir, v) * rotationOffset;

        if (alignRotation)
        {
            hand.rotation = Quaternion.LookRotation(target.right, target.forward) * rotationOffset;
        } else
        {
            hand.rotation = forearm.rotation;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(shoulder.position, target.position);
        var v1 = (target.position - shoulder.position).normalized;
        var v2 = target.right;
        var v3 = Vector3.Cross(v1, Vector3.Cross(v1, v2).normalized);
        var c = Vector3.Distance(shoulder.position, target.position);
        var a = Vector3.Distance(shoulder.position, forearm.position);
        var b = Vector3.Distance(forearm.position, hand.position);
        var y = (c * c + a * a - b * b) / 2 / c;
        var x = Mathf.Sqrt(a * a - y * y);
        Gizmos.DrawLine(shoulder.position + v1 * y, shoulder.position + v1 * y + v3 * x);
    }
}
