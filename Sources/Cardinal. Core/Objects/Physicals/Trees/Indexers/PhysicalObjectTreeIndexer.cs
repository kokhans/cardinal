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
using Cardinal.Core.Objects.Physicals.Trees.Indexers.Abstracts;

namespace Cardinal.Core.Objects.Physicals.Trees.Indexers;

public sealed class PhysicalObjectTreeIndexer : IPhysicalObjectTreeIndexer
{
    public PhysicalObjectTree IndexPhysicalObjectTree(string path)
    {
        ArgumentVerifier.NotNull(path, nameof(path));

        PhysicalObjectTree root = new(new PhysicalDirectory(
                path,
                string.Empty,
                new DirectoryInfo(path).Name
            )
        );
        RecursiveEnumerateDirectories(path, root, path);

        return root;

        static void RecursiveEnumerateDirectories(
            string baseDirectoryPath,
            PhysicalObjectTree root,
            string rootPath
        )
        {
            ArgumentVerifier.NotNull(root, nameof(root));
            ArgumentVerifier.NotNull(rootPath, nameof(rootPath));

            DirectoryInfo directoryInfo = new(rootPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
                root.AddChild(new PhysicalFile(
                        fileInfo.FullName,
                        fileInfo.FullName.Replace(baseDirectoryPath, string.Empty),
                        fileInfo.Name,
                        fileInfo.Extension
                    )
                );

            string[] directories = Directory.GetDirectories(rootPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string directory in directories)
            {
                directoryInfo = new DirectoryInfo(directory);
                PhysicalObjectTree child =
                    root.AddChild(new PhysicalDirectory(
                            directory,
                            directory.Replace(baseDirectoryPath, string.Empty),
                            directoryInfo.Name
                        )
                    );
                RecursiveEnumerateDirectories(baseDirectoryPath, child, Path.Combine(rootPath, directory));
            }
        }
    }

    public PhysicalObjectTree IndexPhysicalObjectTree(
        string basePath,
        string baseDirectory
    )
    {
        ArgumentVerifier.NotNull(basePath, nameof(basePath));
        ArgumentVerifier.NotNull(baseDirectory, nameof(baseDirectory));

        string baseDirectoryPath = Path.Combine(basePath, baseDirectory);

        return IndexPhysicalObjectTree(baseDirectoryPath);
    }

    public PhysicalObjectTree IndexPhysicalObjectTree(IDirectory directory)
    {
        ArgumentVerifier.NotNull(directory, nameof(directory));

        return IndexPhysicalObjectTree(directory.AbsolutePath);
    }
}