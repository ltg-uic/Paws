import System.IO;

var linesMaterial : Material;
var avgLineColor = Color.white;
var lineWidth = 2.5;

var  pointImage: Texture2D =  null;
var  pointsImage: Texture2D =  null;
var  mySkin: GUISkin;

var posX : int;
var posY : int;    
var width : int;
var height : int;
var numYears: int;

private var _summaryFile:String;
private var _showSummaryGraph: boolean = false;
private var _axesColor = Color.white;
private var _axesWidth = 4.0;
private var _currentYear : int;
private var _indexYear: int;
private var _calories: float = 0;
private var _xCurrentPoint = 0;
private var _axesLine: VectorLine;
private var _avgLine: VectorLine;
private var percentageYPoint:float = 0;
private var _capLength = 10.0;

private var _linePoints : Vector2[];
private var _axesPoints : Vector2[];

private var myLine: VectorLine;  // to print the years in the x-axis.

//graph values
private var _graphYears: Array;
private var _graphAVG: Array;
private var _graphNoPlayers: Array;

private var _maxCaloriesValue: float = 700;
private var _randomValues:Array;
private var _randomCount = 0;
	
private var applicationPath:String = "";

function Start(){
 
	_graphYears = new Array();
	_graphAVG = new Array();
	_graphNoPlayers = new Array();
	_randomValues = new Array();

}

function OnGUI(){
   GUI.skin = mySkin;
   _calories = GetComponent(NetworkConnectionServer).burnedCalories;

   if (_showSummaryGraph){ 
        var spanBtwXPoints =  width / numYears;
  
       GUI.Label(Rect(posX + width , Screen.height - posY , 100,40),"Years");      

   	   // Points for the averages       
	    for (var i: int =0;i< numYears;i++){   
	        GUI.Label(Rect(spanBtwXPoints/2  + (i* spanBtwXPoints) + posX - 12, Screen.height - posY ,width,height), _graphYears[i].ToString());      
   	    }  	
   	  
        for (var k: int = 1; k<= (_maxCaloriesValue/50);k++){   
		GUI.Label(Rect(posX - 35, Screen.height - ((percentageYPoint * k* 50) + posY) ,30,30), (k*50).ToString());
		//      Debug.Log( "label " +    (_xAxisValue[j]-(_xAxisValue[j]%xInterval)));
	     } 
		     
   	    if (GetComponent(NetworkConnectionServer).isPlaying){
   	     // Debug.Log("Calories to print " + (percentageYPoint));
   	      
	      GUI.DrawTexture(new Rect(_xCurrentPoint - 18, Screen.height - posY  - 18 - (percentageYPoint * _calories), 36, 36), pointImage);      
	
	   	   for (var j: int = 0; j < _randomCount;j++){
	   	    // Debug.Log(_randomCount + "  " +_randomValues[j]);
	   	   
	   	       GUI.DrawTexture(new Rect(_xCurrentPoint, Screen.height - posY  - 6 - (percentageYPoint * float.Parse(_randomValues[j])), 12, 12), pointsImage);      
	
	   	   }
   	   }
    }
}
   
function SetLinesPointsLabels () {
	DestroyLines();
	
	_linePoints = new Vector2[numYears];
	_axesPoints = new Vector2[3];
  
	_axesLine = new VectorLine("AxesLine", _axesPoints, _axesColor, linesMaterial, _axesWidth,LineType.Continuous,Joins.Weld);
	_avgLine = new VectorLine("AvgLine", _linePoints, avgLineColor, linesMaterial, lineWidth,LineType.Continuous,Joins.Weld);
							
	_avgLine.maxDrawIndex = numYears;
	_axesLine.capLength = _capLength;
}

public function DestroyLines(){
    
	Vector.DestroyLine(_avgLine);
	Vector.DestroyLine(_axesLine);	
}
	
public function ShowSummaryGraph(_value: boolean) {
   _showSummaryGraph = _value;
   
   if (!_showSummaryGraph)
   		SetLinesPointsLabels();
   		
}
   
public function SetCurrentYear(_value: int){
	_currentYear = GetComponent(NetworkConnectionServer).yearList[_value];
	_indexYear = _value;
	}
	
function GetHistoricalAverageValues(){
    var values: String[];
    _graphYears.Clear();
    _graphAVG.Clear();
    _graphNoPlayers.Clear();
    
   	for (var i: int = 0;i< numYears;i++){   
   
	    if (PlayerPrefs.HasKey(GetComponent(NetworkConnectionServer).yearList[i].ToString())){  	 
  		 	values = (PlayerPrefs.GetString(GetComponent(NetworkConnectionServer).yearList[i].ToString())).Split(":"[0]);
  	                  
           _graphYears.Add(GetComponent(NetworkConnectionServer).yearList[i].ToString()); // year;
           _graphAVG.Add(float.Parse(values[0].ToString())); //average;
           _graphNoPlayers.Add(int.Parse(values[1].ToString()));//  #Players;
       //    Debug.Log(i + " - " + _graphYears[i] + " " +_graphAVG[i] + " " + _graphNoPlayers[i]);
   		   _maxCaloriesValue = Mathf.Max(float.Parse(values[0].ToString()),_maxCaloriesValue);
     	}
  		else{
 		   
           _graphYears.Add(GetComponent(NetworkConnectionServer).yearList[i].ToString()); // year;
           _graphAVG.Add(0); //average;
           _graphNoPlayers.Add(0);//  #Players;
      //     Debug.Log(i + " - No hay - " + _graphYears[i] + " " +_graphAVG[i] + " " + _graphNoPlayers[i]);

  		}
   	}
   	/*
   	applicationPath = GetComponent(NetworkConnectionServer).applicationPath;
   	_randomValues.Clear();
   	_randomCount = 0; 
   	if (File.Exists(applicationPath+"/"+_currentYear+".txt")){
	   	var file =  File.ReadAllLines(applicationPath+"/"+_currentYear+".txt");
	   	Debug.Log("File --> " + _currentYear+".txt = " + file.Length);
	   	if (file!=null){
	   	    _randomCount = file.Length;
	   		
	   		for (var j: int = 0 ; j < _randomCount; j++){
	   		   _randomValues.Add(file[j]);
	   		}
	   	}
   	}   
   	*/
}


	
public function UpdateAverageValues(){
   var newNoPlayers: int  = 0;
   var newAvg: float = 0.0f;
   
   if (PlayerPrefs.HasKey(_currentYear.ToString())){
	    var values:String[] = (PlayerPrefs.GetString(_currentYear.ToString())).Split(":"[0]);
        newNoPlayers = Mathf.Round(float.Parse(values[1])) + 1;
        
        newAvg = ((Mathf.Round(float.Parse(values[0])) * Mathf.Round(float.Parse(values[1]))) + _calories) / newNoPlayers;
   
   }
   else
   {
	   newNoPlayers = 1;
	   newAvg = _calories;
   }
   
   PlayerPrefs.SetString(_currentYear.ToString(),(newAvg.ToString()+":"+newNoPlayers.ToString()));
   PlayerPrefs.Save();
      
}


public function UpdateDataByYear()
{
/*
  applicationPath = GetComponent(NetworkConnectionServer).applicationPath;
  var sw =  new StreamWriter(applicationPath+"/"+_currentYear+".txt");
  sw.WriteLine(_calories);
  Debug.Log("File --> " + _currentYear+".txt = " + _randomCount);
  for (var i:int = 0 ; i < _randomCount && i < 10; i++){
      sw.WriteLine(_randomValues[i]);
  }
  sw.Close();
  */
}

function PrintSummaryGraph(){
        
		var spanBtwXPoints =  width / numYears;
	    
	    SetLinesPointsLabels();
	    // max y-point
	    _axesPoints[0] = Vector2( posX, posY + height);
	  
	    // origin 
	    _axesPoints[1] = Vector2( posX, posY);
	   
	     
	    // max x-point
	     _axesPoints[2] = Vector2( (posX + width)  , posY);
	  // 	Debug.Log("Print summary graph " + _maxCaloriesValue + " " + height);
	   	if (_maxCaloriesValue > 0 )  { 
	  // 	Debug.Log("Print summary graph " + _maxCaloriesValue + " " + height);
		    percentageYPoint = height /_maxCaloriesValue;
		    Debug.Log("Print summary graph " + percentageYPoint);
		 }
	      
	   // Points for the averages       
	    for (var i: int =0;i<numYears;i++){
		    var point: Vector2 = Vector2(spanBtwXPoints/2 + (i* spanBtwXPoints) + posX, posY + (percentageYPoint * float.Parse(_graphAVG[i].ToString())));
	   	    _linePoints[i] = point;
	   	    Debug.Log("y point: "+ point); 
	   	    if (_graphYears[i].ToString().CompareTo(_currentYear.ToString()) == 0){
	   	        _xCurrentPoint = spanBtwXPoints/2  + (i* spanBtwXPoints) + posX;
	   	        Debug.Log("x point: "+ _xCurrentPoint);  
	   	    }

   	    }  	    

		
		if (numYears > 0)
			Vector.DrawLine(_avgLine);
			
		Vector.DrawLine(_axesLine);
	
}

 