using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Interfaces
{
    public interface IMachineLearningService
    {
        string PredictFileCategorization(string fileNameToPredict);
        List<FilesDetail> PredictFileCategorization(List<FilesDetail> fileList);
        string TrainAndSaveModel();
    }
}
