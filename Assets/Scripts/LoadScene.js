
var _originalPosition: Vector3 = new Vector3(904,15,620);
var startGame: boolean = false;

var mySkin: GUISkin;
var areaWidth : float = 640;
var areaHeight : float = 480;
var backgroundTexture : Texture;
private var _year:String;

function HideInitialMessage(){
	startGame = false;
}

function OnGUI(){
  GUI.skin = mySkin;

  var ScreenX = ((Screen.width * 0.5) - (areaWidth * 0.5));
  var ScreenY = ((Screen.height * 0.5) - (areaHeight * 0.5));
    
  
  if (startGame){
  	
		GUI.BeginGroup (new Rect (ScreenX, ScreenY, areaWidth, areaHeight));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
		
		GUI.DrawTexture (new Rect (0, 0, areaWidth, areaHeight), backgroundTexture);
		if  (_year == "1975")
	  		GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore\nBeaufort Sea\nin 1975");
  		else if (_year == "2010")
	  		GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore\nBeaufort Sea\nin 2010");
	  	else if (_year == "2045")
			GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore\nBeaufort Sea\nin 2045 ");
		GUI.EndGroup ();

  }

}

function  LoadYear(year :String){
	_year = year;

	 var polarBear:GameObject = GameObject.Find("PolarBear(Clone)");
     var water:GameObject = GameObject.FindGameObjectWithTag("Water");
     var seal:GameObject = GameObject.FindGameObjectWithTag("Goal");
 	 print("LoadScene::LoadYear "+_year);
 	 water.transform.position = new Vector3(842+(parseInt(_year)*2000),10.25,785);
 	 seal.transform.position = new Vector3(903+(parseInt(_year)*2000),10.91528,842.7151);
 	 polarBear.transform.position = Vector3(_originalPosition.x+(parseInt(_year)*2000),_originalPosition.y,_originalPosition.z);
	 
	 startGame = true;
	 yield WaitForSeconds(3.0);
	 startGame = false;
}