using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Lab5
{
    [Activity(Label = "BackActivity")]
    public class BackActivity : Activity
    {
        Button rollButton;
        Button endButton;
        Button newButton;

        TextView turnPtsTextView;
        TextView playerTurnTextView;
        TextView p1PtsTextView;
        TextView p2PtsTextView;

        PigGameLogic game;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BackActivity);
            // Create your application here 
            rollButton = FindViewById<Button>(Resource.Id.roll);
            endButton = FindViewById<Button>(Resource.Id.endTurn);
            newButton = FindViewById<Button>(Resource.Id.newGame);

            var dieImageView = FindViewById<ImageView>(Resource.Id.die);

            var p1NameEditText = FindViewById<EditText>(Resource.Id.p1Name);
            var p2NameEditText = FindViewById<EditText>(Resource.Id.p2Name);

            turnPtsTextView = FindViewById<TextView>(Resource.Id.ptsTurn);
            playerTurnTextView = FindViewById<TextView>(Resource.Id.turn);
            p1PtsTextView = FindViewById<TextView>(Resource.Id.score1);
            p2PtsTextView = FindViewById<TextView>(Resource.Id.score2);

            //Create object
            game = new PigGameLogic();

            //First time running
            if (savedInstanceState != null)
            {
                game.Player1.Score = savedInstanceState.GetInt("p1Score");
                game.Player1.TurnScore = savedInstanceState.GetInt("p1TurnScore");
                game.Player1.Name = savedInstanceState.GetString("p1Name");
                game.Player2.Score = savedInstanceState.GetInt("p2Score");
                game.Player2.TurnScore = savedInstanceState.GetInt("p2TurnScore");
                game.Player2.Name = savedInstanceState.GetString("p2Name");
                game.Player1Turn = savedInstanceState.GetBoolean("p1Turn");
                game.GamePoint = savedInstanceState.GetBoolean("game");
                Update();
            }

            rollButton.Click += delegate
            {
                //Update Names
                if (game.Player1.Name == "")
                {
                    game.Player1.Name = "Player 1";
                }

                if (game.Player2.Name == "")
                {
                    game.Player2.Name = "Player 2";
                }

                //Set the right player as whose turn it is
                if (game.Player1Turn)
                {   
                    //Player 1's Turn
                    playerTurnTextView.Text = game.Player1.Name + "'s Turn";
                }
                else
                {   
                    //Player 2's turn
                    playerTurnTextView.Text = game.Player2.Name + "'s Turn";
                }

                //Display image
                int roll = game.RollDie();
                switch (roll)
                {
                    case 1:
                        dieImageView.SetImageResource(Resource.Drawable.die1);
                        break;
                    case 2:
                        dieImageView.SetImageResource(Resource.Drawable.die2);
                        break;
                    case 3:
                        dieImageView.SetImageResource(Resource.Drawable.die3);
                        break;
                    case 4:
                        dieImageView.SetImageResource(Resource.Drawable.die4);
                        break;
                    case 5:
                        dieImageView.SetImageResource(Resource.Drawable.die5);
                        break;
                    case 6:
                        dieImageView.SetImageResource(Resource.Drawable.die6);
                        break;
                    default:
                        break;
                }

                //Then add points to the turn points and check for win if >0, else Switch players add 0 and make enite turn0 points
                int turnPts = game.UpdateTurnPts(roll);//returns turn points or zero if an 1 is rolled

                if (turnPts == 0)
                {
                    //1 was rolled and lose the turn points, switch players and make some toast, no butter
                    //Disable button
                    rollButton.Enabled = false;
                    Android.Widget.Toast.MakeText(this, "You Lost Your Points!", Android.Widget.ToastLength.Short).Show();
                    turnPtsTextView.Text = "0";
                }

                else//Update text view if you didn't roll an 1
                {
                    //Update turn Pts
                    turnPtsTextView.Text = turnPts.ToString();
                }
            };
            endButton.Click += delegate
            {
                rollButton.Enabled = true;

                //If its game point
                if (game.GamePoint)//someone won
                {
                    //Update scores
                    if (game.Player1Turn)
                    {
                        //Update Score
                        p1PtsTextView.Text = game.UpdatePlayerScore().ToString();
                    }
                    else
                    {   
                        //Update Score
                        p2PtsTextView.Text = game.UpdatePlayerScore().ToString();
                    }

                    //Who won
                    if (game.Player1Won())
                    {
                        //P1 won
                        Android.Widget.Toast.MakeText(this, "Player 1 Wins--Reseting Game", Android.Widget.ToastLength.Short).Show();
                    }
                    else
                    {
                        //P2 won
                        Android.Widget.Toast.MakeText(this, "Player 2 Wins--Reseting Game", Android.Widget.ToastLength.Short).Show();
                    }
                    Reset();
                }

                else//Continue game
                {
                    if (game.Player1Turn)
                    {   
                        //Update Score
                        p1PtsTextView.Text = game.UpdatePlayerScore().ToString();

                        //Player 2's Turn
                        playerTurnTextView.Text = game.Player2.Name + "'s Turn";
                    }
                    else
                    {   
                        //Update Score and set game point if there is one now
                        p2PtsTextView.Text = game.UpdatePlayerScore().ToString();
                        //Player 1's Turn
                        playerTurnTextView.Text = game.Player1.Name + "'s Turn";
                    }
                    turnPtsTextView.Text = "0";

                    //Switch who's turn it is
                    game.Player1Turn = !game.Player1Turn; 
                }
            };

            newButton.Click += delegate
            {
                //reset the ui
                Android.Widget.Toast.MakeText(this, "Reseting Game", Android.Widget.ToastLength.Short).Show(); p1PtsTextView.Text = game.UpdatePlayerScore().ToString();
                Reset();
                var front = new Intent(this, typeof(MainActivity));
                StartActivity(front);
            };
        }

    protected void Update()
    {
        p1PtsTextView.Text = game.Player1.Score.ToString();
        p2PtsTextView.Text = game.Player2.Score.ToString();

        if (game.Player1Turn)
        {
            turnPtsTextView.Text = game.Player1.TurnScore.ToString();
            playerTurnTextView.Text = game.Player1.Name;
        }
        else
        {
            turnPtsTextView.Text = game.Player2.TurnScore.ToString();
            playerTurnTextView.Text = game.Player2.Name;
        }

    }
    private void Reset()
    {
        game.Reset();
        playerTurnTextView.Text = "Player 1's Turn";//Player 2's turn
        p1PtsTextView.Text = game.Player1.Score.ToString();//Reset Score
        p2PtsTextView.Text = game.Player2.Score.ToString();//Reset Score
        turnPtsTextView.Text = "0"; //Reset the Text View
    }
    protected override void OnSaveInstanceState(Bundle outState)
    {
        outState.PutInt("p1Score", game.Player1.Score);
        outState.PutInt("p1TurnScore", game.Player1.TurnScore);
        outState.PutString("p1Name", game.Player1.Name);
        outState.PutInt("p2Score", game.Player2.Score);
        outState.PutInt("p2TurnScore", game.Player2.TurnScore);
        outState.PutString("p2Name", game.Player2.Name);
        outState.PutBoolean("game", game.GamePoint);

        outState.PutBoolean("p1Turn", game.Player1Turn);


        base.OnSaveInstanceState(outState);
    }
        protected override void OnResume()
        {
            base.OnResume();

            game.Player1.Name = Intent.Extras.GetString("P1Name");
            game.Player2.Name = Intent.Extras.GetString("P2Name");
        }
    }
}