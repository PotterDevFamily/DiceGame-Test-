using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceGame{
    public static class DiceGameInfo{
        public enum GameState{
            DiceRolling,
            SelectDirection,
            PlayerMoving,
            Idle
        }
        public static int diceRolledCnt = 0;
        public static GameState currentState = GameState.Idle;
    }
}