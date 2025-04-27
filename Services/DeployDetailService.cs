using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Services;

public class DeployDetailService : IDeployDetailService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<DeployDetailService> _logger; // Logger for logging information and error
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities

    public DeployDetailService(ApplicationContext context, ILogger<DeployDetailService> logger, IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
    }
    
    public async Task<DeployDetailDto> GetDeployDetailById(int id)
    {
        try
        {
            var deployDetail = await _context.DeployDetails.Include(x=>x.DockerConfig).Where(x=>x.Id==id).FirstOrDefaultAsync();
            if (deployDetail == null)
            {
                _logger.LogWarning($"Deploy detail with ID {id} not found.");
                return null;
            }
            return Mapper.FromDeployDetailToDto(deployDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving deploy detail with ID {id}: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<List<DeployDetailDto>> GetDeployDetails(int DockerConfigId)
    {
        try
        {
            var deployDetails = await _context.DeployDetails.Include(x=>x.DockerConfig)
                .Where(x=>x.IsActive && x.DockerConfig.Id==DockerConfigId)
                .ToListAsync();
            
            return deployDetails.Select(_deployDetail => Mapper.FromDeployDetailToDto(_deployDetail)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving deploy details: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DeployDetailDto> AddDeployDetail(DeployDetailDto deployDetailDto)
    {
        try
        {
            var dockerConfig = await _context.DockerConfig.FindAsync(deployDetailDto.DockerConfigId);

            var deployDetail = new DeployDetail()
            {
                Note = deployDetailDto.Note,
                DeployEnd = deployDetailDto.DeployEnd,
                DeployStart = deployDetailDto.DeployStart,
                Duration = deployDetailDto.Duration,
                DockerConfig = dockerConfig,
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                LogFilePath = deployDetailDto.LogFilePath,
                Result = deployDetailDto.Result,

            };
            
            await _context.DeployDetails.AddAsync(deployDetail);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Deploy detail with ID {deployDetail.Id} created.");
            return deployDetailDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding deploy detail: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<DeployDetailDto> UpdateDeployDetail(int id, DeployDetailDto deployDetailDto)
    {
        try
        {
            var existingDeployDetail = await _context.DeployDetails.FindAsync(id);
            
            if (existingDeployDetail == null)
            {
                _logger.LogWarning($"Deploy detail with ID {id} not found.");
                return null!;
            }   

            existingDeployDetail.DeployEnd = deployDetailDto.DeployEnd;
            existingDeployDetail.DeployStart = deployDetailDto.DeployStart;
            existingDeployDetail.Duration = deployDetailDto.Duration;
            existingDeployDetail.Result = deployDetailDto.Result;
            existingDeployDetail.LogFilePath = deployDetailDto.LogFilePath;
            existingDeployDetail.Note = deployDetailDto.Note;
            existingDeployDetail.LastUpdatedDate = DateTime.Now;

            _context.DeployDetails.Update(existingDeployDetail);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Deploy detail with ID {existingDeployDetail.Id} updated.");
            return deployDetailDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating deploy detail: {ex.Message}");
            return null!;
        }
    }
    
    public async Task<bool> DeleteDeployDetail(int id)
    {
        try
        {
            var deployDetail = await _context.DeployDetails.FindAsync(id);
            
            if (deployDetail == null)
            {
                _logger.LogWarning($"Deploy detail with ID {id} not found.");
                return false;
            }

            deployDetail.IsActive = false;
            deployDetail.LastUpdatedDate = DateTime.Now;
            
            _context.DeployDetails.Update(deployDetail);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Deploy detail with ID {id} deleted.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting deploy detail: {ex.Message}");
            return false;
        }
    }
}