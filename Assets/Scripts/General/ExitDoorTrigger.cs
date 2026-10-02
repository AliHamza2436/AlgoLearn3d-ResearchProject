using UnityEngine;

public class ExitDoorTrigger : MonoBehaviour
{
    public GameObject loadingPanel;

    private void Awake()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
        }
    }
}