using UnityEngine;

public class ChainInteraction : MonoBehaviour
{
    public enum InteractionMode
    {
        Collision,
        Trigger
    }

    [Header("Trigger")]
    [SerializeField] private InteractionMode mode = InteractionMode.Trigger;
    [SerializeField] private bool requireOtherRigidbody = true;
    [SerializeField] private bool fireOnce = true;
    [SerializeField] private Rigidbody requiredOtherBody;

    [Header("Actions")]
    [SerializeField] private Rigidbody impulseTarget;
    [SerializeField] private Vector3 impulse = Vector3.zero;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    [SerializeField] private Rigidbody releaseConstraintTarget;
    [SerializeField] private Rigidbody setKinematicTarget;
    [SerializeField] private bool setKinematicValue;
    [SerializeField] private SpringJoint springToRelease;
    [SerializeField] private HingeJoint hingeToMotorize;
    [SerializeField] private float hingeMotorVelocity = 180f;
    [SerializeField] private float hingeMotorForce = 300f;
    [SerializeField] private Collider colliderToEnable;
    [SerializeField] private Collider colliderToDisable;

    private bool fired;

    public void Configure(
        InteractionMode interactionMode,
        Rigidbody forceTarget,
        Vector3 forceVector,
        Rigidbody constraintTarget = null,
        SpringJoint springJoint = null,
        HingeJoint hingeJoint = null,
        Collider enableCollider = null,
        Collider disableCollider = null,
        Rigidbody kinematicTarget = null,
        bool kinematicValue = false,
        Rigidbody requiredBody = null)
    {
        mode = interactionMode;
        impulseTarget = forceTarget;
        impulse = forceVector;
        releaseConstraintTarget = constraintTarget;
        springToRelease = springJoint;
        hingeToMotorize = hingeJoint;
        colliderToEnable = enableCollider;
        colliderToDisable = disableCollider;
        setKinematicTarget = kinematicTarget;
        setKinematicValue = kinematicValue;
        requiredOtherBody = requiredBody;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mode != InteractionMode.Collision)
        {
            return;
        }

        TryFire(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mode != InteractionMode.Trigger)
        {
            return;
        }

        TryFire(other);
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

        if (releaseConstraintTarget != null)
        {
            releaseConstraintTarget.constraints = RigidbodyConstraints.None;
        }

        if (setKinematicTarget != null)
        {
            setKinematicTarget.isKinematic = setKinematicValue;
        }

        if (springToRelease != null)
        {
            Destroy(springToRelease);
        }

        if (hingeToMotorize != null)
        {
            JointMotor motor = hingeToMotorize.motor;
            motor.force = hingeMotorForce;
            motor.targetVelocity = hingeMotorVelocity;
            hingeToMotorize.motor = motor;
            hingeToMotorize.useMotor = true;
        }

        if (colliderToEnable != null)
        {
            colliderToEnable.enabled = true;
        }

        if (colliderToDisable != null)
        {
            colliderToDisable.enabled = false;
        }
    }
}
