// Testing comment for GitHub
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class BearStatus : MonoBehaviour {

	 public bool inWater = false;
	 public bool touchingWater = false;
	 
	 public AudioClip waterSplash;
	 public AudioClip walkingSteps_1;
     public AudioClip walkingSteps_2;
	 public AudioClip waterWalkingSteps_1;
     public AudioClip waterWalkingSteps_2;
     public AudioClip underwaterSound;
	 public AudioClip splashSound;
	
     private float normalWaterHeight;
	
	void Start(){
		normalWaterHeight = GameObject.Find("Water").transform.position.y;
		inWater = false;	
		touchingWater = false;
	}
	
	void OnTriggerEnter(Collider other) {
		 if (other.CompareTag("Water")){
			     inWater = true;
			if(transform.position.y <= normalWaterHeight + 0.275f){
		       SendMessage("InsideWater");
		    }
			touchingWater = false;
	    }
	}
	
	void OnTriggerStay(Collider other){
		
	  if(other.CompareTag("Water")){
			
		if(transform.position.y <= normalWaterHeight + 0.275f  && !inWater){
			inWater = true;
		   SendMessage("InsideWater");
			touchingWater = false;	
		}else if(transform.position.y > normalWaterHeight + 0.275f   && inWater){
			SendMessage("OutOfWater");
			 inWater = false;
			touchingWater = true;
		} 
			
	  } 
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Water")){
			if (touchingWater)
		    {
				SendMessage("OutOfWater");
			}
			inWater = false;		
			touchingWater = false;
		}			
	}
	
	public void setAudioClip(string _clip){
		  audio.volume = 0.8f;
    	  audio.loop = false;
		
    	  switch (_clip)
	      {
			case "inWater":{
				audio.volume = 0.65f;
		    	if (!(audio.isPlaying && audio.clip == splashSound)){
			    		//audio.Stop();
				        audio.PlayOneShot(waterSplash);
			    }	    	
				
				break;
				}
			case "rightStep":{	
			     audio.volume = 0.8f;
			
			    if (!(audio.isPlaying && audio.clip == splashSound)){
			    //	audio.Stop();
				    audio.PlayOneShot(walkingSteps_2);
			    }	
				//audio.clip = walkingSteps_2;
			    //audio.Play();
				
			 	break;
				}
			case "waterRightStep":{	
			    audio.volume = 0.6f;
			    if (!(audio.isPlaying && audio.clip == splashSound)){
			    //	audio.Stop();
				    audio.PlayOneShot(waterWalkingSteps_2);
			    }	
				Debug.Log("touching water right");
				
			 	break;
				}
		    case "leftStep": {
			 audio.volume = 0.8f;
		    	 if (!(audio.isPlaying && audio.clip == splashSound)){
			    //	audio.Stop();
					audio.PlayOneShot(walkingSteps_1);
			    }
			    //audio.clip = walkingSteps_1;
		    	//audio.Play();
				
				break;
				}
			case "waterLeftStep": {
			 audio.volume = 0.6f;
		    	 if (!(audio.isPlaying && audio.clip == splashSound)){
			    //	audio.Stop();
					audio.PlayOneShot(waterWalkingSteps_1);
			    }
			    //audio.clip = walkingSteps_1;
		    	//audio.Play();
				Debug.Log("touching water left");
				break;
				}
			case "underwater":{
			    audio.volume = 0.65f;
			    audio.Stop();
				//audio.clip = underwaterSound;
			    //audio.Play(); 
				audio.PlayOneShot(underwaterSound);
				break;
				}
			default:{
			  break;
			}
		  }
   }
}


