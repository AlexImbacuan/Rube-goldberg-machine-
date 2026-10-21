using UnityEngine;

public class CameraMovementDirector : MonoBehaviour
{
    [SerializeField] private string mainCameraName = "Main Camera";
    [SerializeField] private string finalCameraName = "Camera 4";
    [SerializeField] private float followDistance = 5f;
    [SerializeField] private float followHeight = 2.5f;
    [SerializeField] private float focusHeight = 0.5f;
    [SerializeField] private float followSmoothness = 6f;
    [SerializeField] private float stopSpeed = 0.25f;
    [SerializeField] private float stopDelay = 0.5f;
    [SerializeField] private float retreatDistance = 8f;
    [SerializeField] private float retreatHeight = 3f;
    [SerializeField] private float retreatDuration = 1.5f;
    [SerializeField] private float finalCameraDelay = 2f;

    private Camera mainCamera;
    private Camera finalCamera;
    private Transform followedTarget;
    private Transform retreatingTarget;
    private Vector3 followDirection;
    private Vector3 lastTargetPosition;
    private float stoppedSince = -1f;
    private float retreatStartedAt = -1f;
    private float finalShotAt = -1f;
    private bool finalShotStarted;
    private bool movementWatchersConfigured;
    private int lastHandoffFrame = -1;
    private Transform lastHandoffFrom;
    private Transform lastHandoffTo;

    private void Awake()
    {
        mainCamera = FindCamera(mainCameraName);
        finalCamera = FindCamera(finalCameraName);
        if (finalCamera == null)
        {
            finalCamera = FindCamera("Camera4");
        }
        if (finalCamera == null)
        {
            finalCamera = FindCamera("Cámara 4");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            enabled = false;
            return;
        }

        DisableOtherCameras();
        mainCamera.enabled = true;
        SetAudioListener(mainCamera);

    }

    private void LateUpdate()
    {
        ConfigureMovementWatchers();

        if (followedTarget != null)
        {
            FollowTarget();
        }
        else if (retreatingTarget != null)
        {
            RetreatFromTarget();
        }

        if (!finalShotStarted && finalShotAt > 0f && Time.time >= finalShotAt)
        {
            ShowFinalCamera();
        }
    }

    public void BeginFollowing(Transform target)
    {
        if (finalShotStarted || target == null)
        {
            return;
        }

        followedTarget = target;
        retreatingTarget = null;
        stoppedSince = -1f;
        finalShotAt = -1f;
        lastTargetPosition = target.position;

        Vector3 direction = mainCamera.transform.position - target.position;
        followDirection = direction.sqrMagnitude > 0.01f
            ? direction.normalized
            : new Vector3(0f, 0.35f, -0.94f);
    }

    private void FollowTarget()
    {
        if (followedTarget == null)
        {
            StopFollowingTarget();
            return;
        }

        Vector3 desiredPosition = followedTarget.position +
            followDirection * followDistance +
            Vector3.up * followHeight;

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            1f - Mathf.Exp(-followSmoothness * Time.deltaTime));

        Vector3 lookTarget = followedTarget.position + Vector3.up * focusHeight;
        Vector3 lookDirection = lookTarget - mainCamera.transform.position;
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                desiredRotation,
                1f - Mathf.Exp(-followSmoothness * Time.deltaTime));
        }

        float movementSpeed = (followedTarget.position - lastTargetPosition).magnitude /
            Mathf.Max(Time.deltaTime, 0.0001f);
        lastTargetPosition = followedTarget.position;

        if (movementSpeed <= stopSpeed)
        {
            if (stoppedSince < 0f)
            {
                stoppedSince = Time.time;
            }
            else if (Time.time - stoppedSince >= stopDelay)
            {
                StopFollowingTarget();
            }
        }
        else
        {
            stoppedSince = -1f;
        }
    }

    private void StopFollowingTarget()
    {
        retreatingTarget = followedTarget;
        followedTarget = null;
        stoppedSince = -1f;
        retreatStartedAt = Time.time;
        finalShotAt = -1f;
    }

    private void RetreatFromTarget()
    {
        if (retreatingTarget == null)
        {
            finalShotAt = Time.time + finalCameraDelay;
            return;
        }

        Vector3 desiredPosition = retreatingTarget.position +
            followDirection * (followDistance + retreatDistance) +
            Vector3.up * (followHeight + retreatHeight);

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            1f - Mathf.Exp(-followSmoothness * Time.deltaTime));

        Vector3 lookTarget = retreatingTarget.position + Vector3.up * focusHeight;
        Vector3 lookDirection = lookTarget - mainCamera.transform.position;
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                desiredRotation,
                1f - Mathf.Exp(-followSmoothness * Time.deltaTime));
        }

        if (Time.time - retreatStartedAt >= retreatDuration)
        {
            retreatingTarget = null;
            finalShotAt = Time.time + finalCameraDelay;
        }
    }

    private void ConfigureMovementWatchers()
    {
        if (movementWatchersConfigured)
        {
            return;
        }

        AddMovementWatcher("StarterSphere");
        AddMovementWatcher("Step02_Domino1");
        AddMovementWatcher("Step03_Domino2");
        AddMovementWatcher("Step04_Domino3ToLever");
        AddMovementWatcher("LeverPivot");
        AddMovementWatcher("balon");
        AddMovementWatcher("balon (1)");
        AddMovementWatcher("balon (2)");

        if (followedTarget == null)
        {
            Transform initialTarget = FindTarget("balon");
            if (initialTarget == null)
            {
                initialTarget = FindTarget("StarterSphere");
            }

            if (initialTarget != null)
            {
                BeginFollowing(initialTarget);
            }
        }

        movementWatchersConfigured = true;
    }

    private void AddMovementWatcher(string objectName)
    {
        GameObject movingObject = GameObject.Find(objectName);
        if (movingObject == null ||
            movingObject.GetComponent<MovementCameraFollowTarget>() != null)
        {
            return;
        }

        MovementCameraFollowTarget watcher =
            movingObject.AddComponent<MovementCameraFollowTarget>();
        watcher.Configure(this);
    }

    private Transform FindTarget(string objectName)
    {
        GameObject target = GameObject.Find(objectName);
        return target == null ? null : target.transform;
    }

    public bool IsFollowing(Transform target)
    {
        return followedTarget == target;
    }

    public void FollowCollisionTarget(Transform currentTarget, Collision collision)
    {
        if (!IsFollowing(currentTarget) || collision.rigidbody == null)
        {
            return;
        }

        MovementCameraFollowTarget nextWatcher =
            collision.rigidbody.GetComponent<MovementCameraFollowTarget>();
        if (nextWatcher == null)
        {
            return;
        }

        Transform nextTarget = collision.rigidbody.transform;
        if (Time.frameCount == lastHandoffFrame &&
            lastHandoffFrom == nextTarget &&
            lastHandoffTo == currentTarget)
        {
            return;
        }

        lastHandoffFrame = Time.frameCount;
        lastHandoffFrom = currentTarget;
        lastHandoffTo = nextTarget;
        BeginFollowing(nextTarget);
    }

    private Camera FindCamera(string cameraName)
    {
        if (string.IsNullOrWhiteSpace(cameraName))
        {
            return null;
        }

        GameObject cameraObject = GameObject.Find(cameraName);
        return cameraObject == null ? null : cameraObject.GetComponent<Camera>();
    }

    private void ShowFinalCamera()
    {
        finalShotStarted = true;
        if (finalCamera == null)
        {
            return;
        }

        DisableOtherCameras();
        mainCamera.enabled = false;
        finalCamera.enabled = true;
        SetAudioListener(finalCamera);
    }

    private void DisableOtherCameras()
    {
        Camera[] sceneCameras = FindObjectsOfType<Camera>(true);
        for (int i = 0; i < sceneCameras.Length; i++)
        {
            sceneCameras[i].enabled = sceneCameras[i] == mainCamera ||
                sceneCameras[i] == finalCamera && finalShotStarted;
        }
    }

    private void SetAudioListener(Camera activeCamera)
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].enabled = false;
        }

        AudioListener activeListener = activeCamera.GetComponent<AudioListener>();
        if (activeListener == null)
        {
            activeListener = activeCamera.gameObject.AddComponent<AudioListener>();
        }

        activeListener.enabled = true;
    }
}

public class MovementCameraFollowTarget : MonoBehaviour
{
    private CameraMovementDirector director;

    public void Configure(CameraMovementDirector cameraDirector)
    {
        director = cameraDirector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude <= 0.25f)
        {
            return;
        }

        director.FollowCollisionTarget(transform, collision);
    }
}
