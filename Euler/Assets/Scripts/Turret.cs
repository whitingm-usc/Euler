using Unity.Hierarchy;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public enum AimMode
    {
        Euler,
        Quat,
        QuatLookAt,
        QuatFromEuler
    }
    public Transform rotYaw;
    public Transform rotPitch;
    public Transform gunTip;
    public GameObject target;
    public float m_pitchMax = 360.0f;    // degrees
    public float m_pitchMin = -360.0f;   // degrees
    public float m_yawMax = 360.0f;      // degrees
    public float m_yawMin = -360.0f;     // degrees
    public float lerp = 0.1f;
    Transform parent;
    float yOffset;
    AngleMode.Mode oldMode;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = rotYaw.parent;
        yOffset = gunTip.position.y - rotPitch.position.y;
        oldMode = AngleMode.s_mode;
    }

    // Update is called once per frame
    void Update()
    {
        if (oldMode != AngleMode.s_mode)
        { 
            rotYaw.localRotation = Quaternion.identity;
            rotPitch.localRotation = Quaternion.identity;
            oldMode = AngleMode.s_mode;
        }
        GameObject curTarget = target;
        if (null == curTarget)
        {
            curTarget = FindBestTarget();
        }
        if (curTarget)
        {
            Vector3 targetPos = curTarget.transform.position;
            AimAt(targetPos);
        }
    }

    GameObject FindBestTarget()
    {
        GameObject bestTarget = null;
        float bestDist = float.MaxValue;
        foreach (var target in FindObjectsByType<Target>(FindObjectsSortMode.None))
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestTarget = target.gameObject;
            }
        }
        return bestTarget;
    }

    void AimAt(Vector3 targetPos)
    {
        switch (AngleMode.s_mode)
        {
            case AngleMode.Mode.Euler:
                AimAtEuler(targetPos);
                break;
            case AngleMode.Mode.Quat:
                AimAtQuat(targetPos);
                break;
            case AngleMode.Mode.QuatLookAt:
                AimAtQuatLookAt(targetPos);
                break;
            case AngleMode.Mode.QuatFromEuler:
                AimAtQuatFromEuler(targetPos);
                break;
        }
    }

    void AimAtEuler(Vector3 targetPos)
    {
        Vector3 dir = targetPos - rotYaw.position;

        // yaw
        float rotY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        rotY = Mathf.Clamp(rotY, m_yawMin, m_yawMax);
        LerpEulerYaw(rotYaw, rotY, lerp);

        // pitch
        dir = targetPos - rotPitch.position;
        Vector3 dirXZ = new Vector3(dir.x, 0, dir.z);
        float dXZ = dirXZ.magnitude;

        float rotP = -Mathf.Atan2(dir.y, dXZ);
        // correct for the offset to the gun tip
        float sinOffset = Mathf.Clamp(yOffset / dir.magnitude, -1.0f, 1.0f);
        rotP += Mathf.Asin(sinOffset);
        rotP *= Mathf.Rad2Deg;
        rotP = Mathf.Clamp(rotP, m_pitchMin, m_pitchMax);
        LerpEulerPitch(rotPitch, rotP, lerp);
    }

    void AimAtQuat(Vector3 targetPos)
    {
        Vector3 dir = targetPos - rotPitch.position;
        dir.Normalize();
        Vector3 fwd = parent == null ? Vector3.forward : parent.forward;
        fwd.Normalize();
        Quaternion rot;
        float dot = Vector3.Dot(dir, fwd);
        if (dot == 1.0f)
        {
            rot = Quaternion.identity;
        }
        else if (dot == -1.0f)
        {
            rot = Quaternion.AngleAxis(180, Vector3.up);
        }
        else
        {
            Vector3 axis = Vector3.Cross(fwd, dir);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            rot = Quaternion.AngleAxis(angle, axis);
        }

        LerpQuat(rotPitch, rot, lerp);
    }

    void AimAtQuatLookAt(Vector3 targetPos)
    {
        if (rotPitch != rotYaw)
        {
            rotYaw.localRotation = Quaternion.identity;
        }
        Quaternion rot = Quaternion.LookRotation(targetPos - rotPitch.position, Vector3.up);
        LerpQuat(rotPitch, rot, lerp);
    }

    void AimAtQuatFromEuler(Vector3 targetPos)
    {
        Vector3 dir = targetPos - rotYaw.position;

        // yaw
        float rotY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        rotY = Mathf.Clamp(rotY, m_yawMin, m_yawMax);

        // pitch
        dir = targetPos - rotPitch.position;
        Vector3 dirXZ = new Vector3(dir.x, 0, dir.z);
        float dXZ = dirXZ.magnitude;

        float rotP = -Mathf.Atan2(dir.y, dXZ);
        // correct for the offset to the gun tip
        float sinOffset = Mathf.Clamp(yOffset / dir.magnitude, -1.0f, 1.0f);
        rotP += Mathf.Asin(sinOffset);
        rotP *= Mathf.Rad2Deg;
        rotP = Mathf.Clamp(rotP, m_pitchMin, m_pitchMax);

        Quaternion quat = Quaternion.Euler(rotP, rotY, 0);
        LerpQuat(rotPitch, quat, lerp);
    }

    void LerpQuat(Transform xform, Quaternion targetRot, float f)
    {
        xform.rotation = Quaternion.Slerp(xform.rotation, targetRot, f);
    }

    void LerpEulerYaw(Transform xform, float yaw, float f)
    {
        Vector3 ang = xform.localEulerAngles;
        if (m_yawMin > -180.0f || m_yawMax < 180.0f)
        {   // don't cross the 180 degree boundary if we have limits that are less than 360 degrees.
            if (ang.y > 180.0f)
                ang.y -= 360.0f;
            if (ang.y < -180.0f)
                ang.y += 360.0f;
            ang.y = Mathf.Lerp(ang.y, yaw, f);
        }
        else
            ang.y = Mathf.LerpAngle(ang.y, yaw, f);
        xform.localEulerAngles = ang;
    }

    void LerpEulerPitch(Transform xform, float pitch, float f)
    {
        Vector3 ang = xform.localEulerAngles;
        if (m_pitchMin > -180.0f || m_pitchMin < 180.0f)
        {   // don't cross the 180 degree boundary if we have limits that are less than 360 degrees.
            if (ang.x > 180.0f)
                ang.x -= 360.0f;
            if (ang.x < -180.0f)
                ang.x += 360.0f;
            ang.x = Mathf.Lerp(ang.x, pitch, f);
        }
        else
        {
            ang.x = Mathf.LerpAngle(ang.x, pitch, f);
        }
        xform.localEulerAngles = ang;
    }
}
