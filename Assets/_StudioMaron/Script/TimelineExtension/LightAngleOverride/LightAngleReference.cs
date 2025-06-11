using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Light Angle Controller��Use Reference���[�h�̎��AReference�̃p�����[�^��n�����߂̃N���X
/// </summary>
namespace StudioMaron
{
    public class LightAngleReference : MonoBehaviour
    {
        public LightAngleController lightAngleController;

        private void Start()
        {

        }

        // Reference�̃p�����[�^���X�g��n��
        public List<LightAngleParameter> GetReferenceAngleParameter()
        {
            if (lightAngleController != null)
                return lightAngleController.previousList;
            return new List<LightAngleParameter>();
        }
    }
}
