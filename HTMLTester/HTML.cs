using SyncXW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace SyncHTML
{
    public class BasicAttribute
    {
        private String? id_;
        private List<String> classes_;
        public String? ID
        {
            get { return id_; }
            set { id_ = value; }
        }
        public BasicAttribute()
        {
            classes_ = new List<String>();
        }
        public BasicAttribute(String id)
        {
            id_ = id;
            classes_ = new List<String>();
        }
        public BasicAttribute AddClass(String className) { classes_.Add(className); return this; }
        public BasicAttribute RemoveClass(String className) { classes_.Remove(className); return this; }
        public virtual void WriteTo(Dictionary<String, String> attr)
        {
            if (id_ != null) attr.Add("id", id_);
            if (classes_.Count == 0) { }
            else if (classes_.Count == 1)
            {
                attr.Add("class", classes_.First());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                String joint = "";
                foreach (String s in classes_)
                {
                    sb.Append(joint); joint = " ";
                    sb.Append(s);
                }
                attr.Add("class", sb.ToString());
            }
        }
    }
    public class InputAttribute : BasicAttribute
    {
        private String name_;
        private String value_;
        public InputAttribute(String name, String value)
        {
            name_ = name;
            value_ = value;
        }
        public override void WriteTo(Dictionary<string, string> attr)
        {
            base.WriteTo(attr);
            attr.Add("name", name_);
            attr.Add("value", value_);
        }
    }

    public class HTML : SyncXW.ElementWriter
    {
        public HTML(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("html"))
        { }
    }
    public class HEAD : SyncXW.ElementWriter
    {
        public HEAD(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("head"))
        { }
    }
    public class TITLE : SyncXW.ElementWriter
    {
        public TITLE(SyncXW.XContext ctx, String title) : base(ctx, SyncXW.Node.CreateOneLineElement("title"))
        {
            using (SyncXW.TextNodeWriter t = new(ctx, title))
            {
            }
        }
    }
    public class SCRIPT : SyncXW.ElementWriter
    {
        public SCRIPT(SyncXW.XContext ctx, String src)
            : base(ctx, SyncXW.Node.CreateOneLineElement("script"), (attr) => { attr["type"] = "text/javascript"; attr["src"] = src; })
        { }
    }
    public class STYLESHEET : SyncXW.ElementWriter
    {
        public STYLESHEET(SyncXW.XContext ctx, String href)
            : base(ctx, SyncXW.Node.CreateEmptyElement("link"), attr => { attr["rel"] = "stylesheet"; attr["type"] = "text/css"; attr["href"] = href; })
        { }
    }
    public class BODY : SyncXW.ElementWriter
    {
        public BODY(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("body"))
        { }
    }
    public class DIV : SyncXW.ElementWriter
    {
        public DIV(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("div"))
        { }
        public DIV(SyncXW.XContext ctx, BasicAttribute id_and_class)
            : base(ctx, SyncXW.Node.CreateElement("div"), attr => { id_and_class.WriteTo(attr); })
        { }
    }
    public class P : SyncXW.ElementWriter
    {
        public P(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("p"))
        { }
        public P(SyncXW.XContext ctx, Func<bool> valid)
         : base(ctx, SyncXW.Node.CreateElement("p"), valid)
        { }
        public P(SyncXW.XContext ctx, BasicAttribute id_and_class)
            : base(ctx, SyncXW.Node.CreateElement("p"), attr => { id_and_class.WriteTo(attr); })
        { }
    }
    public class Ps : SyncXW.Seq
    {
        public Ps(SyncXW.XContext ctx, SyncXW.Counter counter, BasicAttribute id_and_class, Func<Counter, bool> valid)
            : base(ctx, counter, SyncXW.Node.CreateElement("p"), valid, (counter, attr) => { id_and_class.WriteTo(attr); })
        { }
    }
    public class BR : SyncXW.ElementWriter
    {
        public BR(SyncXW.XContext ctx)
            : base(ctx, SyncXW.Node.CreateEmptyElement("br"))
        { }
    }
    public class TABLE : SyncXW.ElementWriter
    {
        public TABLE(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("table"))
        { }
    }
    public class TR : SyncXW.ElementWriter
    {
        public TR(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("tr"))
        { }
        public TR(SyncXW.XContext ctx, BasicAttribute id_and_class)
            : base(ctx, SyncXW.Node.CreateElement("tr"), attr => { id_and_class.WriteTo(attr); })
        { }
    }
    public class TRs : SyncXW.Seq
    {
        public TRs(SyncXW.XContext ctx, SyncXW.Counter counter, Func<Counter, bool> valid, Action<Counter, Dictionary<String, String>> add_attribute)
            : base(ctx, counter, SyncXW.Node.CreateElement("tr"), valid, add_attribute)
        { }
    }
    public class TH : SyncXW.ElementWriter
    {
        public TH(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("th"))
        { }
        public TH(SyncXW.XContext ctx, BasicAttribute id_and_class)
            : base(ctx, SyncXW.Node.CreateElement("th"), attr => { id_and_class.WriteTo(attr); })
        { }
    }
    public class TD : SyncXW.ElementWriter
    {
        public TD(SyncXW.XContext ctx) : base(ctx, SyncXW.Node.CreateElement("td"))
        { }
        public TD(SyncXW.XContext ctx, BasicAttribute id_and_class)
            : base(ctx, SyncXW.Node.CreateElement("td"), attr => { id_and_class.WriteTo(attr); })
        { }
    }
    public class FORM : SyncXW.ElementWriter
    {
        public FORM(SyncXW.XContext ctx, String action, String method)
            : base(ctx, SyncXW.Node.CreateElement("form"), attr => { attr["action"] = action; attr["method"] = method; })
        { }
    }
    public class SELECT : SyncXW.ElementWriter
    {
        public SELECT(SyncXW.XContext ctx, String name)
            : base(ctx, SyncXW.Node.CreateElement("select"), attr => { attr["name"] = name; })
        { }
        public SELECT(SyncXW.XContext ctx, BasicAttribute id_and_class, String name)
            : base(ctx, SyncXW.Node.CreateElement("select"), attr => { id_and_class.WriteTo(attr); attr["name"] = name; })
        { }
        public SELECT(SyncXW.XContext ctx, Func<String> name)
            : base(ctx, SyncXW.Node.CreateElement("select"), attr => { attr["name"] = name(); })
        { }
    }
    public class SELECTs : SyncXW.Seq
    {
        public SELECTs(SyncXW.XContext ctx, SyncXW.Counter counter, Func<Counter, bool> valid, Func<Counter, String> name, Action<Counter, Dictionary<String, String>>? add_attribute)
            : base(ctx, counter, SyncXW.Node.CreateElement("select"), valid, (counter, attr) => { attr["name"] = name(counter); add_attribute?.Invoke(counter, attr); })
        { }
        public SELECTs(SyncXW.XContext ctx, SyncXW.Counter counter, Func<Counter, String?> valid, Action<Counter, Dictionary<String, String>>? add_attribute)
            : base(ctx, counter, SyncXW.Node.CreateElement("select"), (counter) => { return valid(counter) != null; }, (counter, attr) => { System.Diagnostics.Debug.Assert(valid != null); attr["name"] = valid(counter); add_attribute?.Invoke(counter, attr); })
        { }
    }
    public class OPTION : SyncXW.ElementWriter
    {
        public OPTION(SyncXW.XContext ctx, String value)
            : base(ctx, SyncXW.Node.CreateOneLineElement("option"), attr => { attr["value"] = value; })
        { }
    }
    public class OPTIONs : SyncXW.Seq
    {
        public OPTIONs(SyncXW.XContext ctx, SyncXW.Counter counter, Func<Counter, bool> valid, Func<Counter, String> name, Action<Counter, Dictionary<String, String>>? add_attribute)
            : base(ctx, counter, SyncXW.Node.CreateOneLineElement("option"), valid, (counter, attr) => { attr["value"] = name(counter); add_attribute?.Invoke(counter, attr); })
        { }
        public OPTIONs(SyncXW.XContext ctx, SyncXW.Counter counter, Func<Counter, String?> valid, Action<Counter, Dictionary<String, String>>? add_attribute)
            : base(ctx, counter, SyncXW.Node.CreateOneLineElement("option"), (counter) => { return valid(counter) != null; }, (counter, attr) => { System.Diagnostics.Debug.Assert(valid != null); attr["value"] = valid(counter); add_attribute?.Invoke(counter, attr); })
        { }
    }
}
