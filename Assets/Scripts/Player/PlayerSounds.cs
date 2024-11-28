using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;

    private float footstepsTimer;
    [SerializeField] private float footstepsTimerMax = .15f; // Per seconds

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepsTimer -= Time.deltaTime;

        if (footstepsTimer < 0f)
        {
            footstepsTimer = footstepsTimerMax;

            if (player.IsWalking())
            {
                float volume = 0.5f;
                SoundManager.Instance.PlayPlayerMovingSound(player.transform.position, volume);
            }
        }
    }
}
