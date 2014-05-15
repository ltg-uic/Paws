var linesMaterial : Material;

private var _axesColor = Color.white;
private var _axesWidth = 2;

private var _lineColor = Color.yellow;
private var _lineWidth = 2;

private var _axesLine: VectorLine;
private var _lineLine: VectorLine;

private var _linePoints: Vector2[];
private var _axesPoints : Vector2[];
private var _spaceXLabel:float;

var posX: int;
var posY: int;
var width: int;
var height: int;
var maxBurnedCalories: float;
var maxXAxisValue:float;
var mySkin: GUISkin;
var typeGraph: int; // 0 time, 1 distance
var xInterval: int;
var noOfPoints: int;
var showGraph: boolean = false;

private var _calories: float;
private var _percentageYPoint: float;
private var _percentageXPoint: float;
private var _spanBtwXPoints: float;
private var _currentPos: int;
private var _xAxisPos: Array;
private var _xAxisValue: Array;
private var _graphUpdateTimer: int;
private var _lastIntervalValue:int;
private var _currentXValue:int;

function Start(){
	_xAxisPos = new Array();
	_xAxisValue = new Array();
	if (maxBurnedCalories <= 0)
		maxBurnedCalories = 100;
	_currentPos = 0;
	_spanBtwXPoints = 0;
	_percentageXPoint = _percentageYPoint = 0;
	_calories = 0;
	if (typeGraph != 0 || typeGraph!=1)
		typeGraph = 0;
 	if (xInterval <= 0)
		xInterval = 20;
	if (maxXAxisValue <=0)
		maxXAxisValue = 500;
}

function Update () {
  // Debug.Log("BurnedCaloriesGraph " + GetComponent(NetworkConnectionIT).isPlaying + " " +GetComponent(NetworkConnectionIT).elapsedTime);
   if (!GetComponent(NetworkConnectionIT).isPlaying){
     if (GetComponent(NetworkConnectionIT).elapsedTime == 0){
	   if (Input.GetKeyDown(KeyCode.T)){  // Print  calories vs time graph
		   typeGraph = 0;
	   }
	   if (Input.GetKeyDown(KeyCode.D)){  // Print  calories vs distance graph
		   typeGraph = 1;
	   }
	 }
  }

	_calories = GetComponent(NetworkConnectionIT).burnedCalories;
	if (typeGraph == 0)
	 _currentXValue = GetComponent(NetworkConnectionIT).elapsedTime;
	else if (typeGraph == 1)
	 _currentXValue = GetComponent(NetworkConnectionIT).meters;

	UpdateGraph();

}

function OnGUI(){
   if (showGraph){
	    GUI.skin = mySkin;
	   
	    switch (typeGraph) {
	       case 0:
		       	GUI.Label(Rect( posX + width - 25, Screen.height - posY  ,120,40),"Time (sec)");      
		       // Points to be drawn.
			    for (var i: int = 1; i< _xAxisPos.Count;i++){   
				      GUI.Label(Rect(_xAxisPos[i], Screen.height - posY,40,30), _xAxisValue[i].ToString());      
			     } 
			    break;
		   case 1:
			    GUI.Label(Rect( posX + width - 40, Screen.height - posY  , 120,40),"Distance (m)");      
			      // Points to be drawn.
			    for (var j: int = 1; j< _xAxisPos.Count;j++){   
				      GUI.Label(Rect(_xAxisPos[j], Screen.height - posY,40,30), (int.Parse(_xAxisValue[j])-(int.Parse(_xAxisValue[j])%xInterval)).ToString());
				//      Debug.Log( "label " +    (_xAxisValue[j]-(_xAxisValue[j]%xInterval)));
			     } 
		       break;
	    }
	    
       for (var k: int = 1; k<= (maxBurnedCalories/50);k++){   
			GUI.Label(Rect(posX - 50, Screen.height - ((_percentageYPoint * k* 50) + posY) ,50,30), (k*50).ToString());
			//      Debug.Log( "label " +    (_xAxisValue[j]-(_xAxisValue[j]%xInterval)));
		     } 
	   
    }
}

public function DestroyLines(){
	_xAxisPos = new Array();
	_xAxisValue = new Array();
	Vector.DestroyLine(_axesLine);
	Vector.DestroyLine(_lineLine);
	_currentPos = 0;
	_calories = 0;
	_currentXValue = 0;
    showGraph = false; 
	_spanBtwXPoints = 0;
	_percentageXPoint = _percentageYPoint = 0;
}

    
function SetLines () {
	DestroyLines();

	_linePoints = new Vector2[noOfPoints];
	_axesPoints = new Vector2[3];
	
	  
	_axesLine = new VectorLine("AxesLine", _axesPoints, _axesColor, linesMaterial, _axesWidth,LineType.Continuous,Joins.Weld);
	_lineLine = new VectorLine("CurrentLine", _linePoints, _lineColor, linesMaterial, _lineWidth,LineType.Continuous,Joins.Weld);

	_axesLine.capLength = _spaceXLabel;
	
}

function PrintSummaryGraph() {
   if (_lineLine!=null && _axesLine!= null){
	 Vector.Active(_lineLine,false);
	 Vector.Active(_axesLine,false);
    }
	 showGraph = false;
}

function HideBurnedCaloriesGraph() {
   if (_lineLine!=null && _axesLine!= null){
	 Vector.Active(_lineLine,false);
	 Vector.Active(_axesLine,false);
    }
	 showGraph = false;
}

public function PrintBurnedCaloriesGraph() {
   if (_lineLine!=null && _axesLine!= null){
	 Vector.Active(_lineLine,true);
	 Vector.Active(_axesLine,true);
   }
	 showGraph = true;		
}

public function DrawCaloriesGraph()
{
    _spanBtwXPoints =  width / noOfPoints;
  
 	SetLines();  
    // max y-point
    _axesPoints[0] = Vector2( posX , posY + height + 2);
    // origin 
    _axesPoints[1] = Vector2( posX , posY);
    // max x-point
     _axesPoints[2] = Vector2( posX + width + 2, posY);

   	
   	 _linePoints[0] = Vector2(posX,posY);
   	 _xAxisValue.Push(0);
	 _xAxisPos.Push(posX);
     _percentageYPoint = height/maxBurnedCalories;
     
     _percentageXPoint =  width/maxXAxisValue;
    
    _lineLine.maxDrawIndex = _currentPos;
	Vector.DrawLine(_axesLine);
	
    _currentPos++;
	showGraph = true;
}

function UpdateGraph() {
	    if (_currentPos > 0 && _currentPos < noOfPoints){
	         var tempX = (_percentageXPoint * _currentXValue) + posX;
	         var tempAxis: int = _xAxisValue[_currentPos-1];
	         temp = tempAxis;
	         if ((_currentXValue - tempAxis) >= xInterval){
	       		
	             _linePoints[_currentPos] =  Vector2( tempX, (_percentageYPoint * _calories) + posY); 
	  		     _xAxisValue.Push(_currentXValue);
		         _xAxisPos.Push(tempX);	
		         _currentPos++;   
	         }       
		     
	     }     
		   	
		   	
	   if (_currentPos > 1){
	        _lineLine.maxDrawIndex = _currentPos-1;
	   		Vector.DrawLine(_lineLine);	 
	   }	 
}
var temp:int = 0;
public function PrintMessage(){
   return ("XValue pos  " + _currentPos.ToString()+" "+temp+" "+xInterval+" " +_currentXValue+" "+noOfPoints);
}
   