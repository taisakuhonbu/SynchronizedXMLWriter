using SyncHTML;

StreamWriter writer = new StreamWriter("index.css");
writer.Write(@"
tr.even-row
{
	background-color: #EDEDED;
}
tr.odd-row
{
	background-color: #9BE8F0;
}");
writer.Close();


StreamWriter w = new ("index.html");
SyncXW.XContext ctx = new (w);

using (HTML html = new(ctx))
{
    using (HEAD head = new(ctx))
    {
        using (TITLE title = new(ctx, "Test HTML")) { }
        using (SCRIPT script0 = new(ctx, "index.js")) { }
        using (STYLESHEET link0 = new(ctx, "index.css")) { }
    }
    using (BODY body = new(ctx))
    {
        using (DIV d0 = new(ctx))
        {
            SyncXW.Counter counter0 = new();
            using (TABLE table = new(ctx))
            {
                using (TRs rows = new(ctx, counter0, index => { return index.Get() < 4; }, (counter, attr) => { if (counter.Get() % 2 == 0) { attr["class"] = "even-row"; } else { attr["class"] = "odd-row"; } }))
                {
                    using (TH th = new(ctx))
                    {
                        using (new SyncXW.TextNodeWriter(ctx, () => { return "Table Head " + counter0.Get().ToString(); }))
                        {
                        }
                    }
                    using (TD td = new(ctx))
                    {
                        using (new SyncXW.TextNodeWriter(ctx, () => { return "Table Data " + counter0.Get().ToString(); }))
                        {
                        }
                    }
                }
            }
        }
        using (DIV d1 = new(ctx))
        {
            SyncXW.Counter index1 = new();
            SyncXW.Counter index2 = new();
            String[] names = ["name-X", "name-Y"];
            String?[][] options = [["X1", "X2", null], ["Y1", "Y2", "Y3", null]];
            using (FORM form = new(ctx, "/cgi-bin/app.cgi", "POST"))
            {
                using (Ps p = new Ps(ctx, index1, new BasicAttribute().AddClass("dropdown"), (counter) => { return counter.Get() < names.Length; }))
                {
                    using (SELECT select = new(ctx, () => { return names[index1.Get()]; }))
                    {
                        using (OPTIONs option = new(ctx, index2, (counter) => { return options[index1.Get()][counter.Get()]; }, (counter, attr) => { if (counter.Get() == 1) { attr["selected"] = "selected"; } }))
                        {
                            using (new SyncXW.TextNodeWriter(ctx, () => { return $"Option {index1.ToString()}, {index2.ToString()}"; }))
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}

