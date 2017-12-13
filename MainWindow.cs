/*
*
* MainWindow.cs
*
* Copyright 2017 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenFile
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Prepare();
        }

        private OperatorEx gop;

        private FileSystemTreeEx top;

        private void Prepare()
        {
            gop = new OperatorEx();
            gop.Prepare(grid);
            gop.AddColumn(@"FileName", @"ファイル名");
            gop.CreateColumns();
            top = new FileSystemTreeEx();
            top.Prepare(tree);
            top.SetGrid(gop);
            top.Tree(@"C:\");
        }

        private class OperatorEx : Grid.Operator
        {
            public void DisplayDirectory(Tree.FileSystemNode arg)
            {
                Blank();
                string path = arg.GetPath();
                System.IO.Directory.EnumerateFiles(path).ToList().ForEach(item =>
                {
                    Grid.RowEntity add = new Grid.RowEntity();
                    add.TrySetMember(Column(0).GetBindName(), item);
                    AddRow(add);
                });
                Refresh();
            }
        }

        private class FileSystemTreeEx : Tree.FileSystemTree
        {
            OperatorEx grid;

            public void SetGrid(OperatorEx arg)
            {
                grid = arg;
            }

            public new void Tree(string path)
            {
                base.Tree(path);
                GetTree().SelectedItemChanged += OnSelect;
            }

            private void OnSelect(object sender, RoutedEventArgs e)
            {
                try
                {
                    TreeView senderObj = sender as TreeView;
                    Tree.FileSystemNode item = senderObj.SelectedItem as Tree.FileSystemNode;
                    grid.DisplayDirectory(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
    }
}
