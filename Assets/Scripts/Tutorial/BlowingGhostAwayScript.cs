using UnityEngine;
using System.Collections;


public class BlowingGhostAwayScript : MonoBehaviour
{
    public static BlowingGhostAwayScript instance;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private Animator animator;
    [SerializeField] private AnimationClip happyGhostAnim;



    private void Awake()
    {
        if(instance==null){instance=this;}
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        defaultPosition = transform.position;
        animator = this.GetComponent<Animator>();
    }

    public void ResetHappyGhostPosition()
    {
        Debug.Log("reseting happy ghost");
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
    }

    public IEnumerator PlayAnimation()
    {
        animator.enabled = true;
        animator.SetBool("isActive", true);
        yield return new WaitForSeconds(happyGhostAnim.length);
        animator.SetBool("isActive", false);
        animator.enabled = false;
        ResetHappyGhostPosition();
        this.gameObject.SetActive(false);
    }
}
