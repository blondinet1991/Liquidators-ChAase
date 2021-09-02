using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rigidbody2D;
    public float SpeedModifer = 2.5f;
    public float JumpModifer = 4.0f;
    
    private float baseSpeedModifier, BaseJumpModifer;

    public float Rarity=500;
    public float Energy=50;
    public float Agressivity=50;
    public float Spookiness=50;
    public float Brain=50;

    public bool isOnGroud=false; // Only Unuse jump if on the ground and went OfftheGround
    public bool isJumpUsed=false; // Only Jump again if jump not already used
    public bool OffGroundAfterJump=false; // is off the ground by jump


    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();

        baseSpeedModifier = SpeedModifer;
        BaseJumpModifer = JumpModifer;

        reCalcStats();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        float ySpeed = JumpModifer;
        float xSpeed = Input.GetAxis("Horizontal") * SpeedModifer;
        rigidbody2D.velocity = new Vector2(xSpeed, rigidbody2D.velocity.y);
        
        bool SpacePressed = Input.GetKey(KeyCode.Space);



        if (isOnGroud && isJumpUsed && !SpacePressed)
            isJumpUsed = false;

        if (isOnGroud && SpacePressed && !isJumpUsed) {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, ySpeed);
            isJumpUsed = true;
            OffGroundAfterJump = false;
        }

        if (OffGroundAfterJump && !isOnGroud && rigidbody2D.velocity.y > 0 && !SpacePressed)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * 0.9f);
            
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Platforms")) {
            isOnGroud=true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Platforms")) {
            isOnGroud=false;
            if (isJumpUsed)
                OffGroundAfterJump = true;
        }
    }

    private void reCalcStats(){
        SpeedModifer = baseSpeedModifier + ((baseSpeedModifier * (Energy/100)) * 0.3f); // benefits 30% speed from energy
        JumpModifer = BaseJumpModifer + ((BaseJumpModifer * (Energy/100)) * 0.3f); // benefits 30% jump Power from energy
    }
}
