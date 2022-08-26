#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Settings.Managers;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class OperationManager : Manager
    {
        #region Values

        public static OperationManager instance;
        private bool done;

        private readonly Queue<OperationsContainer> operationsContainers = new();
        private OperationsContainer currentContainer;

        #endregion

        #region Build In States

        private void Update()
        {
            if (this.currentContainer == null)
            {
                if (this.operationsContainers.Count == 0) return;

                this.currentContainer = this.operationsContainers.Dequeue();

                foreach (IOperation i in this.currentContainer.GetInterfaces())
                    StartCoroutine(i.Operation());
            }
            else
            {
                this.done = true;

                foreach (IOperation i in this.currentContainer.GetInterfaces())
                {
                    if (i.Done()) continue;

                    this.done = false;
                }

                if (!this.done) return;

                foreach (IOperation i in this.currentContainer.GetInterfaces())
                    i.End();

                this.currentContainer = null;
            }
        }

        #endregion

        #region Getters

        public bool GetDone()
        {
            return this.done;
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            yield break;
        }

        public void AddOperationsContainer(OperationsContainer set)
        {
            this.done = false;

            this.operationsContainers.Enqueue(set);
        }

        public void AddOperationsContainer(OperationsContainer[] sets)
        {
            this.done = false;

            foreach (OperationsContainer container in sets)
                this.operationsContainers.Enqueue(container);
        }

        public void AddAsyncOperationsContainer(OperationsContainer container)
        {
            foreach (IOperation i in container.GetInterfaces())
                StartCoroutine(i.Operation());
        }

        public void InsertFront(OperationsContainer set)
        {
            this.done = false;

            List<OperationsContainer> holder = new();
            while (this.operationsContainers.Count > 0)
                holder.Add(this.operationsContainers.Dequeue());

            this.operationsContainers.Enqueue(set);
            foreach (OperationsContainer operationsContainer in holder)
                this.operationsContainers.Enqueue(operationsContainer);
        }

        #endregion
    }

    public class OperationsContainer
    {
        private readonly List<IOperation> operationInterfaces = new();

        public OperationsContainer()
        {
        }

        public OperationsContainer(IOperation set)
        {
            operationInterfaces.Add(set);
        }

        public OperationsContainer(IOperation[] set)
        {
            operationInterfaces.AddRange(set);
        }

        public void Add(IOperation operationInterface)
        {
            if (operationInterface == null)
                return;

            operationInterfaces.Add(operationInterface);
        }

        public void Add(IOperation[] interfaces)
        {
            if (interfaces == null)
                return;

            foreach (IOperation i in interfaces)
            {
                operationInterfaces.Add(i);
            }
        }

        public IEnumerable<IOperation> GetInterfaces()
        {
            return operationInterfaces.ToArray();
        }
    }

    public interface IOperation
    {
        public bool Done();

        public IEnumerator Operation();

        public void End();
    }
}