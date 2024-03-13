using System.Collections;
using UnityEngine;

public class EnterSoundFade : MonoBehaviour 
{
    public AudioSource audioSource;
    public float audioPlayDuration = 10f; // Duration in seconds the audio plays before starting to fade out.
    public float fadeOutDuration = 2f; // Duration in seconds of the fade-out effect.
    public bool allowTriggerAgain = false; // Controls whether the audio can be triggered more than once.

    private Coroutine fadeOutCoroutine;
    private bool hasBeenTriggered = false; // Tracks whether the audio has been triggered.

    void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area, if the audio is not already playing, and if it hasn't been triggered before.
        if (other.CompareTag("Player") && !audioSource.isPlaying && !hasBeenTriggered)
        {
            audioSource.Play();
            hasBeenTriggered = true; // Mark as triggered to prevent future playback.

            // If there's an ongoing fade-out, stop it and reset volume to ensure smooth playback.
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                audioSource.volume = 1f; // Reset volume to max in case it was faded out previously.
            }
            // Start a new coroutine to stop the audio after a certain time.
            StartCoroutine(StopAudioAfterTime(audioPlayDuration));
        }
    }

    IEnumerator StopAudioAfterTime(float delay)
    {
        // Wait for the specified delay duration before starting to fade out.
        yield return new WaitForSeconds(delay);
        // Start fading out the audio.
        fadeOutCoroutine = StartCoroutine(FadeOutAudio(fadeOutDuration));
    }

    IEnumerator FadeOutAudio(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Optionally reset the volume after stopping if you plan to play it again.

        // Optionally allow the trigger to be re-enabled based on the public variable
        if (allowTriggerAgain)
        {
            hasBeenTriggered = false;
        }
    }
}
