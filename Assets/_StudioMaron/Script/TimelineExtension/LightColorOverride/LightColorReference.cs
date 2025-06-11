using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use Referenceモードの時のReferenceを受け渡すためのクラス
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

        // Light Color ControllerにReferenceのパラメータリストを渡す
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

        // Light Flash ControllerにReferenceの係数を渡す
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
