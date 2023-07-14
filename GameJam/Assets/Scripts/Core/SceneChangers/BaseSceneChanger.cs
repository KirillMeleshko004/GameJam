using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Core.SceneChangers
{ 
    //Base claass for all scene changers
    public abstract class BaseSceneChanger : MonoBehaviour, ISceneChanger
    {
        [SerializeField]
        protected int _targetSceneBuildID;
    }
}
