using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField]
    //values between 0 and 1; determines if the birds will wonder more, if less than 0.5, or group more, if more than 0.5
    public float _flockAttractionCoeficient;

    [SerializeField]
    public float _baseSeparationDist; //base min distance of separation between birds
    [SerializeField]
    public float _baseCohesionDist; //base max distance of seperation between birds

    [SerializeField]
    public float _maxFlockReach;

    public Vector2 _avgFlockDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _avgFlockDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update() {
        GetAverageData();
        multipleFlocksHandler();
    }

    //get all the positions of the nearby birds
    //get all the vectorial directions of the nearby birds~
    //calculate the average position of the birds on the flock reach
    //calculate the average direction of the birds on the flock reach
    private void GetAverageData() {
        Vector3 sumPositions = Vector3.zero;
        Vector2 sumDirections = Vector2.zero;

        int count = 0;
        foreach (Movement bird in FindObjectsOfType<Movement>(false)) {
            if (bird != null) {
                Vector3 birdPosition = bird.transform.position;
                Vector3 distanceVector = birdPosition - transform.position;
                
                if (distanceVector.magnitude <= _maxFlockReach) {
                    sumPositions += birdPosition;
                    sumDirections += bird._targetDirection;
                    count++;
                }
            }
        }
        
        if (count <= 0) {
            transform.position = Vector3.zero;
            _avgFlockDirection = Vector2.zero;
            return;
        }

        transform.position = sumPositions / count;
        _avgFlockDirection = sumDirections / count;
    }

    private void multipleFlocksHandler() {
        Flock[] flocks = FindObjectsOfType<Flock>(false);
        foreach (Flock other in flocks) {
            if (other == this) {
                continue;
            }

            Vector3 otherPosition = other.transform.position;
            Vector3 selfPosition = transform.position;
            Vector3 directionVector = otherPosition - selfPosition;

            if (directionVector.sqrMagnitude <= _maxFlockReach * _maxFlockReach) {
                flockDestroyer(other);
                return;
            }
        }
    }

    private void flockDestroyer(Flock other) {
        if (other != null && other.gameObject != null) {
            DestroyImmediate(other.gameObject);
            //Debug.Log("other flock destroyed");
        }

        if (gameObject != null) {
            DestroyImmediate(gameObject);
            //Debug.Log("self flock destroyed");
        }
    }
}
