using fc_minimalApi.Models;

namespace fc_minimalApi.Interfaces
{
    public interface IMachineLearningService
    {
        string PredictFileCategorization(string fileNameToPredict);
        List<FilesDetail> PredictFileCategorization(List<FilesDetail> fileList);
        string TrainAndSaveModel();
    }
}
