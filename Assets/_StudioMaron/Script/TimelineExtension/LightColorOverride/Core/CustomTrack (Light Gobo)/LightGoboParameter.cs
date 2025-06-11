using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timeline clipからコントローラへ変数を渡す時、このクラスを使って渡す
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
