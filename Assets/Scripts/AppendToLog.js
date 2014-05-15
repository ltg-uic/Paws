
/*  
function Start (){

	applicationPath = Application.dataPath;
    if (Application.platform == RuntimePlatform.OSXPlayer) {
        applicationPath += "/../../";
    }
    else if (Application.platform == RuntimePlatform.WindowsPlayer) {
        applicationPath += "/../";
    }		
		
}

public function AppendDataToLog(){
  //Debug.Log("Create log file");
  var sw =  File.AppendText(applicationPath+"/A_Mile_In_My_Paws_Log.txt");
  var logline = DateTime.Now+"|"+GetComponent(NetworkConnectionIT).playerName()+"|"+GetComponent(NetworkConnectionIT).currentYear()+
  				"|"+GetComponent(NetworkConnectionIT).reachedGoal()+"|"+GetComponent(NetworkConnectionIT).timeElapsed()+
  				"|"+GetComponent(NetworkConnectionIT).numberSteps()+"|"+GetComponent(NetworkConnectionIT).meters+"|"+
  				GetComponent(NetworkConnectionIT).burnedCalories+"|"+GetComponent(BurnedCaloriesGraph).typeGraph;
  sw.WriteLine(logline);
  sw.Close(); 
      
}
public function ClientAppendDataToLog(_logline: String){
  if (!savedLog){
      savedLog = true;
 	  Debug.Log("Create log file");
  	  var sw =  File.AppendText(applicationPath+"/A_Mile_In_My_Paws_Log.txt");
 	  sw.WriteLine(DateTime.Now+"|"+_logline);
 	  sw.Close(); 
  }
      
}
public function GetLogLine(){
  
  var logline = DateTime.Now+"|"+GetComponent(NetworkConnectionIT).playerName()+"|"+GetComponent(NetworkConnectionIT).currentYear()+
  				"|"+GetComponent(NetworkConnectionIT).reachedGoal()+"|"+GetComponent(NetworkConnectionIT).timeElapsed()+
  				"|"+GetComponent(NetworkConnectionIT).numberSteps()+"|"+GetComponent(NetworkConnectionIT).meters+"|"+
  				GetComponent(NetworkConnectionIT).burnedCalories+"|"+GetComponent(BurnedCaloriesGraph).typeGraph;
  return logline;
}
*/

//Save log to database
public function AppendDataToLog(){
  
  var logData = new String[14];
  
  logData[0] = DateTime.Now.ToString();
  logData[1] = GetComponent(NetworkConnectionIT).playerName();
  logData[2] = GetComponent(NetworkConnectionIT).currentYear().ToString();
  logData[3] = GetComponent(NetworkConnectionIT).reachedGoal().ToString();
  logData[4] = GetComponent(NetworkConnectionIT).timeElapsed();
  logData[5] = GetComponent(NetworkConnectionIT).walkSteps(); 
  logData[6] = GetComponent(NetworkConnectionIT).swimSteps(); 
  logData[7] = GetComponent(NetworkConnectionIT).walkCalories(); 
  logData[8] = GetComponent(NetworkConnectionIT).swimCalories(); 
  logData[9] = GetComponent(NetworkConnectionIT).meters.ToString();
  logData[10] = GetComponent(NetworkConnectionIT).interpreterID;
  logData[11] = GetComponent(BurnedCaloriesGraph).typeGraph.ToString(); //0 time 1 distance
  logData[12] = GetComponent(NetworkConnectionIT).currentGameDuration();
  logData[13] = (GetComponent(NetworkConnectionIT).serverTest%3).ToString();
  
 
  Debug.Log("AppendToLog::Send data to log"+logData[12]+" "+logData[13]);
  StartCoroutine(GetComponent(DatabaseConnection).SaveSession(logData));
  
      
}
