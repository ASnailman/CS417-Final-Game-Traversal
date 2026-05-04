using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip zoneMusic;
    private AudioSource mainAudio;

    void Start()
    {
        mainAudio = Camera.main.GetComponent<AudioSource>();
        if (mainAudio == null) mainAudio = Camera.main.gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && mainAudio.clip != zoneMusic)
        {
            mainAudio.clip = zoneMusic;
            mainAudio.Play();
        }
    }
}