using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Player
{
    class PlayerController:MonoBehaviour
    {
        private CameraController movementController;
        private SelectionController selectionController;

        void Start()
        {
            movementController = gameObject.AddComponent<CameraController>();
            selectionController = gameObject.AddComponent<SelectionController>();
        }
        public SelectionController GetSelectionController()
        {
            return selectionController;
        }
    }
}
