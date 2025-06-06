using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public float itemCost;
    public int quantity;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        float temp = collision.gameObject.GetComponent<Player_Controller>().gold;

        if(temp - itemCost >= 0)
        {
            //call buy item pass in gameobject with tag for the player that bought item
            Debug.Log("Bought Item!");
        }
        else
        {
            //Call UI to say you dont have enough funds
            Debug.Log("Youre Broke!");
        }
    }
}
