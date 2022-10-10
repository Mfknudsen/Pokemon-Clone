#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Runtime.Player;
using Runtime.Player.Camera;
using Runtime.Systems;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;
using Runtime.UI.Book.Button;
using Runtime.UI.Book.Interfaces;
using Runtime.UI.Book.Light;
using Runtime.UI.Book.Slider;
using Runtime.UI.Book.TextInputField;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Runtime.UI_Book
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

        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private OperationManager operationManager;
        [SerializeField, Required] private UIManager uiManager;
        [SerializeField, Required] private CameraManager cameraManager;

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

        private static readonly int PreRenderTextureID = Shader.PropertyToID("RenderTexture");

        #region Hash

        private static readonly int HashCloseBook = Animator.StringToHash("CloseBook"),
            HashOpenBook = Animator.StringToHash("OpenBook"),
            HashTurnLeft = Animator.StringToHash("TurnLeftToRight"),
            HashTurnRight = Animator.StringToHash("TurnRightToLeft");

        #endregion

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            DontDestroyOnLoad(this.gameObject);
            this.uiManager.SetUIBook(this);

            this.invisiblyUI.transform.localScale /= 10000;

            this.turnRight.SetActive(false);
            this.turnLeft.SetActive(false);

            this.bookLight.Calculate();
            this.transition.CheckMiddle();

            this.cameraManager.SetCurrentRig(this.bookCameraRig, true);
            Cursor.visible = true;

            yield return new WaitWhile(() => this.playerManager.GetController() == null);

            this.playerManager.DisablePlayerControl();

            this.ConstructUI();
            GameObject bookHolder = new("Book Holder")
            {
                transform = { parent = this.playerManager.GetController().transform }
            };

            this.transform.parent = bookHolder.transform;
        }

        #endregion

        #region Getters

        public GameObject GetVisuals()
        {
            return this.visuals;
        }

        #endregion

        #region In

        public void ConstructUI()
        {
            this.CopyTextures();
            this.StartCoroutine(this.ConstructAsync());
        }

        public void Effect(BookTurn turn)
        {
            if (turn == BookTurn.Null) return;

            this.invisiblyUI.SetActive(false);
            this.StartCoroutine(this.AnimationTrigger(turn, 0.5f));

            this.CopyTextures();

            if (turn == BookTurn.Close)
            {
                this.openLeft.GetComponent<Renderer>().material.SetTexture(PreRenderTextureID, this.preRenderTexture);
                this.openRight.GetComponent<Renderer>().material.SetTexture(PreRenderTextureID, this.preRenderTexture);
            }

            OperationsContainer container = new();
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (turn)
            {
                case BookTurn.Open:
                    container.Add(new OpenBook(this, this.transition, this.bookCameraRig, this.bookLight,
                        this.operationManager, this.playerManager));
                    break;

                case BookTurn.Close:
                    container.Add(new CloseBook(this, this.transition, this.bookLight, this.operationManager,
                        this.uiManager,
                        this.playerManager));
                    break;

                case BookTurn.Left:
                    container.Add(new TurnPage(this, false, this.turnLeft, this.turnRight, this.openLeft,
                        this.openRight));
                    break;

                case BookTurn.Right:
                    container.Add(
                        new TurnPage(this, true, this.turnLeft, this.turnRight, this.openLeft, this.openRight));
                    break;
            }

            this.operationManager.AddOperationsContainer(container);
        }

        private void CopyTextures() => Graphics.CopyTexture(this.curRenderTexture, this.preRenderTexture);

        #endregion

        #region Internal

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

        private IEnumerator AnimationTrigger(BookTurn trigger, float time)
        {
            if (trigger == BookTurn.Null) yield break;

            yield return new WaitForSeconds(time);

            int hash = trigger switch
            {
                BookTurn.Close => HashCloseBook,
                BookTurn.Open => HashOpenBook,
                BookTurn.Left => HashTurnLeft,
                BookTurn.Right => HashTurnRight,
                _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null)
            };

            if (trigger is BookTurn.Close or BookTurn.Open) this.playerManager.GetController().TriggerAnimator(hash);

            this.bookAnimator.SetTrigger(hash);
        }

        private static void AddToReference<TElement>(GameObject obj, IDictionary<string, TElement> dictionary)
            where TElement : MonoBehaviour, ICustomGUIElement
        {
            if (obj.GetComponent<TElement>() is { } element)
                dictionary.Add(obj.name, element);
        }

        private void Replace<TReference, TElement>(GameObject obj, IReadOnlyDictionary<string, TReference> list)
            where TReference : MonoBehaviour, ICustomGUIElement
            where TElement : MonoBehaviour, ICustomGUIElementReference
        {
            string n = obj.name;

            if (!list.ContainsKey(n)) return;

            TReference element = list[n];

            if (element is null) return;

            Destroy(obj.GetComponent<TReference>());
            this.StartCoroutine(AddGUIReferenceComponent<TElement>(obj, this, element));
        }

        private static IEnumerator AddGUIReferenceComponent<T>(GameObject obj, UIBook uiBook, ICustomGUIElement element)
            where T : MonoBehaviour, ICustomGUIElementReference
        {
            yield return null;

            if (obj is null || element is null) yield break;

            obj.AddComponent<T>().Setup(uiBook, element);
        }

        private IEnumerator ConstructAsync()
        {
            Debug.Log("Start");
            this.invisiblyUI.SetActive(false);

            GameObject uiCanvas = GameObject.Find("Book UI Canvas");

            while (uiCanvas is null)
            {
                yield return new WaitForSeconds(0.1f);
                uiCanvas = GameObject.Find("UI Canvas");
            }

            foreach (Transform t in this.invisiblyUI.transform)
                Destroy(t.gameObject);

            foreach (Transform t in uiCanvas.transform)
            {
                GameObject transObj = t.gameObject;
                string objName = transObj.name;

                if (!transObj.activeSelf ||
                    objName.Equals("Transition UI") ||
                    objName.Equals("Template"))
                    continue;

                GameObject obj = Instantiate(t.gameObject, this.invisiblyUI.transform);

                this.buttonReferences.Clear();
                this.sliderReferences.Clear();
                this.textInputFieldReferences.Clear();
                foreach (GameObject o in GetAllByRoot(t.gameObject))
                {
                    AddToReference(o, this.buttonReferences);
                    AddToReference(o, this.sliderReferences);
                    AddToReference(o, this.textInputFieldReferences);
                }

                //Clean the copied ui
                foreach (GameObject o in GetAllByRoot(obj))
                {
                    if (!o.activeSelf)
                    {
                        Destroy(o);
                        continue;
                    }

                    this.Replace<BookButton, BookButtonReference>(o, this.buttonReferences);
                    this.Replace<BookSlider, BookSliderReference>(o, this.sliderReferences);
                    this.Replace<BookTextInputField, BookTextInputFieldReference>(o, this.textInputFieldReferences);

                    if (o.GetComponent<Outline>() is { } outline)
                        Destroy(outline);

                    if (o.GetComponent<TextMeshProUGUI>() is { } text)
                        Destroy(text);
                }

                break;
            }

            this.invisiblyUI.SetActive(true);
            Debug.Log("Complete");
        }

        #endregion
    }

    internal sealed class TurnPage : IOperation
    {
        private readonly UIBook uiBook;
        private readonly bool fromLeftToRight;
        private bool done;
        private readonly GameObject turnLeftPaper, turnRightPaper, openLeft, openRight;

        private static readonly int InvertPageID = Shader.PropertyToID("InvertPage");

        public TurnPage(UIBook uiBook, bool fromLeftToRight, GameObject turnLeftPaper, GameObject turnRightPaper,
            GameObject openLeft,
            GameObject openRight)
        {
            this.uiBook = uiBook;
            this.fromLeftToRight = fromLeftToRight;
            this.turnLeftPaper = turnLeftPaper;
            this.turnRightPaper = turnRightPaper;
            this.openLeft = openLeft;
            this.openRight = openRight;
        }

        public bool IsOperationDone()
        {
            return this.done;
        }

        public IEnumerator Operation()
        {
            this.done = false;

            this.SetOpens(false);
            this.SetTurns(true);

            this.turnLeftPaper.GetComponent<Renderer>().material.SetInt(InvertPageID, this.fromLeftToRight ? 0 : 1);
            this.turnRightPaper.GetComponent<Renderer>().material.SetInt(InvertPageID, this.fromLeftToRight ? 1 : 0);

            const float animationTime = 0.5f;

            yield return new WaitForSeconds(animationTime * 0.1f);

            if (!this.fromLeftToRight)
                this.openRight.SetActive(true);
            else
                this.openLeft.SetActive(true);

            yield return new WaitForSeconds(animationTime * 0.9f);

            //SetTurns(false);
            this.SetOpens(true);

            this.uiBook.ConstructUI();
            this.done = true;
        }

        public void OperationEnd()
        {
        }

        #region Internal

        private void SetOpens(bool set)
        {
            this.openLeft.SetActive(set);
            this.openRight.SetActive(set);
        }

        private void SetTurns(bool set)
        {
            this.turnLeftPaper.SetActive(set);
            this.turnRightPaper.SetActive(set);
        }

        #endregion
    }

    internal sealed class CloseBook : IOperation
    {
        private bool done;
        private readonly UIBook uiBook;
        private readonly UIBookCameraTransition transition;
        private readonly UIBookLight bookLight;
        private readonly OperationManager operationManager;
        private readonly UIManager uiManager;
        private readonly PlayerManager playerManager;

        public CloseBook(UIBook uiBook, UIBookCameraTransition transition, UIBookLight bookLight,
            OperationManager operationManager,
            UIManager uiManager, PlayerManager playerManager)
        {
            this.uiBook = uiBook;
            this.transition = transition;
            this.bookLight = bookLight;
            this.operationManager = operationManager;
            this.uiManager = uiManager;
            this.playerManager = playerManager;
        }

        public bool IsOperationDone() => this.done;

        public IEnumerator Operation()
        {
            this.done = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            this.bookLight.TurnOff();

            OperationsContainer container = new();
            this.transition.Direction(true, true);
            container.Add(this.transition);

            CameraEvent cameraEvent = CameraEvent.ReturnToDefaultOverworld(this.playerManager);
            container.Add(cameraEvent);
            this.operationManager.AddAsyncOperationsContainer(container);

            yield return new WaitUntil(this.transition.IsOperationDone);

            this.uiManager.SwitchUI(UISelection.Overworld);

            this.done = true;
        }

        public void OperationEnd()
        {
            this.uiBook.GetVisuals().SetActive(false);
            this.uiManager.SetReadyToPause(true);
            this.playerManager.EnablePlayerControl();
        }
    }

    internal sealed class OpenBook : IOperation
    {
        private bool done;
        private readonly UIBook uiBook;
        private readonly CinemachineVirtualCameraBase bookRig;
        private readonly UIBookCameraTransition transition;
        private readonly UIBookLight bookLight;
        private readonly OperationManager operationManager;
        private readonly PlayerManager playerManager;

        public OpenBook(UIBook uiBook, UIBookCameraTransition transition, CinemachineVirtualCameraBase bookRig,
            UIBookLight bookLight,
            OperationManager operationManager, PlayerManager playerManager)
        {
            this.uiBook = uiBook;
            this.bookLight = bookLight;
            this.transition = transition;
            this.bookRig = bookRig;
            this.operationManager = operationManager;
            this.playerManager = playerManager;

            Transform book = this.uiBook.transform, player = playerManager.GetController().transform;
            book.position = player.position;
            book.rotation = player.GetChild(0).rotation;
        }

        public bool IsOperationDone() => this.done;

        public IEnumerator Operation()
        {
            this.done = false;

            this.playerManager.DisablePlayerControl();

            OperationsContainer container = new();
            this.transition.CheckMiddle();
            this.transition.Direction(false, true);
            container.Add(this.transition);

            CameraEvent cameraEvent = ScriptableObject.CreateInstance<CameraEvent>().Setup(
                this.bookRig,
                CameraSettings.Default(),
                this.transition.GetTimeToComplete(),
                0.75f
            );
            container.Add(cameraEvent);

            this.operationManager.AddAsyncOperationsContainer(container);

            this.uiBook.GetVisuals().SetActive(true);

            yield return new WaitUntil(this.transition.IsOperationDone);

            this.uiBook.ConstructUI();
            this.done = true;
        }

        public void OperationEnd()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            this.bookLight.Calculate();
        }
    }
}