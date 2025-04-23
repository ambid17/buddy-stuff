using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    Checkpoint, Collectible, Respawn, Win,
    Footstep, Land, TerminalVelocity, 
    PortalOpen, PortalIdle, PortalRejected, PortalPassThrough
}

public class PlayerSoundEffects : StaticMonoBehaviour<PlayerSoundEffects>
{
    // Movement
    [Header("Movement audioSources")]
    public AudioSource footstepAudioSource;
    public AudioSource landAudioSource;
    public AudioSource terminalVelocityAudioSource;

    // Timers
    float footstepTimer;
    float footstepInverval = 0.4f;

    float landingTimer;
    float landingInterval = 0.5f;

    [Header("Footstep sounds")]
    public AudioClip footstep1;
    public AudioClip footstep2;
    private bool lastFootstep1;

    private void Update()
    {
        Instance.footstepTimer += Time.deltaTime;
        Instance.landingTimer += Time.deltaTime;
    }

    public static void PlaySoundEffect(SoundEffectType type)
    {
        switch (type)
        {
            // Movement
            case SoundEffectType.Footstep:
                HandleFootstepAudio();
                break;
            case SoundEffectType.Land:
                HandleLandingAudio();
                break;
            case SoundEffectType.TerminalVelocity:
                HandleTerminalVelocityAudio();
                break;

        }
    }


    public static void HandleFootstepAudio()
    {
        if (Instance.footstepTimer > Instance.footstepInverval)
        {
            Instance.footstepTimer = 0;
            if (Instance.lastFootstep1)
            {
                Instance.footstepAudioSource.clip = Instance.footstep1;
                Instance.lastFootstep1 = false;
            }
            else
            {
                Instance.footstepAudioSource.clip = Instance.footstep2;
                Instance.lastFootstep1 = true;
            }
            Instance.footstepAudioSource.Play();
        }
    }

    public static void HandleLandingAudio()
    {
        if (Instance.landingTimer > Instance.landingInterval)
        {
            Instance.landingTimer = 0;
            Instance.landAudioSource.Play();
        }
    }

    public static void HandleTerminalVelocityAudio()
    {
        Instance.terminalVelocityAudioSource.Play();
    }
}
