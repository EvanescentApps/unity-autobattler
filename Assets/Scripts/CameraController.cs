using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5f; 

    void Update()
    {
        float moveForward = Input.GetKey(KeyCode.S) ? 1 : (Input.GetKey(KeyCode.W) ? -1 : 0);
        float moveSideways = Input.GetKey(KeyCode.D) ? -1 : (Input.GetKey(KeyCode.A) ? 1 : 0);

        float zoom = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) zoom = 2f;
        else if (Input.GetKey(KeyCode.DownArrow)) zoom = -2f;

        transform.Translate(0, 0, zoom * speed * Time.deltaTime, Space.Self);

        Vector3 move = new Vector3(moveSideways, 0, moveForward);

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}

