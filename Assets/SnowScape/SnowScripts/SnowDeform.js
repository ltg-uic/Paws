#pragma strict
#pragma implicit
#pragma downcast

private var acum : float;
private var snowFall : boolean;
private var snowRise : boolean;
private var snowDeform : boolean;
private var snowPlow : boolean;
private var transitionSpeed : float;
var riseScale : int;
var maximumHeight : float;
var plowDistance : float;
var plowPower : int;
var plowingEnabled : boolean;
static var plow : GameObject;
private var plowTransform : Transform;
var plowParticles : GameObject;
private var mytransform : Transform;
private var vertices : Vector3[];
private var vertCount : int;
private var snowAcum : GameObject;
private var snowPart : GameObject;

//On Awake, set the vertex count of the deformable ground object's built-in array, 
//to optimizes deformation performance.
function Awake ()
{
	//For performance, use mytransform instead of transform for calculations;
	mytransform = transform;
	var mesh = (GetComponent(MeshFilter) as MeshFilter).mesh;
	vertCount = mesh.vertices.length;
}

//Every fixed frame, set conditions and a toggle for enabling and disabling the deformation.
function FixedUpdate () {
	transitionSpeed = SnowGlobalControl.extTransitionSpeed;
	snowDir = SnowGlobalControl.snowDir;
	acum = SnowGlobalControl.varAcum;
	snowFall = SnowGlobalControl.extSnowFall;

		if(acum < 0.52) // 0.52 is the point at which the snow covers 100% of the ground, depth increase is ugly when there are holes.
		{
			snowRise = false;
		}
		else if (acum < maximumHeight) // only rise if below threshold, so that players can't see through the mesh.
		{
			snowRise = true;
		}
		else
		{
			snowRise = false;
		}
		if (acum > 0.52) //leave plowing on, even when rising has stopped.
			{
				snowDeform = true; 
				snowPlow = true;
				Deform();
			}
		else
			{
				snowDeform = false;
				snowPlow = false;
			}

}
//Get the ground object's vertices into an array, increase the vertices' heights randomly, over time, to a threshold.
//If the ground object is a certain distance from the plow object, then decrease the height of the vertices within range, over time.
function Deform ()
{
	snowDir = SnowGlobalControl.snowDir;
	var mesh = (GetComponent(MeshFilter) as MeshFilter).mesh;
	var vertices : Vector3[] = mesh.vertices;
	plow = GameObject.FindWithTag("Player");
	plowTransform = plow.transform;
	
	for (var i=0;i<vertCount;i++)
	{
		var worldVert = mytransform.TransformPoint(vertices[i]);	
		var dist = Vector3.Distance(plowTransform.position, worldVert);
			if(snowRise == true)
			{
				vertices[i].y += (riseScale * SnowGlobalControl.snowDir * (Time.deltaTime * (transitionSpeed * 0.0005) ));
			}
			if(dist < plowDistance && snowPlow == true && SnowFootprints.playerVelocity > 0.1)
			{
				Instantiate(plowParticles, plowTransform.position , Quaternion.identity);
				vertices[i].y -= plowPower * acum * (Time.deltaTime * 0.02);
			}
	}
//Force deformation into a fixed time-step, greatly increasing performance.
	yield new WaitForFixedUpdate ();
//Apply all vertex calculations to the mesh.
	mesh.vertices = vertices;
	mesh.RecalculateNormals();
}
