using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource ambienceAudioSource;
    [SerializeField] private AudioSource sfxAudioSource; // for other sfx
    [SerializeField] private AudioSource scrubAudioSource; // for scrub
    [SerializeField] private AudioSource dryerAudioSource; // for blow dryer
    [SerializeField] private AudioSource ghostAudioSource; // for ghost
    [SerializeField] private AudioSource roomAmbienceAudioSource; // e.g. bedroom or bathroom ambience

    [Header("Room ambience audio clips")]
    [SerializeField] private AudioClip bedroomAmbience;
    [SerializeField] private AudioClip bathroomAmbience;


    private bool ghostCanLaugh = false;

    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    public void PlayBGM()
    {
        bgmAudioSource.Play();
    }

    public void PauseBGM(bool c)
    {
        if(c)
        {
            bgmAudioSource.Pause();
        }
        else
        {
            bgmAudioSource.UnPause();
        }
    }

    public void PlayScrub(InteractableObjectSO objectDataSO)
    {
        if(scrubAudioSource.isPlaying)
        {
            return;
        }
        AudioClip randomClip = objectDataSO.audioClip[UnityEngine.Random.Range(0, objectDataSO.audioClip.Length)];
        scrubAudioSource.PlayOneShot(randomClip);
    }
    public void EndScrub()
    {
        scrubAudioSource.Stop();
    }

    public void PlayDryer(InteractableObjectSO objectDataSO)
    {
        if(dryerAudioSource.isPlaying)
        {
            return;
        }
        AudioClip randomClip = objectDataSO.audioClip[UnityEngine.Random.Range(0, objectDataSO.audioClip.Length)];
        dryerAudioSource.PlayOneShot(randomClip);
    }
    public void EndDryer()
    {
        dryerAudioSource.Stop();
    }

    public void PlayAudioClip(InteractableObjectSO objectDataSO)
    {
        AudioClip randomClip = objectDataSO.audioClip[UnityEngine.Random.Range(0, objectDataSO.audioClip.Length)];
        PlayAudioClip(randomClip);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void ActivateGhostSounds(bool c)
    {
        ghostCanLaugh = c;
    }

    public void GhostLaugh(AudioClip[] audios)
    {
        if(!ghostCanLaugh)
        {
            return;
        }
        AudioClip randomClip = audios[UnityEngine.Random.Range(0, audios.Length)];
        ghostAudioSource.PlayOneShot(randomClip);
        
    }

    public void PlayRoomAmbience()
    {
        if(GameManager.instance.currentPlayerLocation == "Bedroom")
        {
            roomAmbienceAudioSource.PlayOneShot(bedroomAmbience);
        }
        else if(GameManager.instance.currentPlayerLocation == "Bathroom")
        {
            roomAmbienceAudioSource.PlayOneShot(bathroomAmbience);
        }
    }

    



}
