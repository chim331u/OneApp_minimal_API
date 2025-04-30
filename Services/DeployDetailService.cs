using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Configurations;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;

namespace OneApp_minimalApi.Services;

/// <summary>
/// Provides services for managing deployment details.
/// </summary>
public class DeployDetailService : IDeployDetailService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<DeployDetailService> _logger; // Logger for logging information and errors
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities

    /// <summary>
    /// Initializes a new instance of the <see cref="DeployDetailService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="utilityServices">The utility services for encryption and other utilities.</param>
    public DeployDetailService(ApplicationContext context, ILogger<DeployDetailService> logger, IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
    }

    /// <summary>
    /// Retrieves a deployment detail by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the deployment detail.</param>
    /// <returns>The deployment detail as a <see cref="DeployDetailDto"/> object.</returns>
    public async Task<DeployDetailDto> GetDeployDetailById(int id)
    {
        try
        {
            var deployDetail = await _context.DeployDetails.Include(x => x.DockerConfig).Where(x => x.Id == id).FirstOrDefaultAsync();
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

    /// <summary>
    /// Retrieves a list of deployment details for a specific Docker configuration.
    /// </summary>
    /// <param name="DockerConfigId">The unique identifier of the Docker configuration.</param>
    /// <returns>A list of deployment details as <see cref="DeployDetailDto"/> objects.</returns>
    public async Task<List<DeployDetailDto>> GetDeployDetails(int DockerConfigId)
    {
        try
        {
            var deployDetails = await _context.DeployDetails.Include(x => x.DockerConfig)
                .Where(x => x.IsActive && x.DockerConfig.Id == DockerConfigId)
                .ToListAsync();

            return deployDetails.Select(_deployDetail => Mapper.FromDeployDetailToDto(_deployDetail)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving deploy details: {ex.Message}");
            return null!;
        }
    }

    /// <summary>
    /// Adds a new deployment detail.
    /// </summary>
    /// <param name="deployDetailDto">The deployment detail data to add.</param>
    /// <returns>The added deployment detail as a <see cref="DeployDetailDto"/> object.</returns>
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

    /// <summary>
    /// Updates an existing deployment detail.
    /// </summary>
    /// <param name="id">The unique identifier of the deployment detail to update.</param>
    /// <param name="deployDetailDto">The updated deployment detail data.</param>
    /// <returns>The updated deployment detail as a <see cref="DeployDetailDto"/> object.</returns>
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

    /// <summary>
    /// Deletes a deployment detail by marking it as inactive.
    /// </summary>
    /// <param name="id">The unique identifier of the deployment detail to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
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