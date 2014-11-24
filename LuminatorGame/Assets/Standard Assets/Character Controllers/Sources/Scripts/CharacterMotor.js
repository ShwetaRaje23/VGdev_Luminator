#pragma strict
#pragma implicit
#pragma downcast

// Does this script currently respond to input?
var canControl : boolean = true;

var useFixedUpdate : boolean = true;

// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
// Very handy for organization!

// The current global direction we want the character to move in.
@System.NonSerialized
var inputMoveDirection : Vector3 = Vector3.zero;

// Is the jump button held down? We use this interface instead of checking
// for the jump button directly so this script can also be used by AIs.
@System.NonSerialized
var inputJump : boolean = false;

class CharacterMotorMovement {
	// The maximum horizontal speed when moving
	var maxForwardSpeed : float = 10.0;
	var maxSidewaysSpeed : float = 10.0;
	var maxBackwardsSpeed : float = 10.0;
	
	// Curve for multiplying speed based on slope (negative = downwards)
	var slopeSpeedMultiplier : AnimationCurve = AnimationCurve(Keyframe(-90, 1), Keyframe(0, 1), Keyframe(90, 0));
	
	// How fast does the character change speeds?  Higher is faster.
	var maxGroundAcceleration : float = 30.0;
	var maxAirAcceleration : float = 20.0;

	// The gravity for the character
	var gravity : float = 10.0;
	var maxFallSpeed : float = 20.0;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// The last collision flags returned from controller.Move
	@System.NonSerialized
	var collisionFlags : CollisionFlags; 

	// We will keep track of the character's current velocity,
	@System.NonSerialized
	var velocity : Vector3;
	
	// This keeps track of our current velocity while we're not grounded
	@System.NonSerialized
	var frameVelocity : Vector3 = Vector3.zero;
	
	@System.NonSerialized
	var hitPoint : Vector3 = Vector3.zero;
	
	@System.NonSerialized
	var lastHitPoint : Vector3 = Vector3(Mathf.Infinity, 0, 0);
}

var movement : CharacterMotorMovement = CharacterMotorMovement();

enum MovementTransferOnJump {
	None, // The jump is not affected by velocity of floor at all.
	InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
	PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
	PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

// We will contain all the jumping related variables in one helper class for clarity.
class CharacterMotorJumping {
	// Can the character jump?
	var enabled : boolean = true;

	// How high do we jump when pressing jump and letting go immediately
	var baseHeight : float = 1.0;
	
	// We add extraHeight units (meters) on top when holding the button down longer while jumping
	var extraHeight : float = 4.1;
	
	// How much does the character jump out perpendicular to the surface on walkable surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	var perpAmount : float = 0.0;
	
	// How much does the character jump out perpendicular to the surface on too steep surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	var steepPerpAmount : float = 0.5;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// Are we jumping? (Initiated with jump button and not grounded yet)
	// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
	@System.NonSerialized
	var jumping : boolean = false;
	
	@System.NonSerialized
	var holdingJumpButton : boolean = false;

	// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
	@System.NonSerialized
	var lastStartTime : float = 0.0;
	
	@System.NonSerialized
	var lastButtonDownTime : float = -100;
	
	@System.NonSerialized
	var jumpDir : Vector3 = Vector3.up;
}

var jumping : CharacterMotorJumping = CharacterMotorJumping();

class CharacterMotorMovingPlatform {
	var enabled : boolean = true;
	
	var movementTransfer : MovementTransferOnJump = MovementTransferOnJump.PermaTransfer;
	
	@System.NonSerialized
	var hitPlatform : Transform;
	
	@System.NonSerialized
	var activePlatform : Transform;
	
	@System.NonSerialized
	var activeLocalPoint : Vector3;
	
	@System.NonSerialized
	var activeGlobalPoint : Vector3;
	
	@System.NonSerialized
	var activeLocalRotation : Quaternion;
	
	@System.NonSerialized
	var activeGlobalRotation : Quaternion;
	
	@System.NonSerialized
	var lastMatrix : Matrix4x4;
	
	@System.NonSerialized
	var platformVelocity : Vector3;
	
	@System.NonSerialized
	var newPlatform : boolean;
}

var movingPlatform : CharacterMotorMovingPlatform = CharacterMotorMovingPlatform();

class CharacterMotorSliding {
	// Does the character slide on too steep surfaces?
	var enabled : boolean = true;
	
	// How fast does the character slide on steep surfaces?
	var slidingSpeed : float = 15;
	
	// How much can the player control the sliding direction?
	// If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
	var sidewaysControl : float = 1.0;
	
	// How much can the player influence the sliding speed?
	// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
	var speedControl : float = 0.4;
}

var sliding : CharacterMotorSliding = CharacterMotorSliding();

@System.NonSerialized
var grounded : boolean = true;

@System.NonSerialized
var groundNormal : Vector3 = Vector3.zero;

private var lastGroundNormal : Vector3 = Vector3.zero;

private var tr : Transform;

private var controller : CharacterController;

private var inputMove: float;
private var fwdDown: boolean = false;
private var fwdPress: float = 0.0f;
private var fwdRelease: float = 0.0f;

private var backDown: boolean = false;
private var backPress: float = 0.0f;
private var backRelease: float = 0.0f;

public var groundAttack: AnimationCurve;
public var groundDecay: AnimationCurve;

public var bumpMotion: AnimationCurve;

private var moveSpeed: float = 60.0f;
private var gravity: float = 10.0f;

private var inputRotate: float = 0.0f;
private var rotateSpeed: float = 1.0f;

private var minimumX: float = -60.0f;
private var maximumX: float = 60.0f;

private var val: float = 0.0f;
private var val2: float = 0.0f;	

private var anim: Animator;//= new Animation();

private var audio1: AudioSource;
private var audioFlag: int = 0;
private var audioFlag1: int = 0;

private var move: boolean = true;
private var sink:boolean = false;

private var sinkRotation: Vector3;
private var sinkTranslate: Vector3;

private var sinkFlag:boolean = false;
private var s_count:int = 0;

function Awake () {
	controller = GetComponent (CharacterController);
	anim = GameObject.Find("ForBumpyanimation").GetComponent(Animator);
	audio1 = GetComponent(AudioSource);
	tr = transform;
	sinkRotation = new Vector3(0f,0f,0f);
	sinkTranslate = new Vector3(0f,38f,0f);
		
}


private function UpdateFunction () {
//Debug.Log("function called");
if(move)
{
	var rawInput: float; 
	inputRotate = Input.GetAxis ("Horizontal");
	//Debug.Log("inputRotate: "+inputRotate);
	if(inputRotate>180f)
				inputRotate-=360.0f;
	
	inputRotate = Mathf.Clamp (inputRotate, minimumX, maximumX);
			
		
	rawInput = Input.GetAxis ("Vertical");
						inputMove = 0f;
						//Debug.Log("rawInput: "+rawInput);
						
						if (fwdDown && rawInput <= 0f) {
						//Debug.Log("Entered forward up");
								fwdDown = false;
								fwdPress = 0f;
								fwdRelease = Time.time;
						} else if (!fwdDown && rawInput > 0f) {
						//Debug.Log("Entered forward down");
								fwdDown = true;
								fwdPress = Time.time;
								fwdRelease = 0f;
						}
						if (backDown && rawInput >= 0f) {
							//Debug.Log("Entered back up");
									backDown = false;
								backPress = 0f;
								backRelease = Time.time;
						} else if (!backDown && rawInput < 0f) {
								//Debug.Log("Entered back down");
								backDown = true;
								backPress = Time.time;
								backRelease = 0f;
						}
						if (fwdDown) {
								//Debug.Log("Entered forward down trigger");
								//val=0.2f;
								val = groundAttack.Evaluate ((Time.time - fwdPress)/10.0f);
								//Debug.Log("val: "+val+" time: "+(Time.time-fwdPress));
								
								/*if(val<0.98 && audioFlag != 1)
								{
								Debug.Log("playing sound acc, audioFlag: "+audioFlag);
								audioFlag = 1;
								audio1.Pause();
								audio1.audio.clip = Resources.Load("car_acc") as AudioClip;
								audio1.Play();
								}
								else if(audioFlag != 0 && val>=0.98)
								{
								audioFlag = 0;
								audio1.audio.clip = Resources.Load("car_drive") as AudioClip;
								audio1.Play();
								}*/
								
								if (val < 0.001)
										val = 0f; 
								inputMove += val;
								//Debug.Log("Now input move: "+inputMove);
//								if(moveSpeed<50.5f)
//								{ moveSpeed+=0.5f;}
Debug.Log("Entered fwdDown");
								} 
						else {
						//Debug.Log("Entered fwdUp, val: "+val);

								var tval: float;
								tval = groundDecay.Evaluate ((Time.time - fwdRelease)/10.0f); // is needed if want to show motion slow down
							val=val*tval;
								//Debug.Log("tval: "+tval+" val: "+val);
								if (val < 0.001)
										val = 0f; 
								inputMove += val;
								/*if((audioFlag == 0 || audioFlag == 1) && val!=0f)
								{
								audioFlag = 2;
								audio.Pause();
								//audio1.audio.clip = Resources.Load("car_slow") as AudioClip;
								audio1.PlayOneShot(Resources.Load("car_slow") as AudioClip);}*/
						}
						if (backDown) {
								val2 = groundAttack.Evaluate ((Time.time - backPress)/10.0f);
								
								if(val2<0.98 && audioFlag1 != 1)
								{
								Debug.Log("playing sound acc, audioFlag: "+audioFlag);
								//audioFlag1 = 1;
								//audio1.Pause();
								//audio1.audio.clip = Resources.Load("car_acc") as AudioClip;
								//audio1.Play();
								}
								else if(audioFlag1 != 0 && val2>=0.98)
								{
								//audioFlag1 = 0;
								//audio1.audio.clip = Resources.Load("car_drive") as AudioClip;
								//audio1.Play();
								}
								
								
								if (val2 < 0.001)
										val2 = 0f; 
								inputMove -= val2;
								}
						else {
						var tval1: float;
							tval1 = groundDecay.Evaluate ((Time.time - backRelease)/10.0f); // is needed if want to show motion slow down
							val2=val2*tval1;
								if (val2 < 0.001)
										val2 = 0f; 
							//if((audioFlag1 == 0 || audioFlag1 == 1) && val2!=0f)
							//		{
								//audioFlag1 = 2;
								//audio.Pause();
								//audio1.audio.clip = Resources.Load("car_slow") as AudioClip;
								//audio1.PlayOneShot(Resources.Load("car_slow") as AudioClip);}
							
							inputMove -= val2;
						
						}
						if (inputMove > 1f)
								inputMove = 1f;			
						if (inputMove < -1f)
								inputMove = -1f;
								
						if(inputMove!=0f && !audio.isPlaying)
						{
						//audioFlag1 = 0;
						
								audio1.audio.clip = Resources.Load("car_drive") as AudioClip;
								audio1.Play();
						}
						else if (inputMove == 0f)
							audio1.Pause();
							
							
						horizontalVelocity = new Vector3 (controller.velocity.x, 1, controller.velocity.z);
						horizontalVelocity = transform.InverseTransformDirection (horizontalVelocity);
						horizontalSpeed = horizontalVelocity.z;  // want the plus or minus on speed
						verticalSpeed = controller.velocity.y;
						
						//Debug.Log("inputMove: "+inputMove+" and moveSpeed "+moveSpeed );

						if(inputMove!=0.0f) //anim.Play("Bumpy");
						anim.SetBool("motion", true);
						//	moveVector = new Vector3(0f, 0.05f, inputMove);
						else anim.SetBool("motion", false);
						moveVector = new Vector3(0f, -0.01f, inputMove);
						//moveVector = new Vector3(0f, bumpMotion.Evaluate(Time.time-fwdPress), inputMove);
						moveVector = transform.TransformDirection(moveVector);
        				moveVector *= moveSpeed;
        				
        				transform.Rotate(0f, inputRotate * rotateSpeed, 0f);
		

	// move, and adjust speeds based on collisions.  Need to do this to avoid the horrible sliding motions
						// that the Controller does otherwise
				
				//Debug.Log("MoveVector.y "+moveVector.y);
				collisionFlags = controller.Move (moveVector * Time.deltaTime);	
//	moveVector.y -= 0.05f*moveSpeed;
//	Debug.Log("MoveVector.y "+moveVector.y);
//	collisionFlags = controller.Move (moveVector * Time.deltaTime);	
	
						// did our last move result in "grounding"
						isGrounded = ((collisionFlags & CollisionFlags.CollidedBelow) != 0);
						
						
						
						if(!isGrounded)
						{moveVector.y = -gravity * Time.deltaTime * 2f;
						//Debug.Log("Not grounded and moveVector.y: "+moveVector.y);
						}
			
						if ((collisionFlags & CollisionFlags.CollidedSides) != 0) {
								// keep it moving the same direction but at a VERY small rate (so the collision stays consistently on if the player
								// is pushing in that direction)
								moveVector.x /= 100.0f;
								moveVector.z /= 100.0f;
								moveVector.y /= 2.0f;  // slow down the vertical movement 
						}
		
						if ((collisionFlags & CollisionFlags.CollidedAbove) != 0) {
								// start moving down immediately by a little.  Ouch, my head!
								moveVector.y = -gravity * Time.deltaTime * 2f;
								moveVector.x /= 1.15f;  // slow down sideways movement
								moveVector.z /= 1.15f;  // slow down sideways movement
						}
}
else //move is false
{

	if(sink) //sink animation
	{
	fwdDown = false;
	backDown = false;
	
	if (Input.GetKeyDown (KeyCode.Space))
						s_count++;
		if (s_count > 10)
						{sink=false;
						move=true;s_count = 0;}

							anim.SetBool("motion",false);
							audio1.Pause();
							audio1.audio.clip = Resources.Load("drowning-bubbles") as AudioClip;
							audio1.Play();
							if(sinkRotation.y>=360.0f)
							{Debug.Log("Entered reset rotation");sinkRotation = new Vector3(0f,0f,0f);}
							else
							{sinkRotation+=new Vector3(0f,0.5f,0f);	}
							transform.rotation = Quaternion.Euler(0f,sinkRotation.y,0f);
							if(!sinkFlag)
							{//Debug.Log("Going down, sinkTranslate.y: "+sinkTranslate.y);
							sinkTranslate.y-=0.001f*inputMove*moveSpeed;
							if(sinkTranslate.y<36.0f)
							sinkFlag=!sinkFlag;
							}
							else{
								sinkTranslate.y+=0.005f*inputMove*moveSpeed;
								if(sinkTranslate.y>40f)
							sinkFlag=!sinkFlag;
								}
							
							transform.position=new Vector3(transform.position.x,sinkTranslate.y,transform.position.z);
	}
}					


}

function FixedUpdate () {
	if (useFixedUpdate)
		UpdateFunction();
}

function Update () {
	if (!useFixedUpdate)
		UpdateFunction();
}

private function ApplyInputVelocityChange (velocity : Vector3) {	
	if (!canControl)
		inputMoveDirection = Vector3.zero;
	
	// Find desired velocity
	var desiredVelocity : Vector3;
	if (grounded && TooSteep()) {
		// The direction we're sliding in
		desiredVelocity = Vector3(groundNormal.x, 0, groundNormal.z).normalized;
		// Find the input movement direction projected onto the sliding direction
		var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
		// Add the sliding direction, the spped control, and the sideways control vectors
		desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
		// Multiply with the sliding speed
		desiredVelocity *= sliding.slidingSpeed;
	}
	else
		desiredVelocity = GetDesiredHorizontalVelocity();
	
	if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer) {
		desiredVelocity += movement.frameVelocity;
		desiredVelocity.y = 0;
	}
	
	if (grounded)
		desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
	else
		velocity.y = 0;
	
	// Enforce max velocity change
	var maxVelocityChange : float = GetMaxAcceleration(grounded) * Time.deltaTime;
	var velocityChangeVector : Vector3 = (desiredVelocity - velocity);
	if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
		velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
	}
	// If we're in the air and don't have control, don't apply any velocity change at all.
	// If we're on the ground and don't have control we do apply it - it will correspond to friction.
	if (grounded || canControl)
		velocity += velocityChangeVector;
	
	if (grounded) {
		// When going uphill, the CharacterController will automatically move up by the needed amount.
		// Not moving it upwards manually prevent risk of lifting off from the ground.
		// When going downhill, DO move down manually, as gravity is not enough on steep hills.
		velocity.y = Mathf.Min(velocity.y, 0);
	}
	
	return velocity;
}

private function ApplyGravityAndJumping (velocity : Vector3) {
	
	if (!inputJump || !canControl) {
		jumping.holdingJumpButton = false;
		jumping.lastButtonDownTime = -100;
	}
	
	if (inputJump && jumping.lastButtonDownTime < 0 && canControl)
		jumping.lastButtonDownTime = Time.time;
	
	if (grounded)
		velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
	else {
		velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
		
		// When jumping up we don't apply gravity for some time when the user is holding the jump button.
		// This gives more control over jump height by pressing the button longer.
		if (jumping.jumping && jumping.holdingJumpButton) {
			// Calculate the duration that the extra jump force should have effect.
			// If we're still less than that duration after the jumping time, apply the force.
			if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
				// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
			}
		}
		
		// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
		velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
	}
		
	if (grounded) {
		// Jump only if the jump button was pressed down in the last 0.2 seconds.
		// We use this check instead of checking if it's pressed down right now
		// because players will often try to jump in the exact moment when hitting the ground after a jump
		// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
		// it's confusing and it feels like the game is buggy.
		if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2)) {
			grounded = false;
			jumping.jumping = true;
			jumping.lastStartTime = Time.time;
			jumping.lastButtonDownTime = -100;
			jumping.holdingJumpButton = true;
			
			// Calculate the jumping direction
			if (TooSteep())
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
			else
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
			
			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
			velocity.y = 0;
			velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
			
			// Apply inertia from platform
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
				movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
			) {
				movement.frameVelocity = movingPlatform.platformVelocity;
				velocity += movingPlatform.platformVelocity;
			}
			
			SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
		}
		else {
			jumping.holdingJumpButton = false;
		}
	}
	
	return velocity;
}

function OnControllerColliderHit (hit : ControllerColliderHit) {
	if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
		if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
			groundNormal = hit.normal;
		else
			groundNormal = lastGroundNormal;
		
		movingPlatform.hitPlatform = hit.collider.transform;
		movement.hitPoint = hit.point;
		movement.frameVelocity = Vector3.zero;
	}
}

private function SubtractNewPlatformVelocity () {
	// When landing, subtract the velocity of the new ground from the character's velocity
	// since movement in ground is relative to the movement of the ground.
	if (movingPlatform.enabled &&
		(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
		movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
	) {
		// If we landed on a new platform, we have to wait for two FixedUpdates
		// before we know the velocity of the platform under the character
		if (movingPlatform.newPlatform) {
			var platform : Transform = movingPlatform.activePlatform;
			yield WaitForFixedUpdate();
			yield WaitForFixedUpdate();
			if (grounded && platform == movingPlatform.activePlatform)
				yield 1;
		}
		movement.velocity -= movingPlatform.platformVelocity;
	}
}

private function MoveWithPlatform () : boolean {
	return (
		movingPlatform.enabled
		&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
		&& movingPlatform.activePlatform != null
	);
}

private function GetDesiredHorizontalVelocity () {
	// Find desired velocity
	var desiredLocalDirection : Vector3 = tr.InverseTransformDirection(inputMoveDirection);
	var maxSpeed : float = MaxSpeedInDirection(desiredLocalDirection);
	if (grounded) {
		// Modify max speed on slopes based on slope speed multiplier curve
		var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
		maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
	}
	return tr.TransformDirection(desiredLocalDirection * maxSpeed);
}

private function AdjustGroundVelocityToNormal (hVelocity : Vector3, groundNormal : Vector3) : Vector3 {
	var sideways : Vector3 = Vector3.Cross(Vector3.up, hVelocity);
	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
}

private function IsGroundedTest () {
	return (groundNormal.y > 0.01);
}

function GetMaxAcceleration (grounded : boolean) : float {
	// Maximum acceleration on ground and in air
	if (grounded)
		return movement.maxGroundAcceleration;
	else
		return movement.maxAirAcceleration;
}

function CalculateJumpVerticalSpeed (targetJumpHeight : float) {
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
}

function IsJumping () {
	return jumping.jumping;
}

function IsSliding () {
	return (grounded && sliding.enabled && TooSteep());
}

function IsTouchingCeiling () {
	return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
}

function IsGrounded () {
	return grounded;
}

function TooSteep () {
	return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
}

function GetDirection () {
	return inputMoveDirection;
}

function SetControllable (controllable : boolean) {
	canControl = controllable;
}

// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
// The function returns the length of the resulting vector.
function MaxSpeedInDirection (desiredMovementDirection : Vector3) : float {
	if (desiredMovementDirection == Vector3.zero)
		return 0;
	else {
		var zAxisEllipseMultiplier : float = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
		var temp : Vector3 = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
		var length : float = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
		return length;
	}
}

function SetVelocity (velocity : Vector3) {
	grounded = false;
	movement.velocity = velocity;
	movement.frameVelocity = Vector3.zero;
	SendMessage("OnExternalVelocity");
}

function OnTriggerEnter(other: Collider)
		{
		Debug.Log ("Collision with " + other.name);
		
		if (other.name == "ExitZone") {
		move=false;
		anim.SetBool("motion",false);
		}
		
		if (other.name == "sinkTrigger") {
		move = false;
		sink = true;}
			//trying out PlayerPrefs restriction
		/*	Vector3 pos = transform.position;
			pos.z = -15;
			pos.x =39;
			transform.position = new Vector3(pos.x, transform.position.y, pos.z );
			//transform.position.z = pos.z;


			transform.position = pos;
			anim.SetBool("sink",true);
			audio.PlayOneShot(drowning);
			s_count = 0;*/
//			GameObject playermotion = GameObject.Find("ForBumpyanimation");
//
//
//			
//			for(int loopVariable = 0 ; loopVariable < 3; loopVariable++)
//			{
//				playermotion.animation.Play("SinkMotion");
//				
//				audio.PlayOneShot(drowning);
//				
//				if (Input.GetKeyDown("s"))
//				{
//					playermotion.animation.Stop("SinkMotion");
//					break;
//				}
			//}
			
		}


// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterController)
@script AddComponentMenu ("Character/Character Motor")
