using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanController : MonoBehaviour
{  // this class is control the player's bean character. 
    [SerializeField]
    Bean bean;
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    Transform liftUp;
    [SerializeField]
    Transform liftDown;
    Rigidbody myRigidbody;
    Vector3 lastMousePosition;
    [SerializeField, Min(0.01f)]
    Vector2 mouseSensitivity;
    [SerializeField, Min(0.01f)]
    float MouseWheelZoomLevel = 1.0f;
    Vector3 Movement;

    public bool isInputable
    {
        set; get;
    }
    public bool OnGround { get; set; }
    
    public bool hand{get; set;}
    public ILiftable liftObject { get; set; }
    
    

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        isInputable = true;
        
        lastMousePosition = Input.mousePosition;
        MouseLocker.HideMouse();

        OnGround = true;
        hand = false;
        liftObject = null;
    }

    void Update()
    {
        if (isInputable)
        {
            if(MouseLocker.mouseLocked){                
                RotateCamera();
            }
            ZoomInOut();
            MovePlayerByInput();
            Interact();
            
            if(OnGround){
                Vector3 vel = Movement.normalized * bean.MoveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2.5f : 1);
                vel.y = myRigidbody.velocity.y;
                myRigidbody.velocity = vel;
            }
            bean.UpdateMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    
    private void ZoomInOut(){
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if(wheelInput != 0){
            cameraController.Distance = cameraController.Distance - wheelInput * MouseWheelZoomLevel;
        }
    }

    void RotateCamera()
    {
        Vector3 deltaMousePosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);  //Input.mousePosition - lastMousePosition; 
        Vector3 cameraEular = cameraController.transform.rotation.eulerAngles;
        float cameraEularX = cameraEular.x - deltaMousePosition.y * mouseSensitivity.y;
        if (cameraEularX < 180f)
        {
            cameraEularX = Mathf.Clamp(cameraEularX, -1f, 70.0f);
        }
        else
        {
            cameraEularX = Mathf.Clamp(cameraEularX, 355f, 361.0f);
        }
        cameraController.transform.rotation = Quaternion.Euler(cameraEularX, cameraEular.y + deltaMousePosition.x * mouseSensitivity.x, cameraEular.z);
        //Vector3 rotation = transform.rotation.eulerAngles;
        //transform.rotation = Quaternion.Euler(rotation.x, rotation.y + deltaMousePosition.x, rotation.z);
        lastMousePosition = Input.mousePosition;
    }

    void MovePlayerByInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && OnGround )
        {
            bean.animator.SetTrigger("Jump");
        }
        Movement = bean.Movement = Input.GetAxisRaw("Horizontal") * cameraController.Right + Input.GetAxisRaw("Vertical") * cameraController.Forward;
        if(Input.GetKeyDown(KeyCode.LeftAlt)){
            MouseLocker.ShowMouse();
        }else if(Input.GetKeyUp(KeyCode.LeftAlt)){
            MouseLocker.HideMouse();
        }
    }

    void Interact()
    {
        if (Input.GetMouseButtonDown(0) && liftObject != null) { 
            if(!hand){
                hand = true;
                bean.animator.SetBool("Hand", hand);
                liftObject.LeftShift(liftUp);
            }else{
                hand = false;
                bean.animator.SetBool("Hand", hand);
                liftObject.LeftShift(liftDown);
            }
        }
    }

    public void Jump(float power)
    {
        myRigidbody.AddForce(Vector3.up * power * 0.6f);
        OnGround = false;
    }

    public void Landing(){
        OnGround = true;
        bean.animator.SetTrigger("Landing");
    }

    public void OnSencor(string sencorName)
    {
        if(sencorName == "Foot" && !OnGround )
        {
            OnGround = true;
            bean.animator.SetTrigger("Landing");
        }
    }
}
