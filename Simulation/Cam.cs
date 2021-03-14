using System.Collections;
using UnityEngine;



public class Cam : MonoBehaviour {
    
    
    public float speed = 2.0f;
    float moveSpeed = 10.0f;
    float yOffset = 5.0f;
    Vector3 newPosition = new Vector3(0.0f, 0.0f, -10.0f);
    bool zoomOut = false;

    bool focus = true;

    float zoom;
    float zoomSensitivity = 50.0f;
    float zoomSpeed = 1000.0f;
    float zoomMin = 5.0f;
    float zoomMax = 80.0f;


    public void StopFocus()
    {
        focus = false;
    }


    public void StartFocus()
    {
        focus = true;
    }


    
    public void UpdatePosition(Vector3 pos)
    {
        newPosition = pos;
        newPosition.y += yOffset;
    }


    public void ZoomOut()
    {
        newPosition.z -= 30.0f;
        newPosition.y += 5.0f;
        zoomOut = true;
    }


    public void Reset()
    {
        zoomOut = false;
        Vector3 position = this.transform.position;
        position.x = 0.0f;
        position.y = 0.0f;
        position.z = -10.0f;
        this.transform.position = position;
    }


    void Start()
    {
        zoom = Camera.main.fieldOfView;
    }
    

    void FocusOnLastBlock()
    {
        float interpolation = speed * Time.deltaTime;
        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, newPosition.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, newPosition.x, interpolation);

        if (zoomOut)
        {
            position.z = Mathf.Lerp(this.transform.position.z, newPosition.z, interpolation);
        }
        
        this.transform.position = position;
    }

    void Update ()
    {
        if (focus) FocusOnLastBlock();
        ZoomUpdate();
    }

    void LateUpdate()
    {
        ZoomLateUpdate();
        
        if (!focus) Move(); // how can intuitively alert the user they can only move arrows once done with the first block sequence, perhaps show arrows on screen to move around
    }


    void ZoomUpdate()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    }

    void ZoomLateUpdate()
    {
        GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, zoom, Time.deltaTime * zoomSpeed);
    }


    void Move()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
        }
     
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
        }
     
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        }
    }
}





