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
using Carcass.Logging.Core.Adapters.Abstracts;
using Cardinal.Core.Objects.Physicals.Trees.ChangeWatchers.Abstracts;
using Cardinal.Core.Objects.Physicals.Trees.Indexers.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Cardinal.Core.Objects.Physicals.Trees.ChangeWatchers;

public sealed class PhysicalObjectTreeChangeWatcherFactory : IPhysicalObjectTreeChangeWatcherFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PhysicalObjectTreeChangeWatcherFactory(IServiceScopeFactory serviceScopeFactory)
    {
        ArgumentVerifier.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

        _serviceScopeFactory = serviceScopeFactory;
    }

    public IPhysicalObjectTreeChangeWatcher CreatePhysicalObjectTreeChangeWatcher(
        PhysicalObjectTreeChangeWatcherSettings? settings = default
    )
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        ILoggerAdapterFactory loggerAdapterFactory =
            serviceScope.ServiceProvider.GetRequiredService<ILoggerAdapterFactory>();
        IPhysicalObjectTreeIndexer indexer =
            serviceScope.ServiceProvider.GetRequiredService<IPhysicalObjectTreeIndexer>();

        return new PhysicalObjectTreeChangeWatcher(
            loggerAdapterFactory,
            indexer,
            settings ?? new PhysicalObjectTreeChangeWatcherSettings()
        );
    }
}