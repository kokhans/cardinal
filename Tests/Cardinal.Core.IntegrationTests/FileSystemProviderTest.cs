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

using System.Collections.ObjectModel;
using Cardinal.Core.Objects.Physicals;
using Cardinal.Core.Providers;
using Cardinal.Core.Providers.Abstracts;
using FluentAssertions;
using Xunit;

namespace Cardinal.Core.IntegrationTests;

public sealed class FileSystemProviderTest
{
    private const string BaseDirectory = "root";

    private readonly IFileSystemProvider _provider;
    private readonly string _basePath;

    public FileSystemProviderTest()
    {
        _provider = new FileSystemProvider();
        _basePath = Path.Combine(Environment.CurrentDirectory, "Assets");
    }

    [Theory]
    [InlineData("markdown-file-1", ".md")]
    [InlineData("", ".gitkeep")]
    [InlineData("no-extension", "")]
    public void GivenRootDirectory_WhenGetAllPhysicalFiles_ThenShouldBeAsExpected(string fileName, string fileExtension)
    {
        // Arrange

        // Act
        ReadOnlyCollection<PhysicalFile> physicalFiles =
            _provider.GetAllPhysicalFiles(Path.Combine(_basePath, BaseDirectory));

        // Assert
        physicalFiles.Count.Should().Be(4);

        PhysicalFile physicalFile = physicalFiles.Single(pfe => pfe.Name.Equals(fileName));
        physicalFile.FullName.Should().Be($"{fileName}{fileExtension}");
    }
}