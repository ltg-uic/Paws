
var newSkin: GUISkin;
var areaWidth : float;
var areaHeight : float;
var labelHeight: float;
var posX:float;
var posY:float;
var yearBtnTexture : Texture2D[];
var durationBtnTexture : Texture2D[];

var _showGameParameters: boolean;
var _currentYearIndex: int;
var _gameDurationIndex:int;

function Start () {
	_currentYearIndex = 0;
	_gameDurationIndex = 0;
}

public function ShowOnGUI () {

	if (_showGameParameters){
	   // Debug.Log("Positions "+posX+" " +posY);
	    GUILayout.BeginArea (Rect (posX, posY, areaWidth, labelHeight));
		GUILayout.Label("Year: "+GetComponent(NetworkConnectionServer).yearList[_currentYearIndex]);
		GUILayout.EndArea();

		for (var cnt:int  = 0; cnt < yearBtnTexture.Length; cnt++){
	 	   GUILayout.BeginArea (Rect (posX+105*(cnt),posY+labelHeight, 100, 100));
	 	   if (GUILayout.Button(yearBtnTexture[cnt])){
	 	   		_currentYearIndex = cnt;
	 	   } 
	 	   GUILayout.EndArea();
	    }
				   
				    
		GUILayout.BeginArea (Rect (posX, posY+labelHeight+100, areaWidth, labelHeight));
		GUILayout.Label("Game duration: "+GetComponent(NetworkConnectionServer).gameDurationList[_gameDurationIndex]+" min");
		GUILayout.EndArea();
	
		for (var cnt2:int  = 0; cnt2 < durationBtnTexture.Length; cnt2++){
	 	   GUILayout.BeginArea (Rect (posX+105*(cnt2),posY+(2*labelHeight)+100, 100, 100));
	 	   if (GUILayout.Button(durationBtnTexture[cnt2])){
	 	   		_gameDurationIndex = cnt2;
	 	   } 
	 	   GUILayout.EndArea();
	    } 
	 
      GUILayout.BeginArea (Rect (posX, posY + areaHeight - 60, 140, 60));
	  if (GUILayout.Button ("Start"))
	  {		
		//	if (GetComponent(NetworkConnectionServer).interpreterName.CompareTo("Interpreter...")){
			    GetComponent(CurrentPosInMap).SetCurrentYear(GetComponent(NetworkConnectionServer).yearList[_currentYearIndex]);
			    var  _map : Texture2D = Resources.Load("Images/"+GetComponent(NetworkConnectionServer).yearList[_currentYearIndex].ToString()+"_Location_Map", typeof(Texture2D));
	            GetComponent(CurrentPosInMap).mapImage = _map;
			    showGameParameters(false);
			    GetComponent(NetworkConnectionServer).LoadScores();
			    GetComponent(NetworkConnectionServer).StartGame();
			 
		//	}
		//	else
		//	{
		//	    GetComponent(MessageBox).DisplayMessage("Select an interpreter.");
		//	}
	   }
				
	   GUILayout.EndArea ();
	   
	     GUILayout.BeginArea (Rect (posX + 250, posY + areaHeight - 60, 140, 60));
	     if (GUILayout.Button ("Cancel"))
	     {		
			showGameParameters(false);
	   }
				
	   GUILayout.EndArea ();
     
    }
}

function showGameParameters(_value: boolean){
  _showGameParameters = _value;
}

function getCurrentYearIndex(){
	return _currentYearIndex;
}

function getDurationGameIndex(){
	return _gameDurationIndex;
}
