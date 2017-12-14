/*
*
* OFWindow.cs
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Grid;
using Tree;

namespace OpenFile {
    /// <summary>
    /// OFWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OFWindow : Window {

        private string path;

        public string GetPath() {
            return path;
        }

        public OFWindow() {
            InitializeComponent();

            Prepare();
        }

        private OperatorEx gop;

        private FileSystemTreeEx top;

        private void Prepare() {
            gop = new OperatorEx();
            gop.Prepare(grid);
            gop.AddColumn(@"FileName", @"ファイル名");
            gop.CreateColumns();
            top = new FileSystemTreeEx();
            top.Prepare(tree);
            top.SetGrid(gop);
            top.Tree(@"C:\");

            grid.SelectedCellsChanged += CellChanged;
            button.Click += ButtonClick;
        }

        private void CellChanged(object sender, SelectedCellsChangedEventArgs e) {
            try {
                DataGrid senderObj = sender as DataGrid;
                RowEntity senderRow = senderObj.SelectedItem as RowEntity;
                object path = null;
                if (senderRow.TryGetMember(@"FileName", out path))
                    text.Text = path as string;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e) {
            try {
                path = text.Text;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private class OperatorEx : Operator {
            public void DisplayDirectory(FileSystemNode arg) {
                Blank();
                IEnumerable<string> files = TryGetFiles(arg.GetPath());
                if (files == null)
                    return;
                foreach (string item in files) {
                    RowEntity add = new RowEntity();
                    add.TrySetMember(Column(0).GetBindName(), item);
                    AddRow(add);
                }
                Refresh();
            }

            private IEnumerable<string> TryGetFiles(string path) {
                try {
                    return System.IO.Directory.EnumerateFiles(path);
                }
                catch (UnauthorizedAccessException) {
                    return null;
                }
            }
        }

        private class FileSystemTreeEx : FileSystemTree {
            OperatorEx grid;

            public void SetGrid(OperatorEx arg) {
                grid = arg;
            }

            public new void Tree(string path) {
                base.Tree(path);
                GetTree().SelectedItemChanged += OnSelect;
            }

            private void OnSelect(object sender, RoutedEventArgs e) {
                try {
                    TreeView senderObj = sender as TreeView;
                    FileSystemNode item = senderObj.SelectedItem as FileSystemNode;
                    grid.DisplayDirectory(item);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
    }
}
