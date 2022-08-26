using System.Collections;
using Mfknudsen.Communication;

namespace Mfknudsen.Battle.Systems.Static_Operations
{
    public class ChatOperation : IOperation
    {
        private bool done;
        private readonly Chat[] toPlay;
        private readonly ChatManager chatManager;

        public ChatOperation(Chat[] toPlay)
        {
            this.toPlay = toPlay;
            chatManager = ChatManager.instance;
        }

        public ChatOperation(Chat toPlay)
        {
            this.toPlay = new[] { toPlay };
            chatManager = ChatManager.instance;
        }

        public bool Done()
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

        public void End()
        {
        }
    }
}