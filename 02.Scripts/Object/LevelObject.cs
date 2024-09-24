using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FireEscape.Object
{
    public class LevelObject : MonoBehaviour
    {
        public int ObjectID
        {
            get { return GetInstanceID();}
        }

        public string ObjectName;
    }
    
}

