var iceHitSound : AudioClip;
var waterSplash : AudioClip;
var waterUnder : AudioClip;
var landSteps : AudioClip;

function OnTriggerEnter(hit : Collider){ 
		if(hit.gameObject.tag == "Water"){
			IsInWater();
		}
//		if(hit.gameObject.tag == "UnderWater"){
//			IsUnderWater();
//		}	
}

function OnTriggerExit(hit : Collider){ 
		if(hit.gameObject.tag == "Water"){
			IsOutOfWater();
		}	
}

function IsOnLand(){
	print ("walinkg now");
	audio.clip = landSteps;
	audio.loop = true;
	audio.volume = 1;
	audio.Play();
}

function IsInWater(){
	print ("in Water");
	audio.clip = waterSplash;
	audio.loop = true;
	audio.volume = 1;
	audio.Play();
	
	gameObject.SendMessage("SetGravityToZero");
}

function IsOutOfWater(){
	FadeAudio(1);
}

function OnControllerColliderHit(other : ControllerColliderHit){
		if(other.gameObject.tag == "FloatingIce"){
			HitTheIce();
		}
//		if(other.gameObject.tag == "UnderWater"){
//			IsUnderWater();
//		}
}

function IsUnderWater(){
	print ("drawning");
	audio.clip = waterUnder;
	audio.loop = true;
	audio.volume = 1;
	audio.Play();
}

function HitTheIce(){
	audio.PlayOneShot (iceHitSound);
}

function FadeAudio (timer : float) {
	var start = 1.0;
	var end = 0.0;
	var i = 0.0;
	var step = 1.0/timer;

	while (i <= 1.0) {
		i += step * Time.deltaTime;
		audio.volume = Mathf.Lerp(start, end, i);
		yield;
	}
}

@script RequireComponent(AudioSource)

//
////// A grenade
//// - instantiates a explosion prefab when hitting a surface
//// - then destroys itself
//
//var explosionPrefab : Transform;
//
//function OnCollisionEnter(collision : Collision) {
//    // Rotate the object so that the y-axis faces along the normal of the surface
//    var contact : ContactPoint = collision.contacts[0];
//    var rot : Quaternion = Quaternion.FromToRotation(Vector3.up, contact.normal);
//    var pos : Vector3 = contact.point;
//    Instantiate(explosionPrefab, pos, rot);
//    // Destroy the projectile
//    Destroy (gameObject);
//}