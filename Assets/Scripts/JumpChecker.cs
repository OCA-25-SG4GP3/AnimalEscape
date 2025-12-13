using System;
using UnityEngine;

public class JumpChecker : MonoBehaviour
{
    [SerializeField] private AnimalControlSimple animalControl;

    [NonSerializedAttribute] public bool isGrounded = false;

    void OnTriggerEnter(Collider other)
    {
        // 地面に着地したことを判定
        // 地面と接触した場合にSEを再生
        isGrounded = false;

        if (other.gameObject != animalControl.gameObject) // ignore self
        {
            animalControl.isJumping = false;  // ジャンプフラグを元に戻す
            isGrounded = true; //着地

            //// 着地のSEを再生
            //audioSource.PlayOneShot(landingSound);

            PlayLandingSound();
       
        }
    }
    private void PlayLandingSound()
    {
        if (animalControl.audioSource != null && animalControl.landingSound != null)
        {
            if (!animalControl.audioSource.isPlaying)
            {
                animalControl.audioSource.clip = animalControl.landingSound;
                animalControl.audioSource.Play(); //トラッキングしたいので、AudioSource使う
            }
        }
    }

}
