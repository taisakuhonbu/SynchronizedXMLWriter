using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace SyncXW
{
    internal enum NodeType
    {
        Empty,
        OneLine,
        Normal,
        Text,
        Dummy,
    }
    public class Node
    {
        internal NodeType NodeType { get; set; }
        public String Stuff { get; set; }

        internal Node(NodeType t, String stuff)
        {
            NodeType = t; Stuff = stuff;
        }
        static public Node CreateEmptyElement(String element_name)
        {
            return new Node(NodeType.Empty, element_name);
        }
        static public Node CreateOneLineElement(String element_name)
        {
            return new Node(NodeType.OneLine, element_name);
        }
        static public Node CreateElement(String element_name)
        {
            return new Node(NodeType.Normal, element_name);
        }
        static public Node CreateText(String txt)
        {
            return new Node(NodeType.Text, txt);
        }
    }

    public class XContext
    {
        public enum IndentType
        {
            Space4,
            Space2,
            Tab
        }
        private const IndentType _indent_type = IndentType.Space4;
        private const int _max = 32;
        static public IndentType Indent = _indent_type;
        static public int Max = _max;
        static private char[] _source;
        static XContext()
        {
            _source = new char[]
            {
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
            };
        }


        private System.IO.StreamWriter w;
        private char[] src;
        private int step;
        private System.Collections.Generic.Stack<Node> nodes;
        private NodeType last;
        private int GetDepth()
        {
            int depth = 0;
            foreach(Node n in nodes)
            {
                if(n.NodeType == NodeType.Normal) depth++;
            }
            return depth;
        }
        private bool OneLineExists()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                if (nodes.ElementAt(i).NodeType == NodeType.Normal)
                {
                    return false;
                }
                if(nodes.ElementAt(i).NodeType == NodeType.OneLine)
                {
                    return true;
                }
            }
            return false;
        }
        public XContext(System.IO.StreamWriter writer)
        {
            w = writer;
            nodes = new ();
            root_ = new Kid();
            kid_ = root_;
            if (Indent == IndentType.Space4 && Max == _max)
            {
                src = _source;
                step = 4;
            }
            else
            {
                if (Indent == IndentType.Space4)
                {
                    src = new char[Max * 4];
                    for (int i = 0; i < Max * 4; i++)
                    {
                        src[i] = ' ';
                    }
                    step = 4;
                }
                else if (Indent == IndentType.Space2)
                {
                    src = new char[Max * 2];
                    for (int i = 0; i < Max * 2; i++)
                    {
                        src[i] = ' ';
                    }
                    step = 2;
                }
                else
                {
                    src = new char[Max];
                    for (int i = 0; i < Max; i++)
                    {
                        src[i] = '\t';
                    }
                    step = 1;
                }
            }
            w.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            last = NodeType.Dummy;
        }

        internal void WriteOpen(Node n, Action<Dictionary<String, String>>? add_attributes)
        {
            if (last == NodeType.Empty)
            {
                throw new Exception("空要素の中には書き込めません。");
            }
            if (last == NodeType.Text)
            {
                throw new Exception("テキストノードは子供を持てません。");
            }
            if (n.NodeType == NodeType.Text && add_attributes != null)
            {
                throw new Exception("テキストノードに属性は付与できません。");
            }
            if (kid_.IsRoot() == false)
            {
                Kid k;
                if (n.NodeType == NodeType.Text)
                {
                    k = new Kid(kid_, n, () => { return ""; });
                }
                else
                {
                    k = new Kid(kid_, n, (attr) => { add_attributes?.Invoke(attr); });
                }
                kid_ = kid_.AppendChild(k);
                return;
            }
            // 無条件で書き出せる
            Dictionary<String, String> attr = new Dictionary<string, string>();
            if (add_attributes != null)
            {
                add_attributes(attr);
            }
            WriteOpen(n, attr);
        }
        internal void WriteOpen(Node n, Func<String>? insert_text)
        {
            if (last == NodeType.Empty)
            {
                throw new Exception("空要素の中には書き込めません。");
            }
            if (last == NodeType.Text)
            {
                throw new Exception("テキストノードは子供を持てません。");
            }
            if (kid_.IsRoot() == false)
            {
                Kid k;
                System.Diagnostics.Debug.Assert(n.NodeType == NodeType.Text);
                if(insert_text != null)
                {
                    k = new Kid(kid_, n, insert_text);
                }
                else
                {
                    k = new Kid(kid_, n);
                }
                kid_ = kid_.AppendChild(k);
                return;
            }
            // 無条件で書き出せる
            if (insert_text != null)
            {
                WriteOpen(Node.CreateText(insert_text()));
            }
            else
            {
                WriteOpen(n);
            }
        }
        internal Kid WriteOpen(Node n, Func<bool> valid, Action<Dictionary<String, String>>? add_attributes)
        {
            if (last == NodeType.Empty)
            {
                throw new Exception("空要素の中には書き込めません。");
            }
            if(last == NodeType.Text)
            {
                throw new Exception("テキストノードは子供を持てません。");
            }
            if (n.NodeType == NodeType.Text && add_attributes != null)
            {
                throw new Exception("テキストノードに属性は付与できません。");
            }
            Kid k = new Kid(kid_, n, valid, (attr) => { add_attributes?.Invoke(attr); });
            kid_ = kid_.AppendChild(k);
            return k;
        }
        internal Kid WriteOpen(Counter counter, Node n, Func<Counter, bool> valid, Action<Counter, Dictionary<String, String>>? add_attributes)
        {
            if (last == NodeType.Empty)
            {
                throw new Exception("空要素の中には書き込めません。");
            }
            if (last == NodeType.Text)
            {
                throw new Exception("テキストノードは子供を持てません。");
            }
            Kid k = new Kid(kid_, counter, n, valid, add_attributes);
            kid_ = kid_.AppendChild(k);
            return k;
        }
        internal Kid WriteOpen(Counter counter, Node n, Func<Counter, bool> valid, Func<Counter, String> insert_text)
        {
            if (last == NodeType.Empty)
            {
                throw new Exception("空要素の中には書き込めません。");
            }
            if (n.NodeType == NodeType.Text)
            {
                throw new Exception("テキストノード。");
            }
            Kid k = new Kid(kid_, counter, n, valid, insert_text);
            kid_ = kid_.AppendChild(k);
            return k;
        }
        internal void WriteClose()
        {
            if (kid_.IsRoot())
            {   // 無条件で書き出せる状態
                Node n = nodes.Pop();
                switch(n.NodeType)
                {
                    case NodeType.Empty:
                    case NodeType.Text:
                        break;
                    case NodeType.Normal:
                        w.Write(src, 0, step * GetDepth());
                        w.Write("</");
                        w.Write(n.Stuff);
                        w.WriteLine('>');
                        break;
                    case NodeType.OneLine:
                        w.Write("</");
                        w.Write(n.Stuff);
                        w.WriteLine('>');
                        break;
                }
                if (nodes.Count == 0)
                {
                    w.Close();
                    last = NodeType.Dummy;
                }
                else
                {
                    last = nodes.Peek().NodeType;
                }
                return;
            }

            kid_ = kid_.GetParent();
            if (kid_.IsRoot() == false)
            {   // まだ書き出せない
                return;
            }

            System.Diagnostics.Debug.Assert(root_.Child != null);
            WriteTree(root_.Child);
            root_.Clear();
            kid_ = root_;
        }

        internal void WriteTree(Kid k)
        {
            if (k.WriteOpen(this))
            {
                if (k.Child != null)
                {
                    WriteTree(k.Child);
                }
                k.WriteClose(this);
            }
            if (k.Sibling != null)
            {
                WriteTree(k.Sibling);
            }
        }
        internal void WriteOpen(Node n, Dictionary<String, String> attr)
        {
            if (kid_.IsRoot() == false)
            {
                Kid k = new Kid(kid_, n, attr);
                kid_ = kid_.AppendChild(k);
                return;
            }
            bool exist1l = OneLineExists();
            if (exist1l == false)
            {
                w.Write(src, 0, step * GetDepth());
            }
            if (n.NodeType != NodeType.Text)
            {
                w.Write('<');
            }
            w.Write(n.Stuff);
            foreach (KeyValuePair<String, String> kv in attr)
            {
                w.Write(' ');
                w.Write(kv.Key);
                w.Write("=\"");
                w.Write(kv.Value);
                w.Write("\"");
            }
            switch (n.NodeType)
            {
                case NodeType.Empty:
                    if (exist1l)
                    {
                        w.Write("/>");
                    }
                    else
                    {
                        w.WriteLine("/>");
                    }
                    break;
                case NodeType.OneLine:
                    w.Write(">");
                    break;
                case NodeType.Normal:
                    w.WriteLine(">");
                    break;
                case NodeType.Text:
                    if (exist1l)
                    {
                        w.Write(n.Stuff);
                    }
                    else
                    {
                        w.WriteLine(n.Stuff);
                    }
                    break;
            }
            nodes.Push(n);
            last = n.NodeType;
        }
        internal void WriteOpen(Node n)
        {
            if (kid_.IsRoot() == false)
            {
                Kid k = new Kid(kid_, n);
                kid_ = kid_.AppendChild(k);
                return;
            }
            bool exist1l = OneLineExists();
            if (exist1l == false)
            {
                w.Write(src, 0, step * GetDepth());
            }
            if (n.NodeType != NodeType.Text)
            {
                w.Write('<');
                w.Write(n.Stuff);
            }

            switch (n.NodeType)
            {
                case NodeType.Empty:
                    if (exist1l)
                    {
                        w.Write("/>");
                    }
                    else
                    {
                        w.WriteLine("/>");
                    }
                    break;
                case NodeType.OneLine:
                    w.Write(">");
                    break;
                case NodeType.Normal:
                    w.WriteLine(">");
                    break;
                case NodeType.Text:
                    if (exist1l)
                    {
                        w.Write(n.Stuff);
                    }
                    else
                    {
                        w.WriteLine(n.Stuff);
                    }
                    break;
            }
            last = n.NodeType;
            nodes.Push(n);
        }
        internal void WriteClose(String element_name)
        {
            Node pushed = nodes.Pop();
            if (pushed.NodeType != NodeType.OneLine && OneLineExists() == false)
            {
                w.Write(src, 0, step * GetDepth());
            }
            if (pushed.NodeType == NodeType.Text)
            {
                return;
            }
            System.Diagnostics.Debug.Assert(pushed.Stuff == element_name);
            w.Write("</");
            w.Write(element_name);
            w.WriteLine(">");
        }

        
        internal class Kid
        {
            private Counter? counter_;
            private Node node_;
            private Dictionary<String, String>? attributes_;
            private Func<bool>? single_validate_;
            private Func<Counter, bool>? sequence_validate_;
            private Action<Dictionary<String, String>>? single_add_attributes_;
            private Func<String>? single_insert_text_;
            private Action<Counter, Dictionary<String, String>>? sequence_add_attributes_;
            private Func<Counter, String>? sequence_insert_text_;
            private Kid? child_;
            private Kid? sibling_;
            private Kid parent_;
            internal Kid()
            {
                parent_ = this;
                node_ = new Node(NodeType.Dummy, "");
                single_validate_ = () => true;
            }
            public Kid(Kid parent, Node n)
            {
                parent_ = parent;
                node_ = n;
            }
            public Kid(Kid parent, Node n, Dictionary<String, String> attr)
            {
                parent_ = parent;
                node_ = n;
                attributes_ = attr;
            }
            public Kid(Kid parent, Node n, Action<Dictionary<String, String>>? add_attributes)
            {
                parent_ = parent;
                node_ = n;
                single_add_attributes_ = add_attributes;
            }
            public Kid(Kid parent, Node n, Func<String>? insert_text)
            {
                parent_ = parent;
                node_ = n;
                single_insert_text_ = insert_text;
            }
            public Kid(Kid parent, Node n, Func<bool> valid, Action<Dictionary<String, String>>? add_attributes)
            {
                parent_ = parent;
                node_ = n;
                single_validate_ = valid;
                single_add_attributes_ = add_attributes;
            }
            public Kid(Kid parent, Counter counter, Node n, Func<Counter, bool> valid, Action<Counter, Dictionary<String, String>>? add_attributes)
            {
                parent_ = parent;
                node_ = n;
                counter_ = counter;
                sequence_validate_ = valid;
                sequence_add_attributes_ = add_attributes;
            }
            public Kid(Kid parent, Counter counter, Node n, Func<Counter, bool> valid, Func<Counter, String> insert_text)
            {
                parent_ = parent;
                node_ = n;
                counter_ = counter;
                sequence_validate_ = valid;
                sequence_insert_text_ = insert_text;
            }
            public bool IsRoot() { return node_.NodeType == NodeType.Dummy; }
            public bool IsSingle() { return counter_ == null; }
            private bool OneLinerExists()
            {
                if (node_.NodeType == NodeType.Normal || node_.NodeType == NodeType.Dummy) return false;
                if (node_.NodeType == NodeType.OneLine) return true;
                return parent_.OneLinerExists();
            }
            public Kid? Child { get { return child_; } }
            public Kid? Sibling { get { return sibling_; } }
            public Kid AppendChild(Kid k)
            {
                if (child_ == null)
                {
                    child_ = k;
                }
                else
                {
                    Kid c = child_;
                    while (true)
                    {
                        if (c.sibling_ == null)
                        {
                            c.sibling_ = k;
                            break;
                        }
                        c = c.sibling_;
                    }
                }
                return k;
            }
            public Kid GetParent()
            {
                return parent_;
            }
            public void Clear()
            {
                child_ = null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="ctx"></param>
            /// <returns>子供を持てるなら true</returns>
            public bool WriteOpen(XContext ctx)
            {
                if(counter_ == null)
                {
                    if(single_validate_ != null)
                    {
                        if(single_validate_() == false)
                        {
                            return false;
                        }
                    }
                    if (node_.NodeType != NodeType.Text)
                    {
                        Dictionary<String, String> attr = new();
                        single_add_attributes_?.Invoke(attr);
                        ctx.WriteOpen(node_, attr);
                        return true;
                    }
                    else
                    {
                        if (single_insert_text_ != null)
                        {
                            ctx.WriteOpen(Node.CreateText(single_insert_text_()));
                        }
                        else
                        {
                            ctx.WriteOpen(node_);
                        }
                        return true;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Assert(sequence_validate_ != null);

                    while(sequence_validate_(counter_))
                    {
                        if (node_.NodeType != NodeType.Text)
                        {
                            Dictionary<String, String> attr = new();
                            sequence_add_attributes_?.Invoke(counter_, attr);
                            ctx.WriteOpen(node_, attr);
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(sequence_insert_text_ != null);
                            node_.Stuff = sequence_insert_text_(counter_);
                            ctx.WriteOpen(node_);
                        }

                        if (child_ != null)
                        {
                            ctx.WriteTree(child_);
                        }

                        ctx.WriteClose(node_.Stuff);
                        counter_.Increment();
                    }
                    counter_.Reset();
                    return false;
                }
            }
            public void WriteClose(XContext ctx)
            {
                ctx.WriteClose();
            }
        }
        private Kid root_;
        private Kid kid_;
    }

    public class Counter
    {
        private int counter_;
        public Counter() { counter_ = 0; }
        public int Get() { return counter_; }
        public override String ToString(){ return counter_.ToString(); }
        internal void Increment() { ++counter_; }
        internal void Reset() { counter_ = 0; }
    }
    public enum EmptyElement
    {
        Empty,
        HasChildren
    }

    public class ElementWriter : IDisposable
    {
        private XContext ctx_;
        private Node node;

        /// <summary>
        /// 属性を持たない要素を作る
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n"></param>
        public ElementWriter(XContext ctx, Node n)
        {
            ctx_ = ctx;
            node = n;
            ctx.WriteOpen(n);
        }
        /// <summary>
        /// あったりなかったりする属性を持たない要素を作る
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n"></param>
        /// <param name="valid"></param>
        public ElementWriter(XContext ctx, Node n, Func<bool>valid) : this(ctx, n, valid, null)
        {
        }
        /// <summary>
        /// 属性を持つ要素を作る
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n"></param>
        /// <param name="add_attributes"></param>
        public ElementWriter(XContext ctx, Node n, Action<Dictionary<String, String>> add_attributes)
        {
            ctx_ = ctx;
            node = n;
            ctx.WriteOpen(n, add_attributes);
        }
        /// <summary>
        /// あったりなかったりする要素を作る
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n"></param>
        /// <param name="valid"></param>
        /// <param name="add_attributes"></param>
        public ElementWriter(XContext ctx, Node n, Func<bool> valid, Action<Dictionary<String, String>>? add_attributes)
        {
            ctx_ = ctx;
            node = n;
            ctx.WriteOpen(n, valid, add_attributes);
        }
        public void Dispose()
        {
            ctx_.WriteClose();
        }
    }

    public class TextNodeWriter : IDisposable
    {
        private XContext ctx_;
        private Node node_;
        /// <summary>
        /// テキストを作ります
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        public TextNodeWriter(XContext ctx, String text)
        {
            ctx_ = ctx;
            node_ = Node.CreateText(text);
            ctx_.WriteOpen(node_);
        }
        public TextNodeWriter(XContext ctx, String text, Func<bool> valid)
        {
            ctx_ = ctx;
            node_ = Node.CreateText(text);
            ctx_.WriteOpen(node_, valid, null);
        }
        public TextNodeWriter(XContext ctx, Func<String> insert_text)
        {
            ctx_ = ctx;
            node_ = Node.CreateText("insert_text");
            ctx_.WriteOpen(node_, insert_text);
        }
        internal TextNodeWriter(XContext ctx, int index, Func<int, String> insert_text)
        {
            ctx_ = ctx;
            node_ = Node.CreateText(insert_text(index));
            ctx_.WriteOpen(node_);
        }
        public void Dispose()
        {
            ctx_.WriteClose();
        }
    }

    public class Seq : IDisposable
    {
        private XContext ctx_;
        private XContext.Kid kid_;
        private Node node_;
        private Func<Counter, bool> valid_;
        private Action<Counter, Dictionary<String, String>>? add_attributes_;
        private Func<Counter, String>? insert_text_;
        public Seq(XContext ctx, Counter counter, Node n,
            Func<Counter, bool> valid,
            Action<Counter, Dictionary<String, String>>? add_attributes)
        {
            ctx_ = ctx;
            node_ = n;
            kid_ = ctx.WriteOpen(counter, node_, valid, add_attributes);
            valid_ = valid;
            add_attributes_ = add_attributes;
        }
        public Seq(XContext ctx, Counter counter, Func<Counter, bool> valid, Func<Counter, String> insert_text)
        {
            ctx_ = ctx;
            node_ = Node.CreateText("");
            kid_ = ctx.WriteOpen(counter, node_, valid, insert_text);
            valid_ = valid;
            insert_text_ = insert_text;
        }
        public void Dispose()
        {
            ctx_.WriteClose();
        }
    }
}
