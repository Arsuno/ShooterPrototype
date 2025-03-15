using UnityEngine;

namespace _Project.Source.Data
{
    public class Item : ScriptableObject
    {
        public Sprite Icon;
        public int StartAmount;
        public int AddAmount;
        public int StackAmount;
    }
}
