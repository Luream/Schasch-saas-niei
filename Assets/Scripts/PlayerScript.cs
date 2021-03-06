﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float movementSpeed;
    public float jumpSpeed;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float wallFallBreakMultiplier;
    public bool readyToJump;
    public bool readyToWallJump;
    public Vector3 wallJumpDirection;
    public Vector3 spawnPoint;
    public string nextLevel;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPoint = new Vector3();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetSavePoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }

    private void FixedUpdate()
    {
        if (getLeftInput())
        {
            rb.AddForce(-transform.right * movementSpeed);
        }
        if (getRightInput())
        {
            rb.AddForce(transform.right * movementSpeed);
        }

        if (rb.velocity.y < 0)
        {
            rb.AddForce(Vector2.up * Physics.gravity.y * (fallMultiplier - 1));
        }
        else if (rb.velocity.y > 0 && !getUpInput())
        {
            rb.AddForce(Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1));
        }

        if(rb.velocity.y < 0 && readyToWallJump)
        {
            rb.AddForce(-Vector2.up * Physics.gravity.y * wallFallBreakMultiplier);
        }
    }

    private void Update()
    {
        if (getUpDownInput() && readyToJump)
        {
            rb.AddForce(transform.up * jumpSpeed, ForceMode2D.Impulse);
            readyToJump = false;
            audioSource.Play();
        }
        else if (getUpDownInput() && readyToWallJump)
        {
            rb.AddForce(transform.up * jumpSpeed + wallJumpDirection * jumpSpeed, ForceMode2D.Impulse);
            readyToWallJump = false;
            audioSource.Play();
        }

        if (transform.position.y < -3)
        {
            transform.position = spawnPoint;
        }

        /*
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.down, 1.1f);
        readyToJump = false;
        foreach (RaycastHit2D hit in hits)
            if (hit.transform.gameObject != gameObject)
                readyToJump = true;
        */
    }

    private bool getLeftInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            return true;
        else
            return false;
    }

    private bool getRightInput()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            return true;
        else
            return false;
    }

    private bool getUpInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))
            return true;
        else
            return false;
    }

    private bool getUpDownInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > Mathf.Abs(contact.normal.x))
                readyToJump = true;
            else if (contact.normal.x > 0 && getLeftInput())
            {
                readyToWallJump = true;
                wallJumpDirection = Vector3.right;
            }
            else if (contact.normal.x < 0 && getRightInput())
            {
                readyToWallJump = true;
                wallJumpDirection = Vector3.left;
            }
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        readyToJump = false;
        readyToWallJump = false;
    }

    // Wird bei Kollision ausgeführt
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Wenn das kolldierte Objekt den Tag Enemy hat, wird der Spieler zum Spawnpoint teleportiert
        if (collision.gameObject.tag == "Enemy")
        {
            transform.position = spawnPoint;
        }
        else if (collision.gameObject.tag == "End")
        {
            SceneManager.LoadSceneAsync(nextLevel);
        }
    }
}