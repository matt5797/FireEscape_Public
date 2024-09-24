using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FireEscape.Turn;
using UnityEngine.EventSystems;
using FireEscape.Enviroment;

namespace FireEscape.Object
{
    public class Door : ActionableLevelObject
    {
        public GameObject DoorObject;

        [HideInInspector]
        public bool IsOpen;
        [HideInInspector]
        public bool IsLocked;
        [HideInInspector]
        public bool isMoving;

        public float OpenAngle = 90f;
        public float CloseAngle = 0f;
        public float OpenTime = 2f;

        [HideInInspector]
        public List<Room> rooms = new List<Room>();
    }
}
