using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1;
    [SerializeField]
    private LayerMask playerLayerMask;
    Animator animator;
    Rigidbody2D rigidbody2d;
    PhotonView photonView;
    SpriteRenderer spriteRenderer;
    public bool Frozen { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // Flip Animation
        if (rigidbody2d.velocity.x != 0)
        {
            spriteRenderer.flipX = rigidbody2d.velocity.x < 0;
        }

        if (!photonView.IsMine || Frozen) return;
        Vector2 velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * moveSpeed;
        rigidbody2d.velocity = velocity;
        animator.SetFloat("Speed", velocity.sqrMagnitude);


        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, 1f, playerLayerMask);
            foreach (Collider2D player in players)
            {
                if (player.gameObject.Equals(gameObject)) continue;
                CharacterMovement playerMovement = player.GetComponent<CharacterMovement>();
                if (playerMovement.Frozen) continue;
                playerMovement.Freeze();
            }
        }
    }

    public void Freeze() {
        photonView.RPC("RPCFreeze", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPCFreeze()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        Frozen = true;
    }
}
