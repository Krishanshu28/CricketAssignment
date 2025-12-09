using UnityEngine;

public class BowlingSystem : MonoBehaviour
{
    public Transform ballSpawnPoint;
    public Transform targetMarker;
    public Rigidbody rb;
    public Transform mainCamera;

    [Header("Marker Settings")]
    public float markerMoveSpeed = 5f;
    public float xLimit = 3f;
    public float zLimitMin = 1f;
    public float zLimitMax = 18f;

    [Header("Wicket Settings")]
    public float wicketOffset = 1.5f;
    private bool isOverTheWicket = true;
    private Vector3 initialCameraPos;

    private float activeSwingForce = 0f;
    private float activeSpinForce = 0f;
    private float activeBallSpeed = 25f;
    private bool inAir = false;
    private bool ballThrown = false;

    void Start()
    {
        initialCameraPos = mainCamera.localPosition;
        UpdateSidePosition();
        
    }

    void Update()
    {
        if(!ballThrown)
        {
            MoveMarker();
        }
        
    }

    void FixedUpdate()
    {
        if(inAir && activeSwingForce != 0)
        {
            rb.AddForce(new Vector3(activeSwingForce, 0, 0), ForceMode.Acceleration);
        }
    }

    //side change
    public void ToggleSideChange()
    {
        isOverTheWicket = !isOverTheWicket;
        UpdateSidePosition();
    }

    private void UpdateSidePosition()
    {
        float xPos = isOverTheWicket ? -wicketOffset : wicketOffset;

        Vector3 newSpawnPos = ballSpawnPoint.localPosition;
        newSpawnPos.x = xPos;
        ballSpawnPoint.localPosition = newSpawnPos;

        Vector3 newCameraPos = initialCameraPos;
        newCameraPos.x = xPos * 0.8f;
        mainCamera.localPosition = newCameraPos;
    }

    public void ThrowBall(float speed, float swingVal, float spinVal)
    {
        if(ballThrown) return;
        ballThrown = true;
        inAir = true;

        activeBallSpeed = speed;
        activeSwingForce = swingVal;
        activeSpinForce = spinVal;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = ballSpawnPoint.position;
        rb.rotation = Quaternion.identity;

        Vector3 displacement = targetMarker.position - ballSpawnPoint.position;
        Vector3 planarDisp = new Vector3(displacement.x, 0, displacement.z);

        float time = planarDisp.magnitude / activeBallSpeed;

        Vector3 gravity = Physics.gravity;
        Vector3 swingVec = new Vector3(activeSwingForce, 0, 0);
        Vector3 totalAcc = gravity + swingVec;

        Vector3 initialVel = (displacement - (0.5f * totalAcc *(time * time))) / time;

        rb.linearVelocity = initialVel;
        
    }

    private void MoveMarker()
    {
        float x = Input.GetAxis("Horizontal") * markerMoveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * markerMoveSpeed * Time.deltaTime;

        Vector3 newPos = targetMarker.position + new Vector3(x, 0, z);

        newPos.x = Mathf.Clamp(newPos.x, -xLimit, xLimit);
        newPos.z = Mathf.Clamp(newPos.z, ballSpawnPoint.position.z + zLimitMin, ballSpawnPoint.position.z + zLimitMax);

        targetMarker.position = newPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        inAir = false;

        if(collision.gameObject.CompareTag("Pitch"))
        {
            Vector3 spin = new Vector3(activeSpinForce, 0, 0);
            rb.AddForce(spin, ForceMode.VelocityChange);
        }
    }

    public void ResetSystem()
    {
        ballThrown = false;
        inAir = false;
        activeSwingForce = 0;
        activeSpinForce = 0;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.position = ballSpawnPoint.position;
    }
}
