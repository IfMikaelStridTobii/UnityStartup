using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Player
{
    public class PlayerController:MonoBehaviour
    {
        private CameraController movementController;
        private SelectionController selectionController;
        private EventController unitController;
        void Start()
        {
            movementController = gameObject.AddComponent<CameraController>();
            selectionController = gameObject.AddComponent<SelectionController>();
            unitController = gameObject.AddComponent<EventController>();
        }

        public SelectionController GetSelectionController()
        {
            return selectionController;
        }
    }
}
