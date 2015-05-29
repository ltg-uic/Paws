
var inSplashSound : AudioClip;
var outSplashSound : AudioClip;
var underWaterSound : AudioClip;
var inWater:boolean;
 
function Start(){
	inWater = false;	
}
	
function OutOfWater(){
   if (inWater){
    GetComponent.<AudioSource>().volume = 0.5f;
    ToggleAllEmitters(true,"WaterDripEmitter");
    GetComponent.<AudioSource>().PlayOneShot(outSplashSound);
	yield WaitForSeconds(3);
	ToggleAllEmitters(false,"WaterDripEmitter");
	inWater =false;
   }
}

function InsideWater(){
   if (!inWater){
	ToggleAllEmitters(true,"WaterSplashEmitter");
	GetComponent.<AudioSource>().volume = 0.6f;
    GetComponent.<AudioSource>().PlayOneShot(inSplashSound);
    GetComponent.<AudioSource>().volume = 0.8f;
    GetComponent.<AudioSource>().PlayOneShot(underWaterSound);
	yield WaitForSeconds(3);
	ToggleAllEmitters(false, "WaterSplashEmitter");
	inWater = true;
   }
}

function ToggleAllEmitters(emit : boolean, type : String) {
     var particleSystems = GameObject.FindWithTag(type);
     particleSystems.GetComponent.<ParticleEmitter>().enabled = emit;
}

		