using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Core.SceneControllers
{   
    public abstract class BaseSceneController : MonoBehaviour
    {
        public abstract void NextAction();
    }
}
