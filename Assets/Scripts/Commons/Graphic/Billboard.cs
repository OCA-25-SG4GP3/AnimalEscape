using UnityEngine;

public class Billboard : MonoBehaviour //Quickly rotates object to camera
{
    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
