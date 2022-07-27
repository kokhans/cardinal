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

using System.Text;
using Carcass.Core;
using Carcass.Core.Helpers;

namespace Cardinal.Core.Objects.Physicals.Abstracts;

public abstract class PhysicalObjectIdBuilder<TFileSystemInfo>
    where TFileSystemInfo : FileSystemInfo
{
    static PhysicalObjectIdBuilder()
    {
        Current = new DefaultPhysicalObjectIdBuilder<TFileSystemInfo>();
    }

    public static PhysicalObjectIdBuilder<TFileSystemInfo> Current { get; private set; }

    public static void Reset()
    {
        Current = new DefaultPhysicalObjectIdBuilder<TFileSystemInfo>();
    }

    public static void Set(PhysicalObjectIdBuilder<TFileSystemInfo> builder)
    {
        ArgumentVerifier.NotNull(builder, nameof(builder));

        Current = builder;
    }

    public abstract PhysicalObjectIdBuilder<TFileSystemInfo> Attach(TFileSystemInfo fileSystemInfo);
    public abstract string Build();
}

public sealed class DefaultPhysicalObjectIdBuilder<TFileSystemInfo> : PhysicalObjectIdBuilder<TFileSystemInfo>
    where TFileSystemInfo : FileSystemInfo
{
    private StringBuilder? _stringBuilder;

    public override PhysicalObjectIdBuilder<TFileSystemInfo> Attach(TFileSystemInfo fileSystemInfo)
    {
        ArgumentVerifier.NotNull(fileSystemInfo, nameof(fileSystemInfo));

        _stringBuilder = new StringBuilder();

        _stringBuilder.Append(fileSystemInfo.CreationTime);
        _stringBuilder.Append(fileSystemInfo.CreationTimeUtc);
        _stringBuilder.Append(fileSystemInfo.FullName);
        _stringBuilder.Append(fileSystemInfo.Extension);
        _stringBuilder.Append(fileSystemInfo.Name);
        _stringBuilder.Append(fileSystemInfo.Exists);

        if (fileSystemInfo is FileInfo fileInfo)
            _stringBuilder.Append(fileInfo.DirectoryName);

        return this;
    }

    public override string Build()
    {
        if (_stringBuilder is null)
            throw new InvalidOperationException("File system info is not attached.");

        return HashCodeHelper.GetDeterministicHashCode(_stringBuilder.ToString()).ToString()!;
    }
}