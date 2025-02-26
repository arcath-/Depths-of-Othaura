using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Actors
{
    internal class Player : Actor
    {
        public Player() : base(Color.White, Color.Transparent, '@', zIndex: int.MaxValue, maxHealth: 100)
        {
            IsFocused = true;
        }

        private readonly Dictionary<Keys, Direction> _playerMovements = new()
    {
        {Keys.W, Direction.Up},
        {Keys.A, Direction.Left},
        {Keys.S, Direction.Down},
        {Keys.D, Direction.Right}
    };

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;
            var moved = false;
            foreach (var kvp in _playerMovements)
            {
                if (keyboard.IsKeyPressed(kvp.Key))
                {
                    var moveDirection = kvp.Value;
                    moved = Move(moveDirection);
                    break;
                }
            }
            return base.ProcessKeyboard(keyboard) || moved;
        }
    }
}
