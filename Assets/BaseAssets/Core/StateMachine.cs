using System;

namespace BaseAssets
{
    public class StateMachine
    {
        public enum States { Menu, Game, Win, Lose }

        public static States CurrentState = States.Menu;

        public static Action OnGame;
        public static Action OnEnd;
        public static Action OnWin;
        public static Action OnLose;
        public static Action<States> OnStateChange;

        public static void ChangeState(States newState)
        {
            CurrentState = newState;
            OnStateChange?.Invoke(CurrentState);

            switch (newState)
            {
                case States.Menu:
                    break;
                case States.Game:
                    OnGame?.Invoke();
                    break;
                case States.Win:
                    OnWin?.Invoke();
                    OnEnd?.Invoke();
                    break;
                case States.Lose:
                    OnLose?.Invoke();
                    OnEnd?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}
