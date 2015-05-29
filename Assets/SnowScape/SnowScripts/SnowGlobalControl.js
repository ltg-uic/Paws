#pragma strict
#pragma implicit
#pragma downcast

var snowFall : boolean; // Master control toggle.
var transitionSpeed : float =1.0; // Master time rate.
var offLevel : float = 1.0; // Level at which snow auto-stops. If > 1.0 then snow never stops.
var melt : boolean; //Melting toggle, only works if snowFall is off.
static var varAcum : float; // Master control variable, 1.0 is maximum value.
static var snowDir : int; // Controls whether snow is accumulating or melting.
static var extSnowFall : boolean; // Feeds master toggle to other scripts.
static var extTransitionSpeed : float; // Feeds master time rate to other scripts.

private var mytransform : Transform;
private var vertices : Vector3[];
private var vertCount : int;
private var snowAcum : GameObject;
private var snowPart : GameObject;

//Every fixed frame, set conditions and a toggle for enabling and disabling the accumulation
function FixedUpdate () 
{
	extSnowFall = snowFall;
	extTransitionSpeed = transitionSpeed;
//if snow is falling, increase accumulation and depth.
	if (varAcum >= offLevel)
	{
	snowFall = false;
	}
	if (snowFall == true)
	{ 		
		snowDir = 1; 
	}
//else decrease (evaporate) accumulation and depth
	else  if (melt == true)
	{ 
		snowDir = -1;
	}
	else
	{
		snowDir = 0;
	}
	
	Acumulate();

}

//Get all surfaces tagged "SnowSurfaces", and all particle emitters tagged "SnowParticles",
//then get all surfaces' material opacity levels, and increases or decreases them over time.
function Acumulate ()
{
	var snowAcumulators: GameObject[] = GameObject.FindGameObjectsWithTag("SnowSurfaces");
	var snowParticles : GameObject[] = GameObject.FindGameObjectsWithTag("SnowParticles");
	
	for(var snowAcum : GameObject in snowAcumulators)
	{	
		var currentAcum = snowAcum.GetComponent.<Renderer>().material.color.a;
			varAcum = Mathf.Lerp(currentAcum, currentAcum + snowDir, Time.deltaTime * (transitionSpeed*0.0005)); //0.0005 is an arbitrary basement i chose.
		
		if(varAcum < 1 && varAcum > 0.1) //  VERY IMPORTANT! - this is the basement alpha level, surfaces below this wont accumulate ever. To find the alpha value of an object, divide the color instpector alpha/255.
		{
			
			snowAcum.GetComponent.<Renderer>().material.color.a = varAcum; // all effects are coordinated using the alpha value of our Snow Material.
		}
	}
	
//Enable or disable all particle emitters tagged snowParticles when snowing.
	for(var snowPart : GameObject in snowParticles)
	{
		if (snowFall == true)
		{
			snowPart.GetComponent.<ParticleEmitter>().emit = true;
		}
		else
		{
			snowPart.GetComponent.<ParticleEmitter>().emit = false;
		}
	}
}
