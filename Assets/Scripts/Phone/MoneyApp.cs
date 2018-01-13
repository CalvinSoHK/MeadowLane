using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//App that allows us to pay for and receive money
//When the app is on, it can be used to pay for and receive money from the smart scanners in shops.
//Using the app, we can manage the animations for receiving and paying money. A unique picture will also
//be shown if the player can't afford what they're trying to purchase. The app will need to link with the playerstats to add and remove money.
public class MoneyApp : BasicApp {

    //Possible states for our app
    public enum MoneyAppState { Idle, Paying, Receiving, Broke };

    //The state of the app currently
    MoneyAppState STATE = MoneyAppState.Idle;

    //The script that holds how much money we have
    PlayerStats PLAYER_STATS;

    //Whether or not we can pay or receive cash
    public bool IS_IDLE = false;

    //Function that is called on app being initialized
    public override void InitializeApp(PlayerPhone _PHONE, PhoneLinker _LINKER)
    {
        //Sets the proper things active.
        base.InitializeApp(_PHONE, _LINKER);

        //Find the player stats script, it's on the same transform as the _PHONE.
        PLAYER_STATS = _PHONE.transform.GetComponent<PlayerStats>();
    }

    //Main function that is called every frame.
    public override void RunApp()
    {
        //The base of run app is just, if trigger down, exit app.
        base.RunApp();

        //When our state is in idle mode, we can pay, or receive money, else we can't.
        if(STATE == MoneyAppState.Idle)
        {
            IS_IDLE = true;
        }
        else
        {
            IS_IDLE = false;
        }
    }

    //Helper function that allows us to set the state of the phone.
    public void SetState(MoneyAppState _STATE)
    {
        STATE = _STATE;
    }

    //Attempt to pay money, if we get a false the payment failed and we didnt have enough, if true then we had enough.
    public bool PayMoney(int i)
    {
        //Check if we have enough, return false if no
        if(PLAYER_STATS.GetMoney() < i)
        {
            //Vibrate for feedback
            PHONE.VibratePhone(0.75f, 500f);
            return false;
        }
        else//We have enough, subtract the money 
        {
            //Vibrate for feedback
            PHONE.VibratePhone(0.75f, 500f);

            //There is only addmoney, so just add the negative value.
            PLAYER_STATS.AddMoney(-i);
            return true;
        }
    }

    //Give money to the player
    public void EarnMoney(int i)
    {
        PHONE.VibratePhone(0.75f, 500f);
        PLAYER_STATS.AddMoney(i);
    }

}
