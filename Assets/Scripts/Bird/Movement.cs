using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private GameObject _flockPrefab;

    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private float _panicSpeedCoeficient;

    [SerializeField]
    private float _rotationSpeed;

    //The 3 rule values
    [SerializeField]
    private float _SeparationCoef; //multiplies whit the base value of the flock
    [SerializeField]
    private float _CohesionCoef; //multiplies whit the base value of the flock

    [SerializeField]
    private float _uTurnAngle;
    
    private Rigidbody2D _rigidBody;
    public Vector2 _targetDirection;
    private Flock _flockAwarness;
    private Transform _flockTransform;

    private float _distanceCenterFlock;

    private Quaternion _targetRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _flockAwarness = FindObjectOfType<Flock>();
        _flockTransform = FindObjectOfType<Flock>().transform;
    }

    void Update() {
        updateFlockData();
        UpdateFlockCenterDirection();
        RotateTowardsFlockCenter();
        SetVelocity();
    }

    //flock corelation
    private void updateFlockData() {
        float minDistance = 100000;
        Flock minFlock = null;

        foreach (Flock flock in FindObjectsOfType<Flock>(false)) {
            Vector3 distanceVector = flock.transform.position - transform.position;
            float distance = distanceVector.magnitude;
            if (distance <= flock._maxFlockReach) {
                if (distance <= minDistance ) {
                    minDistance = distance;
                    minFlock = flock;
                }
            }
        }

        if (minFlock == null) {
            GameObject objFlock = Instantiate(_flockPrefab, transform.position, transform.rotation);
            minFlock = objFlock.GetComponent<Flock>();
            //Debug.Log("flock created");
        }

        _flockAwarness = minFlock;
        _flockTransform = minFlock.transform;

        _distanceCenterFlock = Vector2.Distance(transform.position, _flockTransform.position);
    }

    //individual movement
    private void UpdateFlockCenterDirection() {
        Vector2 centerFlockToBird = _flockTransform.position - transform.position;
        float coef = _flockAwarness._flockAttractionCoeficient;
        Vector2 flockDirection = _flockAwarness._avgFlockDirection;
        
        Vector2 weightedSum = (centerFlockToBird * coef) + (flockDirection * (1-coef));
        Vector2 Sum = centerFlockToBird + flockDirection;

        Vector2 averageDirection = weightedSum / Sum;

        _targetDirection = averageDirection.normalized;
    }

    private void RotateTowardsFlockCenter() {
        if (_flockAwarness._baseSeparationDist * _SeparationCoef >= _distanceCenterFlock)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.forward, _targetDirection);
            _targetRotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);

        } else if (_flockAwarness._baseCohesionDist * _CohesionCoef < _distanceCenterFlock) {
            Quaternion rotation = Quaternion.LookRotation(transform.forward, _targetDirection) * Quaternion.Euler(0,0,_uTurnAngle);
            _targetRotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);

        } else {
            return;
        }

        _rigidBody.SetRotation(_targetRotation);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Bird")) {
            transform.rotation *= Quaternion.Euler(0f,0f,_uTurnAngle);
        }

        if (other.gameObject.CompareTag("Border")) {
            transform.position *= -1;
        }
    }

    private void SetVelocity() {
        _rigidBody.linearVelocity = transform.up * _movementSpeed;
    }

    //when the bird is reproducing
    private void FindCouple() {
        //after an X amount of time it tries to coolide whit another bird
        //calls a public method from the Reproduction.cs file to send the coolision bird info and receive back the id
        //the public method will send an id to both birds, and awaits bird's response
        //when a bird receives multiple ids, the first one is accepted, and the others are rejected
        //if id accepted: couple created and invoke private SendValue method after the reproduction timer is over; 
        //if rejected: tries to coolide whit someone else before the reproduction timer ends
        //(Idea 2: if rejected, it is automatically destroyed)
        //coupless birds are destroyed after the reproduction timer whithout passing it's values to the next gen
    }
    private void SendValues(float coupleID) {
        //invoques a public method from the Reproduction.cs file and sends the data whit it's unique couple ID
        //only the two birds of the same couple have the same ID
        //then it destrois it self
    }
}
