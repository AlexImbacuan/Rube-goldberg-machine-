using UnityEngine;

public class ChainInteraction : MonoBehaviour
{
    [SerializeField] private bool requireOtherRigidbody = true;
    [SerializeField] private bool fireOnce = true;
    [SerializeField] private Rigidbody requiredOtherBody;

    [Header("Actions")]
    [SerializeField] private Rigidbody impulseTarget;
    [SerializeField] private Vector3 impulse = Vector3.zero;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

    private bool fired;

    public void Configure(
        Rigidbody forceTarget,
        Vector3 forceVector,
        Rigidbody requiredBody = null)
    {
        impulseTarget = forceTarget;
        impulse = forceVector;
        requiredOtherBody = requiredBody;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryFire(collision.collider);
    }

    private void TryFire(Collider other)
    {
        if (fireOnce && fired)
        {
            return;
        }

        if (requireOtherRigidbody && other.attachedRigidbody == null)
        {
            return;
        }

        if (requiredOtherBody != null && other.attachedRigidbody != requiredOtherBody)
        {
            return;
        }

        fired = true;

        if (impulseTarget != null && impulse != Vector3.zero)
        {
            impulseTarget.AddForce(impulse, forceMode);
        }
    }
}
