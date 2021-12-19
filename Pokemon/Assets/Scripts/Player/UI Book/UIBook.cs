#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Player.Camera;
using Mfknudsen.UI;
using Mfknudsen.UI.Book;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen.Player.UI_Book
{
    #region Enums

    public enum BookTurn
    {
        Null,
        Open,
        Close,
        Left,
        Right
    }

    #endregion

    public class UIBook : MonoBehaviour
    {
        #region Values

        public static UIBook instance;

        [SerializeField] private GameObject invisiblyUI, visuals;

        [SerializeField] private Animator bookAnimator;

        [FoldoutGroup("Render Textures")] [SerializeField]
        private RenderTexture preRenderTexture, curRenderTexture;

        [SerializeField] private CinemachineVirtualCameraBase bookCameraRig;
        [SerializeField] private UIBookCameraTransition transition;

        [FoldoutGroup("Pages")] [SerializeField]
        private GameObject openLeft, openRight, turnLeft, turnRight;

        private readonly Dictionary<string, BookButton> buttonReferences = new Dictionary<string, BookButton>();
        private readonly Dictionary<string, Slider> sliderReferences = new Dictionary<string, Slider>();
        private readonly int preRenderTextureID = Shader.PropertyToID("RenderTexture");

        #endregion

        #region Build In States

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            invisiblyUI.transform.localScale /= 10000;

            turnRight.SetActive(false);
            turnLeft.SetActive(false);

            instance = this;

            Invoke(nameof(ConstructUI), 0.1f);
        }

        #endregion

        #region Getters

        public GameObject GetVisuals()
        {
            return visuals;
        }

        #endregion

        #region In

        public void UpdateBook()
        {
            ConstructUI();
        }

        public void Effect(BookTurn turn)
        {
            if (turn != BookTurn.Null)
            {
                invisiblyUI.SetActive(false);
                StartCoroutine(AnimationTrigger(turn.ToString(), 0.5f));

                CopyTextures();

                if (turn == BookTurn.Close)
                {
                    openLeft.GetComponent<Renderer>().material.SetTexture(preRenderTextureID, preRenderTexture);
                    openRight.GetComponent<Renderer>().material.SetTexture(preRenderTextureID, preRenderTexture);
                }
            }

            OperationsContainer container = new OperationsContainer();
            switch (turn)
            {
                case BookTurn.Open:
                    container.Add(new OpenBook(transition, bookCameraRig));
                    PlayerManager.instance.GetController().TriggerAnimator("OpenBook");
                    break;

                case BookTurn.Close:
                    container.Add(new CloseBook(transition, bookCameraRig));
                    PlayerManager.instance.GetController().TriggerAnimator("CloseBook");
                    break;

                case BookTurn.Left:
                    container.Add(new TurnPage(false, bookAnimator, turnLeft, turnRight, openLeft, openRight));
                    break;

                case BookTurn.Right:
                    container.Add(new TurnPage(true, bookAnimator, turnLeft, turnRight, openLeft, openRight));
                    break;

                case BookTurn.Null:
                default:
                    return;
            }

            OperationManager.instance.AddOperationsContainer(container);
        }

        public void CopyTextures()
        {
            Graphics.CopyTexture(curRenderTexture, preRenderTexture);
        }

        public void SetCanvasActive(bool set)
        {
            invisiblyUI.SetActive(set);
        }

        #endregion

        #region Internal

        private List<GameObject> GetAllByRoot(GameObject obj)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (Transform t in obj.transform)
            {
                result.Add(t.gameObject);
                result.AddRange(GetAllByRoot(t.gameObject));
            }

            return result;
        }

        private void DestroyComponent(GameObject obj, Type t)
        {
            try
            {
                Destroy(obj.GetComponent(t));
            }
            catch
            {
                //Ignore
            }
        }

        private IEnumerator AnimationTrigger(string triggerName, float time)
        {
            yield return new WaitForSeconds(time);

            bookAnimator.SetTrigger(triggerName);
        }

        private void ConstructUI()
        {
            SetCanvasActive(false);

            GameObject uiCanvas = GameObject.Find("UI Canvas");

            foreach (Transform t in invisiblyUI.transform)
                Destroy(t.gameObject);

            foreach (Transform t in uiCanvas.transform)
            {
                if (!t.gameObject.activeSelf || t.gameObject.name.Equals("Transition UI") ||
                    t.name.Equals("Template")) continue;

                GameObject obj = Instantiate(t.gameObject, invisiblyUI.transform);

                buttonReferences.Clear();
                sliderReferences.Clear();
                foreach (GameObject o in GetAllByRoot(t.gameObject))
                {
                    BookButton button = o.GetComponent<BookButton>();
                    if (button != null)
                        buttonReferences.Add(o.name, button);

                    Slider slider = o.GetComponent<Slider>();
                    if (slider != null)
                        sliderReferences.Add(o.name, slider);
                }

                //Clean the copied ui
                foreach (GameObject o in GetAllByRoot(obj))
                {
                    if (!o.activeSelf)
                    {
                        Destroy(o);
                        continue;
                    }

                    if (buttonReferences.ContainsKey(o.name))
                    {
                        BookButton button = buttonReferences[o.name];
                        if (button != null)
                        {
                            Destroy(o.GetComponent<BookButton>());
                            StartCoroutine(AddButtonReferenceComponent(o, button));
                        }
                    }

                    if (sliderReferences.ContainsKey(o.name))
                    {
                        Slider slider = sliderReferences[o.name];
                        if (slider != null)
                        {
                            Destroy(o.GetComponent<Slider>());
                        }
                    }

                    DestroyComponent(o, typeof(Outline));
                    DestroyComponent(o, typeof(TextMeshProUGUI));
                }

                break;
            }

            SetCanvasActive(true);
        }

        private IEnumerator AddButtonReferenceComponent(GameObject obj, BookButton button)
        {
            yield return null;
            obj.AddComponent<BookButtonReference>().Setup(button);
        }

        #endregion
    }

    internal class TurnPage : IOperation
    {
        private readonly bool fromLeftToRight;
        private bool done;
        private readonly Animator bookAnimator;
        private readonly GameObject turnLeft, turnRight, openLeft, openRight;

        private readonly int leftToRight = Animator.StringToHash("TurnLeftToRight"),
            rightToLeft = Animator.StringToHash("TurnRightToLeft");

        private readonly int InvertPage = Shader.PropertyToID("InvertPage");

        public TurnPage(bool fromLeftToRight, Animator bookAnimator, GameObject turnLeft, GameObject turnRight,
            GameObject openLeft,
            GameObject openRight)
        {
            this.fromLeftToRight = fromLeftToRight;
            this.bookAnimator = bookAnimator;
            this.turnLeft = turnLeft;
            this.turnRight = turnRight;
            this.openLeft = openLeft;
            this.openRight = openRight;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            UIBook book = UIBook.instance;
            book.CopyTextures();

            UIManager.instance.SwitchUI(UISelection.Pause);

            openRight.SetActive(false);
            openLeft.SetActive(false);

            turnLeft.SetActive(true);
            turnRight.SetActive(true);

            turnLeft.GetComponent<Renderer>().material.SetInt(InvertPage, fromLeftToRight ? 0 : 1);
            turnRight.GetComponent<Renderer>().material.SetInt(InvertPage, fromLeftToRight ? 1 : 0);

            bookAnimator.SetTrigger(fromLeftToRight ? leftToRight : rightToLeft);

            float animationTime = 0.5f;


            yield return new WaitForSeconds(animationTime * 0.1f);

            if (!fromLeftToRight)
                openRight.SetActive(true);
            else
                openLeft.SetActive(true);

            yield return new WaitForSeconds(animationTime * 0.9f);

            if (fromLeftToRight)
                turnRight.SetActive(false);
            else
                turnLeft.SetActive(false);

            turnLeft.SetActive(false);
            turnRight.SetActive(false);

            openLeft.SetActive(true);
            openRight.SetActive(true);

            book.UpdateBook();
        }

        public void End()
        {
        }
    }

    internal class CloseBook : IOperation
    {
        private bool done;
        private readonly CinemachineVirtualCameraBase bookRig;
        private readonly UIBookCameraTransition transition;

        public CloseBook(UIBookCameraTransition transition, CinemachineVirtualCameraBase bookRig)
        {
            this.transition = transition;
            this.bookRig = bookRig;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            OperationsContainer container = new OperationsContainer();
            transition.InvertDirection(true, true);
            container.Add(transition);
            OperationManager.instance.AddAsyncOperationsContainer(container);

            while (transition.GetTime() < 0.75f)
                yield return null;

            CameraManager.instance.GetDefaultRig().enabled = true;

            while (!transition.Done())
                yield return null;

            bookRig.enabled = false;

            done = true;
        }

        public void End()
        {
            UIBook.instance.GetVisuals().SetActive(false);
        }
    }

    internal class OpenBook : IOperation
    {
        private bool done;
        private readonly CinemachineVirtualCameraBase bookRig;
        private readonly UIBookCameraTransition transition;

        public OpenBook(UIBookCameraTransition transition, CinemachineVirtualCameraBase bookRig)
        {
            this.transition = transition;
            this.bookRig = bookRig;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            OperationsContainer container = new OperationsContainer();
            transition.InvertDirection(false, true);
            container.Add(transition);
            OperationManager.instance.AddAsyncOperationsContainer(container);

            yield break;
        }

        public void End()
        {
            throw new NotImplementedException();
        }
    }
}