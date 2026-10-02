using UnityEngine;
using System.Collections;

public class RoomTransitionTrigger : MonoBehaviour
{
    public GameObject mainPlayerCameraObj;
    public GameObject topDownCameraObj;

    public float lerpDuration = 1.5f;
    public GameObject backgroundColliderToEnable;
    public Rigidbody playerRigidBody;
    public GameObject[] objectsToEnable;
    public DijkstraPlaygroundManager ref_dijkstraPlaygroundManager;
    public AStarPlaygroundManager ref_aStarPlaygroundManager;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            playerRigidBody.isKinematic = true;

            if (ref_dijkstraPlaygroundManager != null)
            {
                ref_dijkstraPlaygroundManager.InitializePuzzle();
            }

            if (ref_aStarPlaygroundManager != null)
            {
                ref_aStarPlaygroundManager.InitializeAStarRoom();
            }

            if (backgroundColliderToEnable != null)
            {
                backgroundColliderToEnable.SetActive(true);
            }

            StartCoroutine(TransitionFromPlayerToTopDown());
        }
    }

    private void EnableObjects()
    {
        foreach (GameObject obj in objectsToEnable)
            obj.SetActive(true);
    }

    private IEnumerator TransitionFromPlayerToTopDown()
    {
        Transform mainTrans = mainPlayerCameraObj.transform;
        Transform tdTrans = topDownCameraObj.transform;

        Vector3 finalTargetPos = tdTrans.position;
        Quaternion finalTargetRot = tdTrans.rotation;

        tdTrans.position = mainTrans.position;
        tdTrans.rotation = mainTrans.rotation;

        topDownCameraObj.SetActive(true);
        mainPlayerCameraObj.SetActive(false);

        Camera tdCamComponent = topDownCameraObj.GetComponent<Camera>();
        if (tdCamComponent != null)
        {
            AStarCameraSwitch cameraSwitch = FindFirstObjectByType<AStarCameraSwitch>();
            if (cameraSwitch != null)
            {
                cameraSwitch.TriggerCameraSwitchEvent(tdCamComponent);
            }
        }

        float elapsedTime = 0f;
        Vector3 startPos = tdTrans.position;
        Quaternion startRot = tdTrans.rotation;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / lerpDuration);

            tdTrans.position = Vector3.Lerp(startPos, finalTargetPos, t);
            tdTrans.rotation = Quaternion.Slerp(startRot, finalTargetRot, t);

            yield return null;
        }

        playerRigidBody.isKinematic = false;
        EnableObjects();
        tdTrans.position = finalTargetPos;
        tdTrans.rotation = finalTargetRot;
    }
}