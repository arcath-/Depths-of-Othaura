using Depths_of_Othaura.Data.Screens;
using GoRogue.FOV;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities
{
    internal sealed class ActorManager
    {
        private readonly Dictionary<Point, Actor> _actors = [];
        public readonly EntityManager EntityComponent;

        public ActorManager()
        {
            EntityComponent = new()
            {
                SkipExistsChecks = true
            };
        }

        public bool Add(Actor actor)
        {
            if (ExistsAt(actor.Position)) return false;
            _actors[actor.Position] = actor;

            actor.PositionChanged += UpdateActorPositionWithinManager;
            EntityComponent.Add(actor);

            return true;
        }

        public bool Remove(Actor actor)
        {
            if (!ExistsAt(actor.Position)) return false;
            _actors.Remove(actor.Position);

            actor.PositionChanged -= UpdateActorPositionWithinManager;
            EntityComponent.Remove(actor);

            return true;
        }

        public Actor Get(Point point)
        {
            if (_actors.TryGetValue(point, out Actor actor))
                return actor;
            return null;
        }

        public bool ExistsAt(Point point)
        {
            return _actors.ContainsKey(point);
        }

        public void Clear()
        {
            foreach (var actor in _actors.Values)
            {
                _ = Remove(actor);
            }
        }

        private void UpdateActorPositionWithinManager(object sender, ValueChangedEventArgs<Point> e)
        {
            if (e.OldValue == e.NewValue) return;
            var actor = (Actor)sender;

            // Remove from previous
            _actors.Remove(e.OldValue);

            // Check if the new position is occupied
            if (ExistsAt(e.NewValue))
            {
                throw new Exception($"Cannot move actor to {e.NewValue} another actor already exists there.");
            }

            _actors.Add(e.NewValue, actor);
        }

        // Renders the NPCs only if they are within FOV.
        public void UpdateVisibility(IFOV fieldOfView = null)
        {
            var fov = fieldOfView ?? ScreenContainer.Instance.World.Player.FieldOfView;
            foreach (var actor in _actors)
            {
                actor.Value.IsVisible = fov.BooleanResultView[actor.Key];
            }
        }

    }
}
