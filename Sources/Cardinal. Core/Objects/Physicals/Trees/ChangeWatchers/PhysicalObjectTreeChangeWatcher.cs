// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Carcass.Core;
using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using Cardinal.Core.Objects.Abstracts;
using Cardinal.Core.Objects.Physicals.Trees.ChangeWatchers.Abstracts;
using Cardinal.Core.Objects.Physicals.Trees.Indexers.Abstracts;

namespace Cardinal.Core.Objects.Physicals.Trees.ChangeWatchers;

public sealed class PhysicalObjectTreeChangeWatcher : Disposable, IPhysicalObjectTreeChangeWatcher
{
    private readonly LoggerAdapter<PhysicalObjectTreeChangeWatcher> _loggerAdapter;
    private readonly IPhysicalObjectTreeIndexer _indexer;
    private readonly PhysicalObjectTreeChangeWatcherSettings _settings;
    private FileSystemWatcher? _fileSystemWatcher;
    private bool _isChangeWatcherStarted;
    private PhysicalObjectTree? _physicalObjectTree;

    public PhysicalObjectTreeChangeWatcher(
        ILoggerAdapterFactory loggerAdapterFactory,
        IPhysicalObjectTreeIndexer indexer,
        PhysicalObjectTreeChangeWatcherSettings settings
    )
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));
        ArgumentVerifier.NotNull(indexer, nameof(indexer));
        ArgumentVerifier.NotNull(settings, nameof(settings));

        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<PhysicalObjectTreeChangeWatcher>();
        _indexer = indexer;
        _settings = settings;
    }

    public event EventHandler<PhysicalObjectTreeChangeEventArgs>? Changed;

    public void StartPhysicalObjectTreeChangeWatcher(string path)
    {
        ArgumentVerifier.NotNull(path, nameof(path));

        if (_isChangeWatcherStarted)
            throw new InvalidOperationException(
                $"{nameof(PhysicalObjectTreeChangeWatcher)} already started."
            );

        _physicalObjectTree = _indexer.IndexPhysicalObjectTree(path);

        _fileSystemWatcher = new FileSystemWatcher(path)
        {
            NotifyFilter = _settings.NotifyFilter,
            Filter = _settings.FileFilter,
            EnableRaisingEvents = true,
            IncludeSubdirectories = true
        };

        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.Created += OnCreated;
        _fileSystemWatcher.Deleted += OnDeleted;
        _fileSystemWatcher.Renamed += OnRenamed;
        _fileSystemWatcher.Error += OnError;

        _isChangeWatcherStarted = true;
    }

    public void StartPhysicalObjectTreeChangeWatcher(
        string basePath,
        string baseDirectory
    )
    {
        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(baseDirectory, nameof(baseDirectory));

        StartPhysicalObjectTreeChangeWatcher(Path.Combine(basePath, baseDirectory));
    }

    public void StartPhysicalObjectTreeChangeWatcher(IDirectory directory)
    {
        ArgumentVerifier.NotNull(directory, nameof(directory));

        StartPhysicalObjectTreeChangeWatcher(directory.AbsolutePath);
    }

    protected override void DisposeManagedResources() => _fileSystemWatcher?.Dispose();

    private void OnChanged(object sender, FileSystemEventArgs eventArgs)
    {
        if (_settings.ChangeType == WatcherChangeTypes.Changed)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Changed)
                return;

            RaisePhysicalObjectTreeChange();
        }
    }

    private void OnCreated(object sender, FileSystemEventArgs eventArgs)
    {
        if (_settings.ChangeType == WatcherChangeTypes.Created)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Created)
                return;

            RaisePhysicalObjectTreeChange();
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs eventArgs)
    {
        if (_settings.ChangeType == WatcherChangeTypes.Deleted)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Deleted)
                return;

            RaisePhysicalObjectTreeChange();
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs eventArgs)
    {
        if (_settings.ChangeType == WatcherChangeTypes.Renamed)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Renamed)
                return;

            RaisePhysicalObjectTreeChange();
        }
    }

    private void OnError(object sender, ErrorEventArgs eventArgs)
    {
        Exception exception = eventArgs.GetException();

        if (_settings.ThrowsOnError) throw exception;

        _loggerAdapter.LogError(exception);
    }

    private void RaisePhysicalObjectTreeChange()
        => Changed?.Invoke(
            this,
            new PhysicalObjectTreeChangeEventArgs(_physicalObjectTree!)
        );
}