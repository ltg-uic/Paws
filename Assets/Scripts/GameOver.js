var reachedGoal: boolean;
private var _gameOver : boolean = false;
private var _consumedCalories : float;
private var ScreenX;
private var ScreenY;

private var _timeOut: boolean = false;
private var _counterMsg: int = 0;

var mySkin: GUISkin;
var areaWidth : float = 640;
var areaHeight : float = 480;
var backgroundTexture : Texture;

var gameOverSound : AudioClip;
var reachGoalSound : AudioClip;

function GameOver(_value: float){
   _gameOver = true;
   _consumedCalories = _value;
   _timeOut = false;
   	yield WaitForSeconds(4.0);
   	_gameOver = false;
}

function StartGame(){
   _gameOver = false;
   _consumedCalories = 0;
   reachedGoal = false;
   _timeOut = false;
}

function GameOverTimeOut(_value: float){
   _gameOver = true;
   _consumedCalories = _value;
   _timeOut = true;
  // 	yield WaitForSeconds(4.0);
   //	_gameOver = false;
   _counterMsg=0;
   yield WaitForSeconds(5.0);
   GameObject.FindWithTag("Goal").audio.mute = true;
 }

function GoalReached(){
   reachedGoal = true;
   GameObject.FindWithTag("Goal").audio.mute = true;
   GameObject.Find("PolarBear(Clone)").audio.PlayOneShot(reachGoalSound);
}

function OnGUI(){
   GUI.skin = mySkin;
  
  ScreenX = ((Screen.width * 0.5) - (areaWidth * 0.5));
  ScreenY = ((Screen.height * 0.5) - (areaHeight * 0.5));
    
  if (_gameOver){  
  		// Make a group on the center of the screen
	//GUI.BeginGroup (new Rect (ScreenX, ScreenY, areaWidh, areaHeight));
		GUI.BeginGroup (new Rect (Screen.width / 2 -320, Screen.height / 2 - 240, areaWidth, areaHeight));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
		  Debug.Log("counterMsg"+_counterMsg);
		GUI.DrawTexture (new Rect (0, 0, areaWidth, areaHeight), backgroundTexture);
		if (!reachedGoal && _timeOut){
		  	GUI.Label (new Rect (0,100,areaWidth,areaHeight), "GAME OVER - TIME OUT");
		}
		else if (!reachedGoal && !_timeOut){
		  	GUI.Label (new Rect (0,100,areaWidth,areaHeight), "GAME OVER");
		  	_counterMsg++;
		  	if (_counterMsg > 120)
		  		_gameOver = false;
		}
	  	else
		  	GUI.Label (new Rect (0, 120, areaWidth, areaHeight), " You reached your target! ");
		if (_consumedCalories >0)
			GUI.Label (new Rect (0,160,areaWidth,areaHeight)," Kcal Burned = " + (Mathf.Round(_consumedCalories*100)/100));

		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();
	}
}
   
   