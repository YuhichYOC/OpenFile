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
using System.Windows;
using System.Windows.Controls;

using Grid;
using OpenFile;
using SAXWrapper;
using Tree;

namespace XmlTreeView {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        //private TreeViewController tree;

        //private DataGridController grid;

        //private BridgeBetweenTreeAndGrid bridge;

        public MainWindow() {
            InitializeComponent();

            View v = new View();
            v.Show();
            //tree = new TreeViewController(TreeView);
            //grid = new DataGridController(DataGrid);
            //grid.AddColumn(@"Name", @"属性");
            //grid.AddColumn(@"Value", @"値");
            //bridge = new BridgeBetweenTreeAndGrid(tree, grid);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            //try
            //{
            //    tree.Tree();
            //    grid.CreateColumns();
            //    bridge.AddHandler();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            //}
        }

        private OperatorEx gop;

        private XMLTreeEx top;

        private void Prepare() {
            gop = new OperatorEx();
            gop.Prepare(DataGrid);
            gop.AddColumn(@"Name", @"属性");
            gop.AddColumn(@"Value", @"値");
            gop.CreateColumns();
            top = new XMLTreeEx();
            top.Prepare(TreeView);
            top.SetGrid(gop);


        }

        private void Menu(object sender, RoutedEventArgs e) {
            try {
                OpenFile();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void OpenFile() {
            OFWindow w = new OFWindow();
            w.ShowDialog();
            string path = w.GetPath();
            SettingReader reader = new SettingReader();
            reader.SetDirectory(System.IO.Path.GetDirectoryName(path));
            reader.SetFileName(System.IO.Path.GetFileName(path));
            reader.Parse();
            top.Tree(reader.GetNode());
        }

        private class OperatorEx : Operator {
            public void DisplayNode(NodeEntity arg) {
                Blank();
                foreach (AttributeEntity a in arg.GetAttrList()) {
                    RowEntity add = new RowEntity();
                    add.TrySetMember(Column(0).GetBindName(), a.GetAttrName());
                    add.TrySetMember(Column(1).GetBindName(), a.GetAttrValue());
                    AddRow(add);
                }
                RowEntity nodeValue = new RowEntity();
                nodeValue.TrySetMember(Column(0).GetBindName(), @"Node Value");
                nodeValue.TrySetMember(Column(1).GetBindName(), arg.GetNodeValue());
                AddRow(nodeValue);
                Refresh();
            }
        }

        private class XMLTreeEx : XMLTree {
            OperatorEx grid;

            public void SetGrid(OperatorEx arg) {
                grid = arg;
            }

            public new void Tree(NodeEntity arg) {
                base.Tree(arg);
                GetTree().SelectedItemChanged += OnSelect;
            }

            private void OnSelect(object sender, RoutedEventArgs e) {
                try {
                    TreeView senderObj = sender as TreeView;
                    XMLNode senderNode = senderObj.SelectedItem as XMLNode;
                    NodeEntity arg = senderNode.Tag as NodeEntity;
                    grid.DisplayNode(arg);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
    }
}
