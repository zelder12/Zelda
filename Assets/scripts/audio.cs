using UnityEngine;

public class SwordAttackSound : MonoBehaviour
{
    public AudioSource swordAttackSound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlaySwordAttackSound();
        }
    }

    void PlaySwordAttackSound()
    {
        if (swordAttackSound != null && !swordAttackSound.isPlaying)
        {
            swordAttackSound.Play();
        }
    }
}
