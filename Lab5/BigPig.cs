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
    class PigGameLogic
    {
        public class Player
        {
            public string Name { get; set; }
            public int Score { get; set; }
            public int TurnScore { get; set; }
            public Boolean Win { get; set; }

            public Player()
            {
                Name = "";
                Score = 0;
                TurnScore = 0;
                Win = false;
            }
        }
        //Creates and intializes new players
        public Player Player1 = new Player();
        public Player Player2 = new Player();

        //keeps track of whose turn it is. It's either player ones turn or not. 
        public Boolean Player1Turn { get; set; } = true; 

        Random rand = new Random();

        //Winning Score
        private const int WINSCORE = 100;

        public Boolean GamePoint { get; set; } = false;

        //roll dice
        public int RollDie()
        {
            return rand.Next(6) + 1; //returns int 1-6
        }

        public int UpdatePlayerScore()
        {
            if (Player1Turn)
            {
                Player1.Score += Player1.TurnScore;
                Player1.TurnScore = 0;//Reset
                if (Player1.Score >= WINSCORE) GamePoint = true;
                return Player1.Score;
            }
            else//player 2 turn
            {
                Player2.Score += Player2.TurnScore;
                if (Player2.Score >= WINSCORE) GamePoint = true;
                Player2.TurnScore = 0;//Reset
                return Player2.Score;
            }
        }
        public int UpdateTurnPts(int Score)
        {
            if (Player1Turn)
            {
                if (Score == 1) Player1.TurnScore = 0;
                else Player1.TurnScore += Score;//Saves score send back to activity to display
                return Player1.TurnScore;
            }
            else//player 2 turn
            {
                if (Score == 1) Player2.TurnScore = 0;
                else Player2.TurnScore += Score;
                return Player2.TurnScore;
            }
        }
        public bool CheckWin()
        {
            if (Player1Turn)
            {
                Player1.Score += Player1.TurnScore;
                Player1.TurnScore = 0;//Reset

                if (Player1.Score >= WINSCORE) Player1.Win = true;//Someone might win
                return Player1.Win;
            }
            else//player 2 turn
            {
                Player2.Score += Player2.TurnScore;
                Player2.TurnScore = 0;//Reset

                if (Player2.Score >= WINSCORE) Player2.Win = true;//Someone might win
                return Player2.Win;
            }
        }

        //Back to square one!
        public void Reset()
        {
            Player1.Score = 0;
            Player1.TurnScore = 0;
            Player1.Win = false;
            Player2.Score = 0;
            Player2.TurnScore = 0;
            Player2.Win = false;
            Player1Turn = true;
            GamePoint = false;
        }

        //User win
        public bool Player1Won()
        {
            if (Player1.Score > Player2.Score)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}