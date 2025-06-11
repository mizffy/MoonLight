using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace StudioMaron
{
    [System.Serializable]
    public class LightGoboBehaviour : PlayableBehaviour
    {
        [Header("===== Gobo Texture ===============================================")]
        [Space(10)]
        public LightGoboParameter.TextureMode textureMode = LightGoboParameter.TextureMode.Order;
        public int randomSeed = 1234;
        [Space(10)]
        public List<Texture2D> cookieTexture = new List<Texture2D>();
        [Space(10)]
        [Header("===== Rotation ===============================================")]
        [Space(10)]
        public LightGoboParameter.RotationMode rotationMode = LightGoboParameter.RotationMode.None;
        [Space(10)]
        [Range(-1f, 1f)] public float rotationSpeed = 0f;
    }
}