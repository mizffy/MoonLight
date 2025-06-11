using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timeline clip����R���g���[���֕ϐ���n�����A���̃N���X���g���ēn��
/// </summary>
namespace StudioMaron
{
    public class LightGoboParameter
    {
        public enum TextureMode
        {
            Order,
            Random
        }
        public enum RotationMode
        {
            None,
            Constant,
            OddReverse,
            HalfReverse
        }
        public RotationMode rotationMode = RotationMode.None;
        [Range(-1f, 1f)] public float rotationSpeed = 0f;

        public TextureMode textureMode = TextureMode.Order;
        public List<Texture2D> cookieTexture = new List<Texture2D>();
        public float clipTime = 0;
        public int randomSeed = 1234;
    }
}
