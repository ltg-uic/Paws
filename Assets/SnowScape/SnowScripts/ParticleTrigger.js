var snowControl : GameObject;

function OnTriggerEnter (other : Collider) {
if(other.gameObject.tag == "Player"){
snowControl.GetComponent(SnowGlobalControl).snowFall = false;
}
}

function OnTriggerExit (other : Collider) {
if(other.gameObject.tag == "Player"){
snowControl.GetComponent(SnowGlobalControl).snowFall = true;
}
}