using System.Collections.Generic;
using UnityEngine;

public class RubeGoldbergMachineBuilder : MonoBehaviour
{
    [SerializeField] private bool buildOnStart = true;

    private readonly List<Material> runtimeMaterials = new List<Material>();

    private void Start()
    {
        if (!buildOnStart || transform.childCount > 0)
        {
            return;
        }

        BuildMachine();
    }

    private void BuildMachine()
    {
        GameObject ground = CreateCube("Ground", new Vector3(0f, -0.5f, 0f), new Vector3(45f, 1f, 18f), Color.gray, false);
        ground.GetComponent<Collider>().sharedMaterial = null;

        GameObject ramp = CreateCube("Ramp", new Vector3(-15f, 1.5f, 0f), new Vector3(8f, 0.5f, 4f), new Color(0.22f, 0.45f, 0.85f), false);
        ramp.transform.rotation = Quaternion.Euler(0f, 0f, -22f);
        GameObject rampExitTrigger = CreateCube("RampExitTrigger", new Vector3(-11.5f, 1f, 0f), new Vector3(1f, 1f, 1.6f), new Color(1f, 1f, 1f, 0.1f), false);
        rampExitTrigger.GetComponent<Collider>().isTrigger = true;

        Rigidbody starterSphere = CreateSphere("StarterSphere", new Vector3(-18.5f, 4f, 0f), 0.7f, new Color(0.95f, 0.8f, 0.15f), true);

        Rigidbody domino1 = CreateDomino("Domino1", new Vector3(-9f, 1f, 0f), new Color(0.9f, 0.3f, 0.3f));
        Rigidbody domino2 = CreateDomino("Domino2", new Vector3(-7.3f, 1f, 0f), new Color(0.85f, 0.35f, 0.2f));
        Rigidbody domino3 = CreateDomino("Domino3", new Vector3(-5.6f, 1f, 0f), new Color(0.8f, 0.4f, 0.15f));

        Rigidbody leverPivot = CreateCubeBody("LeverPivot", new Vector3(-3.5f, 1.2f, 0f), new Vector3(0.5f, 2f, 1f), new Color(0.35f, 0.35f, 0.4f), false);
        Rigidbody leverArm = CreateCubeBody("LeverArm", new Vector3(-3.5f, 2.2f, 0f), new Vector3(4.8f, 0.35f, 1f), new Color(0.1f, 0.75f, 0.65f), true);
        leverArm.mass = 1.2f;
        HingeJoint hinge = leverArm.gameObject.AddComponent<HingeJoint>();
        hinge.connectedBody = leverPivot;
        hinge.axis = Vector3.forward;
        hinge.anchor = new Vector3(-2.2f, 0f, 0f);
        hinge.useLimits = true;
        JointLimits limits = hinge.limits;
        limits.min = -45f;
        limits.max = 35f;
        hinge.limits = limits;

        Rigidbody launchAnchor = CreateCubeBody("LaunchAnchor", new Vector3(1.3f, 2.8f, 0f), new Vector3(0.5f, 0.5f, 0.5f), new Color(0.2f, 0.2f, 0.25f), false);
        Rigidbody launchBall = CreateSphere("LaunchBall", new Vector3(0f, 2.8f, 0f), 0.55f, new Color(0.2f, 0.9f, 0.2f), true);
        SpringJoint spring = launchBall.gameObject.AddComponent<SpringJoint>();
        spring.connectedBody = launchAnchor;
        spring.autoConfigureConnectedAnchor = false;
        spring.anchor = Vector3.zero;
        spring.connectedAnchor = Vector3.zero;
        spring.spring = 180f;
        spring.damper = 4f;
        spring.minDistance = 0.2f;
        spring.maxDistance = 0.35f;

        Rigidbody suspendedCube = CreateCubeBody("SuspendedCube", new Vector3(4.4f, 3.2f, 0f), new Vector3(1.1f, 1.1f, 1.1f), new Color(0.65f, 0.2f, 0.85f), true);
        suspendedCube.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        Rigidbody seesawBase = CreateCubeBody("SeesawBase", new Vector3(8.4f, 1.15f, 0f), new Vector3(0.6f, 1.8f, 1.6f), new Color(0.32f, 0.32f, 0.32f), false);
        Rigidbody seesaw = CreateCubeBody("Seesaw", new Vector3(8.4f, 2.05f, 0f), new Vector3(5f, 0.35f, 1f), new Color(0.2f, 0.6f, 0.95f), true);
        HingeJoint seesawHinge = seesaw.gameObject.AddComponent<HingeJoint>();
        seesawHinge.connectedBody = seesawBase;
        seesawHinge.axis = Vector3.forward;
        seesawHinge.useLimits = true;
        JointLimits seesawLimits = seesawHinge.limits;
        seesawLimits.min = -30f;
        seesawLimits.max = 30f;
        seesawHinge.limits = seesawLimits;

        Rigidbody dropBall = CreateSphere("DropBall", new Vector3(11.1f, 3.9f, 0f), 0.6f, new Color(0.98f, 0.62f, 0.15f), true);
        dropBall.isKinematic = true;

        GameObject ringTrigger = CreateCube("RingTrigger", new Vector3(12.8f, 2.1f, 0f), new Vector3(1f, 1f, 1f), new Color(0.1f, 0.1f, 0.1f, 0.25f), false);
        Collider ringCollider = ringTrigger.GetComponent<Collider>();
        ringCollider.isTrigger = true;

        Rigidbody gate = CreateCubeBody("Gate", new Vector3(15f, 1.5f, 0f), new Vector3(0.45f, 2.6f, 2f), new Color(0.85f, 0.15f, 0.45f), true);
        gate.isKinematic = true;

        Rigidbody finalSphere = CreateSphere("FinalSphere", new Vector3(13.4f, 1f, 0f), 0.7f, new Color(0.95f, 0.95f, 0.95f), true);
        finalSphere.isKinematic = true;

        GameObject finishTrigger = CreateCube("FinishTrigger", new Vector3(19.3f, 1f, 0f), new Vector3(1.4f, 1.4f, 1.4f), new Color(0.15f, 0.85f, 0.35f, 0.25f), false);
        finishTrigger.GetComponent<Collider>().isTrigger = true;

        Rigidbody goalCube = CreateCubeBody("GoalCube", new Vector3(20.7f, 1f, 0f), new Vector3(1.2f, 1.2f, 1.2f), new Color(0.1f, 0.9f, 0.9f), true);

        ChainInteraction step1 = AddInteraction(
            "Step01_RampHit",
            ChainInteraction.InteractionMode.Trigger,
            rampExitTrigger,
            domino1,
            new Vector3(14f, 0f, 0f),
            requiredOtherBody: starterSphere);

        ChainInteraction step2 = AddInteraction(
            "Step02_Domino1",
            ChainInteraction.InteractionMode.Collision,
            domino1.gameObject,
            domino2,
            new Vector3(12f, 0f, 0f),
            requiredOtherBody: starterSphere);

        ChainInteraction step3 = AddInteraction(
            "Step03_Domino2",
            ChainInteraction.InteractionMode.Collision,
            domino2.gameObject,
            domino3,
            new Vector3(12f, 0f, 0f),
            requiredOtherBody: domino1);

        ChainInteraction step4 = AddInteraction(
            "Step04_Domino3ToLever",
            ChainInteraction.InteractionMode.Collision,
            domino3.gameObject,
            leverArm,
            new Vector3(11f, 2f, 0f),
            requiredOtherBody: domino2);

        ChainInteraction step5 = AddInteraction(
            "Step05_LeverToSpring",
            ChainInteraction.InteractionMode.Trigger,
            CreateTriggerFollower("LeverTipTrigger", leverArm.transform, new Vector3(2.3f, 0f, 0f), new Vector3(0.7f, 0.7f, 0.7f)),
            null,
            Vector3.zero,
            null,
            spring,
            hinge,
            requiredOtherBody: leverArm);

        ChainInteraction step6 = AddInteraction(
            "Step06_LaunchBallHit",
            ChainInteraction.InteractionMode.Collision,
            launchBall.gameObject,
            suspendedCube,
            new Vector3(8f, -2f, 0f),
            suspendedCube,
            requiredOtherBody: suspendedCube);

        ChainInteraction step7 = AddInteraction(
            "Step07_SuspendedCubeFall",
            ChainInteraction.InteractionMode.Collision,
            suspendedCube.gameObject,
            seesaw,
            new Vector3(8f, 0f, 0f),
            requiredOtherBody: seesaw);

        ChainInteraction step8 = AddInteraction(
            "Step08_SeesawReleasesDropBall",
            ChainInteraction.InteractionMode.Trigger,
            CreateTriggerFollower("SeesawRightTrigger", seesaw.transform, new Vector3(2.3f, 0.5f, 0f), new Vector3(0.8f, 0.8f, 0.8f)),
            null,
            Vector3.zero,
            null,
            null,
            null,
            setKinematicTarget: dropBall,
            setKinematicValue: false,
            requiredOtherBody: suspendedCube);

        ChainInteraction step9 = AddInteraction(
            "Step09_DropBallThroughRing",
            ChainInteraction.InteractionMode.Trigger,
            ringTrigger,
            null,
            Vector3.zero,
            null,
            null,
            null,
            null,
            null,
            setKinematicTarget: dropBall,
            setKinematicValue: false,
            requiredOtherBody: dropBall);

        ChainInteraction step10 = AddInteraction(
            "Step10_GateDropStartsFinalBall",
            ChainInteraction.InteractionMode.Collision,
            gate.gameObject,
            finalSphere,
            new Vector3(12f, 0f, 0f),
            null,
            null,
            null,
            setKinematicTarget: finalSphere,
            setKinematicValue: false,
            requiredOtherBody: dropBall);

        ChainInteraction step11 = AddInteraction(
            "Step11_FinalBallToGoal",
            ChainInteraction.InteractionMode.Trigger,
            finishTrigger,
            goalCube,
            new Vector3(8f, 0f, 0f),
            requiredOtherBody: finalSphere);

        ChainInteraction step12 = AddInteraction(
            "Step12_GoalImpact",
            ChainInteraction.InteractionMode.Collision,
            goalCube.gameObject,
            null,
            Vector3.zero,
            requiredOtherBody: finalSphere);

        _ = step1;
        _ = step2;
        _ = step3;
        _ = step4;
        _ = step5;
        _ = step6;
        _ = step7;
        _ = step8;
        _ = step9;
        _ = step10;
        _ = step11;
        _ = step12;
    }

    private GameObject CreateTriggerFollower(string name, Transform parent, Vector3 localPosition, Vector3 scale)
    {
        GameObject trigger = CreateCube(name, parent.position + localPosition, scale, new Color(1f, 1f, 1f, 0.1f), false);
        trigger.transform.SetParent(parent, true);
        trigger.transform.localPosition = localPosition;
        Collider col = trigger.GetComponent<Collider>();
        col.isTrigger = true;
        return trigger;
    }

    private ChainInteraction AddInteraction(
        string name,
        ChainInteraction.InteractionMode mode,
        GameObject owner,
        Rigidbody impulseTarget,
        Vector3 impulse,
        Rigidbody releaseConstraintTarget = null,
        SpringJoint springToRelease = null,
        HingeJoint hingeToMotorize = null,
        Collider enableCollider = null,
        Collider disableCollider = null,
        Rigidbody setKinematicTarget = null,
        bool setKinematicValue = false,
        Rigidbody requiredOtherBody = null)
    {
        ChainInteraction interaction = owner.AddComponent<ChainInteraction>();
        interaction.name = name;
        interaction.Configure(
            mode,
            impulseTarget,
            impulse,
            releaseConstraintTarget,
            springToRelease,
            hingeToMotorize,
            enableCollider,
            disableCollider,
            setKinematicTarget,
            setKinematicValue,
            requiredOtherBody);
        return interaction;
    }

    private Rigidbody CreateDomino(string name, Vector3 position, Color color)
    {
        Rigidbody rb = CreateCubeBody(name, position, new Vector3(0.35f, 2f, 1f), color, true);
        rb.mass = 0.4f;
        return rb;
    }

    private Rigidbody CreateCubeBody(string name, Vector3 position, Vector3 scale, Color color, bool dynamicBody)
    {
        GameObject cube = CreateCube(name, position, scale, color, dynamicBody);
        return cube.GetComponent<Rigidbody>();
    }

    private Rigidbody CreateSphere(string name, Vector3 position, float diameter, Color color, bool dynamicBody)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(transform, false);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * diameter;
        SetColor(sphere, color);

        Rigidbody body = sphere.AddComponent<Rigidbody>();
        body.useGravity = true;
        body.isKinematic = !dynamicBody;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        return body;
    }

    private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color, bool dynamicBody)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(transform, false);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        SetColor(cube, color);

        Rigidbody body = cube.AddComponent<Rigidbody>();
        body.useGravity = dynamicBody;
        body.isKinematic = !dynamicBody;
        if (dynamicBody)
        {
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        return cube;
    }

    private void SetColor(GameObject target, Color color)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        runtimeMaterials.Add(material);
        renderer.material = material;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < runtimeMaterials.Count; i++)
        {
            if (runtimeMaterials[i] != null)
            {
                Destroy(runtimeMaterials[i]);
            }
        }

        runtimeMaterials.Clear();
    }
}
