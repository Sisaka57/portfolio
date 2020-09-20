using UnityEngine;
using static Board;

public class MultiPlayerMouseDown : MonoBehaviour
{

	[Photon.Pun.PunRPC]
	void MouseDownOtherPlayer()
	{
		Player.PlayerClass.Coin_index_label.text = ""; // cleans the label.text

		for (int deactiveTowers = 0; deactiveTowers < 26; deactiveTowers++) // deactivates all towers 
			BoardClass.greenTowers[deactiveTowers].SetActive(false);

		BoardClass.CoinDefaultColor = true; // checkers no longer flash's

		for (int i = 0; i < 15; i++)
		{
			if (Player.PlayerClass.player_number == 1)
				BoardClass.WhiteCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.WhiteCoinDefultColor;
			else
				BoardClass.BlackCoinsContainer[i].GetComponent<Renderer>().material = BoardClass.BlackCoinDefultColor;
		}
	}


	public void mouseDown()
    {


		if (/* here by Saad*/  Player.PlayerClass.player_number == Player.PlayerClass.myPlayerNumber/*here by Saad*/)
		{
			GetComponent<Photon.Pun.PhotonView>().RPC("MouseDownOtherPlayer", Photon.Pun.RpcTarget.All, null);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
