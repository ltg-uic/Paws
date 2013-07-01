
var remoteIP = "127.0.0.1";
var remotePort = 25000;
var listenPort = 25000;
var useNAT = false;
var yourIP = "";
var yourPort = "";

var newSkin: GUISkin;

//BLs
var areaWidth : float;
var areaHeight : float;
var backgroundTexture : Texture;
var startBtnTexture: Texture;
var leftBtnTexture: Texture;
var rightBtnTexture: Texture;

var isPlaying: boolean;
var burnedCalories: float;
var gameTime: int;  // Time in minutes to stop the game.
var meters: float;
var elapsedTime: float;
var interpreterName:String;

private var _scoresCount: int;
private var _scoreValues:ArrayList;
private var _scoreNames: ArrayList;
private var _caloriesXWalkSteps:float;
private var _caloriesXSwimStesp:float;
private var _numWalkSteps: int;
private var _numSwimSteps: int;
private var _startTime: float;
private var _metersXWalkSteps:float;
private var _metersXSwimSteps:float;
private var _playerNumber: int;
private var _currentYear:int;
private var _previousYear:int;
private var _showGraphView:boolean;
private var _showSummary:boolean;
private var _goalReached:boolean;
private var _timerReached:boolean;
private var _currentAvg:float;
private var _minYear : int = 0;
private var _maxYear : int = 2;
private var _serverReady : boolean;
private var _playerName : String;
private var _initialPos : Vector2;
private var _currentPos : Vector2;
private var _displaySeconds: int;
private var _displayMinutes: int;
private var _savedAvg: boolean;
private var _savedLog: boolean;
private var _scoresLoaded: boolean;
private var ScreenX: float;
private var ScreenY: float;
private var _showInfo:boolean;
private var _showInterpreterList:boolean = false;

var yearList = [1975,2010,2045];
private var _yearAgoList = [-35,0,35];
private var _yearLabels = ["1975","2010","2045"];

var applicationPath:String = "";

private var _innerController : int;			 
			       
//variable for the heigh of the lable text
private var _labelHeight :float;
			       
function Start(){
  // BL added for screen distribution of elements
    ScreenX = ((Screen.width * 0.5) - (areaWidth * 0.5));
    ScreenY = ((Screen.height * 0.5) - (areaHeight * 0.5));
	
	_labelHeight = areaHeight*0.04;
	
    GetComponent(BurnedCaloriesGraph).posX = ScreenX + areaWidth*0.575;       
	GetComponent(BurnedCaloriesGraph).posY = Screen.height - (ScreenY + areaHeight*0.625);
	
	GetComponent(BurnedCaloriesGraph).width = areaWidth*0.35;       
	GetComponent(BurnedCaloriesGraph).height = areaHeight*0.35;
	
	GetComponent(CurrentPosInMap).posX = 0;
	GetComponent(CurrentPosInMap).posY = 0;
	GetComponent(CurrentPosInMap).mapWidth = areaWidth*0.4;
	GetComponent(CurrentPosInMap).mapHeight = areaHeight*0.4;
	
	GetComponent(SummaryGraph).posX = ScreenX + areaWidth*0.125;
	GetComponent(SummaryGraph).posY = Screen.height - (ScreenY  + areaHeight*0.725);
	GetComponent(SummaryGraph).height = areaHeight*0.45;
	GetComponent(SummaryGraph).width = areaWidth*0.425;
	GetComponent(SummaryGraph).numYears = yearList.Length;

    GetComponent(InfoPolarBear).posX = areaWidth*0.1;
	GetComponent(InfoPolarBear).posY = areaHeight*0.1;
	GetComponent(InfoPolarBear).infoWidth = areaWidth*0.8;
	GetComponent(InfoPolarBear).infoHeight = areaHeight*0.8;
	
    Initialize();
    
    applicationPath = Application.dataPath;
    if (Application.platform == RuntimePlatform.OSXPlayer) {
        applicationPath += "/../../";
    }
    else if (Application.platform == RuntimePlatform.WindowsPlayer) {
        applicationPath += "/../";
    }

	_scoreValues = new ArrayList();
	_scoreNames = new ArrayList();
}

function Initialize(){
	interpreterName = "...";
    _displayMinutes = 0;
    elapsedTime = 0;
	_currentYear = _minYear;
	_previousYear = _currentYear;
	_showGraphView = false;
	isPlaying = false;
	_innerController = 1;
	_serverReady = false;	
	_numSwimSteps = _numWalkSteps = _playerNumber = 0;
	burnedCalories = meters = _displaySeconds = _displayMinutes = 0;
	_playerName = "....";
	_goalReached = false;
	_timerReached = false;
	_initialPos = new Vector2(0.0,0.0);
	_currentPos = new Vector2(0.0,0.0);
    _savedAvg = false;
    _savedLog = false;
	var  _map : Texture2D = Resources.Load("Images/"+yearList[_currentYear]+"_Location_Map", typeof(Texture2D));
	GetComponent(CurrentPosInMap).mapImage = _map;
	GetComponent(BurnedCaloriesGraph).DestroyLines();
	GetComponent(CurrentPosInMap).InitializeMap();
}

function StartGame(){
	_startTime = Time.realtimeSinceStartup;
	GetComponent(SummaryGraph).SetCurrentYear(_currentYear);
}

function OnConnectedToServer () {
    Debug.Log("Connected to Server... ");
    
	// Notify our objects that the level and the network are ready
	for (var go : GameObject in FindObjectsOfType(GameObject))
		go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
}
function Update(){		
	if (Network.peerType == NetworkPeerType.Disconnected){
	    _serverReady = false;
		// If server not connected
		// Creating server
		Network.InitializeServer(10, listenPort,useNAT);
		_numSwimSteps = _numWalkSteps = _playerNumber = 0;
        burnedCalories = 0;
		// Notify our objects that the level and the network is ready
		for (var go : GameObject in FindObjectsOfType(GameObject)){
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}

	}
	else{
	   _serverReady = true;
	  // ipaddress = Network.player.ipAddress;
		//port = Network.player.port.ToString();
    }
	   
	if (isPlaying && !_goalReached && !_timerReached)
	{
	  if (_displayMinutes < gameTime){
	    burnedCalories =  (_numWalkSteps * 0.4445) + (_numSwimSteps * 1.15583174);
   //     Debug.Log("Current pos: " +_currentPos + " initial pos: " + _initialPos);
		meters = Vector2.Distance(_initialPos,_currentPos);

		elapsedTime =  Time.realtimeSinceStartup - _startTime;
	  }			
	}
	
	if(Input.GetKeyDown(KeyCode.H))  // Hide Polar Bear Info.
			GetComponent(InfoPolarBear).ShowInfo(false);
	else if(Input.GetKeyDown(KeyCode.S))
			GetComponent(InfoPolarBear).ShowInfo(true);	
}

function OnGUI () {

   if ( _serverReady){
		
	     DrawViews();
   }
	
	if (Event.current.type == EventType.Layout)
	{
		_innerController = 1;
	}
	
	if (Event.current.type == EventType.Repaint)
	{
		_innerController = 2;
	}
	
	if (_innerController == 2)
	{
		_showSummary = _showGraphView;
		_innerController = 0;
	}
	   
}

function DrawViews(){
	
  	GUI.skin = newSkin;
  	
	    
	GUILayout.BeginArea(Rect(ScreenX,ScreenY, areaWidth, areaHeight));
  	GUI.DrawTexture (Rect (0, 0, areaWidth, areaHeight), backgroundTexture);
  		
  	if (!_showSummary)
  	{  // Draw the main view.
			// Area containing the year slider and selection
			GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.205, areaWidth*0.4, _labelHeight));
			GUILayout.Label("Bird's eye view");	
			GUILayout.EndArea();		
			// Area rendering the map and it's labels
			GUILayout.BeginArea (Rect (areaWidth*0.1,areaHeight*0.25,areaWidth*0.4,areaHeight*0.4));
			if (!isPlaying)
				GetComponent(CurrentPosInMap).SetCurrentYear(_currentYear);
			GetComponent(CurrentPosInMap).DrawMap();
			GUILayout.EndArea();
		
		
			GUILayout.BeginArea (Rect (areaWidth*0.55, areaHeight*0.205, areaWidth*0.4, _labelHeight));
			GUILayout.Label("Bear calories burned: " + (Mathf.Round(burnedCalories*100)/100) + " Calories");  
		    GUILayout.EndArea();
		    
		    GUILayout.BeginArea(Rect (areaWidth*0.55, areaHeight*0.25, areaWidth*0.4, areaHeight*0.4));
		    GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(areaHeight*0.4), GUILayout.Width(areaWidth*0.4));
		    GUILayout.Space(areaHeight*0.4);
		    GUILayout.EndVertical();
		    GUILayout.EndArea();
		    
			
		    GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.7, areaWidth*0.4, _labelHeight));
			//Disntance in meters
			//GUILayout.Label("Distance covered: "+ (Mathf.Round(meters*100)/100) + " meters");
			//Distance in feet
			GUILayout.Label("Distance covered: "+ (Mathf.Round(meters*3.2808*100)/100) + " Ft.");
			GUILayout.EndArea();
			
			GUILayout.BeginArea (Rect (areaWidth*0.55, areaHeight*0.7, areaWidth*0.4, _labelHeight));
			if (isPlaying){
				_displaySeconds = Mathf.CeilToInt(elapsedTime)%60;
				_displayMinutes = Mathf.CeilToInt(elapsedTime)*0.01666667;
			}
			
			GUILayout.Label("Elapsed time: "+ _displayMinutes.ToString("00") + " min "+ _displaySeconds.ToString("00") + " sec");
			GUILayout.EndArea ();
			
			GUILayout.BeginArea (Rect (areaWidth*0.8, areaHeight*0.85, areaWidth*0.15, _labelHeight));
		    if (GUILayout.Button("Show scores")){
			   
			 _showGraphView = true;
		     SendMessage("GetHistoricalAverageValues");
		     if (!_savedAvg && _goalReached){
		       		GetComponent(SummaryGraph).UpdateAverageValues();
		       		_savedAvg = true;
		     }
		     SendMessage("PrintSummaryGraph"); 
		     SendMessage("ShowSummaryGraph",true);  
		    }
		    GUILayout.EndArea();  
	
			_scoresLoaded = false;
			if (!isPlaying){
	
				// Area rendering the map and it's labels 
				GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.1, areaWidth*0.1, _labelHeight));
				GUILayout.Label("Name: ");	
				GUILayout.EndArea();
					
				
				GUILayout.BeginArea (Rect (areaWidth*0.16, areaHeight*0.105, areaWidth*0.20, _labelHeight+areaHeight*0.01));
				_playerName = GUILayout.TextField(_playerName,12);
				GUILayout.EndArea();
			
			    GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.15, areaWidth*0.4, _labelHeight));
				GUILayout.Label("Interpreter: ");	
				GUILayout.EndArea();
				    
				GUILayout.BeginArea (Rect (areaWidth*0.3, areaHeight*0.205, areaWidth*0.1, _labelHeight));
				GUILayout.Label("Year");	
				GUILayout.EndArea();
				
				//Area to display the year selection
				GUILayout.BeginArea (Rect (areaWidth*0.375, areaHeight*0.2, areaWidth*0.05, areaHeight*0.05));
				//GUILayout.BeginHorizontal();
				if (GUILayout.Button(leftBtnTexture, "ImageButton")){
		        	_currentYear --;
		        if (_currentYear < 0)
		        	_currentYear = 0;	
		     	}
		     	GUILayout.EndArea();
		     	
		     	GUILayout.BeginArea (Rect (areaWidth*0.4, areaHeight*0.205, areaWidth*0.05, _labelHeight));
				GUILayout.Label(_yearLabels[_currentYear]);
				GUILayout.EndArea();
		     		
			    GUILayout.BeginArea (Rect (areaWidth*0.45, areaHeight*0.2, areaWidth*0.05, areaHeight*0.05));
				  if (GUILayout.Button(rightBtnTexture,"ImageButton")){
			        _currentYear ++;
			        if (_currentYear == yearList.Length)
			        	_currentYear --;	
			    }
		     	GUILayout.EndArea();
				
				if ( yearList[_currentYear] != yearList[_previousYear])
				{
					_previousYear = _currentYear;
					var  _map : Texture2D = Resources.Load("Images/"+yearList[_currentYear]+"_Location_Map", typeof(Texture2D));
	       			GetComponent(CurrentPosInMap).mapImage = _map;
				}
				GUILayout.BeginArea (Rect (areaWidth*0.85, areaHeight*0.05, areaWidth*0.10, areaHeight*0.15));
	     		if (GUILayout.Button (startBtnTexture, "ImageButton"))
				{		
					if (interpreterName.CompareTo("...")){
					    StartGame();
					    isPlaying = true;
				        networkView.RPC ("LoadLevelInClient", RPCMode.Others, yearList[_currentYear].ToString()+":"+_yearAgoList[_currentYear].ToString());  
						GetComponent(BurnedCaloriesGraph).DrawCaloriesGraph();
					}
					else
					{
						Debug.Log("select an interpreter");
					}
				}
				
		     	GUILayout.EndArea ();
		     	
		     	GUILayout.BeginArea (Rect (areaWidth*0.21, areaHeight*0.15, areaWidth*0.15, areaHeight*0.05));
			    if (GUILayout.Button(interpreterName)){
					
    				_showInterpreterList = !_showInterpreterList;
    			
   			    }
   			    GUILayout.EndArea();
   			    
   			    if (_showInterpreterList){

				 	for (var cnt:int  = 1; cnt < GetComponent(Interpreters).interpreterNames.Count; cnt++){
				 
				 	   GUILayout.BeginArea (Rect (areaWidth*0.21, areaHeight*0.15+_labelHeight*(cnt), areaWidth*0.15, _labelHeight));
				 	   if (GUILayout.Button(GetComponent(Interpreters).interpreterNames[cnt])){
				 	   		interpreterName = GetComponent(Interpreters).interpreterNames[cnt];
				 	   		_showInterpreterList = !_showInterpreterList;
				 	   		GetComponent(Interpreters).updateInterpreter(cnt);
				 	   } 
				 	   GUILayout.EndArea();
				    }
			    } 
			    
		     	/*
			    GUILayout.BeginArea (Rect (areaWidth*0.75, areaHeight*0.65, areaWidth*0.1, _labelHeight));
				if (GUILayout.Button("Distance")){
					GetComponent(BurnedCaloriesGraph).typeGraph = 1 ;
				}
				GUILayout.EndArea();
				GUILayout.BeginArea (Rect (areaWidth*0.85, areaHeight*0.65, areaWidth*0.1, _labelHeight));
			    if (GUILayout.Button("Time")){
					GetComponent(BurnedCaloriesGraph).typeGraph = 0;
				}
				GUILayout.EndArea();
			*/
			}
			else{
		
				GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.1, areaWidth*0.1, _labelHeight));
				GUILayout.Label("Player: " +_playerName);
				GUILayout.EndArea();
			
				GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.15, areaWidth*0.2, _labelHeight));
				GUILayout.Label("Hello, my name is ");
				GUILayout.EndArea();
				
				GUILayout.BeginArea (Rect (areaWidth*0.25, areaHeight*0.15, areaWidth*0.15, areaHeight*0.05));
			    if (GUILayout.Button(interpreterName)){
					
    				_showInterpreterList = !_showInterpreterList;
    			
   			    }
   			    GUILayout.EndArea();
				 if (_showInterpreterList){

				 	for (cnt  = 1; cnt < GetComponent(Interpreters).interpreterNames.Count; cnt++){
				 
				 	   GUILayout.BeginArea (Rect (areaWidth*0.31, areaHeight*0.15+_labelHeight*(cnt), areaWidth*0.15, _labelHeight));
				 	   if (GUILayout.Button(GetComponent(Interpreters).interpreterNames[cnt])){
				 	   		interpreterName = GetComponent(Interpreters).interpreterNames[cnt];
				 	   		_showInterpreterList = !_showInterpreterList;
				 	   		GetComponent(Interpreters).updateInterpreter(cnt);
				 	   } 
				 	   GUILayout.EndArea();
				    }
			    } 
				
		     	GUILayout.BeginArea (Rect (areaWidth*0.4, areaHeight*0.205, areaWidth*0.05, _labelHeight));				
				GUILayout.Label(yearList[_currentYear].ToString());
				GUILayout.EndArea();
			
			}
			
				    
		  /*  GUILayout.BeginArea(Rect (areaWidth*0.1, areaHeight*0.95, areaWidth*0.3, _labelHeight));
		    _showInfo = GUILayout.Toggle(_showInfo, "Show polar bear info");
		    GetComponent(InfoPolarBear).ShowInfo(_showInfo);
		    GUILayout.EndArea();
		    */
		    // Game will stop by user request	
		    if (isPlaying){
		       GUILayout.BeginArea (Rect (areaWidth*0.6, areaHeight*0.85, areaWidth*0.15, _labelHeight));
		       if (_goalReached || _timerReached ){
		       		Debug.Log("Reached the max time");
			       if (GUILayout.Button("Play Again")){
					 isPlaying = false;  
					 GetComponent(InfoPolarBear).ShowInfo(false);
				     Initialize();    
				   }
				   
		       }
		       else if (GUILayout.Button("Cancel Game")){
		         GetComponent(InfoPolarBear).ShowInfo(false);
				 isPlaying = false;  
				 GetComponent(AppendToLog).AppendDataToLog();
			     networkView.RPC ("StopGame", RPCMode.Others,burnedCalories); 
			     Initialize();    
			   }
			   GUILayout.EndArea();  
		
			}
	}
	else{  // Draw the Summary Graph
	
		GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.2, areaWidth*0.45, _labelHeight));
		GUILayout.Label("Average Kcal Burned"); 
		GUILayout.EndArea();
		
		GUILayout.BeginArea (Rect (areaWidth*0.1, areaHeight*0.25, areaWidth*0.5, areaHeight*0.50));
		GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(areaHeight*0.5), GUILayout.Width(areaWidth*0.5));
		GUILayout.Space(areaHeight*0.5);
		GUILayout.EndVertical();
		GUILayout.EndArea();	
		
		//Return to main view button
		 GUILayout.BeginArea(Rect (areaWidth*0.8, areaHeight*0.85, areaWidth*0.15, _labelHeight));
	
		 if (GUILayout.Button("Back")){
		     SendMessage("ShowSummaryGraph",false);
		     _showGraphView = false;
		     GetComponent(BurnedCaloriesGraph).PrintBurnedCaloriesGraph();
		     GetComponent(CurrentPosInMap).PrintMap();
		     if (!isPlaying)
		     {
		        Initialize();  
		     }
	     }
	     GUILayout.EndArea ();
	     
	     LoadScores();
	
		    GUILayout.BeginArea (Rect (areaWidth*0.65, areaHeight*0.2, areaWidth*0.3, _labelHeight));
		  	GUILayout.Label("Top 10 - Year " + yearList[_currentYear]);  
		    GUILayout.EndArea();
		    
		    GUILayout.BeginArea (Rect (areaWidth*0.65, areaHeight*0.25, areaWidth*0.3, areaHeight*0.50));
		    GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(areaHeight*0.50), GUILayout.Width(areaWidth*0.3));
	
		    
		    for (var i:int = 0; i < 10 ; i++){
		       if (i<_scoresCount){
		        var name : String = _scoreNames[i].ToString().Trim();
		        var min : int = Mathf.CeilToInt(float.Parse(_scoreValues[i]))*0.01666667;
		    	var sec : int = Mathf.CeilToInt(float.Parse(_scoreValues[i]))%60;
		    	GUILayout.Label( (i + 1).ToString().PadLeft(2) +"." + name.PadRight(13) + 
		    	min.ToString("00") + ":"+
		    	sec.ToString("00") + " min","ScoreStyle"); 
		    	}
		    	else
		    		GUILayout.Label("   " );
		    }
		    
		    GUILayout.EndVertical();
			GUILayout.EndArea();
		
  	}	
  	  	
  	//End Area for the entire GUI
  
	///  Game will stop when reach timer    
    if ((isPlaying && _displayMinutes == gameTime) || _goalReached ){
       Debug.Log("reachgoal");
       networkView.RPC ("FinishGame", RPCMode.Others,burnedCalories);  
       _timerReached = true; 
       if (!_savedLog){
       	Debug.Log("Saved Log");
 	    GetComponent(AppendToLog).AppendDataToLog();
 	    GetComponent(SummaryGraph).UpdateDataByYear();
 	     // if (_goalReached)
 	      	//	SaveScores();
 	      _savedLog = true;
       }
	}
	
	GUILayout.EndArea();

}

function OnPlayerConnected(newPlayer: NetworkPlayer){
	
    //Called on the server only
     _playerNumber = int.Parse (newPlayer.ToString());
               
}

function LoadScores(){
 if (!_scoresLoaded){
 	_scoresLoaded = true;
 	_scoreValues.Clear();
 	_scoreNames.Clear();
 	_scoresCount = 0;

	GetComponent(DatabaseConnection).GetScores(
	
 	if (File.Exists(applicationPath+"/Scores_"+yearList[_currentYear]+".txt")){
		var file =  File.ReadAllLines(applicationPath+"/Scores_"+yearList[_currentYear]+".txt");
		if (file!=null){
		   	_scoresCount = file.Length;
	   		//Debug.Log("File --> " + randomCount);
	   		for (var j: int = 0 ; j < _scoresCount; j++){
	   		  var values = file[j].Split(":"[0]);
	   		  _scoreValues.Add(values[0]);
	   		  _scoreNames.Add(values[1]);
	   		}
	   	}	
   	}   
  }
}

/*
function LoadScores(){
 if (!_scoresLoaded){
 	_scoresLoaded = true;
 	_scoreValues.Clear();
 	_scoreNames.Clear();
 	_scoresCount = 0;

 	if (File.Exists(applicationPath+"/Scores_"+yearList[_currentYear]+".txt")){
		var file =  File.ReadAllLines(applicationPath+"/Scores_"+yearList[_currentYear]+".txt");
		if (file!=null){
		   	_scoresCount = file.Length;
	   		//Debug.Log("File --> " + randomCount);
	   		for (var j: int = 0 ; j < _scoresCount; j++){
	   		  var values = file[j].Split(":"[0]);
	   		  _scoreValues.Add(values[0]);
	   		  _scoreNames.Add(values[1]);
	   		}
	   	}	
   	}   
  }
}

function SaveScores(){
  Debug.Log(" Save Scores");
  LoadScores();
  var sw =  new StreamWriter(applicationPath+"/Scores_"+yearList[_currentYear]+".txt");
  OrderScores();
  for (var i:int = 0 ; i < _scoresCount && i < 10; i++){
      sw.WriteLine(_scoreValues[i]+":"+_scoreNames[i]);
  }
  sw.Close();
}

function OrderScores(){
      var enter: boolean = false;
      Debug.Log("Entro elapsedTime" + elapsedTime);
	  for (var i:int = 0 ; i < _scoresCount; i++){
	      if (elapsedTime < float.Parse(_scoreValues[i])) {
	        Debug.Log("Entro save scores " + i + " - " + _scoresCount);
			   _scoreValues.Insert(i,elapsedTime);  
			   _scoreNames.Insert(i,_playerName);
			   enter = true;
			   _scoresCount++;
			   break;
		  } 
	  }
	  if (!enter){
	     Debug.Log("No entro save scores " + i + " - " + _scoresCount);
	 
	     _scoreValues.Add(elapsedTime);  
	     _scoreNames.Add(_playerName);
	     _scoresCount++;
	  }
}
*/
public function playerName(){
    return _playerName;
}

public function reachedGoal(){
      return _goalReached;
}

public function timeElapsed(){
      return _displayMinutes.ToString("00") + ":"+ _displaySeconds.ToString("00");
}

public function numberSteps(){
      return _numSwimSteps.ToString() +"|"+ _numWalkSteps.ToString();
}
public function currentYear(){

      return yearList[_currentYear];
}
@RPC
function ReceivedMovementInput (_steps: String ,  info : NetworkMessageInfo)
{
    //Called on the server only
    if (isPlaying){
	    //Called on the server
	    var values = _steps.Split(":"[0]);
	    if (values.length == 4 ){
		    
		    if (parseInt(values[0]) != _numWalkSteps){
		        _numWalkSteps = parseInt(values[0]);
		    }
		    if (parseInt(values[1]) != _numSwimSteps){
		        _numSwimSteps = parseInt(values[1]);
		    }
			GetComponent(CurrentPosInMap).UpdateCurrentPosition(_steps);
			_currentPos = new Vector2(0,parseFloat(values[3]));
	   }
   }
}
@RPC
function SendEventId (_id: int)
{
	 //Called on the server only
	Debug.Log("imagen actual: " + _id);	 
   GetComponent(InfoPolarBear).SetCurrentInfo(_id);
}

@RPC
function ReceivedFinishedLevel (_steps: String ,  info : NetworkMessageInfo)
{
	 //Called on the server only
    _goalReached = true;
    //Commented out for iPad demo
    
    if (!_savedLog){
	    GetComponent(AppendToLog).AppendDataToLog(); 	
	    GetComponent(SummaryGraph).UpdateDataByYear();
	    SaveScores();
	    _savedLog = true;
    }
}

@RPC
function SetGoalBearInMap (_xyValues: String, info : NetworkMessageInfo)
{
	 //Called on the server only
     GetComponent(CurrentPosInMap).SetGoalBearInMap(_xyValues);
     
     var values = _xyValues.Split(":"[0]);
     var _x = parseFloat(values[2]);
     var _z = parseFloat(values[3]);
     _initialPos = new Vector2(0,_z);  
     _currentPos = new Vector2(0,_z);   
}

@RPC
function ShowInfoPolarBear (_value: int, info : NetworkMessageInfo)
{
	 //Called on the server only
     GetComponent(InfoPolarBear).SetCurrentInfo(_value);
     
}
@RPC
function SaveLogDocent (_level: String)
{
	 //Called on the Clients
}
@RPC
function LoadLevelInClient (_level: String)
{
	 //Called on the Clients
}
@RPC
function FinishGame (_calories: float ,  info : NetworkMessageInfo)
{
	 //Called on the Clients
}
@RPC
function StopGame (_calories: float ,  info : NetworkMessageInfo)
{
	 //Called on the Clients
}
