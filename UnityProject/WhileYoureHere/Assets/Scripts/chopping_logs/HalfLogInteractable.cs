using Interactable;
using Interactable.Holdable;
using screen;
using UnityEngine.SceneManagement;

namespace chopping_logs
{
    public class HalfLogInteractable : InteractableBehaviour
    {
        private HoldableObjectBehaviour _holdable;
        private LogBasket _parentBasket;

        protected override void Awake()
        {
            base.Awake();
            _holdable = GetComponent<HoldableObjectBehaviour>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            _parentBasket = GetComponentInParent<LogBasket>();
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;
            if (_holdable == null) return false;

            if (interactor.HeldObject != null) return false;

            return _parentBasket != null;
        }

        public override void Interact(IInteractor interactor)
        {
            if (!IsInteractableBy(interactor)) return;
            if (_holdable == null) return;

            _holdable.PickUpByInteractor(interactor);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!IsGameplayScene(scene.name))
            {
                Destroy(gameObject);
            }
        }

        private static bool IsGameplayScene(string sceneName)
        {
            foreach (var s in SceneHandler.GameplayScenes)
                if (s == sceneName) return true;

            return false;
        }
    }
}