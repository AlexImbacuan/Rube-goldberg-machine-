using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RubeGoldbergMachineBuilder : MonoBehaviour
{
    [SerializeField] private bool buildOnStart = true;

    private readonly List<Material> runtimeMaterials = new List<Material>();

    private void Awake()
    {
        ConfigureCameraDirector();
    }

    private void Start()
    {
        ConfigureBall();

        if (!buildOnStart || transform.childCount > 0)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            BuildInEditor();
            return;
        }
#endif

        BuildMachine();
    }

    private void ConfigureCameraDirector()
    {
        if (!Application.isPlaying || FindObjectOfType<CameraMovementDirector>() != null)
        {
            return;
        }

        gameObject.AddComponent<CameraMovementDirector>();
    }

    private void ConfigureBall()
    {
        ConfigureBall("balon");
        ConfigureBall("balon (1)");
        ConfigureBall("balon (2)");
    }

    private void ConfigureBall(string ballName)
    {
        GameObject ball = GameObject.Find(ballName);
        if (ball == null)
        {
            return;
        }

        Rigidbody body = ball.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = ball.AddComponent<Rigidbody>();
        }

        body.useGravity = true;
        body.isKinematic = false;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Collider collider = ball.GetComponent<Collider>();
        if (collider != null)
        {
            PhysicsMaterial material = new PhysicsMaterial("BallBounce");
            material.bounciness = 0.85f;
            material.dynamicFriction = 0.15f;
            material.staticFriction = 0.15f;
            material.bounceCombine = PhysicsMaterialCombine.Maximum;
            material.frictionCombine = PhysicsMaterialCombine.Minimum;
            collider.material = material;
        }

        if (ball.GetComponent<BallImpactLauncher>() == null)
        {
            ball.AddComponent<BallImpactLauncher>();
        }
    }

    private void BuildMachine()
    {
        GameObject ground = CreateCube("Ground", new Vector3(0f, -0.5f, 0f), new Vector3(45f, 1f, 18f), Color.gray, false);
        ground.GetComponent<Collider>().sharedMaterial = null;

        GameObject ramp = CreateCube("Ramp", new Vector3(-15f, 1.5f, 0f), new Vector3(8f, 0.5f, 4f), new Color(0.22f, 0.45f, 0.85f), false);
        ramp.transform.rotation = Quaternion.Euler(0f, 0f, -22f);

        Rigidbody starterSphere = CreateSphere("StarterSphere", new Vector3(-18.5f, 4f, 0f), 0.7f, new Color(0.95f, 0.8f, 0.15f), true);

        Rigidbody domino1 = CreateDomino("Step02_Domino1", new Vector3(-9f, 1f, 0f), new Color(0.9f, 0.3f, 0.3f));
        Rigidbody domino2 = CreateDomino("Step03_Domino2", new Vector3(-7.3f, 1f, 0f), new Color(0.85f, 0.35f, 0.2f));
        Rigidbody domino3 = CreateDomino("Step04_Domino3ToLever", new Vector3(-5.6f, 1f, 0f), new Color(0.8f, 0.4f, 0.15f));

        Rigidbody leverPivot = CreateCubeBody("LeverPivot", new Vector3(-3.5f, 1.2f, 0f), new Vector3(0.5f, 2f, 1f), new Color(0.35f, 0.35f, 0.4f), false);
        AddInteraction(
            "Step02_Domino1",
            domino1.gameObject,
            domino2,
            new Vector3(12f, 0f, 0f),
            requiredOtherBody: starterSphere);

        AddInteraction(
            "Step03_Domino2",
            domino2.gameObject,
            domino3,
            new Vector3(12f, 0f, 0f),
            requiredOtherBody: domino1);

        AddInteraction(
            "Step04_Domino3ToLever",
            domino3.gameObject,
            leverPivot,
            new Vector3(11f, 2f, 0f),
            requiredOtherBody: domino2);

    }

    private ChainInteraction AddInteraction(
        string name,
        GameObject owner,
        Rigidbody impulseTarget,
        Vector3 impulse,
        Rigidbody requiredOtherBody = null)
    {
        ChainInteraction interaction = owner.AddComponent<ChainInteraction>();
        interaction.name = name;
        interaction.Configure(
            impulseTarget,
            impulse,
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

        // URP first; fallback to Built-in if URP is not installed
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.color = color;

        // URP uses _BaseColor; Built-in uses _Color. Set both if available.
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        runtimeMaterials.Add(material);
        renderer.material = material;
    }

    private void OnDestroy()
    {
        ClearRuntimeMaterials();
    }

#if UNITY_EDITOR
    [ContextMenu("Build Machine (Editor)")]
    private void BuildInEditor()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        ClearRuntimeMaterials();
        BuildMachine();
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif

    private void ClearRuntimeMaterials()
    {
        for (int i = 0; i < runtimeMaterials.Count; i++)
        {
            if (runtimeMaterials[i] == null)
            {
                continue;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(runtimeMaterials[i]);
                continue;
            }
#endif

            Destroy(runtimeMaterials[i]);
        }

        runtimeMaterials.Clear();
    }
}
