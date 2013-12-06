var isCalledFinished = false;

private var secretKey: String="AMIMP"; // Edit this value and make sure it's the same as the one stored on the server
//var addScoreUrl="http://localhost/unity_test/addscore.php?"; //be sure to add a ? to your url
private var hs_get: WWW ;
var dbURL:String = "http://paws.evl.uic.edu/";

var saveSessionData:String="http://paws.evl.uic.edu/SaveSession.php";

// Get the scores from the MySQL DB
function GetPrompts() {
  /*  gameObject.guiText.text = "Loading Prompts";
    hs_get = WWW(highscoreUrl);
    yield hs_get;
 
    if(hs_get.error) {
    	print("There was an error getting the prompts: " + hs_get.error);
    } else {
        gameObject.guiText.text = hs_get.text; // this is a GUIText that will display the scores in game.
    }*/
}

// Mirlanda: Create php(s) that load Interpreters Id and name. (1) order alphabetically (2) order by the last used
function GetInterpreters() {
    
    hs_get = WWW(dbURL+"getInterpreters.php");
    yield hs_get;
 
    if(hs_get.error) {
    	print("DatabaseConnection::There was an error getting the interpreters: " + hs_get.error);
    } else {
     
	   GetComponent(Interpreters).interpreters = hs_get.text; 
	//   GetComponent(Interpreters).interpreters = "JohnS:John|CathyH:Cathy";
	   GetComponent(Interpreters).SetInterpreters();
    }
}

function SaveSession(_parameters: String[]){

    Debug.Log("DatabaseConnection::Save data to log..."+dbURL+"SaveSession.php"); 
    var session_url:String;
    var session_data: WWWForm = new WWWForm();
    
    session_data.AddField("DateTime",_parameters[0]);
    session_data.AddField("PlayerName",_parameters[1]);
    session_data.AddField("PlayedYear",_parameters[2]);
    session_data.AddField("ReachedGoal",_parameters[3]);
    session_data.AddField("TimeElapsed",_parameters[4]);
    session_data.AddField("WalkSteps",_parameters[5]);
    session_data.AddField("SwimSteps",_parameters[6]);
    session_data.AddField("WalkCalories",_parameters[7]);
    session_data.AddField("SwimCalories",_parameters[8]);
    session_data.AddField("Meters",_parameters[9]);
    session_data.AddField("InterpreterID",_parameters[10]);
    session_data.AddField("TypeGraph",_parameters[11]);    
    session_data.AddField("GameDuration",_parameters[12]);
    session_data.AddField("ServerTest",_parameters[13]);
    
    var web_request:WWW = new WWW(dbURL+"SaveSession.php",session_data);

    yield web_request;
    
}

// Mirlanda: Create php that load scores
//_parameters[0] = year , _parameters[1] = number_of_results_to_get
// If you can figure out the way to return an array is better, if not the format of the data 
//should be Name1:time|Name2:time|
function GetScores(_parameters: int[]){
  //  Debug.Log("DatabaseConnection::Get scores for parameters "+_parameters[0]+" " +_parameters[1]);
    var _url = "getTopScores.php" + "?year=" + _parameters[0] + "&top="+_parameters[1];
    var hs_get = WWW(dbURL+_url);
    yield hs_get; // Wait until the download is done
    if(hs_get.error) {
        print("DatabaseConnection::There was an error getting the scores: " + hs_get.error);
    }
    else{
	    if (hs_get.text.Length > 0){
	      //Debug.Log("DatabaseConnection::Get scores for parameters "+_parameters[0]+" " +_parameters[1] + "--"+ hs_get.text);
			GetComponent(NetworkConnectionServer).topScores = hs_get.text;
			GetComponent(NetworkConnectionServer).SetScores();
		}
	}
}

function AppendDataToUILog(_parameter1:String,_parameter2:int,_parameter3:String,_parameter4:String){

    Debug.Log("DatabaseConnection::Save data to UI log..."+dbURL+"SaveUILog.php "+_parameter1+ " " +_parameter2+ " "+_parameter3+ " "+_parameter4); 
    var session_url:String;
    var session_data: WWWForm = new WWWForm();
    
    session_data.AddField("ui_type",_parameter1);
    session_data.AddField("interaction",_parameter2); // 0 down, 1 up
    session_data.AddField("observation",_parameter3);
    session_data.AddField("date_time",_parameter4);
 	session_data.AddField("interpreter",GetComponent(NetworkConnectionServer).interpreterID);
 	
    var web_request:WWW = new WWW(dbURL+"SaveUILog.php",session_data);

    yield web_request;
    
}
