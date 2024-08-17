using UnityEngine;
using UnityEngine.Assertions;

namespace m039.Common.Pathfindig
{
    public class Seeker : MonoBehaviour
    {
        Pathfinder _pathfinder;

        public Pathfinder Pathfinder
        {
            get
            {
                return _pathfinder;
            }
        }

        IGraphController _graphController;

        void Awake()
        {
            _graphController = GetComponent<IGraphController>();
            Assert.IsNotNull(_graphController);
            Init();
        }

        void Init()
        {
            _graphController.onGraphChanged += OnGraphChanged;
            CreatePathfinder();
        }

        void OnGraphChanged()
        {
            CreatePathfinder();
        }

        void CreatePathfinder()
        {
            _pathfinder = _graphController.Graph.CreatePahtfinder();

            var modifiers = GetComponents<IModifier>();
            if (modifiers != null)
            {
                foreach (var m in modifiers)
                {
                    _pathfinder.AddModifier(m);
                }
            }
        }

        public Path Search(Vector3 v1, Vector3 v2)
        {
            return _pathfinder.Search(_graphController.GetNodeAt(v1), _graphController.GetNodeAt(v2));
        }
    }
}
