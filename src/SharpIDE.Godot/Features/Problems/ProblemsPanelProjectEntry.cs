using Godot;
using R3;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Problems;

public partial class ProblemsPanelProjectEntry : MarginContainer
{
    public SharpIdeProjectModel Project { get; set; } = null!;

    public override void _Ready()
    {
        GetNode<Label>("Label").Text = Project?.Name ?? "NULL";
        if (Project is null) return;
        Observable.EveryValueChanged(this, s => s.Project.Diagnostics.Count)
            .Subscribe(s => Visible = s is not 0);
        
    }
}