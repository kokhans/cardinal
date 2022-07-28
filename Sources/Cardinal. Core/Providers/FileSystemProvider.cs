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
using System.Text;
using Carcass.Core;
using Carcass.Core.Extensions;
using Cardinal.Core.Extensions;
using Cardinal.Core.Objects.Abstracts;
using Cardinal.Core.Objects.Physicals;
using Cardinal.Core.Providers.Abstracts;

namespace Cardinal.Core.Providers;

public sealed class FileSystemProvider : IFileSystemProvider
{
    public void CreateDirectory(string path)
    {
        ArgumentVerifier.NotNull(path, nameof(path));

        Directory.CreateDirectory(path);
    }

    public void CreateDirectory(string basePath, string directoryName)
    {
        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(directoryName, nameof(directoryName));

        CreateDirectory(Path.Combine(basePath, directoryName));
    }

    public void CreateDirectory(IDirectory directory)
    {
        ArgumentVerifier.NotNull(directory, nameof(directory));

        CreateDirectory(directory.AbsolutePath);
    }

    public void DeleteDirectory(string path)
    {
        ArgumentVerifier.NotNull(path, nameof(path));

        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    public void DeleteDirectory(string basePath, string directoryName)
    {
        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(directoryName, nameof(directoryName));

        DeleteDirectory(Path.Combine(basePath, directoryName));
    }

    public void DeleteDirectory(IDirectory directory)
    {
        ArgumentVerifier.NotNull(directory, nameof(directory));

        DeleteDirectory(directory.AbsolutePath);
    }

    public ReadOnlyCollection<PhysicalFile> GetAllPhysicalFiles(string path)
    {
        ArgumentVerifier.NotNull(path, nameof(path));

        return new DirectoryInfo(path)
            .EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Select(fi => fi.AsPhysicalFile(path))
            .ToList()
            .AsReadOnlyCollection();
    }

    public ReadOnlyCollection<PhysicalFile> GetAllPhysicalFiles(string basePath, string directoryName)
    {
        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(directoryName, nameof(directoryName));

        return GetAllPhysicalFiles(Path.Combine(basePath, directoryName));
    }

    public ReadOnlyCollection<PhysicalFile> GetAllPhysicalFiles(IDirectory directory)
    {
        ArgumentVerifier.NotNull(directory, nameof(directory));

        return GetAllPhysicalFiles(directory.AbsolutePath);
    }

    public async Task CopyFileAsync(
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(sourcePath, nameof(sourcePath));
        ArgumentVerifier.NotNull(destinationPath, nameof(destinationPath));

        FileInfo fileInfo = new(destinationPath);
        CreateDirectory(fileInfo.Directory!.FullName);

        await using FileStream sourceFileStream = File.Open(sourcePath, FileMode.Open);
        await using FileStream destinationFileStream = File.Create(destinationPath);

        await sourceFileStream.CopyToAsync(destinationFileStream, cancellationToken);
    }

    public async Task CopyFileAsync(
        IFile sourceFile,
        IFile destinationFile,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(sourceFile, nameof(sourceFile));
        ArgumentVerifier.NotNull(destinationFile, nameof(destinationFile));

        await CopyFileAsync(
            sourceFile.AbsolutePath,
            destinationFile.AbsolutePath,
            cancellationToken
        );
    }

    public async Task CreateFileAsync(
        string? content,
        string path,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(path, nameof(path));

        FileInfo fileInfo = new(path);
        Directory.CreateDirectory(fileInfo.Directory!.FullName);

        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    public async Task CreateFileAsync(
        string? content,
        string basePath,
        string baseDirectory,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(baseDirectory, nameof(baseDirectory));

        await CreateFileAsync(content, Path.Combine(basePath, baseDirectory), cancellationToken);
    }

    public async Task CreateFileAsync(
        string? content,
        IFile file,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(file, nameof(file));

        await CreateFileAsync(content, file.AbsolutePath, cancellationToken);
    }

    public async Task<string> ReadFileAsTextAsync(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(path, nameof(path));

        await using FileStream fileStream = new(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            true
        );
        using StreamReader streamReader = new(fileStream, Encoding.UTF8);

        return await streamReader.ReadToEndAsync();
    }

    public async Task<string> ReadFileAsTextAsync(
        IFile file,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(file, nameof(file));

        return await ReadFileAsTextAsync(file.AbsolutePath, cancellationToken);
    }
}