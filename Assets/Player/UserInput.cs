using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {

    public float rotateAmount = 10f;
    public float rotateSpeed = 100f;
    public float scrollSpeed = 25f;

    private Ship selectedShip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        MoveCamera();
        RotateCamera();
        MouseActivity();
	}

    private void MoveCamera() {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        bool mouseScroll = false;
        Vector3 movement = new Vector3(0, 0, 0);

        // Direction keys
        if (Input.GetKey(KeyCode.UpArrow)) {
            movement.z += scrollSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            movement.z -= scrollSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            movement.x += scrollSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            movement.x -= scrollSpeed;
        }

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt of the camera to get sensible scrolling
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        // Away from ground movement
        movement.y = scrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        // Calculate desired camera position based on received input
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        // If a change in position is detected perform the necessary update
        if (destination != origin) {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * scrollSpeed);
        }
    }

    private void RotateCamera() {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        // Detect rotation amount if ALT is being held and the Right mouse button is down
        if (Input.GetMouseButton(1)) {
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }

        // If a change in position is detected perform the necessary update
        if (destination != origin) {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }

    private void MouseActivity() {
        if (Input.GetMouseButtonDown(0)) {
            LeftMouseClick();
        } else if (Input.GetMouseButton(1)) {
            RightMouseClick();
        }

        // MouseHover();
    }

    private void LeftMouseClick() {
        GameObject hitObject = FindHitObject(Input.mousePosition);

        if (hitObject) {
            Ship ship = hitObject.transform.parent.GetComponent<Ship>();

            if (ship) {
                selectedShip = ship;
                ship.SetSelection(true);
            }
        }
    }

    private void RightMouseClick() {
        if (selectedShip) {
            selectedShip.SetSelection(false);
            selectedShip = null;
        }
    }

    private GameObject FindHitObject(Vector3 origin) {
        Ray ray = Camera.main.ScreenPointToRay(origin);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            return hit.collider.gameObject;
        } else {
            return null;
        }
    }
}
