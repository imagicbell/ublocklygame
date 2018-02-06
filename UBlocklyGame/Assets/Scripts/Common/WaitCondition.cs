using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UBlocklyGame
{
    public abstract class WaitCondition : CustomYieldInstruction
    {
        protected WaitCondition() {}

        private float mTimeLimit = -1f;
        private float mTimeElapsed = 0;

        private int mFrameCheckInterval = 10;//每10帧检查一次
        private int mFrameElapsed = 10;

        public sealed override bool keepWaiting
        {
            get
            {
                if (mTimeLimit > 0)
                {
                    if (mTimeElapsed >= mTimeLimit)
                    {
                        mTimeElapsed = 0;
                        Debug.LogError(string.Format("WaitCondition---Operation timed out: {0}\n{1}", this, Environment.StackTrace));
                    }
                    mTimeElapsed += Time.unscaledDeltaTime;    
                }

                if (mFrameElapsed == mFrameCheckInterval)
                {
                    mFrameElapsed = 0;
                    return !Satisfied();
                }
                mFrameElapsed++;
                return true;
            }
        }

        /// <summary>
        /// set the max time limit to wait
        /// </summary>
        public void SetTimeLimit(float timeLimit)
        {
            mTimeLimit = timeLimit;
        }

        /// <summary>
        /// set how many frames to check once
        /// </summary>
        public void SetFrameCheckInterval(int interval)
        {
            mFrameCheckInterval = interval;
            mFrameElapsed = mFrameCheckInterval;
        }

        protected abstract bool Satisfied();
    }
    
    public class WaitObjectAppeared : WaitCondition
    {
        protected string path;
        public GameObject o;

        public WaitObjectAppeared(string path)
        {
            this.path = path;
        }

        protected override bool Satisfied()
        {
            o = GameObject.Find(path);
            return o != null && o.activeInHierarchy;
        }

        public override string ToString()
        {
            return "ObjectAppeared(" + path + ")";
        }
    }
    
    public class WaitObjectAppeared<T> : WaitCondition where T : Component
    {
        public T Obj { get; private set; }

        protected override bool Satisfied()
        {
            Obj = Object.FindObjectOfType<T>();
            return Obj != null && Obj.gameObject.activeInHierarchy;
        }
    }
    
    public class WaitObjectDisappeared : WaitObjectAppeared
    {
        public WaitObjectDisappeared(string path) : base(path) {}
        
        protected override bool Satisfied()
        {
            return !base.Satisfied();
        }

        public override string ToString()
        {
            return "ObjectDisappeared(" + path + ")";
        }
    }
    
    public class WaitObjectDisappeared<T> : WaitObjectAppeared<T> where T : Component
    {
        protected override bool Satisfied()
        {
            return !base.Satisfied();
        }
    }

	public class WaitBoolCondition : WaitCondition
	{
		private Func<bool> _getter;

		public WaitBoolCondition(Func<bool> getter)
		{
			_getter = getter;
		}

		protected override bool Satisfied()
		{
			if (_getter == null) return false;
			return _getter();
		}

		public override string ToString()
		{
			return "BoolCondition(" + _getter + ")";
		}
	}

    public class WaitFrame : CustomYieldInstruction
    {
        private int mFrameCounter = 0;
        
        public WaitFrame(int frameCount)
        {
            mFrameCounter = frameCount;
        }

        public override bool keepWaiting
        {
            get
            {
                mFrameCounter--;
                return mFrameCounter > 0;
            }
        }
    }
}