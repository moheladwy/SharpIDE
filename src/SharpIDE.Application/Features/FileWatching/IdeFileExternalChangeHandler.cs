using Ardalis.GuardClauses;
using SharpIDE.Application.Features.Events;
using SharpIDE.Application.Features.SolutionDiscovery;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Application.Features.FileWatching;

public class IdeFileExternalChangeHandler
{
	private readonly FileChangedService _fileChangedService;
	public SharpIdeSolutionModel SolutionModel { get; set; } = null!;
	public IdeFileExternalChangeHandler(FileChangedService fileChangedService)
	{
		_fileChangedService = fileChangedService;
		GlobalEvents.Instance.FileSystemWatcherInternal.FileChanged.Subscribe(OnFileChanged);
		GlobalEvents.Instance.FileSystemWatcherInternal.FileCreated.Subscribe(OnFileCreated);
	}

	private async Task OnFileCreated(string filePath)
	{
		// Create a new sharpIdeFile, update SolutionModel
		var sharpIdeFile = SolutionModel.AllFiles.SingleOrDefault(f => f.Path == filePath);
		if (sharpIdeFile == null)
		{
			// If sharpIdeFile is null, it means the file was created externally, and we need to create it and add it to the solution model
			// sharpIdeFile = TODO;
		}
		Guard.Against.Null(sharpIdeFile, nameof(sharpIdeFile));
		await _fileChangedService.SharpIdeFileAdded(sharpIdeFile, await File.ReadAllTextAsync(filePath));
	}

	private async Task OnFileChanged(string filePath)
	{
		var sharpIdeFile = SolutionModel.AllFiles.SingleOrDefault(f => f.Path == filePath);
		if (sharpIdeFile is null) return;
		if (sharpIdeFile.SuppressDiskChangeEvents is true) return;
		if (sharpIdeFile.LastIdeWriteTime is not null)
		{
			var now = DateTimeOffset.Now;
			if (now - sharpIdeFile.LastIdeWriteTime.Value < TimeSpan.FromMilliseconds(300))
			{
				Console.WriteLine($"IdeFileExternalChangeHandler: Ignored - {filePath}");
				return;
			}
		}
		Console.WriteLine($"IdeFileExternalChangeHandler: Changed - {filePath}");
		var file = SolutionModel.AllFiles.SingleOrDefault(f => f.Path == filePath);
		if (file is not null)
		{
			await _fileChangedService.SharpIdeFileChanged(file, await File.ReadAllTextAsync(file.Path), FileChangeType.ExternalChange);
		}
	}
}
