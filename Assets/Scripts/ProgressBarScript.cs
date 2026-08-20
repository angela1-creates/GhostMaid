using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{

    [SerializeField] private Image progressBar;

    [SerializeField] private float totalCleaned, totalDirtied;


    // Update is called once per frame
    void Update()
    {
        SetProgress();
    }

    private void SetProgress()
    {
        progressBar.fillAmount = totalCleaned / totalDirtied;
    }
}
