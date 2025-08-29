using Godot;
using SharpIDE.Application.Features.Debugging;
using SharpIDE.Application.Features.Events;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Debug_.Tab.SubTabs;

public partial class ThreadsVariablesSubTab : Control
{
	private PackedScene _threadListItemScene = GD.Load<PackedScene>("res://Features/Debug_/Tab/SubTabs/ThreadListItem.tscn");
	private VBoxContainer _threadsVboxContainer = null!;
	public SharpIdeProjectModel Project { get; set; } = null!;

	public override void _Ready()
	{
		_threadsVboxContainer = GetNode<VBoxContainer>("%ThreadsPanel/VBoxContainer");
		GlobalEvents.DebuggerExecutionStopped += OnDebuggerExecutionStopped;
		
	}

	private async Task OnDebuggerExecutionStopped(ExecutionStopInfo arg)
	{
		var result = await Singletons.RunService.GetInfoAtStopPoint();
		var scenes = result.Threads.Select(s =>
		{
			var threadListItem = _threadListItemScene.Instantiate<Control>();
			threadListItem.GetNode<Label>("Label").Text = $"{s.Id}: {s.Name}";
			return threadListItem;
		}).ToList(); 
		await this.InvokeAsync(() =>
		{
			foreach (var scene in scenes)
			{
				_threadsVboxContainer.AddChild(scene);
			}
		});
	}
}