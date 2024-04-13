using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Thubg.Messages
{
    public record CompetitionUpdate : Message
    {
        public override string MessageType { get;} = "COMPETITION_UPDATE";
        public Info info { get; }
        public List<Player> players { get; }
        public List<Event> events { get; }
        public record Info
        {
            public int elapsedTime { get; }
            public string stage{ get; }
        }

        public record Player
        {
            public string playerId { get; }
            public string armor { get; }
            public int health { get; }
            public float speed { get; }
            public Firearm firearm { get; }
            public List<Inventory> inventory { get; }
            public Position position { get; }
            public record Firearm
            {
                public string name { get; }
                public float windup { get; }
                public int distance { get; }
            }
            public record Inventory
            {
                public string name { get; }
                public int num { get; }
            }
        }
        public record Event
        {
            public virtual string eventType { get; }
        }

        public record PlayerAttackEvent : Event
        {
            public override string eventType { get; } = "PLAYER_ATTACK";
            public int playerId { get; }
            public Position targetPosition { get; }
        }
        public record PlayerSwitchArmEvent : Event
        {
            public override string eventType { get; } = "PLAYER_SWITCH_ARM";
            public int playerId { get; }
            public string targetFirearm { get; }
        }
        public record PlayerPickUpEvent : Event
        {
            public override string eventType { get; } = "PLAYER_PICKUP";
            public int playerId { get; }
            public string targetSupply { get; }
            public Position targetPosition { get; }
        }
        public record PlayerUseMedicineEvent : Event
        {
            public override string eventType { get; } = "PLAYER_USE_MEDICINE";
            public int playerId { get; }
            public string medicine { get; }
        }
        public record PlayerUseGrenadeEvent : Event
        {
            public override string eventType { get; } = "PLAYER_USE_GRENADE";
            public int playerId { get; }
            public Position targetPosition { get; }
        }
        public record PlayerAbandonEvent : Event
        {
            public override string eventType { get; } = "PLAYER_ABANDON";
            public int playerId { get; }
            public List<string> targetPosition { get; }
        }
    }
}