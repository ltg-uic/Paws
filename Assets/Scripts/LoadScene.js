
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
	 if (_year == "1975"){
	     print("Loading "+_year);
	     polarBear.transform.position = _originalPosition;
	     water.transform.position = new Vector3(842,10.25,785);
	     seal.transform.position = new Vector3(903.1138,10.91528,842.7151);
	 }
	 else if (_year == "2010"){
	  print("Loading "+_year);
	     polarBear.transform.position = Vector3(_originalPosition.x+2000,_originalPosition.y,_originalPosition.z);
         water.transform.position = new Vector3(2842,10.25,785);
         seal.transform.position = new Vector3(2903,11.1774,843);
	  }  
	 else if(_year == "2045"){
	  print("Loading "+_year);
	    polarBear.transform.position = Vector3(_originalPosition.x+4000,_originalPosition.y,_originalPosition.z);
	     water.transform.position = new Vector3(4842,10.25,785);
	     seal.transform.position = new Vector3(4903,10.86695,841);
	 }	
	 startGame = true;
	 yield WaitForSeconds(3.0);
	 startGame = false;
}