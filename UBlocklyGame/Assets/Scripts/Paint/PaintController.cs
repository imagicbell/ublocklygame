using System.Collections;
using UToolbox;
using UBlockly;
using UnityEngine;

namespace UBlocklyGame.Paint
{
    public class PaintController : MonoSingleton<PaintController>
    {
        private PaintBehavior mBehavior;

        /// <summary>
        /// angle 0: y direction. clockwise 
        /// </summary>
        private float mCurAngle;

        /// <summary>
        /// current painting position(x, y, 0) 
        /// </summary>
        private Vector3 mCurPos;
        
        private float mMoveCounter;
        private float mTurnCounter;

        private float mMoveSpeed
        {
            get { return PaintDefine.MOVE_SPEED * PaintDefine.SPEED_FACTOR; }
        }

        private float mTurnSpeed
        {
            get { return PaintDefine.TURN_SPEED * PaintDefine.SPEED_FACTOR; }
        }

        private float mJumpDuration
        {
            get { return PaintDefine.JUMP_DURATION / PaintDefine.SPEED_FACTOR; }
        }

        private void Awake()
        {
            mBehavior = Object.FindObjectOfType<PaintBehavior>();
            mBehavior.Init(Screen.width, Screen.height);
            
            //todo: if (mBehavior == null)
            
            Reset();
        }

        public void Reset()
        {
            //todo: configurable
            mCurAngle = 180;
            mCurPos = 0.5f * new Vector2(Screen.width, Screen.height);
            
            mMoveCounter = mMoveSpeed;
            mTurnCounter = mTurnSpeed;
        }

        public IEnumerator DoMove(Direction dir, float distance)
        {
            Vector3 moveDir = Quaternion.Euler(0, 0, mCurAngle) * Vector3.up;
            if (dir == Direction.Back)
                moveDir = -moveDir;

            while (distance > 0)
            {
                float move = Mathf.Min(distance, mMoveCounter);
                Vector3 targetPos = mCurPos + move * moveDir;
                mBehavior.DrawLine(mCurPos, targetPos);

                mCurPos = targetPos;
                distance -= move;

                mMoveCounter -= move;
                if (mMoveCounter <= 0)
                {
                    mMoveCounter = mMoveSpeed;
                    yield return null;
                }
            }
        }

        public IEnumerator DoTurn(float angle)
        {
            while (angle > 0)
            {
                float turn = Mathf.Min(angle, mTurnCounter);
                mCurAngle += turn;
                angle -= turn;

                mTurnCounter -= turn;
                if (mTurnCounter <= 0)
                {
                    mTurnCounter = mTurnSpeed;
                    yield return null;
                }
            }
        }
        
        public IEnumerator DoJump(Direction dir, float distance)
        {
            Vector3 moveDir = Quaternion.Euler(0, 0, mCurAngle) * Vector3.up;
            if (dir == Direction.Back)
                moveDir = -moveDir;

            mCurPos += distance * moveDir;
            yield return new WaitForSeconds(mJumpDuration);
        }

        public void DoChangeColor(Color color)
        {
            mBehavior.PaintColor = color * 255;
        }
    }
}
