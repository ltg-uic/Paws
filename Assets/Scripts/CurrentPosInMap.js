
var mapWidth: int = 200;
var mapHeight: int = 200;

var posX: int = 200;
var posY: int = 200;

var  mapImage: Texture2D =  null;
var  bearImage: Texture2D[];
var  trailImage: Texture2D = null;
var  startImage: Texture2D = null;
var  endImage: Texture2D = null;

static var POS_X: int = 0 ;
static var POS_Y: int = 1 ;

private var _currentYear: int;
private var _terrainMaxX: float;
private var _terrainMaxY: float;
private var _bearPos = [-1,-1];
private var _bearPreviousPos = [-1,-1];
private var _startPos = [-1,-1];
private var _endPos = [-1,-1];
private var _showMap = false;

private var _showSummaryGraph: boolean;
private var _trail: ArrayList;
private var _setPosValues = false;

private var _framesPerSecond = 5;
private var _shiftX = 10;
private var _shiftY = 14;

function Start(){

	_terrainMaxX = mapWidth;
	_terrainMaxY = mapWidth;
	_trail = new ArrayList();

}
    
function DrawMap()
{
	 	GUILayout.BeginArea(new Rect(posX, posY, mapWidth, mapHeight));
	 	
		 // Draw the map on the screen
		 if (mapImage != null)
			GUI.DrawTexture(new Rect(0, 0, mapWidth, mapHeight), mapImage);
		 
		 if (GetComponent(NetworkConnectionServer).isPlaying){
		 	 // Draw the bear on the map
		     if (_startPos[POS_X] > 0){
		       GUI.DrawTexture(new Rect(_startPos[POS_X]-_shiftX, _startPos[POS_Y] - _shiftY, 24, 24), startImage);
		       GUI.DrawTexture(new Rect(_endPos[POS_X]-_shiftX, _endPos[POS_Y] - _shiftY, 24, 24), endImage);
		       var index:int = (Time.time * _framesPerSecond) % bearImage.Length ;
               GUI.DrawTexture(new Rect(_bearPos[POS_X]-_shiftX, _bearPos[POS_Y] - _shiftY, 24, 24), bearImage[index]);
			 
			 }
            // for(var pos:Vector2 in _trail){
            if (_trail.Count > 0){
	            for (var i:int = 0; i < (_trail.Count-1); i++){
	              var _trailVector : Vector2 = _trail[i];
	                 GUI.DrawTexture(new Rect(_trailVector.x-_shiftX, _trailVector.y - _shiftY, 24, 24), trailImage);
	             }
             }
	     }

		GUILayout.EndArea();

}

public function SetCurrentYear(_value: int) {
   _currentYear = _value;
   _showMap = true;		
}

public function SetGoalBearInMap(_xyValues: String){

    var values = _xyValues.Split(":"[0]);
    
	_terrainMaxX = parseInt(values[0]);
	_terrainMaxY = parseInt(values[1]);
	  
    _startPos[POS_X] = _bearPreviousPos[POS_X] = _bearPos[POS_X] = (parseFloat(values[2])-(2000*_currentYear))*mapWidth/_terrainMaxX;
	_startPos[POS_Y] = _bearPreviousPos[POS_Y] = _bearPos[POS_Y] = mapHeight - parseFloat(values[3])*mapHeight/_terrainMaxY;
	 
	_endPos[POS_X] = (parseFloat(values[4])-(2000*_currentYear))*mapWidth/_terrainMaxX;
	_endPos[POS_Y] = mapHeight - parseFloat(values[5])*mapHeight/_terrainMaxY;
		
	_trail = new ArrayList();
	
//	Debug.Log("Meters : " + _startPos[POS_Y] + " " + _endPos[POS_Y]);

}

public function UpdateCurrentPosition(_xyValues: String){

	
    var values = _xyValues.Split(":"[0]);
    var _x = (parseFloat(values[2])-(2000*_currentYear))*mapWidth/_terrainMaxX;
    var _y = mapHeight - parseFloat(values[3])*mapHeight/_terrainMaxY;
    
	_bearPos[POS_X] = _x;
	_bearPos[POS_Y] = _y;
            

	if (Mathf.Abs(_bearPreviousPos[POS_X] - _x) > 12 ||  Mathf.Abs(_bearPreviousPos[POS_Y] - _y) > 12){
		
	   _bearPreviousPos[POS_X] = _x;
	   _bearPreviousPos[POS_Y] = _y;
	   _trail.Add(Vector2(_x,_y));
	   	   
	}
}


public function ShowSummaryGraph(_value: boolean) {
   _showSummaryGraph = _value;
   _showMap = false;	
}

public function PrintMap(){
   _showMap = true;
   }
   
public function InitializeMap(){
   _bearPos = [-1,-1];
   _bearPreviousPos = [-1,-1];
   _startPos = [-1,-1];
   _endPos = [-1,-1];
   _trail = new ArrayList();
   
   }
   
 public function PrintMessage(){
 	return ("Meters : " + _bearPos[POS_X] + " " + _bearPos[POS_Y]+" - "+_currentYear);
 }