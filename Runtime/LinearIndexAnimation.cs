using System;
using UnityEngine;
namespace qb.Animation
{
    /// <summary>
    /// Provides frame-based index animation with configurable delays, looping, and play modes. Supports linear and yoyo
    /// playback, and can be updated over time to advance the animation index.
    /// </summary>
    /// <remarks>
    /// Use this class to animate an integer index over a sequence of frames, where each frame can
    /// have a custom delay. The animation can be played, stopped, rewound, and updated via the Update method. Looping
    /// and playback mode (linear or yoyo) are configurable. The animation can optionally respect a global time scale.
    /// Subscribe to the OnUpdate event to receive notifications when the index changes.
    /// This class must be disposed to
    /// unregister from the animation manager and release resources.
    /// </remarks>
    public class LinearIndexAnimation:IDisposable, IUpdatable
    {
        float[] delays;
        float currentDelay;
        int framesCount;
        int maxIndex;
        int step = 1;
        float time;
        int index;
        int loop;
        public int Index => index;
        public enum EPlayMode
        {
            linear,
            yoyo,
        }
        public EPlayMode playMode;
        public int loopCount = 0;

        float speed = 1;
        public float Speed
        {
            get => speed;
            set
            {
                if (value > 0)
                {
                    speed = value;
                }
            }
        }

        bool isPlaying = false;
        public bool IsPlaying => isPlaying;

        public readonly float duration;

        #region event 
        protected event Action<LinearIndexAnimation,int> onUpdate;
        public event Action<LinearIndexAnimation,int> OnUpdate
        {
            add
            {
                if(onUpdate != null)
                {
                    var invocationList = onUpdate.GetInvocationList();
                    foreach (var invocation in invocationList)
                    {
                        if (invocation == value as Delegate)
                        {
#if !NO_DEBUG_LOG_WARNING
                            Debug.LogWarning($"Duplicate subscription to OnUpdate of LinearIndexAnimation");
#endif
                            return;
                        }
                    }
                }
                onUpdate += value;
            }
            remove
            {
                if (value != null && !(value.Target.Equals(null)))
                {
                    if (onUpdate != null)
                        onUpdate -= value;
                    else
                    {
#if !NO_DEBUG_LOG_WARNING
                        Debug.LogWarning($"Try to unsubcribe from null OnUpdate of LinearIndexAnimation");
#endif
                    }
                }
                else
                {
#if !NO_DEBUG_LOG_WARNING
                    Debug.LogWarning($"Try to unsubcribe null delegate action to OnUpdate of LinearIndexAnimation");
#endif
                    ClearInvalidSubscriptions();
                }
            }
        }
        object cleanUpLocker = new object();
        /// <summary>
        /// Remove all invalid subscriptions from onUpdate Action in case of behaviours deletion
        /// </summary>
        void ClearInvalidSubscriptions()
        {
            if (onUpdate != null)
            {
                lock (cleanUpLocker)
                {
                    var invocationList = onUpdate.GetInvocationList();
                    int validInvocationCount = 0;
                    foreach (var invocation in invocationList)
                    {
                        if (invocation != null && !(invocation.Target.Equals(null)))
                        {
                            validInvocationCount++;
                        }
                    }
                    if (validInvocationCount != invocationList.Length)
                    {
                        onUpdate = null;
                        foreach (var invocation in invocationList)
                        {
                            if (invocation != null && !(invocation.Target.Equals(null)))
                            {
                                onUpdate += invocation as Action<LinearIndexAnimation, int>;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        bool useTimeScale;
        public bool UseTimeScale => useTimeScale;

        public LinearIndexAnimation(float[] delays, bool useTimeScale=true,int loopCount=0,EPlayMode playMode = EPlayMode.linear)
        {
            this.delays = delays;
            this.useTimeScale = useTimeScale;
            this.loopCount = loopCount;
            this.playMode = playMode;
            if (delays == null || delays.Length<2)
            {
                framesCount = 1;
                maxIndex = 0;
                currentDelay = 0.1f;
#if !NO_DEBUG_LOG_WARNING
                Debug.LogWarning("Linear animation has no or only one frame!");
#endif
            }
            else
            {
                framesCount = delays.Length;
                maxIndex = framesCount - 1;
                index = 0;
                currentDelay = delays[index];
                duration = 0;
                foreach (var entry in delays)
                {
                    duration += entry;
                }
                UpdatableManager.Register(this);
            }
        }
        ~LinearIndexAnimation()
        {
            Dispose();
        }
        public void Dispose()
        {
            UpdatableManager.Unregister(this);
        }

        public void Play()
        {
            if (isPlaying) return;
            isPlaying = true;
            time = 0;
        }
        public void Stop()
        {
            isPlaying = false;
        }
        public void Rewind()
        {
            time = 0;
            index = 0;
            currentDelay = delays[index];
            loop = loopCount;
        }
        public void Update(float deltaTime)
        {
            if (isPlaying && framesCount>1)
            {
                time -= deltaTime*speed;
                if (time<=0)
                {
                    index = NextIndex();
                    currentDelay = delays[index];
                    while (time < 0 && isPlaying)
                    {
                        time += currentDelay;
                        if(time < 0)
                        {
                            index = NextIndex();
                            currentDelay = delays[index];
                        }
                    }
                    onUpdate?.Invoke(this,index);
                }
            }
        }
        int NextIndex()
        {
            int i = index + step;
            
            if (step == 1)
            {
                if (i > maxIndex)
                {
                    if (loopCount > 0)
                    {
                        loop--;
                        if (loop <= 0)
                        {
                            Stop();
                            return i;
                        }
                    }
                    if (playMode == EPlayMode.linear)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = maxIndex - 1;
                        step = -1;
                    }
                }
            }
            else
            {
                if (i < 0)
                {
                    if (loopCount > 0)
                    {
                        loop--;
                        if (loop <= 0)
                        {
                            Stop();
                            return i;
                        }
                    }
                    if (playMode == EPlayMode.linear)
                    {
                        i = maxIndex;
                    }
                    else
                    {
                        i = 1;
                        step = 1;
                    }
                }
            }
            return i;
        }
    }
}
