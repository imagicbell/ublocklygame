using System.Collections;
using UnityEngine;

namespace UBlocklyGame.Maze
{
    public class MazeAvatar : MonoBehaviour
    {
        private Transform mTrans;

        private void Awake()
        {
            mTrans = transform;
        }

        public void Init(Vector3 startPos, Vector3 startDir)
        {
            mTrans.position = startPos;
            mTrans.forward = startDir;
        }

        public IEnumerator Move(Vector3 targetPos)
        {
            float distance = Vector3.Distance(targetPos, mTrans.position);
            Vector3 dir = (targetPos - mTrans.position).normalized;
            float speed = distance / (MazeDefine.MOVE_SPEED / MazeDefine.SPEED_FACTOR);
            while (distance > 0)
            {
                float move = Mathf.Min(distance, speed * Time.deltaTime);
                mTrans.Translate(move * dir, Space.World);
                distance -= move;

                yield return null;
            }
        }

        public IEnumerator Turn(float angle)
        {
            float speed = angle / (MazeDefine.TURN_SPEED / MazeDefine.SPEED_FACTOR);
            while (Mathf.Abs(angle) > 0)
            {
                float turn = speed * Time.deltaTime;
                if (Mathf.Abs(turn) > Mathf.Abs(angle))
                    turn = angle;
                
                mTrans.Rotate(Vector3.up, turn, Space.Self);
                angle -= turn;

                yield return null;
            }
        }
    }
}