
var inSplashSound : AudioClip;
var outSplashSound : AudioClip;
var underWaterSound : AudioClip;
var inWater:boolean;
 
function Start(){
	inWater = false;	
}
	
function OutOfWater(){
   if (inWater){
    audio.volume = 0.5f;
    ToggleAllEmitters(true,"WaterDripEmitter");
    audio.PlayOneShot(outSplashSound);
	yield WaitForSeconds(3);
	ToggleAllEmitters(false,"WaterDripEmitter");
	inWater =false;
   }
}

function InsideWater(){
   if (!inWater){
	ToggleAllEmitters(true,"WaterSplashEmitter");
	audio.volume = 0.6f;
    audio.PlayOneShot(inSplashSound);
    audio.volume = 0.8f;
    audio.PlayOneShot(underWaterSound);
	yield WaitForSeconds(3);
	ToggleAllEmitters(false, "WaterSplashEmitter");
	inWater = true;
   }
}

function ToggleAllEmitters(emit : boolean, type : String) {
     var particleSystems = GameObject.FindWithTag(type);
     particleSystems.particleEmitter.enabled = emit;
}

		