using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System.Collections.Generic;
using System.Linq;

namespace BrightnessTrainAPI.Models
{
    public class BrightnessTrain
    {
        private string _dataPath;
        private LearningPipeline _pipeline;

        public BrightnessTrain(string dataPath)
        {
            this._dataPath = dataPath;

            this.init();
        }

        private void init()
        {
            this._pipeline = new LearningPipeline() {
                new TextLoader(this._dataPath).CreateFrom<b>(separator: ','),
                new Dictionarizer("Label"),
                new ColumnConcatenator("Features", "Time"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" }
            };
        }

        public string Train(float time)
        {
            var model = this._pipeline.Train<b, bPrediction>();
            var prediction = model.Predict(new b() { Time = time });

            return prediction.PredictedLabels;
        }
        public IEnumerable<string> Train(List<float> times)
        {
            var model = this._pipeline.Train<b, bPrediction>();
            var prediction = model.Predict(from t in times select new b() { Time = t });

            return prediction.Select(m => m.PredictedLabels);
        }

        private class b
        {
            /// <summary>
            /// 时间,HHmmss
            /// </summary>
            [Column("0")]
            public float Time;
            [Column("1")]
            public string Label { get; set; }
            //public string Alpha;
        }
        private class bPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }
    }
}
