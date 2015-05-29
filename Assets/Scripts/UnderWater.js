//Define variables
var underwaterLevel = 10;
var underwaterSound : AudioClip;


//The scene's default fog settings
private var defaultFog = RenderSettings.fog;
private var defaultFogColor = RenderSettings.fogColor;
private var defaultFogDensity = RenderSettings.fogDensity;
private var defaultSkybox = RenderSettings.skybox;
var noSkybox : Material;

function Start () {
    //Set the background color
    GetComponent.<Camera>().backgroundColor = Color (0, 0.4, 0.7, 1);
}

function Update () {
    if (transform.position.y < underwaterLevel) {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color (0, 0.4, 0.7, 0.6);
        RenderSettings.fogDensity = 0.04;
        RenderSettings.skybox = noSkybox;
       	GetComponent.<AudioSource>().PlayOneShot(underwaterSound);
        //        yield return new WaitForSeconds(audio.clip.length);
    }

    else {
        RenderSettings.fog = defaultFog;
        RenderSettings.fogColor = defaultFogColor;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.skybox = defaultSkybox;
        //audio.Stop(underwaterSound);
    }
}

//function OnCollisionEnter (Collider.gameObject) {
//	if (gameObject.tag == "water"){
//	audio.PlayOneShot(underwaterSound);
//	}
//	else audio.Stop(underwaterSound);
//  }