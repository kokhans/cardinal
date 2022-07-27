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
using Cardinal.Core.Objects.Virtuals;

namespace Cardinal.Core.Objects.Physicals;

public sealed record PhysicalFile : PhysicalObject, IFile
{
    public PhysicalFile(
        string absolutePath,
        string relativePath,
        string fullName,
        string extension
    ) : base(absolutePath, relativePath, fullName, false)
    {
        ArgumentVerifier.NotNull(extension, nameof(extension), true);

        Id = PhysicalObjectIdBuilder<FileInfo>.Current.Attach(new FileInfo(absolutePath)).Build();
        Name = extension.Equals(string.Empty)
            ? fullName
            : fullName.Replace(extension, string.Empty);
        Extension = extension;
    }

    public override string Id { get; }
    public string Name { get; }
    public string Extension { get; }
    public VirtualFile? VirtualFile { get; private set; }

    public void CreateVirtualFile() => VirtualFile = new VirtualFile(this);
    public void DeleteVirtualFile() => VirtualFile = null;
}