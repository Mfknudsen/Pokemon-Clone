#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public class OperationManager : Manager
    {
        #region Values

        public static OperationManager instance;
        private bool done;

        private readonly Queue<OperationsContainer> operationsContainers = new();
        private OperationsContainer currentContainer;

        private readonly Dictionary<OperationsContainer, List<Coroutine>> activeAsyncCoroutines = new();

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

        public void AddOperationsContainer(IEnumerable<OperationsContainer> sets)
        {
            this.done = false;

            foreach (OperationsContainer container in sets)
                this.operationsContainers.Enqueue(container);
        }

        public void AddAsyncOperationsContainer(OperationsContainer container)
        {
            this.activeAsyncCoroutines.Add(container, new List<Coroutine>());

            foreach (IOperation i in container.GetInterfaces())
                this.activeAsyncCoroutines[container].Add(StartCoroutine(i.Operation()));
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

        public void StopAsyncContainer(OperationsContainer toStop, bool triggerEnd = false)
        {
            foreach (Coroutine coroutine in this.activeAsyncCoroutines[toStop]) StopCoroutine(coroutine);

            if (!triggerEnd) return;

            foreach (IOperation operation in toStop.GetInterfaces()) operation.End();
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
            this.operationInterfaces.Add(set);
        }

        public OperationsContainer(IOperation[] set)
        {
            this.operationInterfaces.AddRange(set);
        }

        public void Add(IOperation operationInterface)
        {
            if (operationInterface == null)
                return;

            this.operationInterfaces.Add(operationInterface);
        }

        public void Add(IOperation[] interfaces)
        {
            if (interfaces == null)
                return;

            foreach (IOperation i in interfaces) this.operationInterfaces.Add(i);
        }

        public IEnumerable<IOperation> GetInterfaces() => this.operationInterfaces.ToArray();
    }

    public interface IOperation
    {
        public bool Done();

        public IEnumerator Operation();

        public void End();
    }
}