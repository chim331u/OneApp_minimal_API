﻿using Microsoft.ML.Data;

namespace OneApp_minimalApi.Models.MachineLearning
{
    public class MlFileName
    {
        [LoadColumn(0)]
        public int Id { get; set; }

        [LoadColumn(1)]
        public string Area { get; set; }

        [LoadColumn(2)]
        public string FileName { get; set; }
    }
}