var reachedGoal: boolean;
private var _gameOver : boolean = false;
private var _consumedCalories : float;
private var ScreenX;
private var ScreenY;

var startGame: boolean = false;

var mySkin: GUISkin;
var areaWidth : float = 640;
var areaHeight : float = 480;
var backgroundTexture : Texture;

var gameOverSound : AudioClip;
var reachGoalSound : AudioClip;

function GameOver(_value: float){
   _gameOver = true;
   _consumedCalories = _value;
}

function StartGame(){
   _gameOver = false;
   _consumedCalories = 0;
   reachedGoal = false;
}

function HideInitialMessage(){
	startGame = false;
}
function GoalReached(){
   reachedGoal = true;
   GameObject.Find("PolarBear(Clone)").audio.PlayOneShot(reachGoalSound);
}

function OnGUI(){
   GUI.skin = mySkin;
  
  ScreenX = ((Screen.width * 0.5) - (areaWidth * 0.5));
  ScreenY = ((Screen.height * 0.5) - (areaHeight * 0.5));
    
  if (_gameOver && _consumedCalories > 0){  
  		// Make a group on the center of the screen
	//GUI.BeginGroup (new Rect (ScreenX, ScreenY, areaWidh, areaHeight));
		GUI.BeginGroup (new Rect (Screen.width / 2 -320, Screen.height / 2 - 240, areaWidth, areaHeight));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
		
		GUI.DrawTexture (new Rect (0, 0, areaWidth, areaHeight), backgroundTexture);
		if (!reachedGoal)
		  	GUI.Label (new Rect (0,60,areaWidth,areaHeight), "GAME OVER");
	  	else
		  	GUI.Label (new Rect (0, 120, areaWidth, areaHeight), " You reached your target! ");
		
		GUI.Label (new Rect (0,160,areaWidth,areaHeight)," Kcal Burned = " + (Mathf.Round(_consumedCalories*100)/100));

		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();
	}
}
   
   