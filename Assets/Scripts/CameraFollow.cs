using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;        // Drag the Player here

    [Header("Follow Settings")]
    public float smoothSpeed = 8f;  // Higher = snappier, lower = more lag

    void Start()
    {
        // Warn if target was not assigned in the Inspector
        if (target == null)
            Debug.LogError("[CameraFollow] Target is not assigned! Drag the Player into the Target field.");
    }

    // LateUpdate runs after ALL Update() and FixedUpdate() calls this frame.
    // This is critical — if we ran in Update(), the camera would sample
    // the player position before physics moved it, causing one-frame jitter.
    void LateUpdate()
    {
        if (target == null) return;

        // Build the desired rig position:
        // Follow player X and Z, but keep the rig's own Y (height stays fixed)
        Vector3 desiredPos = new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z
        );

        // Lerp smoothly toward the desired position each frame
        // smoothSpeed * Time.deltaTime ensures consistent speed on all machines
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );
    }
}

// ── SETUP STEPS ──────────────────────────────────────────────
// 1. GameObject → Create Empty → name it "CameraRig"
// 2. Set CameraRig Position to (0, 0, 0)
// 3. In Hierarchy, drag Main Camera ONTO CameraRig (makes it a child)
// 4. Select Main Camera → set LOCAL Position to (0, 3, -6)
//    and LOCAL Rotation to (20, 0, 0)  ← aims slightly downward
// 5. Attach CameraFollow.cs to CameraRig and drag Player into Target
// ─────────────────────────────────────────────────────────────