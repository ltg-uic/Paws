import System.IO;
import System;

private var applicationPath:String = "";
  var startDateTime;
  var savedLog = false;
  
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
public function StartLog(){
	savedLog = false;
	startDateTime = DateTime.Now;
}
public function GetLogLine(){
  
  var logline = DateTime.Now+"|"+GetComponent(NetworkConnectionServer).playerName()+"|"+GetComponent(NetworkConnectionServer).currentYear()+
  				"|"+GetComponent(NetworkConnectionServer).reachedGoal()+"|"+GetComponent(NetworkConnectionServer).timeElapsed()+
  				"|"+GetComponent(NetworkConnectionServer).numberSteps()+"|"+GetComponent(NetworkConnectionServer).meters+"|"+
  				GetComponent(NetworkConnectionServer).burnedCalories+"|"+GetComponent(BurnedCaloriesGraph).typeGraph;
  return logline;
}