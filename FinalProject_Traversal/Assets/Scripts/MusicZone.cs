using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip zoneMusic;
    private AudioSource mainAudio;

    void Start()
    {
        if (Camera.main != null)
        {
            mainAudio = Camera.main.GetComponent<AudioSource>();
            if (mainAudio == null) 
            {
                mainAudio = Camera.main.gameObject.AddComponent<AudioSource>();
            }
            mainAudio.loop = true; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object is the Main Camera, has a Camera component, or is the Player
        if (other.CompareTag("MainCamera") || other.GetComponentInChildren<Camera>() != null || other.CompareTag("Player"))
        {
            if (mainAudio != null && mainAudio.clip != zoneMusic)
            {
                mainAudio.clip = zoneMusic;
                mainAudio.Play();
                Debug.Log("Music changed to: " + zoneMusic.name);
            }
        }
    }
}