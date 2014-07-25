import System.IO;

var linesMaterial : Material;
var avgLineColor = Color.yellow;
var lineWidth = 2.5;

var  pointImage: Texture2D =  null;
var  pointsImage: Texture2D =  null;
var  mySkin: GUISkin;

var posX : int;
var posY : int;    
var width : int;
var height : int;
var numYears: int;

private var _showSummaryGraph: boolean = false;
private var _axesColor = Color.white;
private var _axesWidth = 4.0;
private var _currentYear : int;
private var _gameDuration: int;
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
private var _graphYears:Array;
private var _graphAVG: Array;
public  var avgCalories: String;
public  var historicalValues1: String;
public  var historicalValues2: String;
public  var historicalValues3: String;
private var _graphHistory1:Array;
private var _graphHistory2:Array;
private var _graphHistory3:Array;
private var _maxCaloriesValue: float = 700;

function Start(){
	_graphYears = new Array();
	_graphAVG =  new Array();
	_graphHistory1 = new Array();
	_graphHistory2 = new Array();
	_graphHistory3 = new Array();
}

function OnGUI(){
   GUI.skin = mySkin;
   _calories = GetComponent(NetworkConnectionIT).burnedCalories;

   if (_showSummaryGraph){ 
        var spanBtwXPoints =  width / numYears;
  
       GUI.Label(Rect(posX + width - 25, Screen.height - posY , 120,40),"Years");      
       GUI.Label(Rect(posX +25, Screen.height - posY + 40 , width,40),"Average of the bear's calories burned in the last 10 games");      
     
	    for (var i: int =0;i< numYears;i++){   
	        GUI.Label(Rect(spanBtwXPoints/2  + (i* spanBtwXPoints) + posX - 12, Screen.height - posY ,width,height), _graphYears[i].ToString());      
   	    }  	
   	  
        for (var k: int = 1; k<= ((_maxCaloriesValue+100)/50);k++){   
		       GUI.Label(Rect(posX - 45, Screen.height - ((percentageYPoint * k* 50) + posY) ,40,30), (k*50).ToString());
		//      Debug.Log( "label " +    (_xAxisValue[j]-(_xAxisValue[j]%xInterval)));
	     } 
		  
		for (var j: int = 0; j < _graphHistory1.length;j++){
	   	    // Debug.Log(_randomCount + "  " +_randomValues[j]);
	   	 
	   	       GUI.DrawTexture(new Rect(spanBtwXPoints/2 + posX - 12, Screen.height - posY  - 6 - (percentageYPoint * float.Parse(_graphHistory1[j])), 12, 12), pointsImage);      
	
	   	}
	   	for (j = 0; j < _graphHistory2.length;j++){
	   	    // Debug.Log(_randomCount + "  " +_randomValues[j]);
	   	 
	   	       GUI.DrawTexture(new Rect(spanBtwXPoints/2  + (1* spanBtwXPoints) + posX - 12, Screen.height - posY  - 6 - (percentageYPoint * float.Parse(_graphHistory2[j])), 12, 12), pointsImage);      
	
	   	}		
	   	for (j = 0; j < _graphHistory3.length;j++){
	   	    // Debug.Log(_randomCount + "  " +_randomValues[j]);
	   	 
	   	       GUI.DrawTexture(new Rect(spanBtwXPoints/2  + (2* spanBtwXPoints) + posX - 12, Screen.height - posY  - 6 - (percentageYPoint * float.Parse(_graphHistory3[j])), 12, 12), pointsImage);      
	
	   	}   
   	    if (_calories > 0){
   	     // Debug.Log("Calories to print " + (percentageYPoint));
   	      
	      GUI.DrawTexture(new Rect(_xCurrentPoint - 18, Screen.height - posY  - 18 - (percentageYPoint * _calories), 36, 36), pointImage);      

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
   
   if (!_showSummaryGraph){
   		SetLinesPointsLabels();
   		HideHistoricalGraph();
   		}
    else	
    	PrintHistoricalGraph();
}
   
   
function HideHistoricalGraph() {
	if (_avgLine!=null && _axesLine!= null){
	 Vector.Active(_avgLine,false);
	 Vector.Active(_axesLine,false);
	}
}

function PrintHistoricalGraph() {
   if (_avgLine!=null && _axesLine!= null){
	 Vector.Active(_avgLine,true);
	 Vector.Active(_axesLine,true);
   }	
}

public function SetParameters(currentYearIndex: int, gameDuration:int){
	_currentYear = GetComponent(NetworkConnectionIT).yearList[currentYearIndex];
	_indexYear = currentYearIndex;
	_gameDuration = gameDuration;
	_maxCaloriesValue = 0;
	}
	
public function SetAVGCalories(){
    
    _graphAVG = new Array();
    _graphYears = new Array();

    if (avgCalories.Length > 0){
      
	    var avgCaloriesEntries = avgCalories.Split('|'[0]);
	     
		for (var entry in avgCaloriesEntries)
		{
		   if (entry.Length > 1){
		       var avgValue : String[]= entry.Split(':'[0]);
		        
		       if (avgValue.Length > 0){		      
				   _graphYears.Add(avgValue[0]);
				   _graphAVG.Add(avgValue[1]); 
				   _maxCaloriesValue = Mathf.Max(float.Parse(avgValue[1].ToString()),_maxCaloriesValue);
			   }
		   }
		}
	}
	
}
public function SetHistoricalValues(_year : String){
    
    if (_year == "1975"){
        _graphHistory1 = new Array();
	    if (historicalValues1.Length > 0){
		    var caloriesEntries = historicalValues1.Split('|'[0]);
		 
			for (var entry in caloriesEntries)
			{
			   if (entry.Length > 1){         
				      // Debug.Log("Values::Received data:"+avgValue[ 0 ]+"  "+avgValue[ 1 ]);
					   _graphHistory1.Push(entry);
					   _maxCaloriesValue = Mathf.Max(float.Parse(entry),_maxCaloriesValue);
			   }
			}
		}
	}
	else if (_year == "2010"){
	  _graphHistory2 = new Array();
	    if (historicalValues2.Length > 0){
		    var caloriesEntries2 = historicalValues2.Split('|'[0]);
		 
			for (var entry in caloriesEntries2)
			{
			   if (entry.Length > 1){         
				      // Debug.Log("Values::Received data:"+avgValue[ 0 ]+"  "+avgValue[ 1 ]);
					   _graphHistory2.Push(entry);
					   _maxCaloriesValue = Mathf.Max(float.Parse(entry),_maxCaloriesValue);
			   }
			}
		}
	}
	else if (_year == "2045"){
	    _graphHistory3 = new Array();
	    if (historicalValues3.Length > 0){
		    var caloriesEntries3 = historicalValues3.Split('|'[0]);
		 
			for (var entry in caloriesEntries3)
			{
			   if (entry.Length > 1){         
				      // Debug.Log("Values::Received data:"+avgValue[ 0 ]+"  "+avgValue[ 1 ]);
					   _graphHistory3.Push(entry);
					   _maxCaloriesValue = Mathf.Max(float.Parse(entry),_maxCaloriesValue);
			   }
			}
		}
	}
}

public function GetHistoricalAverageValues(){
Debug.Log("get historial");
    GetComponent(DatabaseConnection).GetHistoricalValues(_currentYear,_gameDuration);
}


	
public function UpdateAverageValues(){

    GetComponent(DatabaseConnection).GetAVGCalories(_gameDuration);
      
}


function PrintSummaryGraph(){
        
		var spanBtwXPoints =  width / numYears;
	    
	    SetLinesPointsLabels();
	    // max y-point
	    _axesPoints[0] = Vector2( posX, posY + height + 2);
	  
	    // origin 
	    _axesPoints[1] = Vector2( posX, posY);
	   
	     
	    // max x-point
	     _axesPoints[2] = Vector2( (posX + width + 2)  , posY);
	   	Debug.Log("Print summary graph " + _maxCaloriesValue + " " + height);

 	   	if (_maxCaloriesValue > 0 )  { 
	     	Debug.Log("Print summary graph " + _maxCaloriesValue + " " + height);
		    percentageYPoint = height /(_maxCaloriesValue+100);
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

 