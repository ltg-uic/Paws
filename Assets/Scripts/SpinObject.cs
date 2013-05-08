using UnityEngine;
using System.Collections;

public class example : MonoBehaviour {
    void Update() {
        transform.Rotate(Vector3.right * Time.deltaTime *10);
        transform.Rotate(Vector3.up * Time.deltaTime, Space.Self);
    }
}