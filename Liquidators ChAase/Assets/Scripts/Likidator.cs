using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Likidator : MonoBehaviour
{
    public GameCore gameCore;
    float passedTime;
    public Transform TargetPosition;
    public Player playerTarget;
    private Vector3 moveDirection;

    public List<Transform> platformsPath;

    private Rigidbody2D LKrigidBody2D;

    public float SpeedModifer = 2f;
    public float JumpModifer = 7.0f;
    public float maxJumpRecord = 0f;
    float groundcheckRadius = 0.15f;
    float bodyCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask TrapLayer;

    public Transform groundcheck;
    

    public Transform leftBodyCheck1;
    public Transform leftBodyCheck2;
    public Transform rightBodyCheck1;
    public Transform rightBodyCheck2;

    public Transform leftLandingDirection;
    public Transform rightLandingDirection;


    public Transform leftJumpDirection;
    public Transform rightJumpDirection;

    public bool leftBodyHittingWalls;
    public bool rightBodyHittingWalls;

    public bool isOnGroud=false; // Only Unuse jump if on the ground and went OfftheGround
    public bool isJumpUsed=false; // Only Jump again if jump not already used
    public bool canJump=false; // can Jump Straight ?!
    public bool canJumpLeft=false; // can jump left ?
    public bool canJumpRight=false; // can jump right ?
    public bool OffGroundAfterJump=false; // is off the ground by jump

    private bool tryingToLand=false;

    private bool tryToJumpRight=false;
    private bool tryToJumpLeft=false;

    public Transform CurrentLKPlatform;
    public Transform CurrentPlayerPlatform;

    public Transform LastPlayerPlatform;
    public Transform LastLKPlatform;

    private float pathfindingTime;
    private float pathfindingTimeLimit=5f;

    private void Awake() {
        
        LKrigidBody2D = GetComponent<Rigidbody2D>();
        platformsPath = new List<Transform>();


    }

    void Start()
    {
        passedTime = 0f;
        setTarget(gameCore.Player);
        playerTarget = TargetPosition.gameObject.GetComponent<Player>();
        StartCoroutine("StartDetectPlatform");
        LastPlayerPlatform = CurrentPlayerPlatform;
        LastLKPlatform = CurrentLKPlatform;
        StartCoroutine("StartAI");

    }

    void Update()
    {
        passedTime += Time.deltaTime;
    }


    private void FixedUpdate() {
        if (moveDirection != Vector3.zero) {
            canJump = !Physics2D.Raycast(transform.position, Vector3.up, 2.5f, groundLayer);
            canJumpLeft = !Physics2D.Raycast(transform.position, (leftJumpDirection.transform.position - transform.position).normalized, 3f, groundLayer);
            canJumpRight = !Physics2D.Raycast(transform.position, (rightJumpDirection.transform.position - transform.position).normalized, 3f, groundLayer);
            
            bool playerIsOnHigherPlatform=false;

            if (CurrentPlayerPlatform != null && CurrentLKPlatform != null)
                playerIsOnHigherPlatform = CurrentPlayerPlatform.position.y > CurrentLKPlatform.position.y;


            isOnGroud = Physics2D.OverlapCircle(groundcheck.position, groundcheckRadius, groundLayer);
            leftBodyHittingWalls = Physics2D.OverlapCircle(leftBodyCheck1.position, bodyCheckRadius, groundLayer) || Physics2D.OverlapCircle(leftBodyCheck1.position, bodyCheckRadius, playerLayer) || Physics2D.OverlapCircle(leftBodyCheck1.position, bodyCheckRadius, TrapLayer) || Physics2D.OverlapCircle(leftBodyCheck2.position, bodyCheckRadius, groundLayer);
            rightBodyHittingWalls = Physics2D.OverlapCircle(rightBodyCheck1.position, bodyCheckRadius, groundLayer) || Physics2D.OverlapCircle(rightBodyCheck1.position, bodyCheckRadius, playerLayer) || Physics2D.OverlapCircle(rightBodyCheck1.position, bodyCheckRadius, TrapLayer) || Physics2D.OverlapCircle(rightBodyCheck2.position, bodyCheckRadius, groundLayer);


                if (moveDirection.y > 0 && playerIsOnHigherPlatform && isOnGroud) {
                    if (CurrentPlayerPlatform.position.x >= CurrentLKPlatform.position.x) {
                        tryToJumpLeft = false;
                        tryToJumpRight = true;
                    }
                    else
                    {
                        tryToJumpRight = false;
                        tryToJumpLeft = true;
                    }
                }
                else
                {
                      tryToJumpRight = false;
                      if (!rightBodyHittingWalls && moveDirection.x > 0)
                            walkRight();
                        else if (!leftBodyHittingWalls && moveDirection.x < 0){
                            walkLeft();
                      }
                }



        if (isOnGroud && !isJumpUsed && moveDirection.y > 0 && playerIsOnHigherPlatform) { // if target is higher and LK can jump
            if (tryToJumpRight && canJumpRight && canJump) {
                tryToJumpRight = false;
                Jump();
            } else if (tryToJumpLeft && canJumpLeft && canJump) {
                tryToJumpLeft = false;
                Jump();
            } else if (tryToJumpRight && !canJumpRight && !canJump) {
                walkLeft();
            } else if (tryToJumpLeft && !canJumpLeft && !canJump) {
                walkRight();
            }
        }
        //  else if (isOnGroud && !canJump && moveDirection.y > 0){
        //     if (canJumpLeft)
        //         walkLeft();
        //     else if (canJumpRight)
        //         walkRight();
        // }

        if (isOnGroud && isJumpUsed)
            isJumpUsed = false;

        if (!isOnGroud && isJumpUsed && moveDirection.x < 0 && moveDirection.y < 0) // Lk is jumping left
        {
            bool landInLeft = Physics2D.Raycast(transform.position, (leftLandingDirection.transform.position - transform.position).normalized, 5f, groundLayer);
            if (landInLeft)
                tryingToLand = true;
            else tryingToLand = false;

        } else if (!isOnGroud && isJumpUsed && moveDirection.x > 0 && moveDirection.y < 0) { // lk is jumping right
            bool landInRight = Physics2D.Raycast(transform.position, (rightLandingDirection.transform.position - transform.position).normalized, 5f, groundLayer);
            if (landInRight)
                tryingToLand = true;
            else tryingToLand = false;
        }

        if (OffGroundAfterJump && !isOnGroud && LKrigidBody2D.velocity.y > 0 && tryingToLand)
            LKrigidBody2D.velocity = new Vector2(LKrigidBody2D.velocity.x, LKrigidBody2D.velocity.y * 0.6f);
            

        if (isOnGroud && isJumpUsed)
        OffGroundAfterJump = true;
        }

        if (CurrentLKPlatform != null) {
             if (maxJumpRecord < transform.position.y - CurrentLKPlatform.position.y)
                maxJumpRecord = transform.position.y - CurrentLKPlatform.position.y;
        }


        tryingToLand = false;
        tryToJumpRight = false;
        tryToJumpLeft = false;
    }


    public void setTarget(Transform target) {
        TargetPosition = target;
    }

    public void noTarget() {
        TargetPosition.position = Vector3.zero;
    }

    void think() {
        if (passedTime > 2f) { //2 sec for enterance animation. maybe ?
                if(CurrentPlayerPlatform != null && CurrentLKPlatform != null) {
                       if (!CurrentPlayerPlatform.GetInstanceID().Equals(LastPlayerPlatform.GetInstanceID()) || !CurrentLKPlatform.GetInstanceID().Equals(LastLKPlatform.GetInstanceID()))
                            platformsPath = findpathV2();
                }
               
                if (TargetPosition.position != Vector3.zero) // we have a target to seek !
                {

                    if (platformsPath != null && platformsPath.Count >= 1) {
                        moveDirection = (platformsPath[0].position - transform.position).normalized; 

                    }
                        
                    else{
                        moveDirection = (TargetPosition.position - transform.position).normalized; // normalize it to get the direction only and use it to move
                        //Debug.Log("Not wokring");
                    }
                        
                    
                } else {
                    moveDirection = new Vector3(0,0);
                }
            }
    }


    void walkLeft() {
        float xSpeed = -1 * SpeedModifer;
        LKrigidBody2D.velocity = new Vector2(xSpeed, LKrigidBody2D.velocity.y);
    }

    void walkRight(){
        float xSpeed = SpeedModifer;
        LKrigidBody2D.velocity = new Vector2(xSpeed, LKrigidBody2D.velocity.y);
    }
 
    void Jump() {
        float ySpeed = JumpModifer;
        LKrigidBody2D.velocity = new Vector2(LKrigidBody2D.velocity.x, ySpeed);
        isJumpUsed = true;
        OffGroundAfterJump = false;
    }


List<Transform> findpathV2() {
    if (CurrentLKPlatform != null && CurrentPlayerPlatform != null) {
        pathfindingTime = 0f;
        bool PathFound = false;
        List<KeyValuePair<GameObject,GameObject>> CurrentPairs = new List<KeyValuePair<GameObject, GameObject>>();

        List<List<KeyValuePair<GameObject,GameObject>>> pathes = new List<List<KeyValuePair<GameObject,GameObject>>>();

        Transform nextPlatform = CurrentPlayerPlatform;
        LastPlayerPlatform = CurrentPlayerPlatform;

        List<int> PathCounts = new List<int>();
        List<float> PathDistances = new List<float>();

        List<List<Transform>> nominatedPathes = new List<List<Transform>>();
        int BestIndex=0;

        int PlatformCounter = 0;

        List<Transform> NearbyPlatforms;

            List<KeyValuePair<GameObject, GameObject>> init = new List<KeyValuePair<GameObject, GameObject>>();
            init.Add(new KeyValuePair<GameObject, GameObject>(CurrentPlayerPlatform.gameObject, CurrentPlayerPlatform.gameObject));
            pathes.Add(init); // added for start only delete after loop is started

        for (;;) {
            CurrentPairs = new List<KeyValuePair<GameObject, GameObject>>();
            int ItemCounter = 0;


            foreach (var item in pathes[pathes.Count-1])
            {
            
            //previous pathes values must exclude from accessible list
            NearbyPlatforms = getAccessiblePlatforms(item.Key.transform, gameCore.PlatformsTransformList.ToArray(), pathes);
            
            if (PlatformCounter==0 && pathes.Count > 0) // first item is added manualy for loop bug fix so we delete it here
                pathes.Clear();

            foreach (var nearPlatform in NearbyPlatforms)
                {
                    CurrentPairs.Add(new KeyValuePair<GameObject, GameObject>(nearPlatform.gameObject, item.Key));

                if (CurrentLKPlatform.gameObject.GetInstanceID().Equals(nearPlatform.gameObject.GetInstanceID())) {
                    PathFound = true;
                    List<Transform> Path = new List<Transform>();
                    float totalDistance=0f;
                    //Path.Add(nearPlatform);
                    GameObject nextPathPlat = item.Key;
                    Path.Add(nextPathPlat.transform);

                    totalDistance = Vector3.Distance(nearPlatform.position, nextPathPlat.transform.position);

                    for (int i=PlatformCounter-1; i>=0; i--) {

                        totalDistance += Vector3.Distance(nextPathPlat.transform.position, findFirstValueByKey(nextPathPlat,pathes[i]).transform.position);
                        nextPathPlat = findFirstValueByKey(nextPathPlat,pathes[i]);
                        Path.Add(nextPathPlat.transform);
                    }

                    nominatedPathes.Add(Path);
                    PathCounts.Add(ItemCounter);
                    PathDistances.Add(totalDistance);


                }

                    ItemCounter++;
                } 
            }
            
            pathes.Add(CurrentPairs);

            if (PathFound) {
                if (nominatedPathes.Count > 1) {
                    for (int i=0; i<nominatedPathes.Count-1; i++) {
                        if (PathDistances[i] < PathDistances[i+1])
                            BestIndex = i;
                        else BestIndex = i+1;
                    }
                }

                break;
            }

            PlatformCounter++;

            pathfindingTime += Time.deltaTime;
            if (pathfindingTime > pathfindingTimeLimit) {
                //Debug.Log("Pathfinding Took Too Long! (" + pathfindingTime+")");
                break;
            }
                
        }

        if (nominatedPathes.Count>0)
            return nominatedPathes[BestIndex];
        else 
            return null;

    } else {
        return null;
    }
   

}


GameObject findFirstValueByKey(GameObject item, List<KeyValuePair<GameObject,GameObject>> Items){
    GameObject tmpObj= null;
    foreach (var Pair in Items)
    {
        if (Pair.Key.gameObject.GetInstanceID().Equals(item.GetInstanceID()))
            tmpObj = Pair.Value;
    }
    return tmpObj;
}

bool isDuplicate(GameObject item, List<List<KeyValuePair<GameObject,GameObject>>> ItemsList){
    bool isDup=false;
    foreach (var Pairs in ItemsList)
    {
        foreach (var pair in Pairs)
        {
            if (item.gameObject.GetInstanceID().Equals(pair.Value))
                isDup = true;
        }
    }

    return isDup;
}

bool canMoveToPlatform(Transform targetPlatform) {
    if (Vector3.Distance(targetPlatform.position, transform.position) < JumpModifer)
        return true;
    else return false;
}
bool canMoveToPlatform2(Transform from, Transform targetPlatform) {
    if (Vector3.Distance(from.position, targetPlatform.position) < (4.5f+ (Mathf.Abs(from.position.y-targetPlatform.position.y)<2?2f:0)))
        return true;
    else return false;
}

List<Transform> getAccessiblePlatforms(Transform From, Transform[] Platforms,  List<List<KeyValuePair<GameObject,GameObject>>> excludes) {
    List<Transform> trList = new List<Transform>();

    foreach (Transform p in Platforms) {
        bool isNew = true;
        foreach (var Pairs in excludes)
        {
            foreach (var pair in Pairs)
            {
                if (p.gameObject.GetInstanceID().Equals(pair.Value.GetInstanceID()))
                    isNew = false;
            }
        }
        if (canMoveToPlatform2(From, p) && isNew && !From.gameObject.GetInstanceID().Equals(p.gameObject.GetInstanceID()))
            trList.Add(p);
    }

    return trList;
}

Transform GetClosestPlatform(Transform currentPlatform, Transform[] Platforms, List<Transform> ExcludeList)
{
    Transform tMin = null;
    float minDist = Mathf.Infinity;
    Vector3 currentPos = currentPlatform.position;
    foreach (Transform p in Platforms)
    {
        float dist = Vector3.Distance(p.position, currentPos);
        if (dist < minDist && !ExcludeList.Contains(p))
        {
            tMin = p;
            minDist = dist;
        }
    }
    return tMin;
}

void detectPlatform(){
   CurrentPlayerPlatform = playerTarget.currentPlayerPlatform; 
   CurrentLKPlatform = Physics2D.Raycast(transform.position, Vector3.down, 7f, groundLayer).transform;
}

IEnumerator StartAI() 
{
    for(;;) 
    {
        think();
        yield return new WaitForSeconds(.3f);
    }
}

IEnumerator StartDetectPlatform() 
{
    for(;;) 
    {
        detectPlatform();
        yield return new WaitForSeconds(.1f);
    }
}
}
