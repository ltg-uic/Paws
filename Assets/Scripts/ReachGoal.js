public var levelToLoad : String;

function OnCollisionEnter(my_collider : Collision)
{
    if(my_collider.gameObject.tag == "Goal")
    {
    	GetComponent(GameOver).reachedGoal = true;
        GetComponent.<NetworkView>().RPC ("ReceivedFinishedLevel", RPCMode.Server, "");
    }
}
