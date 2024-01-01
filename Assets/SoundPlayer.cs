using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource attack;
    public AudioSource specialAttack;
    public AudioSource damage;
    public AudioSource electric;

    public void PlayAttackSound() 
    {
        attack.Play();
    }

    public void PlaySpecialAttackSound()
    {
        specialAttack.Play();
    }

    public void PlayDamageSound()
    {
        damage.Play();
    }

    public void PlayElectricSound()
    {
        if (electric != null)
        {
            electric.mute = false;
            electric.Play();
        }
    }

    public void MuteElectricSound()
    {
        if (electric != null)
        {
            electric.mute = true;
        }        
    }
}
