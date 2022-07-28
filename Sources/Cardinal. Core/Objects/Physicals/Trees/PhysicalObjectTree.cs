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
using Carcass.Core;
using Cardinal.Core.Objects.Physicals.Abstracts;

namespace Cardinal.Core.Objects.Physicals.Trees;

public sealed class PhysicalObjectTree
{
    private readonly List<PhysicalObjectTree> _children;

    public PhysicalObjectTree(PhysicalObject root)
    {
        ArgumentVerifier.NotNull(root, nameof(root));

        Value = root;
        _children = new List<PhysicalObjectTree>();
    }

    public PhysicalObjectTree? Parent { get; private init; }
    public ReadOnlyCollection<PhysicalObjectTree> Children => _children.AsReadOnly();
    public PhysicalObject Value { get; }

    public PhysicalObjectTree AddChild(PhysicalObject child)
    {
        ArgumentVerifier.NotNull(child, nameof(child));

        PhysicalObjectTree node = new(child) { Parent = this };
        _children.Add(node);

        return node;
    }

    public IEnumerable<PhysicalObject> Flatten()
        => new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));

    public PhysicalObject GetSingle<TPhysicalObject>(ShortGuid id)
        where TPhysicalObject : PhysicalObject
        => (TPhysicalObject) Flatten().Single(vo => vo.Id == id);

    public IEnumerable<TPhysicalObject> GetMany<TPhysicalObject>(params ShortGuid[] ids)
        where TPhysicalObject : PhysicalObject
        => Flatten().Where(vo => ids.Contains(vo.Id)).Cast<TPhysicalObject>();
}