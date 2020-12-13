using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidbody;
    public GameObject particlesPrefab;
    public TrailRenderer trail;
    
    private Vector2 startPos;
    private Transform bottomPlane;
    private float planeWidth;
    private ObjectPool particlesPool;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        bottomPlane = GameObject.FindWithTag("BottomPlane").transform;
        trail.emitting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] planeVertices = bottomPlane.gameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
        planeWidth = Mathf.Abs(planeVertices[0].x - planeVertices[10].x);
        particlesPool = new ObjectPool(particlesPrefab, transform.parent, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.isGamePaused) return;
        
        if(transform.position.y < bottomPlane.position.y) Messenger.instance.ballFell();
            
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    continueDragging(touch.position);
                    break;
            }
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            if(Input.GetMouseButton(0)) continueDragging(mousePos);
        }
    }

    private void continueDragging(Vector3 pos)
    {
        float newX = 0;
        float screenBorder = Screen.width * 0.1f;
        if (pos.x > Screen.width - screenBorder) pos.x = Screen.width - screenBorder;
        pos.x -= screenBorder;
        if (pos.x < 0) pos.x = 0;
        float relativePosX = pos.x / (Screen.width - 2 * screenBorder);
        newX = -planeWidth / 2f + relativePosX * planeWidth;
        
        Vector3 position = rigidbody.position;
        position.x = newX;
        rigidbody.position = position;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(!other.collider.GetComponentInChildren<PlatformColliding>().isScoreCountingEnabled()) return;
        spawnParticles(other.GetContact(0).point);
    }
    
    private void spawnParticles(Vector3 position)
    {
        if(particlesPool == null) return;
        GameObject particles = particlesPool.Spawn(position);
        particles.GetComponent<CollidingParticles>().init(particlesPool);
        Vibration.VibratePop();
    }

    private void OnBecameInvisible()
    {
        if(GameController.instance.isGamePaused) gameObject.SetActive(false);
    }
}
