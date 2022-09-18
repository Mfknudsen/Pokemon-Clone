#region Packages

using System.Collections;
using Runtime.Communication;
using Runtime.Systems.Operation;

#endregion

namespace Runtime.Battle.Systems.Static_Operations
{
    public class ChatOperation : IOperation
    {
        private bool done;
        private readonly Chat[] toPlay;
        private readonly ChatManager chatManager;

        public ChatOperation(Chat[] toPlay)
        {
            this.toPlay = toPlay;
        }

        public ChatOperation(Chat toPlay)
        {
            this.toPlay = new[] { toPlay };
        }

        public bool IsOperationDone()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            chatManager.Add(toPlay);

            while (!chatManager.GetIsClear())
                yield return null;

            done = true;
        }

        public void OperationEnd()
        {
        }
    }
}