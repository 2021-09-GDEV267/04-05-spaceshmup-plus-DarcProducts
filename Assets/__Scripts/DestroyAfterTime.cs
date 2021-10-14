using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 1;

    void Start() => StartCoroutine(nameof(DelayedDestroy));

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);
        Destroy(gameObject);
    }
}