using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    [Serializable]
    public readonly struct BlackboardKey: IEquatable<BlackboardKey>
    {
        readonly string name;
        readonly int hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey=name.ComputeFNV1aHash();
        }
        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;

        public override bool Equals(object obj)=> obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;

       // public static bool operator ==(BlackboardKey lhs , BlackboardKey rhs)=> lhs.hashedKey==rhs.hashedKey;

    }
    [Serializable]
    class Blackboard
    {
        Dictionary<string, object> entries = new();
    }

}
