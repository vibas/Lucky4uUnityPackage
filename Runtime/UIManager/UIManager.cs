using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIFramework
{
    public class UIManager : MonoBehaviour
    {
        private abstract class QueuedScreen
        {
            public UIFramework.UIScreen.Id id;
        }


        private class QueuedScreenPush : QueuedScreen
        {
            public UIFramework.UIScreen.Data data;
            public string prefabName;
            public PushedDelegate callback;

            public override string ToString()
            {
                return string.Format("[Push] {0}", id);
            }
        }


        private class QueuedScreenPop : QueuedScreen
        {
            public PoppedDelegate callback;

            public override string ToString()
            {
                return string.Format("[Pop] {0}", id);
            }
        }

        public delegate void PushedDelegate (UIScreen screen);
        public delegate void PoppedDelegate (UIScreen.Id id);

        public string resourcePrefabDirectory;
        public Canvas rootCanvas;
        public Camera uiCamera;

        /// <summary>
        /// [Ryan] A fix for input order not obeying the render order of the screens.
        /// This is a bug as of Unity 2019.1.9f1
        /// </summary>
        public bool inputOrderFixEnabled = true;

        private CanvasScaler _rootCanvasScalar;
        private Dictionary<string, UIScreen> _cache;
        private Queue<QueuedScreen> _queue;
        private List<UIScreen> _stack;
        private HashSet<UIFramework.UIScreen.Id> _stackIdSet;
        private State _state;

        private PushedDelegate _activePushCallback;
        private PoppedDelegate _activePopCallback;

        public Vector2 ReferenceResolution { get { return _rootCanvasScalar.referenceResolution; } }

        private enum State
        {
            Ready,
            Push,
            Pop
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _rootCanvasScalar = rootCanvas.GetComponent<CanvasScaler>();
            if (_rootCanvasScalar == null)
            {
                throw new System.Exception(string.Format("{0} must have a CanvasScalar component attached to it for UIManager.", rootCanvas.name));
            }

            _cache = new Dictionary<string, UIScreen>();
            _queue = new Queue<QueuedScreen>();
            _stack = new List<UIScreen>();
            _state = State.Ready;

            // Remove any objects that may be lingering underneath the root.
            foreach (Transform child in rootCanvas.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Queue the screen to be pushed onto the screen stack. 
        /// Callback will be invoked when the screen is pushed to the stack.
        /// </summary>
        public void ShowScreen (UIFramework.UIScreen.Id id, UIFramework.UIScreen.Data data, string prefabName = null, PushedDelegate callback = null)
        {
            string prefab = prefabName ?? id.defaultPrefabName;
            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePush id: {0}, prefabName: {1}", id, prefab));
            #endif

            if (GetScreen(id) != null)
            {
                Debug.LogWarning(string.Format("Screen {0} already exists in the stack. Ignoring push request.", id));
                return;
            }

            //if (ScreenWillExist(id))
            //{
            //    Debug.LogWarning(string.Format("Screen {0} will exist in the stack after the queue is fully executed. Ignoring push request.", id));
            //    return;
            //}

            QueuedScreenPush push = new QueuedScreenPush();
            push.id = id;
            push.data = data;
            push.prefabName = prefab;
            push.callback = callback;

            _queue.Enqueue(push);

            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}, Frame: {1}", push, Time.frameCount));
            #endif

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the screen to be popped from the screen stack. This will pop all screens on top of it as well.
        /// Callback will be invoked when the screen is reached, or popped if 'include' is true.
        /// </summary>
        public void QueuePopTo (UIFramework.UIScreen.Id id, bool include, PoppedDelegate callback = null)
        {
            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePopTo id: {0}, include: {1}", id, include));
            #endif

            bool found = false;

            for (int i = 0; i < _stack.Count; i++)
            {
                var screen = _stack[i];

                if (screen.id != id)
                {
                    var queuedPop = new QueuedScreenPop();
                    queuedPop.id = screen.id;

                    _queue.Enqueue(queuedPop);

                    #if PRINT_QUEUE
                    DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
                    #endif
                }
                else
                {
                    if (include)
                    {
                        var queuedPop = new QueuedScreenPop();
                        queuedPop.id = screen.id;
                        queuedPop.callback = callback;

                        _queue.Enqueue(queuedPop);

                        #if PRINT_QUEUE
                        DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", queuedPop));
                        #endif
                    }

                    if (callback != null)
                        callback(screen.id);

                    found = true;
                    break;
                }
            }

            if (!found)
                Debug.LogWarning(string.Format("[UIManager] {0} was not in the stack. All screens have been popped.", id));

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the top-most screen to be popped from the screen stack.
        /// Callback will be invoked when the screen is popped from the stack.
        /// </summary>
        public void HideScreen (PoppedDelegate callback = null)
        {
            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] QueuePop"));
            #endif

            UIScreen topScreen = GetTopScreen();
            if (topScreen == null)
                return;

            QueuedScreenPop pop = new QueuedScreenPop();
            pop.id = topScreen.id;
            pop.callback = callback;

            _queue.Enqueue(pop);

            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Enqueued Screen: {0}", pop));
            #endif

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        public void DisplayInfoPopup(string message)
        {
            var screenData = new UIScreen.Data();
            screenData.Add("message", message);

            //sShowScreen(UltraMetaUI.PopupId_InfoPopUp, screenData, "InfoPopup", null);
        }

        public void HideGenericPopup()
        {
            HideScreen(null);
        }

        public void OnUpdate ()
        {
            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        public UIScreen GetTopScreen ()
        {
            if (_stack.Count > 0)
                return _stack[0];

            return null;
        }

        public UIScreen GetScreen (UIFramework.UIScreen.Id id)
        {
            int count = _stack.Count;
            for (int i = 0; i < count; i++)
            {
                if (_stack[i].id == id)
                    return _stack[i];
            }

            return null;
        }

        public T GetScreen<T> (UIFramework.UIScreen.Id id) where T : UIFramework.UIScreen
        {
            UIScreen screen = GetScreen(id);
            return (T)screen;
        }

        public void SetVisibility (bool visible)
        {
            var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = rootCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = visible ? 1.0f : 0.0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public bool IsVisible() {
            var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup == null) {
                return true;
            }

            bool isVisible = canvasGroup.alpha > 0.0f && 
                            canvasGroup.interactable == true &&
                            canvasGroup.blocksRaycasts == true;

            return isVisible;
        }

        private bool CanExecuteNextQueueItem ()
        {
            if (_state == State.Ready)
            {
                if (_queue.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExecuteNextQueueItem ()
        {
            // Get next queued item.
            QueuedScreen queued = _queue.Dequeue();

            #if PRINT_QUEUE
            DebugPrintQueue(string.Format("[UIManager] Dequeued Screen: {0}, Frame: {1}", queued, Time.frameCount));
            #endif

            if (queued is QueuedScreenPush)
            {
                // Push screen.
                QueuedScreenPush queuedPush = (QueuedScreenPush)queued;
                UIScreen screenInstance;

                if (_cache.TryGetValue(queuedPush.prefabName, out screenInstance))
                {
                    // Use cached instance of screen.
                    _cache.Remove(queuedPush.prefabName);

                    #if PRINT_CACHE
                    DebugPrintCache(string.Format("[UIManager] Screen retrieved from Cache: {0}", queuedPush.prefabName));
                    #endif

                    // Move cached to the front of the transfrom heirarchy so that it is sorted properly.
                    screenInstance.transform.SetAsLastSibling();

                    screenInstance.gameObject.SetActive(true);
                }
                else
                {
                    // Instantiate new instance of screen.
                    string path = System.IO.Path.Combine(resourcePrefabDirectory, queuedPush.prefabName);
                    UIScreen prefab = Resources.Load<UIScreen>(path);

                    screenInstance = Object.Instantiate(prefab, rootCanvas.transform);
                    screenInstance.Setup(queuedPush.id, queuedPush.prefabName);
                }

                if (this.inputOrderFixEnabled) {
                    this.UpdateSortOrderOverrides();
                }

                // Tell previous top screen that it is losing focus.
                var topScreen = GetTopScreen();
                if (topScreen != null)
                {
                    #if PRINT_FOCUS
                    Debug.Log(string.Format("[UIManager] Lost Focus: {0}", topScreen.id));
                    #endif

                    topScreen.OnFocusLost();
                }

                // Insert new screen at the top of the stack.
                _state = State.Push;
                _stack.Insert(0, screenInstance);

                _activePushCallback = queuedPush.callback;

                #if PRINT_STACK
                DebugPrintStack(string.Format("[UIManager] Pushing Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
                #endif

                screenInstance.onPushFinished += HandlePushFinished;
                screenInstance.OnPush(queuedPush.data);

                if (_queue.Count == 0)
                {
                    #if PRINT_FOCUS
                    Debug.Log(string.Format("[UIManager] Gained Focus: {0}", screenInstance.id));
                    #endif

                    // Screen gains focus when it is on top of the screen stack and no other items in the queue.
                    screenInstance.OnFocus();
                }
            }
            else
            {
                // Pop screen.
                QueuedScreenPop queuedPop = (QueuedScreenPop)queued;
                UIScreen screenToPop = GetTopScreen();

                if (screenToPop.id != queued.id)
                {
                    throw new System.Exception(string.Format("The top screen does not match the queued pop. " +
                                                             "TopScreen: {0}, QueuedPop: {1}", screenToPop.id, queued.id));
                }

                #if PRINT_FOCUS
                Debug.Log(string.Format("[UIManager] Lost Focus: {0}", screenToPop.id));
                #endif

                screenToPop.OnFocusLost();

                _state = State.Pop;
                _stack.RemoveAt(0);

                // Tell new top screen that it is gaining focus.
                var newTopScreen = GetTopScreen();
                if (newTopScreen != null)
                {
                    if (_queue.Count == 0)
                    {
                        #if PRINT_FOCUS
                        Debug.Log(string.Format("[UIManager] Gained Focus: {0}", newTopScreen.id));
                        #endif

                        // Screen gains focus when it is on top of the screen stack and no other items in the queue.
                        newTopScreen.OnFocus();
                    }
                }

                _activePopCallback = queuedPop.callback;

                #if PRINT_STACK
                DebugPrintStack(string.Format("[UIManager] Popping Screen: {0}, Frame: {1}", queued.id, Time.frameCount));
                #endif

                screenToPop.onPopFinished += HandlePopFinished;
                screenToPop.OnPop();
            }
        }

        private void UpdateSortOrderOverrides() {
            int managedOrder = 0;
            
            int childCount = this.rootCanvas.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                var screen = this.rootCanvas.transform.GetChild(i).GetComponent<UIScreen>();
                if (screen != null) {
                    var canvas = screen.GetComponent<Canvas>();
                    if (canvas != null) {
                        canvas.overrideSorting = true;

                        if (screen.overrideManagedSorting) {
                            canvas.sortingOrder = screen.overrideSortValue;
                        } else {
                            canvas.sortingOrder = managedOrder;
                            managedOrder++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check to see if the screen will exist after the queue has been fully executed.
        /// </summary>
        //private bool ScreenWillExist (BlitzyUI.Screen.Id id)
        //{
        //    return false;

        //    // TODO: Infer if the screen will exists after the queue is fully executed.
        //}

        private void DebugPrintStack (string optionalEventMsg)
        {
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(optionalEventMsg))
                sb.AppendLine(optionalEventMsg);

            sb.AppendLine("[UIManager Screen Stack]");

            for (int i = 0; i < _stack.Count; i++)
            {
                sb.AppendLine(string.Format("{0}", _stack[i].id));
            }

            Debug.Log(sb.ToString());
        }

        private void DebugPrintQueue (string optionalEventMsg)
        {
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(optionalEventMsg))
                sb.AppendLine(optionalEventMsg);

            sb.AppendLine("[UIManager Screen Queue]");

            foreach (QueuedScreen queued in _queue)
            {
                sb.AppendLine(queued.ToString());
            }

            Debug.Log(sb.ToString());
        }

        private void DebugPrintCache (string optionalEventMsg)
        {
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(optionalEventMsg))
                sb.AppendLine(optionalEventMsg);

            sb.AppendLine("[UIManager Screen Cache]");

            foreach (KeyValuePair<string, UIScreen> cached in _cache)
            {
                sb.AppendLine(cached.Key);
            }

            Debug.Log(sb.ToString());
        }

        private void HandlePushFinished (UIScreen screen)
        {
            screen.onPushFinished -= HandlePushFinished;

            _state = State.Ready;

            if (_activePushCallback != null)
            {
                _activePushCallback(screen);
                _activePushCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        private void HandlePopFinished (UIScreen screen)
        {
            screen.onPopFinished -= HandlePopFinished;

            if (screen.keepCached)
            {
                // Store in the cache for later use.
                screen.gameObject.SetActive(false);

                // TODO: Need to have a better cache storage mechanism that supports multiple screens of the same prefab?
                if (!_cache.ContainsKey(screen.PrefabName))
                {
                    _cache.Add(screen.PrefabName, screen);

                    #if PRINT_CACHE
                    DebugPrintCache(string.Format("[UIManager] Screen added to Cache: {0}", screen.PrefabName));
                    #endif
                }
            }
            else
            {
                // Destroy screen.
                Object.Destroy(screen.gameObject);
            }

            _state = State.Ready;

            if (_activePopCallback != null)
            {
                _activePopCallback(screen.id);
                _activePopCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }
    }
}