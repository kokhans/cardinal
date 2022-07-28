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
using Cardinal.Core.Objects.Abstracts;
using Cardinal.Core.Objects.Physicals.Abstracts;

namespace Cardinal.Core.Objects.Virtuals.Abstracts;

public abstract record VirtualObject : IObject
{
    protected VirtualObject(
        string absolutePath,
        string relativePath,
        string fullName,
        bool isDirectory
    )
    {
        ArgumentVerifier.NotNull(absolutePath, nameof(absolutePath));
        ArgumentVerifier.NotNull(relativePath, nameof(relativePath), true);
        ArgumentVerifier.NotNull(fullName, nameof(fullName), true);

        AbsolutePath = absolutePath;
        RelativePath = relativePath;
        FullName = fullName;
        IsDirectory = isDirectory;
    }

    protected VirtualObject(PhysicalObject physicalObject)
    {
        ArgumentVerifier.NotNull(physicalObject, nameof(physicalObject));

        AbsolutePath = physicalObject.AbsolutePath;
        RelativePath = physicalObject.RelativePath;
        FullName = physicalObject.FullName;
        IsDirectory = physicalObject.IsDirectory;
    }

    public string AbsolutePath { get; protected set; }
    public string RelativePath { get; protected set; }
    public string FullName { get; protected set; }
    public bool IsDirectory { get; }
}