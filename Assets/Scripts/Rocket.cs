using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float thrustSpeed = 30f;
    [SerializeField] private AudioClip rocketThrusterSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private ParticleSystem rocketThrusterParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem successParticles;

    Rigidbody rb;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State currentState = State.Alive;
    Boolean collisionsDisabled = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentState == State.Alive) {
            ProcessInput();
        }
	}

    private void ProcessInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ThrustRocket();
        } else {
            StopRocketThrust();
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            RotateRocketLeft();
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            RotateRocketRight();
        }

        if (Debug.isDebugBuild) {
            if (Input.GetKey(KeyCode.N)) {
                WinLevel();
            }
            if (Input.GetKey(KeyCode.C)) {
                collisionsDisabled = !collisionsDisabled;
            }
        }
    }

    private void StopRocketThrust() {
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        if (rocketThrusterParticles.isPlaying) {
            rocketThrusterParticles.Stop();
        }
    }

    private void ThrustRocket() {
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(rocketThrusterSound);
        }
        rocketThrusterParticles.Play();
        rb.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);
    }

    private void RotateRocketRight() {
        Rotate(-rotationSpeed);
    }

    private void RotateRocketLeft() {
        Rotate(rotationSpeed);
    }

    private void Rotate(float angle) {
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationZ;
        transform.Rotate(Vector3.forward, angle * Time.deltaTime);
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.constraints = rb.constraints ^ RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnCollisionEnter(Collision collision) {
        if (currentState != State.Alive || collisionsDisabled) {
            return;
        }

            switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                WinLevel();
                break;
            default:
                Die();
                break;
        }
    }

    private void WinLevel() {
        currentState = State.Transcending;
        StopRocketThrust();
        audioSource.PlayOneShot(successSound);
        successParticles.Play();
        Invoke("LoadNextScene", 1f);
    }

    private void Die() {
        currentState = State.Dying;
        StopRocketThrust();
        LetRocketFloatFree();
        audioSource.PlayOneShot(deathSound, 0.65f);
        deathParticles.Play();
        Invoke("ReloadCurrentScene", 1f);
    }

    private void LetRocketFloatFree() {
        
        rb.constraints = RigidbodyConstraints.None;
        rb.AddRelativeForce(0, 0, UnityEngine.Random.Range(-10f, 10f), ForceMode.Impulse);
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
