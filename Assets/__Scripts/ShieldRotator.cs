using UnityEngine;

public class ShieldRotator : MonoBehaviour
{
    public Vector3 rotateVector;

    void FixedUpdate() => transform.localRotation *= Quaternion.Euler(rotateVector * Time.fixedDeltaTime);
}