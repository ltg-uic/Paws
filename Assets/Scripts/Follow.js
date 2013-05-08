
var target : Transform;
var polarBear : Transform;
function Start(){
  polarBear = GameObject.Find("PolarBear(Clone)").transform;
}
function Update () {

if (Vector3.Distance(polarBear.position,target.position) < 20)
{
    transform.LookAt(target);
 }
 
 
}