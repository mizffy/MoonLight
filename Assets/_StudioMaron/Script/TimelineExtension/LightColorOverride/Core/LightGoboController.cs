using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LightコンポーネントのCookieを設定する
/// Light Color Controllerと一緒にアタッチされる
/// Timelineで制御する用なので、制御されない時は何もしないという処理にする
/// </summary>
namespace StudioMaron
{
    public class LightGoboController : MonoBehaviour
    {
        // ライトを参照するコントローラ
        LightColorController controller;
        List<Light> lightList = new List<Light>();

        // Light Gobo Parameterのリスト
        // Timelineからこのリストを更新し、Update時に計算する
        [HideInInspector] public List<LightGoboParameter> previousList = new List<LightGoboParameter>();

        // Timelineからパラメータの更新があったフラグ
        bool parameterChangedByTimeline;

        // 計算用の変数
        float fps = 60;
        float deltaTime;
        int count;
        float coef;

        // ゲームオブジェクトに付与した時、Light Color Controllerを取得
        void Reset()
        {
            controller = GetComponent<LightColorController>();
        }


        // Start時にLight Gobo Parameterを作って初期化
        private void Start()
        {
            CreateParameter();
        }
        // 空のLight Gobo Parameterを作ってpreviousListに入れる
        void CreateParameter()
        {
            var param = new LightGoboParameter();
            SetParameter(new List<LightGoboParameter>() { param });
        }
        // Light Gobo Parameterのリストを作成する
        void SetParameter(List<LightGoboParameter> list)
        {
            previousList.Clear();
            previousList.AddRange(list);
        }

        // フレームレート60(1.6msごと)に処理を行う
        private void Update()
        {
            // フレームレート60に制限する処理
            deltaTime += Time.deltaTime;
            if (deltaTime < 1 / fps) return;

            count = (int)(deltaTime * fps);
            deltaTime -= count / fps;

            // Timelineによる変更がない場合、前回使ったLightColorParameterの内部時間を進めて再利用する
            if (!parameterChangedByTimeline)
            {
                foreach (var o in previousList)
                    o.clipTime += count / fps;
            }
            parameterChangedByTimeline = false;

            // Cookieの更新
            UpdateCookie();
        }
        // Cookieの更新
        void UpdateCookie()
        {
            // nullチェック
            if (controller == null)
                controller = GetComponent<LightColorController>();

            if (controller == null) return;

            // ライトのリストを更新
            lightList = controller.lightList;

            // ライトごとに処理
            for (int i = 0; i < lightList.Count; i++)
            {
                // ライトがnullなら次へ
                if (lightList[i] == null) continue;

                // Light Gobo Parameterごとに処理をする
                // ただ、Timeline clipはMixできないので送られてくるParameterは最大1つ。よって1回までしか処理しない
                for(int j = 0; j < previousList.Count; j++)
                {
                    // Goboテクスチャが空の場合はCookie = null
                    if (previousList[j].cookieTexture.Count == 0)
                    {
                        lightList[i].cookie = null;
                    }
                    else
                    {
                        switch (previousList[j].textureMode)
                        {
                            // Orderモードではテクスチャを順番に割り当てる
                            case LightGoboParameter.TextureMode.Order:
                                lightList[i].cookie = previousList[j].cookieTexture[i % previousList[j].cookieTexture.Count];
                                break;
                            // Randomモードではランダムに割り当てる
                            case LightGoboParameter.TextureMode.Random:
                                lightList[i].cookie = previousList[j].cookieTexture[(int)(0.9999f * RandomFloat(previousList[j].randomSeed + i) * previousList[j].cookieTexture.Count)];
                                break;
                        }
                    }

                    switch (previousList[j].rotationMode)
                    {
                        case LightGoboParameter.RotationMode.None:
                            break;
                        // Constantモードでは全体を一定スピードで回転
                        case LightGoboParameter.RotationMode.Constant:
                            coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                            lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            break;
                        // Odd Reverseモードでは奇数番目は逆回転
                        case LightGoboParameter.RotationMode.OddReverse:
                            if (i % 2 == 0)
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            else
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z - previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            break;
                        // Half Reverseモードでは後ろ半分は逆回転、配列の数が奇数の場合、ちょうど真ん中のライトは回転しない
                        case LightGoboParameter.RotationMode.HalfReverse:
                            if (i < (lightList.Count - 1) / 2f)
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            else if(i == (lightList.Count - 1) / 2f)
                                lightList[i].transform.localRotation = Quaternion.identity;
                            else
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z - previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            break;
                    }
                }
            }
        }
        // Timeline clipから発動する処理
        public void SetParameterFromTimeline(List<LightGoboParameter> parameter)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                SetParameter(parameter);
            }
            else
            {
                SetParameter(parameter);
                UpdateCookie();
            }
#else
                SetParameter(parameter);
#endif
            // Timelineによる更新フラグ
            parameterChangedByTimeline = true;
        }

        // Timelineのプレビュー終了時の処理
        public void ReapplyParameter()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                CreateParameter();
            }
#endif
        }

        // 疑似乱数を作る
        Vector2 st = new Vector2();
        float RandomFloat(int seed)
        {
            st = new Vector2(seed, seed);
            return frac(Mathf.Sin(Vector2.Dot(st, new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        }
        float frac(float f)
        {
            return f - Mathf.Floor(f);
        }
    }
}
