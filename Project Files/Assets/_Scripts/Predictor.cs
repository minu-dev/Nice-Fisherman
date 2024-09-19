using UnityEngine;

namespace TensorFlowLite
{
    public class Predictor : BaseImagePredictor<float>
    {
        public struct Result
        {
            public int index;
            public float score;
        }

        float[,] outputs0 = new float[1, 40];
        Result result;

        public Predictor(string modelPath) : base(modelPath, true)
        {
        }


        public override void Invoke(Texture inputTex)
        {
            ToTensor(inputTex, input0);

            interpreter.SetInputTensorData(0, input0);
            interpreter.Invoke();
            interpreter.GetOutputTensorData(0, outputs0);
        }

        public Result GetResults()
        {
            float max = -99.0f;

            for (int i = 0; i < 40; i++)
            {
                if (outputs0[0, i] > max)
                {
                    result.index = i;

                    max = outputs0[0, i];
                    result.score = max;
                }
            }
            return result;
        }
    }
}
