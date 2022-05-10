#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Player.Camera;
using Mfknudsen.UI;
using Mfknudsen.UI.Book.Button;
using Mfknudsen.UI.Book.Interfaces;
using Mfknudsen.UI.Book.Light;
using Mfknudsen.UI.Book.Slider;
using Mfknudsen.UI.Book.TextInputField;
using Sirenix.OdinInspector;
using TMPro;
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

    public enum BookAnimations
    {
        Close,
        Open
    }

    #endregion

    public class UIBook : MonoBehaviour
    {
        #region Values

        public static UIBook instance;

        [BoxGroup("References"), FoldoutGroup("References/Render Textures")] [SerializeField]
        private RenderTexture preRenderTexture, curRenderTexture;

        [FoldoutGroup("References/Camera")] [SerializeField]
        private CinemachineVirtualCameraBase bookCameraRig;

        [FoldoutGroup("References/Camera")] [SerializeField]
        private UIBookCameraTransition transition;

        [FoldoutGroup("References/Pages")] [SerializeField]
        private GameObject openLeft, openRight, turnLeft, turnRight;

        [FoldoutGroup("References/Animation")] [SerializeField]
        private Animator bookAnimator;

        [FoldoutGroup("References/Animation")] [SerializeField]
        private GameObject invisiblyUI, visuals;

        [BoxGroup("References")] [SerializeField]
        private UIBookLight bookLight;

        private readonly Dictionary<string, BookButton> buttonReferences = new();
        private readonly Dictionary<string, BookSlider> sliderReferences = new();

        private readonly Dictionary<string, BookTextInputField> textInputFieldReferences =
            new();

        private readonly int preRenderTextureID = Shader.PropertyToID("RenderTexture");

        #region Hash

        private static readonly int HashCloseBook = Animator.StringToHash("CloseBook"),
            HashOpenBook = Animator.StringToHash("OpenBook"),
            HashTurnLeft = Animator.StringToHash("TurnLeftToRight"),
            HashTurnRight = Animator.StringToHash("TurnRightToLeft");

        #endregion

        #endregion

        #region Build In States

        private void Start()
        {
            if (instance != null)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            invisiblyUI.transform.localScale /= 10000;

            turnRight.SetActive(false);
            turnLeft.SetActive(false);

            bookLight.Calculate();

            instance = this;
            transition.CheckMiddle();
            StartCoroutine(DelayedStart());
        }

        #endregion

        #region Getters

        public GameObject GetVisuals()
        {
            return visuals;
        }

        #endregion

        #region In

        public void ConstructUI()
        {
            CopyTextures();
            StartCoroutine(ConstructAsync());
        }

        public void Effect(BookTurn turn)
        {
            if (turn == BookTurn.Null) return;

            invisiblyUI.SetActive(false);
            StartCoroutine(AnimationTrigger(turn, 0.5f));

            CopyTextures();

            if (turn == BookTurn.Close)
            {
                openLeft.GetComponent<Renderer>().material.SetTexture(preRenderTextureID, preRenderTexture);
                openRight.GetComponent<Renderer>().material.SetTexture(preRenderTextureID, preRenderTexture);
            }

            OperationsContainer container = new();
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (turn)
            {
                case BookTurn.Open:
                    container.Add(new OpenBook(transition, bookCameraRig, bookLight));
                    break;

                case BookTurn.Close:
                    container.Add(new CloseBook(transition, bookLight));
                    break;

                case BookTurn.Left:
                    container.Add(new TurnPage(false, turnLeft, turnRight, openLeft, openRight));
                    break;

                case BookTurn.Right:
                    container.Add(new TurnPage(true, turnLeft, turnRight, openLeft, openRight));
                    break;
            }

            OperationManager.instance.AddOperationsContainer(container);
        }

        private void CopyTextures()
        {
            Graphics.CopyTexture(curRenderTexture, preRenderTexture);
        }

        #endregion

        #region Internal

        private IEnumerator DelayedStart()
        {
            yield return new WaitWhile(() => CameraManager.instance == null);

            CameraManager.instance.SetCurrentRig(bookCameraRig, true);
            Cursor.visible = true;

            yield return new WaitWhile(() => PlayerManager.instance == null);

            PlayerManager.instance.DisablePlayerControl();

            ConstructUI();
        }

        private static List<GameObject> GetAllByRoot(GameObject obj)
        {
            List<GameObject> result = new();

            foreach (Transform t in obj.transform)
            {
                result.Add(t.gameObject);
                result.AddRange(GetAllByRoot(t.gameObject));
            }

            return result;
        }

        private static void DestroyComponent<TComponent>(GameObject obj)
            where TComponent : MonoBehaviour
        {
            try
            {
                Destroy(obj.GetComponent<TComponent>());
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        private IEnumerator AnimationTrigger(BookTurn trigger, float time)
        {
            if (trigger == BookTurn.Null) yield break;

            yield return new WaitForSeconds(time);

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            int hash = trigger switch
            {
                BookTurn.Close => HashCloseBook,
                BookTurn.Open => HashOpenBook,
                BookTurn.Left => HashTurnLeft,
                BookTurn.Right => HashTurnRight,
                _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null)
            };

            if (trigger is BookTurn.Close or BookTurn.Open)
                PlayerManager.instance.GetController().TriggerAnimator(hash);

            bookAnimator.SetTrigger(hash);
        }

        private static void AddToReference<TElement>(GameObject obj, IDictionary<string, TElement> dictionary)
            where TElement : MonoBehaviour, ICustomGUIElement
        {
            TElement element = obj.GetComponent<TElement>();

            if (element != null)
                dictionary.Add(obj.name, element);
        }

        private void Replace<TReference, TElement>(GameObject obj, IReadOnlyDictionary<string, TReference> list)
            where TReference : MonoBehaviour, ICustomGUIElement
            where TElement : MonoBehaviour, ICustomGUIElementReference
        {
            string n = obj.name;

            if (!list.ContainsKey(n)) return;

            TReference element = list[n];

            if (element == null) return;

            Destroy(obj.GetComponent<TReference>());
            StartCoroutine(AddGUIReferenceComponent<TElement>(obj, element));
        }

        private static IEnumerator AddGUIReferenceComponent<T>(GameObject obj, ICustomGUIElement element)
            where T : MonoBehaviour, ICustomGUIElementReference
        {
            yield return null;

            if (obj == null || element == null) yield break;
            obj.AddComponent<T>().Setup(element);
        }

        private IEnumerator ConstructAsync()
        {
            invisiblyUI.SetActive(false);

            GameObject uiCanvas = GameObject.Find("Book UI Canvas");

            while (uiCanvas == null)
            {
                yield return new WaitForSeconds(0.1f);
                uiCanvas = GameObject.Find("UI Canvas");
            }

            foreach (Transform t in invisiblyUI.transform)
                Destroy(t.gameObject);

            foreach (Transform t in uiCanvas.transform)
            {
                GameObject transObj = t.gameObject;
                string objName = transObj.name;

                if (!transObj.activeSelf ||
                    objName.Equals("Transition UI") ||
                    objName.Equals("Template"))
                    continue;

                GameObject obj = Instantiate(t.gameObject, invisiblyUI.transform);

                buttonReferences.Clear();
                sliderReferences.Clear();
                textInputFieldReferences.Clear();
                foreach (GameObject o in GetAllByRoot(t.gameObject))
                {
                    AddToReference(o, buttonReferences);
                    AddToReference(o, sliderReferences);
                    AddToReference(o, textInputFieldReferences);
                }

                //Clean the copied ui
                foreach (GameObject o in GetAllByRoot(obj))
                {
                    if (!o.activeSelf)
                    {
                        Destroy(o);
                        continue;
                    }

                    Replace<BookButton, BookButtonReference>(o, buttonReferences);
                    Replace<BookSlider, BookSliderReference>(o, sliderReferences);
                    Replace<BookTextInputField, BookTextInputFieldReference>(o, textInputFieldReferences);

                    DestroyComponent<Outline>(o);
                    DestroyComponent<TextMeshProUGUI>(o);
                }

                break;
            }

            invisiblyUI.SetActive(true);
        }

        #endregion
    }

    internal class TurnPage : IOperation
    {
        private readonly bool fromLeftToRight;
        private bool done;
        private readonly GameObject turnLeft, turnRight, openLeft, openRight;

        private readonly int invertPage = Shader.PropertyToID("InvertPage");

        public TurnPage(bool fromLeftToRight, GameObject turnLeft, GameObject turnRight,
            GameObject openLeft,
            GameObject openRight)
        {
            this.fromLeftToRight = fromLeftToRight;
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
            done = false;
            UIBook book = UIBook.instance;

            SetOpens(false);
            SetTurns(true);

            turnLeft.GetComponent<Renderer>().material.SetInt(invertPage, fromLeftToRight ? 0 : 1);
            turnRight.GetComponent<Renderer>().material.SetInt(invertPage, fromLeftToRight ? 1 : 0);

            const float animationTime = 0.5f;

            yield return new WaitForSeconds(animationTime * 0.1f);

            if (!fromLeftToRight)
                openRight.SetActive(true);
            else
                openLeft.SetActive(true);

            yield return new WaitForSeconds(animationTime * 0.9f);

            //SetTurns(false);
            SetOpens(true);

            book.ConstructUI();
            done = true;
        }

        public void End()
        {
        }

        #region Internal

        private void SetOpens(bool set)
        {
            openLeft.SetActive(set);
            openRight.SetActive(set);
        }

        private void SetTurns(bool set)
        {
            turnLeft.SetActive(set);
            turnRight.SetActive(set);
        }

        #endregion
    }

    internal class CloseBook : IOperation
    {
        private bool done;
        private readonly UIBookCameraTransition transition;
        private readonly UIBookLight bookLight;

        public CloseBook(UIBookCameraTransition transition, UIBookLight bookLight)
        {
            this.transition = transition;
            this.bookLight = bookLight;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            bookLight.TurnOff();

            OperationsContainer container = new();
            transition.Direction(true, true);
            container.Add(transition);

            CameraEvent cameraEvent = CameraEvent.ReturnToDefaultOverworld();
            container.Add(cameraEvent);
            OperationManager.instance.AddAsyncOperationsContainer(container);

            yield return new WaitUntil(transition.Done);

            UIManager.instance.SwitchUI(UISelection.Overworld);

            done = true;
        }

        public void End()
        {
            UIBook.instance.GetVisuals().SetActive(false);
            UIManager.instance.SetReadyToPause(true);
            PlayerManager.instance.EnablePlayerControl();
        }
    }

    internal class OpenBook : IOperation
    {
        private bool done;
        private readonly CinemachineVirtualCameraBase bookRig;
        private readonly UIBookCameraTransition transition;
        private readonly UIBookLight bookLight;

        public OpenBook(UIBookCameraTransition transition, CinemachineVirtualCameraBase bookRig, UIBookLight bookLight)
        {
            this.bookLight = bookLight;
            this.transition = transition;
            this.bookRig = bookRig;

            Transform book = UIBook.instance.transform, player = PlayerManager.instance.GetController().transform;
            book.position = player.position;
            book.rotation = player.GetChild(0).rotation;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            PlayerManager.instance.DisablePlayerControl();

            OperationsContainer container = new OperationsContainer();
            transition.CheckMiddle();
            transition.Direction(false, true);
            container.Add(transition);

            CameraEvent cameraEvent = new CameraEvent(
                bookRig,
                CameraSettings.Default(),
                transition.GetTimeToComplete(),
                0.75f
            );
            container.Add(cameraEvent);

            OperationManager.instance.AddAsyncOperationsContainer(container);

            UIBook book = UIBook.instance;
            book.GetVisuals().SetActive(true);

            yield return new WaitUntil(transition.Done);

            book.ConstructUI();
            done = true;
        }

        public void End()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            bookLight.Calculate();
        }
    }
}