using UnityEngine;

public class ColorMatcher : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    [SerializeField] Renderer projectileRenderer;

    // Update is called once per frame
    void LateUpdate() => trail.material.color = projectileRenderer.material.color;
}