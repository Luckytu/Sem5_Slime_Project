using UnityEngine;
using UnityEngine.Events;

namespace Global
{
    public class GameState : MonoBehaviour
    {
        public delegate void _onGameStateChanged(State newGameState);
        public static event _onGameStateChanged onGameStateChanged;

        public enum State
        {
            Simulation,
            Paused,
            EditCameraInGame
        }

        public const State Simulation = State.Simulation;
        public const State Paused = State.Paused;
        public const State EditCameraInGame = State.EditCameraInGame;
        
        private static State _state = Paused;
        public static State state
        {
            get => _state;
            set
            {
                if(_state != value)
                {
                    onGameStateChanged?.Invoke(value);
                }
                
                _state = value; 
            }
        }
    }
}
