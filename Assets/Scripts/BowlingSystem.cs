using UnityEngine;

public class BowlingSystem : MonoBehaviour
{
    [Header("References")]
    public Transform ballSpawnPoint;
    public Transform targetMarker;
    public Rigidbody rb;
    public Transform mainCamera;

    [Header("Settings")]
    public float markerMoveSpeed = 5f;
    public float xLimit = 3f;
    public float zLimitMin = 2f;
    public float zLimitMax = 20f;
    public float wicketOffset = 1.5f;

    [Header("Physics Tuning")]
    public float groundImpactHeight = 0.2f; 

    private float activeSwingForce = 0f;
    private float activeSpinForce = 0f;
    private bool ballThrown = false;
    private bool inAir = false;
    private bool isOverTheWicket = true;
    private Vector3 initialCameraPos;

    void Start() {
        if (mainCamera != null) initialCameraPos = mainCamera.localPosition;
        UpdateSidePosition(); 
    }

    void Update() {
        if (!ballThrown) {
            MoveMarker();
        }
    }

    void FixedUpdate() {
        if (inAir && activeSwingForce != 0) {
            rb.AddForce(new Vector3(activeSwingForce, 0, 0), ForceMode.Acceleration);
        }
    }

    public void ThrowBall(float speed, float swingVal, float spinVal) {
        if (ballThrown) return;
        ballThrown = true;
        inAir = true;
        activeSwingForce = swingVal;
        activeSpinForce = spinVal;

        rb.isKinematic = false; 
        rb.position = ballSpawnPoint.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 start = ballSpawnPoint.position;
        Vector3 target = targetMarker.position;

        Vector3 targetCenter = new Vector3(target.x, groundImpactHeight, target.z);

        Vector3 displacement = targetCenter - start;
        Vector3 totalAcceleration = Physics.gravity + new Vector3(swingVal, 0, 0);

        
        float time = displacement.magnitude / speed;

        
        Vector3 requiredVelocity = (displacement - (0.5f * totalAcceleration * time * time)) / time;

        
        rb.velocity = requiredVelocity;
    }

    
    public void ResetSystem() {
        ballThrown = false;
        inAir = false;
        activeSwingForce = 0;
        activeSpinForce = 0;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; 
        
        rb.position = ballSpawnPoint.position;
        rb.rotation = Quaternion.identity;
    }

    public void ToggleSideChange() {
        isOverTheWicket = !isOverTheWicket;
        UpdateSidePosition();
    }

    private void UpdateSidePosition() {
        float xPos = isOverTheWicket ? -wicketOffset : wicketOffset;
        ballSpawnPoint.localPosition = new Vector3(xPos, ballSpawnPoint.localPosition.y, ballSpawnPoint.localPosition.z);
        if (mainCamera != null) mainCamera.localPosition = new Vector3(xPos * 0.8f, initialCameraPos.y, initialCameraPos.z);
    }

    private void MoveMarker() {
        float x = Input.GetAxis("Horizontal") * markerMoveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * markerMoveSpeed * Time.deltaTime;
        Vector3 newPos = targetMarker.position + new Vector3(x, 0, z);
        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);
        newPos.z = Mathf.Clamp(newPos.z, zLimitMin, zLimitMax);
        targetMarker.position = newPos;
    }

    void OnCollisionEnter(Collision col) {
        inAir = false;
        if (col.gameObject.CompareTag("Pitch")) {
            rb.AddForce(new Vector3(activeSpinForce, 0, 0), ForceMode.VelocityChange);
        }
    }
}