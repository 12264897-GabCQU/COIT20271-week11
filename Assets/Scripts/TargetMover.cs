using UnityEngine;

public class TargetMover : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRange = 6f;     // How far left/right to travel
    public float speed       = 1.5f;   // Patrol speed multiplier

    // Axis: 0 = horizontal (X), 1 = vertical (Z), 2 = both (diagonal)
    [Range(0, 2)]
    public int moveAxis = 0;

    // ── Private ─────────────────────────────────────────────────
    private Vector3 origin;        // World position when this target spawned
    private float   timeOffset;    // Random start phase — prevents sync between targets

    void Start()
    {
        origin     = transform.position;

        // ✅ FIX: random offset so every target starts at a different
        // point in its patrol — previously all targets moved in sync
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // PingPong returns a value that bounces 0 → range → 0 → range…
        // Adding timeOffset desynchronises multiple instances
        float t    = Mathf.PingPong((Time.time + timeOffset) * speed, 1f);
        float dist = Mathf.Lerp(-patrolRange, patrolRange, t);

        Vector3 offset = Vector3.zero;
        switch (moveAxis)
        {
            case 0: offset = new Vector3(dist, 0f, 0f);   break; // Horizontal
            case 1: offset = new Vector3(0f, 0f, dist);   break; // Depth
            case 2: offset = new Vector3(dist, 0f, dist); break; // Diagonal
        }

        transform.position = origin + offset;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (GameManager.instance != null)
            GameManager.instance.HandleTargetPlayerCollision(gameObject, 1f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (GameManager.instance != null)
            GameManager.instance.HandleTargetPlayerCollision(gameObject, 1f);
    }

    // OnDrawGizmos draws the patrol path in the Scene view (editor only)
    void OnDrawGizmos()
    {
        Vector3 o   = Application.isPlaying ? origin : transform.position;
        Vector3 pA  = o + new Vector3(-patrolRange, 0f, 0f);
        Vector3 pB  = o + new Vector3( patrolRange, 0f, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pA, pB);
        Gizmos.DrawSphere(pA, 0.25f);
        Gizmos.DrawSphere(pB, 0.25f);
    }
}

// ── TARGET PREFAB SETUP CHECKLIST ────────────────────────────
// 1. GameObject → 3D Object → Cube
// 2. Scale: (1, 1, 1)
// 3. In the Inspector top bar, set Tag to "Target"  ← CRITICAL
//    (Create the tag first via Add Tag if it doesn't exist)
// 4. Box Collider is already on the Cube — leave Is Trigger = OFF
// 5. Attach TargetMover.cs
// 6. Apply a red Material
// 7. Drag into Assets/Prefabs to save as prefab
// ─────────────────────────────────────────────────────────────