
using UnityEngine;
using System.Collections;

public class PolarBearControl : MonoBehaviour {
 
    const int MIN_MAX_VALUE = 100;
    const int LEFT = 0;
	const int RIGHT = 1;
    const float inWRotation = 14;
    const float outWRotation = 14;
    public Transform polarBearTransform;
    public bool testing;
    public float moveSpeed = 7;
    public float swimSpeed = 7;
    public float defaultGravity = 20;  
	public GameObject polarBear;
	public string polarBearTag;
	public CharacterController _controller;
    public BearStatus bs;
	public int NumSwimSteps;
	public int NumWalkSteps;
	public string typeActivity;
	public int rightStep;
	public int leftStep;
	public int[] previousStep;
	public bool inWater;
	public int timeBanner;
	
	private Vector3 _moveDirection;
	private Transform _myTransform;
		
	private float _accYR;
	private float _accYL;
	private float _accZL;
	private float _accZR;
	
	private int _prevTL;
	private int _prevBL;
	
	private float _pAccYL;
	private float _pAccYR;	
	private float[] _maxAccY;
	private float[] _minAccY;
	private bool[] _getDir;
	
	private float[] _stroke;
	private bool _changeStepsSound;
	private float _gravity;
	public bool _gameOver;
	public bool _firstStep;
	private bool startScene = false;
    public GameObject other;
    private bool updatedValues = false;
	
    
	void Start () {
	   
		Initialize();
	}
	
	void Initialize(){
	    _moveDirection = Vector3.zero;
		_getDir = new bool[2];
	    _getDir[LEFT] = _getDir[RIGHT] = false;
		_maxAccY = new float[2];
		_minAccY = new float[2];
		_stroke = new float[2];
	    _maxAccY[LEFT] = _maxAccY[RIGHT] =  _minAccY[LEFT] = _minAccY[RIGHT] = MIN_MAX_VALUE;
	    inWater = false;
	    _gravity = defaultGravity;
	    _changeStepsSound = false;
		previousStep = new int[2];
		previousStep[LEFT] = -1;
		previousStep[RIGHT] = -1;
		NumSwimSteps = NumWalkSteps = 0;
	   
	   Instantiate (polarBearTransform,transform.position,transform.rotation);
	   polarBearTag = "PolarBear(Clone)";
	   polarBear = GameObject.Find(polarBearTag);
	   _controller = polarBear.GetComponent<CharacterController>();		
       _gameOver = false;
	   _firstStep = false;
		
	}

   
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.T))
			testing = true;
		else if(Input.GetKeyDown(KeyCode.R))
				testing = false;
				
		if (inWater){
		  if (Mathf.Abs(Camera.mainCamera.transform.rotation.eulerAngles.x - inWRotation) > 1){
	           Camera.mainCamera.transform.Rotate(new Vector3(inWRotation-Camera.mainCamera.transform.rotation.eulerAngles.x,0,0));
		   }
		}
		else{
			if (Mathf.Abs(Camera.mainCamera.transform.rotation.eulerAngles.x - outWRotation) > 1){
			   
				Camera.mainCamera.transform.Rotate(outWRotation-Camera.mainCamera.transform.rotation.eulerAngles.x,0,0);
			}
		}
		
		
		if (!_gameOver){
			if (!Application.loadedLevelName.Equals("StudyScene")){
				if (Vector3.Distance(_controller.transform.position,GameObject.FindWithTag("Goal").transform.position) < 4)
				{
					GameObject.FindWithTag("Goal").audio.mute = true;
					SendMessage("GoalReached");
				    networkView.RPC ("ReceivedFinishedLevel", RPCMode.Server, "");
				}
			}
					
		    BearStatus bs = polarBear.GetComponent<BearStatus>();
		    inWater = bs.inWater;
		    if (inWater)
		       _gravity = 0;
		    else 
		       _gravity = defaultGravity;
		       
			_moveDirection = Vector3.zero;
			if (Application.loadedLevelName.Equals("StudyScene")){
				if(Input.GetKeyDown(KeyCode.S))
				{
					typeActivity = "S";
					NumSwimSteps = 0;
					_controller.transform.position = new Vector3(1000,12,550);	
				}
				else if(Input.GetKeyDown(KeyCode.W)){
					typeActivity = "W";
					NumWalkSteps = 0;
					_controller.transform.position = new Vector3(980,12,550);	
				}
				if(Input.GetKeyDown(KeyCode.L))
				{
					 SendMessage("AppendDataToLog",("|"+typeActivity+"|"+(typeActivity.Equals("S")?NumSwimSteps:NumWalkSteps)));	
				}
			}
			
			if (testing){//Utiliza las teclas direccionales
				
				_maxAccY[LEFT] = _maxAccY[RIGHT] = _minAccY[RIGHT] = _minAccY[RIGHT] = MIN_MAX_VALUE;
				_getDir[LEFT] = _getDir[RIGHT] = false;
				
				if (!inWater){
					if (_controller.isGrounded){
					   
						if (!startScene){
							startScene = true;
							SendMessage("HideInitialMessage");
						}
						//Debug.Log("Test - Esta en tierra");
						if (Input.GetKeyDown(KeyCode.UpArrow)){
						//	Debug.Log("Test - Caminando...");
						    
							MoveForward("Walk");

							_moveDirection = new Vector3(0, 0, 1);
							_moveDirection = transform.TransformDirection(_moveDirection);
			                _moveDirection *= moveSpeed;
			                 
			                   
								if (_changeStepsSound)
								{
									if (!bs.touchingWater){
							      	  bs.setAudioClip("leftStep");
									}
									else {
										bs.setAudioClip("waterLeftStep");
									}
							    }
								else {
									if (!bs.touchingWater){
							      	  bs.setAudioClip("rightStep");
									}
									else {
										bs.setAudioClip("waterRightStep");
									}
							    }
							
					
				             	_changeStepsSound = !_changeStepsSound;
				             	
									
						}
					}
				
			        _moveDirection.y -= _gravity * Time.deltaTime;
			
	    			 // Move the controller
	   			    _controller.Move(_moveDirection * Time.deltaTime);

				}
				else if (inWater){
					//Debug.Log("Test - Esta en agua");
			
					  if (Input.GetKeyDown(KeyCode.UpArrow)){
						 //  Debug.Log("Nadando");
				 		   
	   					    MoveForward("Swim");
	   					    
	   					    bs.setAudioClip("inWater");
	   					    _moveDirection = new Vector3(0, 0, 1);
						    _moveDirection = transform.TransformDirection(_moveDirection);
	  				        _moveDirection *= swimSpeed;
						   
					 }
	
				    _moveDirection.y = Mathf.Sin(Time.time*4)/4;
			
		    	    // Move the controller
		     	    _controller.Move(_moveDirection * Time.deltaTime);

					 
				}
			
			 }	
			 else{  // Wii as input control
				if (!inWater){
					_maxAccY[LEFT] = _maxAccY[RIGHT] = _minAccY[LEFT] = _minAccY[RIGHT] = MIN_MAX_VALUE;
					_getDir[LEFT] = _getDir[RIGHT] = false;
				
					if (_controller.isGrounded){
						if (!startScene){
							startScene = true;
							SendMessage("HideInitialMessage");
						}
				       // Debug.Log(leftStep + " " + rightStep);
						
					   // if (!(rightStep == 1 && leftStep == 1) && (leftStep != previousStep[LEFT] || rightStep != previousStep[RIGHT])){
				       if (rightStep != leftStep && updatedValues){
				   			updatedValues = false;
							MoveForward("Walk");
	
							_moveDirection = new Vector3(0, 0, 1);
							_moveDirection = transform.TransformDirection(_moveDirection);
			                _moveDirection *= moveSpeed;
					                 
						    
							if (_changeStepsSound)
						        bs.setAudioClip("leftStep");
					      	else 
						        bs.setAudioClip("rightStep");
									
							
						    _changeStepsSound = !_changeStepsSound;
										
						    previousStep[LEFT] = leftStep;
							previousStep[RIGHT] = rightStep;
						}
						
					}
			
					
		
			        _moveDirection.y -= _gravity * Time.deltaTime;
			
	    			 // Move the controller
	   			    _controller.Move(_moveDirection * Time.deltaTime);				
					
					
				}
				else if (inWater){
				
					SensingLeftPaw();
					SensingRightPaw();
					
					if (_getDir[LEFT] && Mathf.Abs(_minAccY[LEFT]) < MIN_MAX_VALUE && Mathf.Abs(_maxAccY[LEFT]) < MIN_MAX_VALUE) {

							Debug.Log("Nadando.. direction L " + _stroke[LEFT] +" min "+_minAccY[LEFT] + " max " + _maxAccY[LEFT] );
						    				
						    MoveForward("Swim");
   					    
   						   _moveDirection = new Vector3(0, 0, 1);
		                   _moveDirection = transform.TransformDirection(_moveDirection);
  				           _moveDirection *= swimSpeed;

						   _getDir[LEFT]  = false;
						    bs.setAudioClip("inWater");
							
							Debug.Log("Move left");
		                
					 }
	    			else if (_getDir[RIGHT] && Mathf.Abs(_minAccY[RIGHT]) < MIN_MAX_VALUE && Mathf.Abs(_maxAccY[RIGHT]) < MIN_MAX_VALUE) {

							Debug.Log("Nadando.. direction R " + _stroke[RIGHT] +" min "+_minAccY[RIGHT] + " max " + _maxAccY[RIGHT] );
						    				
						    MoveForward("Swim");
   					    
   						   _moveDirection = new Vector3(0, 0, 1);
		                   _moveDirection = transform.TransformDirection(_moveDirection);
  				           _moveDirection *= swimSpeed;

						   _getDir[RIGHT]  = false;
						    bs.setAudioClip("inWater");
							Debug.Log("Move Right");
		                
					 }
					
				    _moveDirection.y = Mathf.Sin(Time.time*4)/4;
		    	    // Move the controller
		     	    _controller.Move(_moveDirection * Time.deltaTime);    
		     	    
	
			   }	
			} 
		}
	}

	
	public void yValueL(float _value) {
	   _pAccYL = _accYL;
	   _accYL = _value;
    }
	
	public void yValueR(float _value) {
	   _pAccYR = _accYR;
	   _accYR = _value;
    }
	
	public void zValueL(float _value) {
	
	   _accZL = _value;
    }
    public void zValueR(float _value) {
	
	   _accZR = _value;
    }
    
    public void setRightStep(int _value) {
		rightStep = _value;
		updatedValues = true;
    }

    public void setLeftStep(int _value) {
		leftStep = _value;
		updatedValues = true;
    }
	
	public void GameOver(float _value){
	   _gameOver = true;
	   startScene = false;
	}
    	
	public void StartGame(){
	Debug.Log("Starting game");
		 SendMessage("StartLog");
	  _gameOver = false;
	  _firstStep = false;
	  Initialize();
	 
	}
	
	public void SaveLog(){
	  var logline = "W="+NumWalkSteps.ToString() + "|S=" + NumSwimSteps.ToString();
	  SendMessage("ClientAppendDataToLog", logline);	
	}
	
	private void MoveForward(string _str){
		if (_str.Equals("Walk"))
				NumWalkSteps++;
			else
			    NumSwimSteps++;
		
		if (!Application.loadedLevelName.Equals("StudyScene")){
			string msgToSend = "";
			if (!_firstStep){
				SendMessage("SetCurrentYear",int.Parse(Application.loadedLevelName));
				_firstStep = true;
			    Terrain terrain = Terrain.activeTerrain;
	    		 msgToSend = terrain.terrainData.size.x.ToString() + ":" 
	                            + terrain.terrainData.size.z.ToString() + ":"
	                            + _controller.transform.position.x.ToString()+ ":"
	                            + _controller.transform.position.z.ToString()+ ":"
	                            + GameObject.FindWithTag("Goal").transform.position.x.ToString()+ ":"
	                            + GameObject.FindWithTag("Goal").transform.position.z.ToString();
	    
	    
	  		    networkView.RPC ("SetGoalBearInMap", RPCMode.Server, msgToSend);
			
			}
			SendMessage("SetTrigger",_controller.transform.position.z);		
		    msgToSend = NumWalkSteps.ToString() + ":" + NumSwimSteps.ToString()+":"+ _controller.transform.position.x.ToString()+":"+_controller.transform.position.z.ToString();
		    networkView.RPC ("ReceivedMovementInput", RPCMode.Server, msgToSend);
		}
	}
	
   private void SensingLeftPaw(){
		var prev = _pAccYL;
		var accY = _accYL;

		if (!_getDir[LEFT]){

			Debug.Log("L Asking direction  z "  + _accZL + " " + accY+" " + prev + " " + _maxAccY[LEFT]);
			if (_accZL < 0){
				if ( accY > prev && _maxAccY[LEFT] == MIN_MAX_VALUE ){ //Wiimote pointing up

					_getDir[LEFT] = true;
					_minAccY[LEFT] = prev;
				}
				else if (accY < prev){
					_maxAccY[LEFT] = 	MIN_MAX_VALUE;
				}
			}
			
		}
	    else {
			Debug.Log("L Tiene direction  " + accY +" min "+_minAccY[LEFT] + " prev " + prev);
		
			if (accY < prev){
					Debug.Log("L Cambio direction  " + accY +" min "+_minAccY[LEFT] + " prev " + prev);
		
				if (Mathf.Sign(prev) == Mathf.Sign(_minAccY[LEFT])){
					if (Mathf.Abs(prev - _minAccY[LEFT]) >= 0.3){
						_maxAccY[LEFT] = prev;
					}
				}
				else{
					if (Mathf.Abs(prev) + Mathf.Abs(_minAccY[LEFT]) >= 0.3){
						_maxAccY[LEFT] = prev;
					}
				}
				
			}
		}
	}
	
    private void SensingRightPaw(){
	 var prev = _pAccYR;
	 var accY = _accYR;

		if (!_getDir[RIGHT]){

			Debug.Log("R Asking direction  z R"  + _accZR + " " + accY+" " + prev + " " + _maxAccY[RIGHT]);
			
			if ( _accZR< 0 && accY > prev && _maxAccY[RIGHT] == MIN_MAX_VALUE ){ //Wiimote pointing up
               Debug.Log("R Getting direction  "  + accY+" " + prev + " " + _maxAccY[RIGHT]);
				_getDir[RIGHT] = true;
				_minAccY[RIGHT] = prev;
			}
			else if (accY < prev){
				_maxAccY[RIGHT] = 	MIN_MAX_VALUE;
			}
			
		}
	    else if (_getDir[RIGHT]) {
			Debug.Log("R Tiene direction  " + accY +" min "+_minAccY[RIGHT] + " prev " + prev);
					
				 if (accY > _minAccY[RIGHT]){
					if (accY > prev){
					    if (_minAccY[RIGHT] < 0 && accY > 0 && (accY - _minAccY[RIGHT]) >= 0.3){
						   _stroke[RIGHT] = accY - _minAccY[RIGHT];
						   _maxAccY[RIGHT] = accY;
							
						}
						else if (_minAccY[RIGHT] < 0 && accY < 0 && Mathf.Abs(accY - _minAccY[RIGHT]) >= 0.3){
						   _stroke[RIGHT] = Mathf.Abs(accY - _minAccY[RIGHT]);
							_maxAccY[RIGHT] = accY;
						}
						else if (_minAccY[RIGHT] > 0 && Mathf.Abs(accY - _minAccY[RIGHT]) >= 0.3){
							_stroke[RIGHT] = Mathf.Abs(accY - _minAccY[RIGHT]);
							_maxAccY[RIGHT] = accY;
						}
					}
					else{
						if  (prev < 0 && _minAccY[RIGHT] < 0 && (Mathf.Abs(prev - _minAccY[RIGHT])) >= 0.3 ){ //Wiimote changing direction
						_maxAccY[RIGHT] = prev;
						_stroke[RIGHT] = Mathf.Abs(prev - _minAccY[RIGHT]);
					     }
						 else if (prev > 0 && (Mathf.Abs(prev - _minAccY[RIGHT] )) >= 0.3){ //Wiimote changing direction
							_maxAccY[RIGHT] = prev;
							_stroke[RIGHT] = Mathf.Abs(prev - _minAccY[RIGHT]);
						 }
		
					}
				 }
				 else if (accY < _minAccY[RIGHT]){
		     		if  (prev < 0 && _minAccY[RIGHT] < 0 && (Mathf.Abs(prev - _minAccY[RIGHT])) >= 0.3 ){ //Wiimote changing direction
						_maxAccY[RIGHT] = prev;
						_stroke[RIGHT] = Mathf.Abs(prev - _minAccY[RIGHT]);
				     }
					
					 else if (prev > 0 && (Mathf.Abs(prev - _minAccY[RIGHT] )) >= 0.3){ //Wiimote changing direction
						_maxAccY[RIGHT] = prev;
						_stroke[RIGHT] = Mathf.Abs(prev - _minAccY[RIGHT]);
					 }
					else {
						_getDir[RIGHT] = false;
					}
				 }
		}
	}
	
			
}
