using System;
using UnityEngine;

public class JumpChecker : MonoBehaviour
{
    [SerializeField] private AnimalControlSimple animalControl;
    GameClearManager gameClearManager;
    [NonSerializedAttribute] public bool isGrounded = false;

    private void Awake()
    {
        gameClearManager = GameObject.FindAnyObjectByType<GameClearManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 地面に着地したことを判定
        // 地面と接触した場合にSEを再生

        if (other.gameObject != animalControl.gameObject) // ignore self
        {
            // Set justLandedFromJump flag if we were jumping
            var playerInfo = animalControl.GetComponent<PlayerInfo>();
            if (playerInfo != null && animalControl.isJumping)
            {
                playerInfo.justLandedFromJump = true;
            }

            // Delay resetting isJumping by one frame to allow other triggers (ColorPanel) to check it first
            StartCoroutine(ResetJumpingNextFrame());
            if(!gameClearManager.isFinish) animalControl.UnlockInput();

            //animal.SetMoveSpeed(animal.baseMoveSpeed);
            isGrounded = true; //着地

            //// 着地のSEを再生
            //audioSource.PlayOneShot(landingSound);

            PlayLandingSound();

        }
    }

    private System.Collections.IEnumerator ResetJumpingNextFrame()
    {
        yield return null; // Wait one frame
        animalControl.isJumping = false;  // ジャンプフラグを元に戻す
    }

    void OnTriggerStay(Collider other)
    {
        // Continuously confirm ground contact while staying on ground
        if (other.gameObject != animalControl.gameObject) // ignore self
        {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Left the ground
        if (other.gameObject != animalControl.gameObject) // ignore self
        {
            isGrounded = false;

            // Clear justLandedFromJump when leaving ground
            var playerInfo = animalControl.GetComponent<PlayerInfo>();
            if (playerInfo != null)
            {
                playerInfo.justLandedFromJump = false;
            }
        }
    }
    private void PlayLandingSound()
    {
        if (animalControl.audioSourceWalk != null && animalControl.landingSound != null)
        {
            //if (!animalControl.audioSource.isPlaying)
            {
                //This is not possible because it's audioSource is either used or stopped every frame.
                //animalControl.audioSource.clip = animalControl.landingSound;
                //animalControl.audioSource.Play(); //トラッキングしたいので、AudioSource使う
                animalControl.PlaySFX(animalControl.landingSound, 1.0f); //トラッキングしたいので、AudioSource使う
            }
        }
    }

}
