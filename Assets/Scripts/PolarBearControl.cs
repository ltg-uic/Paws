
using UnityEngine;
using System.Collections;
using System.IO;
using OpenNI;
using System;

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
		//test github update
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
	private string trialNumber;
	private TextWriter logFile;
	
	private float[] _stroke;
	private bool _changeStepsSound;
	private float _gravity;
	public bool _gameOver;
	public bool _firstStep;
    public GameObject other;
	
	//Mike's Variables for Kinect
	float leftStep_cur = 0;
	float rightStep_cur = 0;
	float leftStep_pre = 0;
	float rightStep_pre = 0;
	float leftStep_index = 0;
	float rightStep_index = 0;
	float leftStepMax = 0;
	float rightStepMax = 0;
	float leftStepMin_saved = 0;
	float rightStepMin_saved = 0;
	float leftStepMin = 0;
	float rightStepMin = 0;
	float totalLeftStep = 0;
	float totalRightStep = 0;
	int leftA = 0;
	int rightA = 0;
	int leftB = 0;
	int rightB = 0;
	int leftC = 0;
	int rightC = 0;
	float _leftStep = 0;
	float _rightStep = 0;
	int lefti = 0;
	int leftj = 0;
	int righti = 0;
	int rightj = 0;
	float step_distance = 0;
	//Scale factors for Step and Stroke
	float stepScale = 20;
	float strokeScale  = 1;
	float strokeLength = 0;
	//Mike's Variables for Arm Strokes
	float leftStroke_cur = 0;
	float leftStroke_pre = 0;
	float leftStroke_index = 0;
	float leftStrokeMax = 0;
	float leftStrokeMax_saved = 0;
	float leftStrokeMin = 0;
	float totalLeftStroke = 0;
	int leftStrokeA = 0;
	int leftStrokeB = 0;
	int leftStrokeC = 0;
	float leftStroke = 0;
	int leftStrokei = 0;
	int leftStrokej = 0;
	float rightStroke_cur = 0;
	float rightStroke_pre = 0;
	float rightStroke_index = 0;
	float rightStrokeMax = 0;
	float rightStrokeMax_saved = 0;
	float rightStrokeMin = 0;
	float totalRightStroke = 0;
	int rightStrokeA = 0;
	int rightStrokeB = 0;
	int rightStrokeC = 0;
	float rightStroke = 0;
	int rightStrokei = 0;
	int rightStrokej = 0;
	
	void Start () {
		Instantiate (polarBearTransform,transform.position,transform.rotation);

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
	   
	    polarBearTag = "PolarBear(Clone)";
	   polarBear = GameObject.Find(polarBearTag);
	   _controller = polarBear.GetComponent<CharacterController>();		
       _gameOver = true;
	   _firstStep = false;
		trialNumber = DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");
		
	}

	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.T))
			testing = true;
		else if(Input.GetKeyDown(KeyCode.R))
				testing = false;
				
		if (inWater){
			if (Mathf.Abs(Camera.main.transform.rotation.eulerAngles.x - inWRotation) > 1){
				Camera.main.transform.Rotate(new Vector3(inWRotation-Camera.main.transform.rotation.eulerAngles.x,0,0));
		   }
		}
		else{
			if (Mathf.Abs(Camera.main.transform.rotation.eulerAngles.x - outWRotation) > 1){
			   
				Camera.main.transform.Rotate(outWRotation-Camera.main.transform.rotation.eulerAngles.x,0,0);
			}
		}
		
		
		if (!_gameOver){
			//Debug.Log("No game over");
			if (Vector3.Distance(_controller.transform.position,GameObject.FindWithTag("Goal").transform.position) < 4)
			{
				GameObject.FindWithTag("Goal").audio.mute = true;
				SendMessage("GoalReached");
			    networkView.RPC ("ReceivedFinishedLevel", RPCMode.Server, "");
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
						//Debug.Log("Test - Esta en tierra");
						if (Input.GetKeyDown(KeyCode.UpArrow)){
							//Debug.Log("Test - Caminando...");
						    
							
							MoveForward("Walk");
							
							_moveDirection = new Vector3(0, 0, 1);
							_moveDirection = transform.TransformDirection(_moveDirection);
			                _moveDirection *= moveSpeed;
			               
			                   
								if (_changeStepsSound)
								{
									if (!bs.touchingWater){
							      	  //bs.setAudioClip("leftStep");
									}
									else {
										//bs.setAudioClip("waterLeftStep");
									}
							    }
								else {
									if (!bs.touchingWater){
							      	 // bs.setAudioClip("rightStep");
									}
									else {
										//bs.setAudioClip("waterRightStep");
									}
							    }
							
					
				             	//_changeStepsSound = !_changeStepsSound;
				             	
									
						}
					}
				
			        _moveDirection.y -= _gravity * Time.deltaTime;
			
	    			 // Move the controller
	   			    _controller.Move(_moveDirection * Time.deltaTime);

				}
				else if (inWater){
					//Debug.Log("Test - Esta en agua");
			
					  if (Input.GetKeyDown(KeyCode.UpArrow)){
						  // Debug.Log("Nadando");
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
			 else{  // Kinect as input control
				
				
				if (!inWater){
					//Mike's Added Code for Left Step
					
					var HipCenter = GameObject.Find("Torso");
					var FootLeft = GameObject.Find("LeftFoot");
					
					
					Vector3 positionLF = FootLeft.GetComponent<UnityEngine.Transform>().position;
					Vector3 positionHC = HipCenter.GetComponent<UnityEngine.Transform>().position;
					LogMovementData("W");
					leftStep_cur = Mathf.Abs(positionHC.y - positionLF.y);
					leftStep_pre = leftStep_index;
					leftStep_index = Mathf.Abs(positionHC.y - positionLF.y);
			    	float yHC = positionHC.y;
					float yLF = positionLF.y;
					bool speedbool = false;
	
					var FootRight = GameObject.Find("RightFoot");
					Vector3 positionRF = FootRight.GetComponent<UnityEngine.Transform>().position;
					
					rightStep_cur = Mathf.Abs(positionHC.y - positionRF.y);
					rightStep_pre = rightStep_index;
					rightStep_index = Mathf.Abs(positionHC.y - positionRF.y);
			
					float yRF = positionRF.y;
					bool speedboolRight = false;
					if(yHC!=0 && yLF!=0){
						speedbool = true;
					}
					if(speedbool){
						//test for decreasing
						if(leftStep_cur<leftStep_pre){
							lefti++;
							if(lefti>1){
								//flag as decreasing and set current stroke to step min
								leftA = 1;
								leftj = 0;
								leftStepMin = leftStep_cur;
							}
						}
						//test for increasing
						if(leftStep_cur>leftStep_pre){
							leftj++;
							if (leftj>1){
								//flag as increasing and set current stroke to step max
								leftB = 1;
								lefti = 0;
								leftStepMax = leftStep_cur;
							}
						}
						//if user has gone from decreasing to increasing save strokemax and flag A as zero (not increasing)
						if (leftA==1 && leftB==1 && leftC==0){
							leftA = 0;
							leftC = 1;
							leftStepMin_saved = leftStepMin;
						}
						//user has switched from decreasing back to increasing and therefore completed a stroke
						if (leftA==1 && leftB==1 && leftC==1){
							_leftStep = Mathf.Abs(leftStepMin_saved-leftStepMax);
							leftA = 0;
							leftB = 0;
							leftC = 0;
							leftStepMax = 0;
							leftStepMin_saved = 0;
							leftStepMin = 0;
							if(_leftStep>0.05){
								totalLeftStep = totalLeftStep + _leftStep;
								//f.WriteLine(_leftStep + "\t" + totalLeftStep);
							}
						}
					}
					//Mike's Added Code for Right Step
					if(yHC!=0 && yRF!=0){
						speedboolRight = true;
					}
					if(speedboolRight){
						//test for decreasing
						if(rightStep_cur<rightStep_pre){
							righti++;
							if(righti>1){
								//flag as decreasing and set current stroke to step min
								rightA = 1;
								rightj = 0;
								rightStepMin = rightStep_cur;
							}
						}
						//test for increasing
						if(rightStep_cur>rightStep_pre){
							rightj++;
							if (rightj>1){
								//flag as increasing and set current stroke to step max
								rightB = 1;
								righti = 0;
								rightStepMax = rightStep_cur;
							}
						}
						//if user has gone from decreasing to increasing save strokemax and flag A as zero (not increasing)
						if (rightA==1 && rightB==1 && rightC==0){
							rightA = 0;
							rightC = 1;
							rightStepMin_saved = rightStepMin;
						}
						//user has switched from decreasing back to increasing and therefore completed a stroke
						if (rightA==1 && rightB==1 && rightC==1){
							_rightStep = Mathf.Abs(rightStepMin_saved-rightStepMax);
							rightA = 0;
							rightB = 0;
							rightC = 0;
							rightStepMax = 0;
							rightStepMin_saved = 0;
							rightStepMin = 0;
							if(_rightStep>0.05){
								totalRightStep = totalRightStep + _rightStep;
								//f1.WriteLine(_rightStep + "\t" + totalRightStep);
							}
						}
					}
					

					if(totalLeftStep>=0.15 || totalRightStep>=0.15){
						if(totalLeftStep>=0.15){
							step_distance = totalLeftStep*stepScale;
							totalLeftStep = 0;	
						}
						if(totalRightStep>=0.15){
							step_distance = totalRightStep*stepScale;
							totalRightStep = 0;
						}
						MoveForward("Walk");
						//I changed the move from 0,0,1 to 0,0,5 because 1 seemed very slow
						_moveDirection = new Vector3(0, 0, step_distance);
						_moveDirection = transform.TransformDirection(_moveDirection);
						_moveDirection *= moveSpeed;
						if (_changeStepsSound)
							bs.setAudioClip("leftStep");
						else 
							bs.setAudioClip("rightStep");
						_changeStepsSound = !_changeStepsSound;
					}
			
					//f.Close();
					//f1.Close();

			        _moveDirection.y -= _gravity * Time.deltaTime;
			
	    			 // Move the controller
	   			    _controller.Move(_moveDirection * Time.deltaTime);				
					
					
				}
				else if (inWater){
				//Code for Left Stroke
				
					var HipCenter = GameObject.Find("Torso");
					var HandLeft = GameObject.Find("LeftHand");
					var HandRight = GameObject.Find("RightHand");
					Vector3 positionLH = HandLeft.GetComponent<UnityEngine.Transform>().position;
					Vector3 positionRH = HandRight.GetComponent<UnityEngine.Transform>().position;
					Vector3 positionHC = HipCenter.GetComponent<UnityEngine.Transform>().position;
					LogMovementData("S");
					leftStroke_cur = (-1*positionHC.z - (-1*positionLH.z));
					leftStroke_pre = leftStroke_index;
					leftStroke_index = (-1*positionHC.z - (-1*positionLH.z));
					rightStroke_cur = (-1*positionHC.z - (-1*positionRH.z));
					rightStroke_pre = rightStroke_index;
					rightStroke_index = (-1*positionHC.z - (-1*positionRH.z));

					float zHC = positionHC.z;
					float zLH = positionLH.z;
					float zRH = positionRH.z;
					bool speedboolStroke1 = false;
					bool speedboolStroke2 = false;
					if(zHC!=0 && zLH!=0){
						speedboolStroke1 = true;
			
					}
					if(speedboolStroke1){
						//test for increasing
						if(leftStroke_cur>leftStroke_pre){
							leftStrokei++;
							if(leftStrokei>1){
								//flag as increasing and set current stroke to stroke max
								leftStrokeA = 1;
								leftStrokej = 0;
								leftStrokeMax = leftStroke_cur;
							}
						}
						//test for decreasing
						if(leftStroke_cur<leftStroke_pre){
							leftStrokej++;
							if (leftStrokej>1){
								//flag as decreasing and set current stroke to stroke min
								leftStrokeB = 1;
								leftStrokei = 0;
								leftStrokeMin = leftStroke_cur;
							}
						}
						//if user has gone from increasing to decreasing save strokemax and flag A as zero (not increasing)
						if (leftStrokeA==1 && leftStrokeB==1 && leftStrokeC==0){
							leftStrokeB = 0;
							leftStrokeC = 1;
							leftStrokeMax_saved = leftStrokeMax;
						}
						//user has switched from decreasing back to increasing and therefore completed a stroke
						if (leftStrokeA==1 && leftStrokeB==1 && leftStrokeC==1){
							leftStroke = Mathf.Abs(leftStrokeMax_saved-leftStrokeMin);
							leftStrokeA = 0;
							leftStrokeB = 0;
							leftStrokeC = 0;
							leftStrokeMax = 0;
							leftStrokeMax_saved = 0;
							leftStrokeMin = 0;
							if(leftStroke>0.1){
								totalLeftStroke = totalLeftStroke + leftStroke;
								//f2.WriteLine(leftStroke + "\t" + totalLeftStroke);
							}
						}
					}
					//f2.Close();	
					//Right Stroke Code
					if(zHC!=0 && zRH!=0){
						speedboolStroke2 = true;
			
					}
					if(speedboolStroke2){
						//test for increasing
						if(rightStroke_cur>rightStroke_pre){
							rightStrokei++;
							if(rightStrokei>1){
								//flag as increasing and set current stroke to stroke max
								rightStrokeA = 1;
								rightStrokej = 0;
								rightStrokeMax = rightStroke_cur;
							}
						}
						//test for decreasing
						if(rightStroke_cur<rightStroke_pre){
							rightStrokej++;
							if (rightStrokej>1){
								//flag as decreasing and set current stroke to stroke min
								rightStrokeB = 1;
								rightStrokei = 0;
								rightStrokeMin = rightStroke_cur;
							}
						}
						//if user has gone from increasing to decreasing save strokemax and flag A as zero (not increasing)
						if (rightStrokeA==1 && rightStrokeB==1 && rightStrokeC==0){
							rightStrokeB = 0;
							rightStrokeC = 1;
							rightStrokeMax_saved = rightStrokeMax;
						}
						//user has switched from decreasing back to increasing and therefore completed a stroke
						if (rightStrokeA==1 && rightStrokeB==1 && rightStrokeC==1){
							rightStroke = Mathf.Abs(rightStrokeMax_saved-rightStrokeMin);
							rightStrokeA = 0;
							rightStrokeB = 0;
							rightStrokeC = 0;
							rightStrokeMax = 0;
							rightStrokeMax_saved = 0;
							rightStrokeMin = 0;
							if(rightStroke>0.1){
								totalRightStroke = totalRightStroke + rightStroke;
								//f3.WriteLine(rightStroke + "\t" + totalRightStroke);
							}
						}
					}
					//f3.Close();	
					if(totalLeftStroke>=0.1 || totalRightStroke>=0.1){
						if(totalLeftStroke>=0.1){
							strokeLength = totalLeftStroke*strokeScale;
							totalLeftStroke=0;
						}
						if(totalRightStroke>=0.1){
							strokeLength = totalRightStroke*strokeScale;
							totalRightStroke=0;
						}
						MoveForward("Swim");
						_moveDirection = new Vector3(0, 0, strokeLength);
						_moveDirection = transform.TransformDirection(_moveDirection);
						_moveDirection *= swimSpeed;
					}
						    bs.setAudioClip("inWater");
						
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

	public void SetSpeed(float _speed){
		Debug.Log("Updating speed " + _speed);
		this.moveSpeed = _speed;
		this.swimSpeed = _speed;
	}

	public void GameOver(float _value){
	   _gameOver = true;
	 
	   if (logFile!=null)
	   	   logFile.Close();
	}
    	
	public void StartGame(){
	 
	  Initialize();
	  _firstStep = false;
	  _gameOver = false;
	 
	}
	public void LoadYear(String _year){
		Debug.Log("Starting game");
		string logPath = "LogMovement_"+trialNumber+"_YEAR"+_year+".txt";
		logFile = new StreamWriter(logPath, true);
	}
	
	public void SaveLog(){
	  var logline = "W="+NumWalkSteps.ToString() + "|S=" + NumSwimSteps.ToString();
	  SendMessage("ClientAppendDataToLog", logline);	
	}
	
	public void LogMovementData(String _str){
	
		if (_firstStep && !_gameOver) {

			var HipCenter = GameObject.Find ("Torso");
			var FootLeft = GameObject.Find ("LeftFoot");
			var FootRight = GameObject.Find ("RightFoot");
			var KneeLeft = GameObject.Find ("LeftKnee");
			var KneeRight = GameObject.Find ("RightKnee");
			var Head = GameObject.Find ("Head");
			var HandLeft = GameObject.Find ("LeftHand");
			var HandRight = GameObject.Find ("RightHand");

			Vector3 positionLF = FootLeft.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionHC = HipCenter.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionRF = FootRight.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionKL = KneeLeft.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionKR = KneeRight.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionH = Head.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionHL = HandLeft.GetComponent<UnityEngine.Transform> ().position;
			Vector3 positionHR = HandRight.GetComponent<UnityEngine.Transform> ().position;

			logFile.WriteLine (_str + "\t" + positionH.x + "\t" + positionH.y + "\t" + positionH.z + "\t" + 
					positionHC.x + "\t" + positionHC.y + "\t" + positionHC.z + "\t" + 
					positionHL.x + "\t" + positionHL.y + "\t" + positionHL.z + "\t" + 
					positionHR.x + "\t" + positionHR.y + "\t" + positionHR.z + "\t" + 
					positionKL.x + "\t" + positionKL.y + "\t" + positionKL.z + "\t" + 
					positionKR.x + "\t" + positionKR.y + "\t" + positionKR.z + "\t" + 
					positionLF.x + "\t" + positionLF.y + "\t" + positionLF.z + "\t" +
					positionRF.x + "\t" + positionRF.y + "\t" + positionRF.z + "\t");
			/*  Debug.Log("Writing at log..."+positionH.x + "\t" + positionH.y + "\t" + positionH.z  + "\t" + 
				positionHC.x + "\t" + positionHC.y + "\t" + positionHC.z + "\t" + 
				positionHL.x + "\t" + positionHL.y + "\t" + positionHL.z + "\t" + 
				positionHR.x + "\t" + positionHR.y + "\t" + positionHR.z + "\t" + 
				positionKL.x + "\t" + positionKL.y + "\t" + positionKL.z + "\t" + 
				positionKR.x + "\t" + positionKR.y + "\t" + positionKR.z + "\t" + 
				positionLF.x + "\t" + positionLF.y + "\t" + positionLF.z + "\t" +
				positionRF.x + "\t" + positionRF.y + "\t" + positionRF.z + "\t");*/
		}
	}
	
	private void MoveForward(string _str){
		if (_str.Equals("Walk"))
				NumWalkSteps++;
			else
			    NumSwimSteps++;
		
		string msgToSend = "";
		if (!_firstStep){
			_firstStep = true;
		    Terrain terrain = Terrain.activeTerrain;
    		 msgToSend = terrain.terrainData.size.x.ToString() + ":" 
                            + terrain.terrainData.size.z.ToString() + ":"
                            + _controller.transform.position.x.ToString()+ ":"
                            + _controller.transform.position.z.ToString()+ ":"
                            + GameObject.FindWithTag("Goal").transform.position.x.ToString()+ ":"
                            + GameObject.FindWithTag("Goal").transform.position.z.ToString();
    
			Debug.Log("Set Goal -  current map position");	
  		    networkView.RPC ("SetGoalBearInMap", RPCMode.Server, msgToSend);
		
		}
	
	    msgToSend = NumWalkSteps.ToString() + ":" + NumSwimSteps.ToString()+":"+ _controller.transform.position.x.ToString()+":"+_controller.transform.position.z.ToString();
	    networkView.RPC ("ReceivedMovementInput", RPCMode.Server, msgToSend);
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

		if (!_getDir [RIGHT]) {

				Debug.Log ("R Asking direction  z R" + _accZR + " " + accY + " " + prev + " " + _maxAccY [RIGHT]);

				if (_accZR < 0 && accY > prev && _maxAccY [RIGHT] == MIN_MAX_VALUE) { //Wiimote pointing up
						Debug.Log ("R Getting direction  " + accY + " " + prev + " " + _maxAccY [RIGHT]);
						_getDir [RIGHT] = true;
						_minAccY [RIGHT] = prev;
				} else if (accY < prev) {
						_maxAccY [RIGHT] = MIN_MAX_VALUE;
				}

		} else if (_getDir [RIGHT]) {
				Debug.Log ("R Tiene direction  " + accY + " min " + _minAccY [RIGHT] + " prev " + prev);
			
				if (accY > _minAccY [RIGHT]) {
						if (accY > prev) {
								if (_minAccY [RIGHT] < 0 && accY > 0 && (accY - _minAccY [RIGHT]) >= 0.3) {
										_stroke [RIGHT] = accY - _minAccY [RIGHT];
										_maxAccY [RIGHT] = accY;
					
								} else if (_minAccY [RIGHT] < 0 && accY < 0 && Mathf.Abs (accY - _minAccY [RIGHT]) >= 0.3) {
										_stroke [RIGHT] = Mathf.Abs (accY - _minAccY [RIGHT]);
										_maxAccY [RIGHT] = accY;
								} else if (_minAccY [RIGHT] > 0 && Mathf.Abs (accY - _minAccY [RIGHT]) >= 0.3) {
										_stroke [RIGHT] = Mathf.Abs (accY - _minAccY [RIGHT]);
										_maxAccY [RIGHT] = accY;
								}
						} else {
								if (prev < 0 && _minAccY [RIGHT] < 0 && (Mathf.Abs (prev - _minAccY [RIGHT])) >= 0.3) { //Wiimote changing direction
										_maxAccY [RIGHT] = prev;
										_stroke [RIGHT] = Mathf.Abs (prev - _minAccY [RIGHT]);
								} else if (prev > 0 && (Mathf.Abs (prev - _minAccY [RIGHT])) >= 0.3) { //Wiimote changing direction
										_maxAccY [RIGHT] = prev;
										_stroke [RIGHT] = Mathf.Abs (prev - _minAccY [RIGHT]);
								}

						}
				} else if (accY < _minAccY [RIGHT]) {
						if (prev < 0 && _minAccY [RIGHT] < 0 && (Mathf.Abs (prev - _minAccY [RIGHT])) >= 0.3) { //Wiimote changing direction
								_maxAccY [RIGHT] = prev;
								_stroke [RIGHT] = Mathf.Abs (prev - _minAccY [RIGHT]);
						} else if (prev > 0 && (Mathf.Abs (prev - _minAccY [RIGHT])) >= 0.3) { //Wiimote changing direction
								_maxAccY [RIGHT] = prev;
								_stroke [RIGHT] = Mathf.Abs (prev - _minAccY [RIGHT]);
						} else {
								_getDir [RIGHT] = false;
						}
				}
		}
	}
}
