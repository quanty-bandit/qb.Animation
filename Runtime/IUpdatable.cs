using UnityEngine;
namespace qb.Animation
{
    public interface IUpdatable
    {
        public void Update(float deltaTime); 
        public bool UseTimeScale { get; }
    }
}
