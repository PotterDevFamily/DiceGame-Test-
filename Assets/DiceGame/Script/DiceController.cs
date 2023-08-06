using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public bool diceSleeping;
    public int diceRoll;
    public int diceType;
    private float topSide;
    public Rigidbody myRigidbody;

    

    // Update is called once per frame
    void Update()
    {
        //Only execute when the dice has stopped rolling
        if (myRigidbody.IsSleeping())
        {
            //Only execute until we got a result
            if (!diceSleeping)
            {

                //Reset the variable that contains the topmost side of the dice
                topSide = -50000;


                //Loop through the sides of the dice and find which one has the highest transform.Y
                for (int index = 0; index < diceType; index++)
                {
                    var getChild = gameObject.transform.GetChild(index);
                    if (getChild.position.y > topSide)
                    {
                        topSide = getChild.position.y;
                        diceRoll = (index / 4) + 1;
                    }
                }
            }
            diceSleeping = true;
        }
        else
        {
            diceSleeping = false;
        }

    }
}
