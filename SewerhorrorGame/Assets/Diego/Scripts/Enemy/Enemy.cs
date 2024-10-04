using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private float movementSpeed, idleTimer_Script = 3;
    public float editor_WalkSpeed = 1f, editor_RunSpeed = 2f, maxDistance = 5, idleTimer = 3, enemyHP = 100, stoppingDistance = 0.5f;

    [SerializeField] private float walkingRange = 50;

    [SerializeField] private NavMeshAgent agent;
    public Animator enemyAnimator;

    [SerializeField] private EnemyState currentState;
    [SerializeField] private Transform player;
    public bool hasHP = false;

    private bool pullRandomPath = true;

    public AudioSource chaseSound, attackSound, chaseMusic;  // Voeg chaseMusic hier toe

    public static bool isAttacking = false, isPlayingSound = false, isRunning;
    public bool screamEvent = false, isPlayingEvent = false;
    private bool eventDone = true, killedPlayer = false, killSpamCorrector = false;

    public GameObject player_GameObject, scareCam;

    private void Start()
    {
        isRunning = false;
        idleTimer_Script = idleTimer;
        pullRandomPath = true;
        currentState = EnemyState.Passive;
        isAttacking = false;
        isPlayingSound = false;
        isPlayingEvent = false;
        eventDone = true;
        killedPlayer = false;
        killSpamCorrector = false;
    }

    private void Update()
    {
        if (!killedPlayer)
        {
            EnemyBehaviour();
            MovementLogic();
        }
        else KillBehaviour();

        if (hasHP) HPBehaviour();

        // Zorg ervoor dat chaseMusic wordt afgespeeld wanneer isRunning true is
        HandleChaseMusic();
    }

    private void KillBehaviour()
    {
        if (!killSpamCorrector)
        {
            attackSound.Play();
            chaseSound.Stop();
            player_GameObject.SetActive(false);
            scareCam.SetActive(true);
            agent.speed = 0;
            agent.Stop();
            enemyAnimator.SetTrigger("jumpScare");
            killSpamCorrector = true;
        }
    }

    private void HPBehaviour()
    {

    }

    private void EnemyBehaviour()
    {
        switch (currentState)
        {
            case EnemyState.Passive:
                PassiveState();
                break;
            case EnemyState.Roaming:
                RoamingBehaviour();
                break;
            case EnemyState.Chase:
                ChaseBehaviour();
                break;
            default:
                break;
        }
    }

    private void PassiveState()
    {
        if (agent.hasPath == false)
        {
            if (!screamEvent)
            {
                if (!isPlayingEvent && !eventDone)
                {
                    enemyAnimator.SetBool("screamEvent", false);
                    FindObjectOfType<FieldOfView>().enabled = true;
                    eventDone = true;
                }
                else if (!isPlayingEvent && eventDone)
                {
                    if (pullRandomPath)
                    {
                        currentState = EnemyState.Roaming;
                        pullRandomPath = false;
                    }
                    else
                    {
                        idleTimer_Script -= Time.deltaTime;
                        if (idleTimer_Script <= 0)
                        {
                            pullRandomPath = true;
                            idleTimer_Script = idleTimer;
                            return;
                        }
                    }
                }
            }
            else
            {
                if (!isPlayingEvent)
                {
                    eventDone = false;
                    enemyAnimator.SetBool("screamEvent", true);
                    FindObjectOfType<FieldOfView>().enabled = false;
                    isPlayingEvent = true;
                }
                else screamEvent = false;
            }
        }
    }

    private void RoamingBehaviour()
    {
        if (agent.hasPath == false) agent.SetDestination(RandomPosition());
        else currentState = EnemyState.Passive;
    }

    private void ChaseBehaviour()
    {
        agent.SetDestination(player.transform.position);

        if (FieldOfView.canSeePlayer == false)
        {
            agent.stoppingDistance = 0;
            currentState = EnemyState.Passive;
            isRunning = false;
            isPlayingSound = false;
        }
    }

    private void MovementLogic()
    {
        agent.speed = movementSpeed;
        if (enemyAnimator != null) enemyAnimator.SetFloat("velocity", agent.velocity.magnitude);

        if (FieldOfView.canSeePlayer == true)
        {
            agent.stoppingDistance = stoppingDistance;
            currentState = EnemyState.Chase;
            isRunning = true;
        }

        if (FieldOfView.canSeePlayer == true && !isAttacking && !isPlayingSound) isAttacking = true;

        if (isAttacking)
        {
            chaseSound.Play();
            isPlayingSound = true;
            isAttacking = false;
        }

        if (!isPlayingSound) chaseSound.Stop();

        if (isRunning) movementSpeed = editor_RunSpeed;
        else if (!isRunning && agent.hasPath == true) movementSpeed = editor_WalkSpeed;
        else if (!isRunning && agent.hasPath == false || isPlayingEvent && !isRunning && agent.hasPath == false) agent.speed = 0;
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkingRange;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkingRange, NavMesh.AllAreas);

        return hit.position;
    }

    private void HandleChaseMusic()
    {
        if (isRunning && !killedPlayer)
        {
            // Start chase music als het niet al aan het spelen is
            if (!chaseMusic.isPlaying)
            {
                chaseMusic.Play();
            }
        }
        else
        {
            // Stop de chase music als de vijand stopt met rennen
            if (chaseMusic.isPlaying)
            {
                chaseMusic.Stop();
            }
        }
    }

    private enum EnemyState
    {
        Passive,
        Roaming,
        Chase
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) killedPlayer = true;
    }
}
