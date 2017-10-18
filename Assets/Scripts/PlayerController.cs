using UnityEngine;
using System.Collections;

[System.Serializable]

public class Boundary {
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    private AudioSource audioSource;

    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    public Camera camera;

    private float nextFire;
    private Quaternion calibrationQuaternion;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        CalibrateAccellerometer();
    }

    void Update() {
        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            audioSource.Play();
        }
    }

    void CalibrateAccellerometer() {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }

    Vector3 FixAccelleration(Vector3 acceleration) {
        Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
        return fixedAcceleration;
    }

    void FixedUpdate() {
        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        //Vector3 accelerationRaw = Input.acceleration;
        //Vector3 acceleration = FixAccelleration(accelerationRaw);
        //Vector3 movement = new Vector3(acceleration.x, 0.0f, acceleration.y);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 movement = camera.ScreenToWorldPoint(new Vector3(touchDeltaPosition.x, touchDeltaPosition.y, 0.0f) + camera.WorldToScreenPoint(rb.position));
            //rb.velocity = movement * speed;
            //rb.position += movement;
            //Debug.Log(touchDeltaPosition);
            //Debug.Log(movement);
            //rb.transform.Translate(movement.x, 0.0f, movement.z);
            rb.position = new Vector3(
                Mathf.Clamp(movement.x, boundary.xMin, boundary.xMax),
                0.0f,
                Mathf.Clamp(movement.z, boundary.zMin, boundary.zMax)
            );
        }

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }

}
