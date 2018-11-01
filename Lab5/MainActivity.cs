using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Content.Res;
using Android.Content.PM;

namespace Lab5
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
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
            RequestedOrientation = ScreenOrientation.Portrait;

            SetContentView(Resource.Layout.FrontActivity);
            bool isDualPane = false;

            var startButton = FindViewById<Button>(Resource.Id.startGame);
            var p1NameEditText = FindViewById<EditText>(Resource.Id.p1Name);
            var p2NameEditText = FindViewById<EditText>(Resource.Id.p2Name);

            if (startButton != null)
            {
                isDualPane = true;

                startButton.Click += delegate
                {
                    var back = new Intent(this, typeof(BackActivity));
                    back.PutExtra("P1Name", p1NameEditText.Text );
                    back.PutExtra("P2Name", p2NameEditText.Text);
                    StartActivity(back);
                };
            }
            if (!isDualPane)
            {
                rollButton = FindViewById<Button>(Resource.Id.roll);
                endButton = FindViewById<Button>(Resource.Id.endTurn);
                newButton = FindViewById<Button>(Resource.Id.newGame);

                var dieImageView = FindViewById<ImageView>(Resource.Id.die);

                turnPtsTextView = FindViewById<TextView>(Resource.Id.ptsTurn);
                playerTurnTextView = FindViewById<TextView>(Resource.Id.turn);
                p1PtsTextView = FindViewById<TextView>(Resource.Id.score1);
                p2PtsTextView = FindViewById<TextView>(Resource.Id.score2);

                //Create object
                game = new PigGameLogic();


                rollButton.Click += delegate
                {
                    //Update Names
                    if (p1NameEditText.Text == "")
                    {
                        game.Player1.Name = "Player 1";
                    }
                    else game.Player1.Name = p1NameEditText.Text;

                    if (p2NameEditText.Text == "")
                    {
                        game.Player2.Name = "Player 2";
                    }
                    else game.Player2.Name = p2NameEditText.Text;

                    //Set the right player as whose turn it is
                    if (game.Player1Turn)
                    {
                        //Player 1's turn
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
                        //1 was rolled, lose the turn points, switch players and make some toast
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
                            p1PtsTextView.Text = game.UpdatePlayerScore().ToString();//Update score
                        }
                        else
                        {
                            p2PtsTextView.Text = game.UpdatePlayerScore().ToString();//Update score
                        }

                        //Who won
                        if (game.Player1Won())
                        {
                            //P1 won
                            Android.Widget.Toast.MakeText(this, "Player 1 Wins!! Now Reseting Game.", Android.Widget.ToastLength.Short).Show();

                        }
                        else
                        {
                            //P2 won
                            Android.Widget.Toast.MakeText(this, "Player 2 Wins!! Now Reseting Game.", Android.Widget.ToastLength.Short).Show();
                        }
                        Reset();
                    }
                    else//Continue game
                    {
                        if (game.Player1Turn)
                        {   
                            //Update score
                            p1PtsTextView.Text = game.UpdatePlayerScore().ToString();

                            //Player 2's Turn
                            playerTurnTextView.Text = game.Player2.Name + "'s Turn";
                        }
                        else
                        {
                            //Update score and set game point if there is one now
                            p2PtsTextView.Text = game.UpdatePlayerScore().ToString();

                            //Player 1's
                            playerTurnTextView.Text = game.Player1.Name + "'s Turn";
                        }
                        //Reset points text
                        turnPtsTextView.Text = "0";

                        //Switch who's turn it is
                        game.Player1Turn = !game.Player1Turn; 
                    }
                };
                newButton.Click += delegate
                {
                    //Reset the ui
                    Android.Widget.Toast.MakeText(this, "Now Reseting Game", Android.Widget.ToastLength.Short).Show(); p1PtsTextView.Text = game.UpdatePlayerScore().ToString();
                    Reset();
                };
            }
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
            //Reset the game
            game.Reset();

            //Player 2's turn
            playerTurnTextView.Text = "Player 1's Turn";

            //Reset Score
            p1PtsTextView.Text = game.Player1.Score.ToString();

            //Reset Score
            p2PtsTextView.Text = game.Player2.Score.ToString();

            //Reset the Text View
            turnPtsTextView.Text = "0"; 
        }
    }
}