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

using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using Cardinal.Core.Objects.Physicals;
using Cardinal.Core.Objects.Physicals.Trees;
using Cardinal.Core.Objects.Physicals.Trees.Indexers;
using Cardinal.Core.Objects.Physicals.Trees.Indexers.Abstracts;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Cardinal.Core.IntegrationTests;

public sealed class PhysicalObjectTreeIndexerTest
{
    private const string BaseDirectory = "root";

    private readonly IPhysicalObjectTreeIndexer _indexer;
    private readonly string _basePath;

    public PhysicalObjectTreeIndexerTest()
    {
        ILoggerFactory loggerFactory = Substitute.For<ILoggerFactory>();
        LoggerAdapter<PhysicalObjectTreeIndexer> loggerAdapter = new(loggerFactory);
        ILoggerAdapterFactory loggerAdapterFactory = Substitute.For<ILoggerAdapterFactory>();
        loggerAdapterFactory
            .CreateLoggerAdapter<PhysicalObjectTreeIndexer>()
            .Returns(loggerAdapter);

        _indexer = new PhysicalObjectTreeIndexer();
        _basePath = Path.Combine(Environment.CurrentDirectory, "Assets");
    }

    [Fact]
    public void GivenRootDirectory_WhenIndexPhysicalObjectTree_ThenShouldBeAsExpected()
    {
        // Arrange

        // Act
        PhysicalObjectTree tree = _indexer.IndexPhysicalObjectTree(_basePath, BaseDirectory);

        // Assert
        tree.Flatten().Count().Should().BeGreaterThan(3);

        tree.Parent.Should().BeNull();

        tree.Children.Should().NotBeEmpty();

        PhysicalDirectory? physicalDirectory =
            tree.Value.Should().BeAssignableTo<PhysicalDirectory>().Subject;

        physicalDirectory.AbsolutePath.Should().Be(Path.Combine(_basePath, BaseDirectory));

        physicalDirectory.RelativePath.Should().BeEmpty();

        physicalDirectory.FullName.Should().Be("root");

        physicalDirectory.IsDirectory.Should().BeTrue();
    }
}