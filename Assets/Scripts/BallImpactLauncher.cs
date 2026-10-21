using UnityEngine;

public class BallImpactLauncher : MonoBehaviour
{
    [SerializeField] private float impactForce = 18f;
    [SerializeField] private float upwardForce = 3f;
    [SerializeField] private float minimumImpactSpeed = 1f;
    [SerializeField] private float cooldown = 0.08f;

    private Rigidbody body;
    private float nextImpactTime;
    private bool hasCollided;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (body == null ||
            Time.time < nextImpactTime ||
            collision.relativeVelocity.sqrMagnitude < minimumImpactSpeed * minimumImpactSpeed)
        {
            return;
        }

        Collider collider = GetComponent<Collider>();
        if (!hasCollided && collider != null && collider.material != null)
        {
            collider.material.bounciness = 0f;
            hasCollided = true;
        }

        Vector3 direction = transform.position - collision.GetContact(0).point;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
        {
            direction = -collision.relativeVelocity;
            direction.y = 0f;
        }

        direction.Normalize();
        body.AddForce(direction * impactForce + Vector3.up * upwardForce, ForceMode.Impulse);
        nextImpactTime = Time.time + cooldown;
    }
}
