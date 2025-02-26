using Depths_of_Othaura.Data.Screens;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Actors
{
    internal abstract class Actor : Entity
    {
        protected Actor(Color foreground, Color background, int glyph, int zIndex, int maxHealth) : base(foreground, background, glyph, zIndex)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
        }

        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public bool IsAlive => Health > 0;

        public bool Move(int x, int y)
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            var actorManager = ScreenContainer.Instance.World.ActorManager;

            if (!IsAlive) return false;

            // If the position is out of bounds, don't allow movement
            if (!tilemap.InBounds(x, y)) return false;

            // If another actor already exists at the location, don't allow movement
            if (actorManager.ExistsAt((x, y))) return false;

            // Don't allow movement for these cases
            var obstruction = tilemap[x, y].Obstruction;
            switch (obstruction)
            {
                case World.ObstructionType.FullyBlocked:
                case World.ObstructionType.MovementBlocked:
                    return false;
            }

            // Set new position
            Position = new Point(x, y);
            return true;
        }

        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }
    }
}
