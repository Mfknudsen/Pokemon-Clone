#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class OperationManager : MonoBehaviour
    {
        #region Values

        public static OperationManager instance;
        private bool done;
        
        private Queue<OperationsContainer> operationsContainers;

        #endregion

        private void Start()
        {
            if (instance == null)
            {
                instance = this;

                operationsContainers = new Queue<OperationsContainer>();
                
                StartCoroutine(QueueManager());
            }
            else
                Destroy(gameObject);
        }

        #region Getters

        public bool GetDone()
        {
            return done;
        }

        #endregion

        #region In

        public void AddOperationsContainer(OperationsContainer set)
        {
            done = false;

            operationsContainers.Enqueue(set);
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

        #region Internal

        private IEnumerator QueueManager()
        {
            while (true)
            {
                while (operationsContainers.Count > 0)
                {
                    done = false;
                    OperationsContainer container = operationsContainers.Dequeue();

                    IEnumerable<IEnumerator> operations = container.GetOperations();
                    IEnumerable<IOperation> interfaces = container.GetInterfaces();

                    foreach (IEnumerator i in operations)
                        StartCoroutine(i);

                    foreach (IOperation i in interfaces)
                    {
                        while (!i.Done())
                            yield return null;

                        i.End();
                    }
                }

                if (operationsContainers.Count == 0 && !done)
                {
                    done = true;
                    yield return new WaitForSeconds(1);
                }

                yield return null;
            }

            //This should newer end while the battle is ongoing.
            //When the battle is over the battle manager will end it.
            // ReSharper disable once IteratorNeverReturns
        }

        #endregion
    }

    public struct OperationsContainer
    {
        private List<IEnumerator> operations;
        private List<IOperation> operationInterfaces;

        public void Add(IOperation operationInterface)
        {
            if (operationInterface == null)
                return;

            operations ??= new List<IEnumerator>();
            operationInterfaces ??= new List<IOperation>();

            operations.Add(operationInterface.Operation());
            operationInterfaces.Add(operationInterface);
        }

        public void Add(IOperation[] interfaces)
        {
            if (interfaces == null)
                return;

            operations ??= new List<IEnumerator>();
            operationInterfaces ??= new List<IOperation>();

            foreach (IOperation i in interfaces)
            {
                operations.Add(i.Operation());
                operationInterfaces.Add(i);
            }
        }

        public IEnumerable<IEnumerator> GetOperations()
        {
            return operations.ToArray();
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