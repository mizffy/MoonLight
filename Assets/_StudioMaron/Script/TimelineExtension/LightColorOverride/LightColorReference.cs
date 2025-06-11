using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use Reference���[�h�̎���Reference���󂯓n�����߂̃N���X
/// </summary>
namespace StudioMaron
{
    public class LightColorReference : MonoBehaviour
    {
        public enum LightColorControllerType
        {
            LightColorController,
            EmissionColorController
        }
        public LightColorControllerType controllerType;
        public LightColorController lightColorController;
        public EmissionColorController emissionColorController;
        LightFlashController lightFlashController;

        // Light Color Controller��Reference�̃p�����[�^���X�g��n��
        public List<LightColorParameter> GetReferenceColorParameter()
        {
            switch (controllerType)
            {
                case LightColorControllerType.LightColorController:
                    if (lightColorController != null)
                        return lightColorController.previousList;
                    break;
                case LightColorControllerType.EmissionColorController:
                    if (emissionColorController != null)
                        return emissionColorController.previousList;
                    break;
            }
            return new List<LightColorParameter>();
        }

        // Light Flash Controller��Reference�̌W����n��
        public float GetReferenceFlashCoef(int n, int length)
        {
            switch (controllerType)
            {
                case LightColorControllerType.LightColorController:
                    if (lightColorController != null)
                        lightFlashController = lightColorController.flashController;
                    break;
                case LightColorControllerType.EmissionColorController:
                    if (emissionColorController != null)
                        lightFlashController = emissionColorController.flashController;
                    break;
            }

            if (lightFlashController != null)
                return lightFlashController.IntensityCoef(n, length, true);
            return 1f;
        }
    }
}
