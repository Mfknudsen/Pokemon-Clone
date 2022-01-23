#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Settings.Manager;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class OperationManager : Manager
    {
        #region Values

        public static OperationManager instance;
        private bool done;

        private readonly Queue<OperationsContainer> operationsContainers = new Queue<OperationsContainer>();
        private OperationsContainer currentContainer;

        #endregion

        #region Build In States

        private void Update()
        {
            if (currentContainer == null)
            {
                if (operationsContainers.Count <= 0) return;

                currentContainer = operationsContainers.Dequeue();

                foreach (IOperation i in currentContainer.GetInterfaces())
                    StartCoroutine(i.Operation());
            }
            else
            {
                done = true;

                foreach (IOperation i in currentContainer.GetInterfaces())
                {
                    if (i.Done()) continue;

                    done = false;
                }

                if (!done) return;

                foreach (IOperation i in currentContainer.GetInterfaces())
                    i.End();

                currentContainer = null;
            }
        }

        #endregion

        #region Getters

        public bool GetDone()
        {
            return done;
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
            done = false;

            operationsContainers.Enqueue(set);
        }

        public void AddOperationsContainer(OperationsContainer[] sets)
        {
            done = false;

            foreach (OperationsContainer container in sets)
                operationsContainers.Enqueue(container);
        }

        public void AddAsyncOperationsContainer(OperationsContainer container)
        {
            foreach (IOperation i in container.GetInterfaces())
                StartCoroutine(i.Operation());
        }

        public void InsertFront(OperationsContainer set)
        {
            done = false;

            List<OperationsContainer> holder = new List<OperationsContainer>();
            while (operationsContainers.Count > 0)
                holder.Add(operationsContainers.Dequeue());

            operationsContainers.Enqueue(set);
            foreach (OperationsContainer operationsContainer in holder)
                operationsContainers.Enqueue(operationsContainer);
        }

        #endregion
    }

    public class OperationsContainer
    {
        private readonly List<IOperation> operationInterfaces = new List<IOperation>();

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