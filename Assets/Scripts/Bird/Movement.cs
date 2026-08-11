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
    [SerializeField]
    private float _gravityCoeficient; //changes the force of gravity
    [SerializeField]
    private bool _gravityToggle; //toggle the slight pull down directional vector
    
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
        //update info whit the flock
        updateFlockData();
        UpdateBirdDirection();
        
        //updates the direction of movement
        RotateTowardsFlockCenter();

        //makes the bird move
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
    private void UpdateBirdDirection() {
        Vector2 centerFlockToBird = _flockTransform.position - transform.position;
        float coef = _flockAwarness._flockAttractionCoeficient;
        Vector2 flockDirection = _flockAwarness._avgFlockDirection;
        Vector2 gravityVector = GetGravityVector();
        
        Vector2 weightedSum = (centerFlockToBird * (1 - coef)) + (flockDirection * coef);
        Vector2 Sum = centerFlockToBird + flockDirection;

        Vector2 averageDirection = weightedSum / Sum;

        if (_gravityToggle) {
            weightedSum = (averageDirection * coef) + (gravityVector * (1-coef));
            Sum = centerFlockToBird + gravityVector;

            averageDirection = weightedSum / Sum;
        }

        _targetDirection = averageDirection.normalized;
    }

    private void RotateTowardsFlockCenter() {
        if (_flockAwarness._baseSeparationDist * _SeparationCoef >= _distanceCenterFlock)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.forward, _targetDirection);
            rotation = RotateSprite(rotation);
            _targetRotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);

        } else if (_flockAwarness._baseCohesionDist * _CohesionCoef < _distanceCenterFlock) {
            Quaternion rotation = Quaternion.LookRotation(transform.forward, _targetDirection) * Quaternion.Euler(0,0,_uTurnAngle);
            rotation = RotateSprite(rotation);
            _targetRotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);

        } else {
            return;
        }
        _rigidBody.SetRotation(_targetRotation);
    }

    //TO FIX: birds aren't rotating in the X and Y axis???
    private Quaternion RotateSprite(Quaternion rotateTowards) { //for aesthetic reasons
        //When the rotation of the Z axis of the bird is in between:
        //- [0º,90º[: the rotation of the bird in the X and Y axis will be 0º;
        //- [90º,180º[: the rotation of the bird in the X axis will be 180º and in the Y axis will be 0º;
        //- [180º,270º[: the rotation of the bird in the X and Y axis will be 180º;
        //- [270º,360º[: the rotation of the bird in the X axis will be 0º and in the Y axis will be 180º.

        float Z = rotateTowards.eulerAngles.z;
        Quaternion spriteRotation;


        if (Z >= 0 && Z < 90) spriteRotation = Quaternion.Euler(0,0,Z);
        else if (Z >= 90 && Z < 180) spriteRotation = Quaternion.Euler(180,0,Z);
        else if (Z <= 0 && Z > -90) spriteRotation = Quaternion.Euler(180,180,Z);
        else if (Z <= -90 && Z > -180) spriteRotation = Quaternion.Euler(0,180,Z);
        else spriteRotation = Quaternion.Euler(0,0,Z);
        //Debug.Log($"Z = {Z}");
        //Debug.Log($"spriteRotation = {spriteRotation.eulerAngles}");

        return spriteRotation;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Bird")) {
            transform.rotation *= RotateSprite(Quaternion.Euler(0f,0f,_uTurnAngle));
        }

        if (other.gameObject.CompareTag("Border")) {
            transform.position *= -1;
        }
    }

    private void SetVelocity() {
        _rigidBody.linearVelocity = transform.up * _movementSpeed;
    }

    private Vector2 GetGravityVector() {
        return Vector2.down * _gravityCoeficient;
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
