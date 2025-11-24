using UnityEngine;
using qb.Pattern;
using System.Collections.Generic;
namespace qb.Animation
{
    public class AnimationsManager : MBSingleton<AnimationsManager>
    {
        public override bool IsPersistent => true;
        public override EDuplicatedSingletonInstanceAction DuplicatedInstanceAction => EDuplicatedSingletonInstanceAction.DestroyInstance;

        static HashSet<IUpdatable> timeScaled = new HashSet<IUpdatable>();
        static HashSet<IUpdatable> timeUnscaled = new HashSet<IUpdatable>();

        static HashSet<IUpdatable> timeScaledToRemove = new HashSet<IUpdatable>();
        static HashSet<IUpdatable> timeUnscaledToRemove = new HashSet<IUpdatable>();

        static HashSet<IUpdatable> timeScaledToAdd = new HashSet<IUpdatable>();
        static HashSet<IUpdatable> timeUnscaledToAdd = new HashSet<IUpdatable>();

        static object locker = new object();
        public static void Register(IUpdatable updatable)
        {
            lock (locker)
            {
                if (updatable.UseTimeScale)
                {
                    if (!timeScaledToAdd.Contains(updatable))
                        timeScaledToAdd.Add(updatable);
#if !NO_DEBUG_LOG_WARNING
                    else
                    {
                        Debug.LogWarning("[AnimationManager.Register] The updatable is already registered!");
                    }
#endif
                }
                else
                {
                    if (!timeUnscaledToAdd.Contains(updatable))
                        timeUnscaledToAdd.Add(updatable);
#if !NO_DEBUG_LOG_WARNING
                    else
                    {
                        Debug.LogWarning("[AnimationManager.Register] The updatable is already registered!");
                    }
#endif
                }
            }
        }
        public static void Unregister(IUpdatable updatable)
        {
            lock (locker)
            {
                if (updatable.UseTimeScale)
                {
                    if (!timeScaledToRemove.Contains(updatable))
                        timeScaledToRemove.Add(updatable);
                }
                else
                {
                    if (!timeUnscaledToRemove.Contains(updatable))
                        timeUnscaledToRemove.Add(updatable);
                }
            }
        }

        void Update()
        {
            lock (locker)
            {
                #region add animations
                foreach (var item in timeScaledToAdd)
                {
                    if (!timeScaled.Contains(item))
                        timeScaled.Add(item);
                }
                timeScaledToAdd.Clear();
                foreach (var item in timeUnscaledToAdd)
                {
                    if (!timeUnscaled.Contains(item))
                        timeUnscaled.Remove(item);
                }
                timeUnscaledToAdd.Clear();
                #endregion

                #region remove animations
                foreach (var item in timeScaledToRemove)
                {
                    if (timeScaled.Contains(item))
                        timeScaled.Remove(item);
                }
                timeScaledToRemove.Clear();
                foreach (var item in timeUnscaledToRemove)
                {
                    if (timeUnscaled.Contains(item))
                        timeUnscaled.Remove(item);
                }
                timeUnscaledToRemove.Clear();
                #endregion

                #region update animations
                foreach (var item in timeScaled)
                {
                    item.Update(Time.deltaTime);
                }
                foreach (var item in timeUnscaled)
                {
                    item.Update(Time.unscaledDeltaTime);
                }
                #endregion
            }
        }
    }
}
