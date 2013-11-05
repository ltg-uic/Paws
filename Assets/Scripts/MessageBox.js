
private var message: String = " ";
var newSkin: GUISkin;
var areaWidth : float;
var areaHeight : float;
function Start () {
	message = "";
	areaWidth = GetComponent(NetworkConnectionServer).areaWidth;
	areaHeight = GetComponent(NetworkConnectionServer).areaHeight;
}

public function ShowOnGUI () {
    GUI.skin = newSkin; 
	if (message.length > 0){
	  var x = areaWidth/2-areaWidth*0.2;
	  var y = areaHeight/2-areaHeight*0.2;
	  var w = areaWidth*0.4;
	  var h = areaHeight*0.2;
      GUI.Box(Rect(x,y,w,h),message);
      
      if (GUI.Button(Rect(x+w/2-areaWidth*0.05,y+h-areaHeight*0.05,areaWidth*0.1,areaHeight*0.04),"OK")){
      	  message = "";
      }
    }
}

function DisplayMessage(_message: String){
  message = _message;
}