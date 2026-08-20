using UnityEngine;
using UnityEngine.SceneManagement;


public class CutsceneUI : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PlaySound(AudioClip clip)
    {
        AudioManager.instance.PlayAudioClip(clip);
    }

    public void PlayMusic()
    {
        AudioManager.instance.PlayBGM();
    }
}
