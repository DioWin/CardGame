using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float spawnLength = 10f; // довжина від центру вправо і вліво
    [SerializeField] private int count = 3;
    [SerializeField] private float spawnDelay = 3f;
    [SerializeField] private Transform enemyContainer;

    private void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError($"EnemySpawner: No prefabs assigned. {enemyPrefabs.Count}");
            return;
        }

        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        for (int i = 0; i < count; i++)
        {
            float offset = Random.Range(-spawnLength / 2f, spawnLength / 2f); // випадкове зміщення по X
            Vector3 pos = transform.position + new Vector3(offset, 0f, 0f);

            int index = Random.Range(0, enemyPrefabs.Count);
            var newObject = Instantiate(enemyPrefabs[index], pos, Quaternion.identity, enemyContainer);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(spawnLength, 1f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}
