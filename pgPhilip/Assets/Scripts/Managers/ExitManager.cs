using UnityEngine;

public class ExitManager : MonoBehaviour
{
    public GameObject exitPortal;
    public Material ExitRedMAT;
    public Material ExitGreenMAT;

    private int enemyCount;
    private bool portalActive = false;

    void Start()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        exitPortal.SetActive(true);
        UpdateExitMaterial();
    }

    public void EnemyDefeated()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            ActivatePortal();
        }
    }

    private void ActivatePortal()
    {
        portalActive = true;
        UpdateExitMaterial();
    }

    private void UpdateExitMaterial()
    {
        if (exitPortal != null)
        {
            Renderer renderer = exitPortal.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = portalActive ? ExitGreenMAT : ExitRedMAT;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (portalActive)
            {
                Debug.Log("Player entered the portal!");
            }
            else
            {
                Debug.Log("Portal is not active yet!");
            }
        }
    }
}
