using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] HotspotController hotspotController; // lack of time

    public void SwitchAudioAtPainting(GameObject stand)
    {
        if (audioSource == null || stand == null) return;
        if (stand.TryGetComponent<PaintingData>(out var data))
        {
            AudioClip clip = data.paintingData.paintingClip;
            if (clip != null)
            {
                if (clip == audioSource.clip)
                {
                    if (audioSource.isPlaying)
                        audioSource.Pause();
                    else
                        audioSource.UnPause();
                }
                else
                {
                    audioSource.clip = clip;
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                    else
                        audioSource.Pause();
                }
            }
        }
    }

    public void PlayAudioFromClosest()
    {
        GameObject stand = hotspotController.GetClosestHotspotGameObject();
        if (stand == null) return;
        SwitchAudioAtPainting(stand);
    }

    public void StopPlayingAudio()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }

    public void PauseAudio()
    {
        if (audioSource == null) return;
        audioSource.Pause();
    }
}
