public var levelToLoad : String;

function OnCollisionEnter(my_collider : Collision)
{
    if(my_collider.gameObject.tag == "Goal")
    {
    	GetComponent(GameOver).reachedGoal = true;
        networkView.RPC ("ReceivedFinishedLevel", RPCMode.Server, "");
    }
}
