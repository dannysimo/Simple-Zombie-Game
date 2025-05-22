using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;       
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float rotationSpeed = 5f;   
    public float minYAngle = -20f; 
    public float maxYAngle = 60f;  

    private float currentX = 0f;   
    private float currentY = 0f;   

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  
        Cursor.visible = false;                   
    }

    void Update()
    {
        
        if (Time.timeScale > 0)
        {
            
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }
        else
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        
        if (Time.timeScale > 0)
        {
            
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position = player.position + rotation * offset;

            
            transform.LookAt(player.position + Vector3.up * 2); 
        }
    }
}