using System.Collections.Specialized;
using Godot;
using ObservableCollections;
using R3;
using SharpIDE.Application.Features.SolutionDiscovery.VsPersistence;

namespace SharpIDE.Godot.Features.Problems;

public partial class ProblemsPanel : Control
{
    public SharpIdeSolutionModel? Solution { get; set; }
    
	private Tree _tree = null!;
    private TreeItem _rootItem = null!;
    // TODO: Use observable collections in the solution model and downwards
    private readonly ObservableHashSet<SharpIdeProjectModel> _projects = [];

    public override void _Ready()
    {
        _tree = GetNode<Tree>("ScrollContainer/Tree");
        _rootItem = _tree.CreateItem();
        _rootItem.SetText(0, "Problems");
        Observable.EveryValueChanged(this, manager => manager.Solution)
            .Where(s => s is not null)
            .Subscribe(s =>
            {
                GD.Print($"ProblemsPanel: Solution changed to {s?.Name ?? "null"}");
                _projects.Clear();
                _projects.AddRange(s!.AllProjects);
            });
        BindToTree(_projects);
    }

    private class TreeItemContainer
    {
        public TreeItem? Value { get; set; }
    }
    public void BindToTree(ObservableHashSet<SharpIdeProjectModel> list)
    {
        var view = list.CreateView(x =>
        {
            var treeItem = _tree.CreateItem(_rootItem);
            treeItem.SetText(0, x.Name);
            var projectDiagnosticsView = x.Diagnostics.CreateView(y =>
            {
                return new TreeItemContainer();
            });
            var test = projectDiagnosticsView.ObserveChanged()
                .SubscribeAwait(async (e, ct) =>
                {
                    if (e.Action is NotifyCollectionChangedAction.Add)
                    {
                        await this.InvokeAsync(() =>
                        {
                            var diagItem = _tree.CreateItem(treeItem);
                            diagItem.SetText(0, e.NewItem.Value.GetMessage());
                            e.NewItem.View.Value = diagItem;
                        });
                    }
                    if (e.Action is NotifyCollectionChangedAction.Remove)
                    {
                        await this.InvokeAsync(() =>
                        {
                            e.OldItem.View.Value?.Free();
                        });
                    }
                });
            Observable.EveryValueChanged(x, s => s.Diagnostics.Count)
                .Subscribe(s => treeItem.Visible = s is not 0);
            return treeItem;
        });
        view.ViewChanged += OnViewChanged;
    }
    
    private static void OnViewChanged(in SynchronizedViewChangedEventArgs<SharpIdeProjectModel, TreeItem> eventArgs)
    {
        GD.Print("View changed: " + eventArgs.Action);
        if (eventArgs.Action == NotifyCollectionChangedAction.Remove)
        {
            var treeItem = eventArgs.OldItem.View;
            Callable.From(() => treeItem.Free()).CallDeferred();
        }
    }
}