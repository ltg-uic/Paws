#pragma strict
#pragma implicit
#pragma downcast

var footPrintsEnabled : boolean;
var footprintR : GameObject; // Right footprint object.
var footprintL : GameObject; // Left footprint object.
var footDelay : float; // Time between footprints.
var decayRate : int; // Time at which footprints disappear.
var minSpeed : float; // Lowest speed (of main camera) to draw footprints.
var groundDistance : float; // Lowest distance (from player) to draw footprints. For jumping.
var offset : Vector3; // Offset footprint positions.
var footSounds : AudioClip[];

static var playerRigidbody : Rigidbody;
static var playerVelocity : float;
private var lastPrint : float; 
private var rightLeft : boolean = true;
private var mytransform : Transform;
private var feet : GameObject;
private var reflected : Quaternion;
private var ry: float;
static var acum: float;


//Set transform to mytransform, for performance;
function Awake ()
{
	mytransform = transform;
}

//Every fixed frame, if the character controller is moving, and the accumulation level is above a threshold, make footprints.
function FixedUpdate ()
{
	playerRigidbody = rigidbody;
	playerVelocity = playerRigidbody.velocity.magnitude;
	var delay : float = (playerVelocity) * (0.035 * footDelay) ;
	acum = SnowGlobalControl.varAcum;
	
	if (playerVelocity > minSpeed && acum > 0.45 && footPrintsEnabled == true)
	{
		FootPrints();
	}
}

//When a down cast ray hits a surface tagged "Ground" , at an interval, a right foot print is instantiated, then a left one, at the point of impact.
//The x and z  euler angles (rotation) of the footprints are set towards the normal of the impact, while the y value is set to the player object's.
//The footprints are then destroyed after a time.
function FootPrints () 
{
        var hit : RaycastHit;
		var nextSound : AudioClip = footSounds[Random.Range(0,footSounds.length)];
		var castPosition : Vector3 = mytransform.position + offset;
		
        if (Physics.Raycast (castPosition, -Vector3.up, hit, groundDistance) && hit.collider.CompareTag("Ground")) 
        {
				
			if (Time.time > lastPrint + footDelay)
			{
				reflected = Quaternion.FromToRotation (Vector3.up, hit.normal);
				ry = mytransform.eulerAngles.y;
				
				if(rightLeft == true)
				{
				feet = Instantiate (footprintR, (hit.point + offset), reflected) as GameObject;
				feet.transform.Rotate(0,(ry),0, Space.Self);
				rightLeft = false;
				audio.clip = nextSound;
				audio.Play();
				}
				else
				{
			
				feet = Instantiate (footprintL, (hit.point + offset), reflected) as GameObject;
				feet.transform.Rotate(0,(ry),0, Space.Self);
				rightLeft = true;
				audio.clip = nextSound;
				audio.Play();
				}
				
				
				lastPrint = Time.time;
				
				Destroy (feet, decayRate);
			}
        }
}


