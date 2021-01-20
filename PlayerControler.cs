using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;   
    private bool isFront;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isDashing = false;
    private float dir;
    
    public Transform frontCheck;
    public float speed;
    public float wallSlideSpeed;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;
    public float jumpforce;
    public float dashforce;

    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask platformLayerMask;

    // Use this for initialization
    void Start()
    {

        body = GetComponent<Rigidbody2D>();
        boxCollider = transform.GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        JumpControl();

        if (Input.GetKeyDown(KeyCode.Z))
        {

            StartCoroutine(DashControl());
        }

        WallJumpControl();

    }

    void FixedUpdate()
    {
        if (!isWallJumping)
        {
            dir = Input.GetAxisRaw("Horizontal");

            if (!isDashing)
            {
                body.velocity = new Vector2(dir * speed, body.velocity.y);
            }
        }
    }

    private bool OnGround()
    {
        float extraHeight = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);


        return ((raycastHit.collider != null));

    }




    private void JumpControl()
    {

        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
        {


            body.velocity = Vector2.up * jumpforce;
            //canJump = false;

        }

    }

    private void WallJumpControl(){
         isFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, platformLayerMask);
        if (isFront && !OnGround() && dir != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, wallSlideSpeed, float.MaxValue));
        }

        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            isWallJumping = true;
            StartCoroutine(WallJumpCooldown());
        }

        if (isWallJumping)
        {
            
            body.velocity = new Vector2(xWallForce * -dir, yWallForce);
            
        }
    }

    private IEnumerator DashControl()
    {
        isDashing = true;

        body.velocity = new Vector2(dir  * dashforce, body.velocity.y);

        yield return new WaitForSeconds(0.2f);
        isDashing = false;


    }

    private IEnumerator WallJumpCooldown()
    {
        
        yield return new WaitForSeconds(wallJumpTime);
        isWallJumping = false;
    }
}
