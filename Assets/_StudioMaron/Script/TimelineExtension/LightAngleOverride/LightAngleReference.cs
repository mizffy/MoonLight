using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Light Angle ControllerがUse Referenceモードの時、Referenceのパラメータを渡すためのクラス
/// </summary>
namespace StudioMaron
{
    public class LightAngleReference : MonoBehaviour
    {
        public LightAngleController lightAngleController;

        private void Start()
        {

        }

        // Referenceのパラメータリストを渡す
        public List<LightAngleParameter> GetReferenceAngleParameter()
        {
            if (lightAngleController != null)
                return lightAngleController.previousList;
            return new List<LightAngleParameter>();
        }
    }
}
