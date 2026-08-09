using UnityEngine;

public class GlobalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _birdPrefab;
    [SerializeField]
    private GameObject _flockPrefab;
    [SerializeField]
    private float _numberOfBirds;
    [SerializeField]
    private float _maxPositionRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        GameObject flock = Instantiate(_flockPrefab, transform.position, transform.rotation);

        int i;
        for (i = 0; i < _numberOfBirds; i++) {
            GameObject bird = Instantiate(_birdPrefab, GetRandomPosition(), GetRandomRotation());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetRandomPosition() {
        return new Vector3(Random.Range(_maxPositionRange*(-1),_maxPositionRange),
        Random.Range(_maxPositionRange*(-1),_maxPositionRange), 0);
    }

    private Quaternion GetRandomRotation() {
        return Quaternion.Euler(0, 0, Random.Range(0,360));
    }
}
