using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerCameraPrefab;

    private Transform attachedCamera;

    private Rigidbody2D rigidbody2D=null;

    public Transform playerSprite;

    public float SpeedModifer = 3.5f;
    public float JumpModifer = 7.0f;
    
    private float baseSpeedModifier, BaseJumpModifer;


    public float hangTimeAllowed = 0.15f;
    private float hangCounter;
    public float Rarity=500;
    public float Energy=50;
    public float Agressivity=50;
    public float Spookiness=50;
    public float Brain=50;

    float groundcheckRadius = 0.15f;
    float bodyCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundcheck;

    public Transform leftBodyCheck;
    public Transform rightBodyCheck;

    public bool leftBodyHittingWalls;
    public bool rightBodyHittingWalls;

    public bool isOnGroud=false; // Only Unuse jump if on the ground and went OfftheGround
    public bool isJumpUsed=false; // Only Jump again if jump not already used
    public bool OffGroundAfterJump=false; // is off the ground by jump


    public GameObject JumpParticlePrefab;
    private Transform JumpPartcilePosition;
    public GameObject moveParticles;

    public float aheadAmount, aheadSpeed;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        JumpPartcilePosition = transform.Find("JumpParticlePosition");
        baseSpeedModifier = SpeedModifer;
        BaseJumpModifer = JumpModifer;

        reCalcStats();

        attachedCamera = Instantiate(playerCameraPrefab, playerCameraPrefab.transform.position, Quaternion.identity, transform).transform;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        float ySpeed = JumpModifer;
        float xSpeed = Input.GetAxis("Horizontal") * SpeedModifer;

        leftBodyHittingWalls = Physics2D.OverlapCircle(leftBodyCheck.position, bodyCheckRadius, groundLayer);
        rightBodyHittingWalls = Physics2D.OverlapCircle(rightBodyCheck.position, bodyCheckRadius, groundLayer);

        if ((rightBodyHittingWalls && xSpeed > 0) || (leftBodyHittingWalls && xSpeed < 0))
            xSpeed = 0;

        rigidbody2D.velocity = new Vector2(xSpeed, rigidbody2D.velocity.y);
        
        bool SpacePressed = Input.GetKey(KeyCode.Space);

        isOnGroud = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, groundLayer);



        if (isOnGroud){
            hangCounter = hangTimeAllowed;
        } else {
            hangCounter -= Time.deltaTime;
        }

        isOnGroud = isOnGroud || (hangCounter>0 && rigidbody2D.velocity.y<=0);

        if (isOnGroud && isJumpUsed && !SpacePressed)
            isJumpUsed = false;

        if (isOnGroud && SpacePressed && !isJumpUsed) {
            GameObject newJumpParticle = Instantiate(JumpParticlePrefab, JumpPartcilePosition.position, JumpPartcilePosition.rotation).gameObject;
            Destroy(newJumpParticle, 0.5f);
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, ySpeed);
            isJumpUsed = true;
            OffGroundAfterJump = false;
        }

        if (OffGroundAfterJump && !isOnGroud && rigidbody2D.velocity.y > 0 && !SpacePressed)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * 0.8f);
            

       if (isOnGroud && isJumpUsed)
        OffGroundAfterJump = true;

        if (Input.GetAxis("Horizontal") != 0) {
            attachedCamera.localPosition = new Vector3(Mathf.Lerp(attachedCamera.localPosition.x, aheadAmount * Input.GetAxis("Horizontal"), aheadSpeed * Time.deltaTime) , attachedCamera.localPosition.y, attachedCamera.localPosition.z);
        }
    }


    private void Update() {
        float particlePlaybackSpeed;
        if (rigidbody2D.velocity.x != 0 || rigidbody2D.velocity.y > 0) {
           particlePlaybackSpeed = Mathf.Abs((Mathf.Abs(rigidbody2D.velocity.x) / SpeedModifer) + (rigidbody2D.velocity.y>0f?(rigidbody2D.velocity.y/JumpModifer):0f));
            moveParticles.SetActive(true);
            var main = moveParticles.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = particlePlaybackSpeed*2;
        } else {
            moveParticles.SetActive(false);
        }

        playerSprite.rotation = new Quaternion(playerSprite.rotation.x, playerSprite.rotation.y, ((rigidbody2D.velocity.x*-8)/180f) , playerSprite.rotation.w);
    }

    private void reCalcStats(){
        SpeedModifer = baseSpeedModifier + ((baseSpeedModifier * (Energy/100)) * 0.3f); // benefits 30% speed from energy
        JumpModifer = BaseJumpModifer + ((BaseJumpModifer * (Energy/100)) * 0.3f); // benefits 30% jump Power from energy
    }
}
