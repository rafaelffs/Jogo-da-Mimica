using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip secondsSound;
    public static AudioClip endSound;
    public static AudioClip jumpSound;
    public static AudioClip correctSound;
    public static AudioClip endGameSound;
    static AudioSource audioSource;
    void Start()
    {
        secondsSound = Resources.Load<AudioClip>("Sounds/secondblip");
        endSound = Resources.Load<AudioClip>("Sounds/endblip");
        jumpSound = Resources.Load<AudioClip>("Sounds/jumpblip");
        correctSound = Resources.Load<AudioClip>("Sounds/correctblip");
        endGameSound = Resources.Load<AudioClip>("Sounds/endgame");
        audioSource = GetComponent<AudioSource>();
    }

    public static void playSecondSound()
    {
        audioSource.PlayOneShot(secondsSound);
    }

    public static void playEndSound()
    {
        audioSource.PlayOneShot(endSound);
    }

    public static void playJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public static void playCorrectSound()
    {
        audioSource.PlayOneShot(correctSound);
    }

    public static void playEndGameSound()
    {
        audioSource.PlayOneShot(endGameSound);
    }
}
