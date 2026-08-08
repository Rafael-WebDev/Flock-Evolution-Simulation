using UnityEngine;

public class GlobalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _birdPrefab;
    [SerializeField]
    private GameObject _flockPrefab;
    [SerializeField]
    private float _numberOfBirds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        GameObject flock = Instantiate(_flockPrefab, transform.position, transform.rotation);

        int i;
        for (i = 0; i < _numberOfBirds; i++) {
        GameObject bird = Instantiate(_birdPrefab, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
