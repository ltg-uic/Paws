import System.IO;
import System;

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
  var logline = DateTime.Now+"|"+GetComponent(NetworkConnectionServer).playerName()+"|"+GetComponent(NetworkConnectionServer).currentYear()+
  				"|"+GetComponent(NetworkConnectionServer).reachedGoal()+"|"+GetComponent(NetworkConnectionServer).timeElapsed()+
  				"|"+GetComponent(NetworkConnectionServer).numberSteps()+"|"+GetComponent(NetworkConnectionServer).meters+"|"+
  				GetComponent(NetworkConnectionServer).burnedCalories+"|"+GetComponent(BurnedCaloriesGraph).typeGraph;
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
  
  var logline = DateTime.Now+"|"+GetComponent(NetworkConnectionServer).playerName()+"|"+GetComponent(NetworkConnectionServer).currentYear()+
  				"|"+GetComponent(NetworkConnectionServer).reachedGoal()+"|"+GetComponent(NetworkConnectionServer).timeElapsed()+
  				"|"+GetComponent(NetworkConnectionServer).numberSteps()+"|"+GetComponent(NetworkConnectionServer).meters+"|"+
  				GetComponent(NetworkConnectionServer).burnedCalories+"|"+GetComponent(BurnedCaloriesGraph).typeGraph;
  return logline;
}
*/

//Save log to database
public function AppendDataToLog(){
  
  var logData = new String[13];
  
  logData[0] = DateTime.Now.ToString();
  logData[1] = GetComponent(NetworkConnectionServer).playerName();
  logData[2] = GetComponent(NetworkConnectionServer).currentYear().ToString();
  logData[3] = GetComponent(NetworkConnectionServer).reachedGoal().ToString();
  logData[4] = GetComponent(NetworkConnectionServer).timeElapsed();
  logData[5] = GetComponent(NetworkConnectionServer).walkSteps(); 
  logData[6] = GetComponent(NetworkConnectionServer).swimSteps(); 
  logData[7] = GetComponent(NetworkConnectionServer).walkCalories(); 
  logData[8] = GetComponent(NetworkConnectionServer).swimCalories(); 
  logData[9] = GetComponent(NetworkConnectionServer).meters.ToString();
  logData[10] = GetComponent(NetworkConnectionServer).interpreterID;
  logData[11] = GetComponent(BurnedCaloriesGraph).typeGraph.ToString(); //0 time 1 distance
  logData[12] = (GetComponent(NetworkConnectionServer).serverTest%3).ToString();
 
  Debug.Log("AppendToLog::Send data to log");
  StartCoroutine(GetComponent(DatabaseConnection).SaveSession(logData));
  
      
}