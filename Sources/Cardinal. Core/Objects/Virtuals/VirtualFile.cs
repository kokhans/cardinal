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
using Cardinal.Core.Objects.Physicals;
using Cardinal.Core.Objects.Virtuals.Abstracts;

namespace Cardinal.Core.Objects.Virtuals;

public sealed record VirtualFile : VirtualObject, IFile
{
    public VirtualFile(
        string absolutePath,
        string relativePath,
        string fullName,
        string extension
    ) : base(absolutePath, relativePath, fullName, false)
    {
        ArgumentVerifier.NotNull(extension, nameof(extension), true);

        Name = extension.Equals(string.Empty)
            ? fullName
            : fullName.Replace(extension, string.Empty);
        Extension = extension;
    }

    public VirtualFile(PhysicalFile physicalFile) : base(physicalFile)
    {
        Name = physicalFile.FullName;
        Extension = physicalFile.Extension;
    }

    public string Name { get; private set; }
    public string Extension { get; private set; }

    public void UpdateFullName(string fullName)
    {
        ArgumentVerifier.NotNull(fullName, nameof(fullName));

        string name = Path.GetFileNameWithoutExtension(AbsolutePath);
        string extension = fullName.Replace(name, string.Empty);

        AbsolutePath = AbsolutePath.Replace(FullName, fullName);
        FullName = fullName;
        Name = name;
        Extension = extension;
    }

    public void UpdateName(string name)
    {
        ArgumentVerifier.NotNull(name, nameof(name), true);

        string fullName = $"{name}{Extension}";

        AbsolutePath = AbsolutePath.Replace(FullName, fullName);
        FullName = fullName;
        Name = name;
    }

    public void UpdateExtension(string extension)
    {
        ArgumentVerifier.NotNull(extension, nameof(extension), true);

        string fullName = $"{Name}{extension}";

        AbsolutePath = AbsolutePath.Replace(FullName, fullName);
        FullName = fullName;
        Extension = extension;
    }
}