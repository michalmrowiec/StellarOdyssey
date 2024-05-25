using UnityEngine;

public class BlinkLight : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("Blink", 1f, 2f);
    }

    void Blink()
    {
        animator.SetTrigger("Blink");
    }

}
