using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int   scoreValue = 10;    // Points awarded when this bullet hits a target
    public float lifespan   = 4f;   // Seconds before auto-destroy (safety net)

    void Start()
    {
        // Destroy this bullet after lifespan seconds even if it hits nothing
        Destroy(gameObject, lifespan);
    }

    // OnTriggerEnter fires when our trigger collider overlaps another collider
    // Requirement: this Bullet's Collider must have "Is Trigger" = ON
    void OnTriggerEnter(Collider other)
    {
        // ── Ignore collision with the Player that fired us ─────────
        // Without this check, the bullet instantly hits the player on spawn
        if (other.gameObject.CompareTag("Player")) return;

        // ── Did we hit a Target? ────────────────────────────────────
        if (other.gameObject.CompareTag("Target"))
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayScore();

            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(scoreValue);
                GameManager.instance.SpawnTarget(1.5f);
            }

            Destroy(other.gameObject);
        }

        // ── Destroy this bullet after any valid hit ─────────────────
        // (Player tag already returned early above, so we won't destroy
        //  the bullet on self-collision)
        Destroy(gameObject);
    }
}

// ── BULLET PREFAB SETUP CHECKLIST ────────────────────────────
// 1. GameObject → 3D Object → Sphere
// 2. Scale: (0.2, 0.2, 0.2)
// 3. Add Component → Rigidbody
//      ✓ Use Gravity = OFF
//      ✓ Is Kinematic = OFF (we set velocity directly)
// 4. Select the Sphere Collider component
//      ✓ Is Trigger = ON  ← this enables OnTriggerEnter
// 5. Attach Bullet.cs
// 6. Apply a bright yellow/orange Material for visibility
// 7. Drag from Hierarchy into Assets/Prefabs → creates the Prefab
// 8. Delete the original from the scene (we Instantiate from script)
// ─────────────────────────────────────────────────────────────