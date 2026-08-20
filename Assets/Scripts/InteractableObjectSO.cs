using UnityEngine;

[CreateAssetMenu(fileName = "ObjectData", menuName = "ScriptableObjects/InteractableObjectSO")]
public class InteractableObjectSO : ScriptableObject
{
    public Sprite clean;
    public Sprite dirty;
    public Sprite[] trashSprites;
    public Sprite whiteVer;
    public AudioClip[] audioClip;
}
