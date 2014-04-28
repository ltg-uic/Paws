#pragma strict
var i:int ;

function Update () {
	if (Input.GetKey("1")){
		Application.LoadLevel(0);
	 	print("loading.."+(i++));
     }
}
