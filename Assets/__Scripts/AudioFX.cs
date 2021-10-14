using UnityEngine;

public class AudioFX : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clip;
    [SerializeField] Vector2 minMaxVolume;
    [SerializeField] Vector2 minMaxPitch;

    public void ActivateFX()
    {
        if (source != null && clip != null)
        {
            if (!source.isPlaying)
            {
                source.volume = Random.Range(minMaxVolume.x, minMaxVolume.y);
                source.pitch = Random.Range(minMaxPitch.x, minMaxPitch.y);
                source.PlayOneShot(clip);
            }
        }
    }

    public void ActivateFXWithoutPlayCheck()
    {
        if (source != null && clip != null)
        {
            source.volume = Random.Range(minMaxVolume.x, minMaxVolume.y);
            source.pitch = Random.Range(minMaxPitch.x, minMaxPitch.y);
            source.PlayOneShot(clip);
        }
    }

    public void StopAudio() => source.Stop();

    public float GetClipLength()
    {
        if (clip != null)
            return clip.length;
        return 0;
    }
}