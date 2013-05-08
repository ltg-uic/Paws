
var startGame: boolean = false;

var mySkin: GUISkin;
var areaWidth : float = 640;
var areaHeight : float = 480;
var backgroundTexture : Texture;

private var yearList: Array;
private var refYear: int;

function Start(){
	LoadingLevelValues();
}

function LoadingLevelValues(){
    yearList = new Array();
    
    if (PlayerPrefs.HasKey("Level1")){	
		yearList.Add(int.Parse(PlayerPrefs.GetString("Level1")));
		yearList.Add(int.Parse(PlayerPrefs.GetString("Level2")));
		yearList.Add(int.Parse(PlayerPrefs.GetString("Level3")));
	  	refYear = int.Parse(PlayerPrefs.GetString("refLevel"));
  	}else{
  		PlayerPrefs.SetString("Level1","1975");
  		yearList.Add(1975);
  		PlayerPrefs.SetString("Level2","2010");
  		yearList.Add(2010);
  		PlayerPrefs.SetString("Level3","2045");
  		yearList.Add(2045);
  		PlayerPrefs.SetString("refLevel","2010");
  		refYear = 2010;
 	    
   		PlayerPrefs.Save();
  	}
}

function OnLevelWasLoaded (level : int) {
	startGame = true;
}

function HideInitialMessage(){
	yield WaitForSeconds(4.0);
	startGame = false;
}

function OnGUI(){
  GUI.skin = mySkin;

  var ScreenX = ((Screen.width * 0.5) - (areaWidth * 0.5));
  var ScreenY = ((Screen.height * 0.5) - (areaHeight * 0.5));
    
  
  if (startGame){
  		var index: int = Application.loadedLevel;
  		var yearsAgo : int = (int.Parse(yearList[index])) - refYear;

		
		GUI.BeginGroup (new Rect (ScreenX, ScreenY, areaWidth, areaHeight));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
		
		GUI.DrawTexture (new Rect (0, 0, areaWidth, areaHeight), backgroundTexture);
		if (yearsAgo < 0)
	  		GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore Beaufort Sea \n" + Mathf.Abs(yearsAgo) + " years ago.");
  		else if (yearsAgo == 0)
	  		GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore Beaufort Sea.");
	  	else 
			GUI.Label (new Rect (0,60,areaWidth,areaHeight), "Explore Beaufort Sea \n" + yearsAgo + " years from now. ");
		GUI.EndGroup ();

  }

}
   
   