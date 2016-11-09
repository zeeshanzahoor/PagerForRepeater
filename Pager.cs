    public class Pager : CompositeControl
    {
        public Button Previous;
        public Button Next;
        public Button First;
        public Button Last;
        public TextBox CustomPageText;
        public Label TotalPageLabel;
        public Repeater Repeater;
        public Panel PagerElements;

        public string ControlId { get; set; }
        public int PageSize { get; set; }
        public decimal CurrentPage { get { return Convert.ToDecimal(this.ViewState["PageIndex"]); } set { this.ViewState["PageIndex"] = value; } }
        public decimal RecordCount { get { return Convert.ToDecimal(this.ViewState["RecordCount"]); } set { this.ViewState["RecordCount"] = value; } }

        public bool LoadOnPageLoad { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                PagerElements.Visible = true;
            }
            if (this.LoadOnPageLoad && !Page.IsPostBack)
            {
                this.Initialize();
                PagerElements.Visible = true;
            }
        }
        protected override void CreateChildControls()
        {
            PagerElements = new Panel();
            Previous = new Button(); Previous.ID = "Previous"; Previous.Click += PageChanged_Click; Previous.Text = "<"; Previous.CssClass = "pgr-previous";
            Next = new Button(); Next.ID = "Next"; Next.Click += PageChanged_Click; Next.Text = ">"; Next.CssClass = "pgr-next";
            CustomPageText = new TextBox(); CustomPageText.Text = (this.CurrentPage + 1).ToString();  CustomPageText.TextMode = TextBoxMode.Number; CustomPageText.TextChanged += CustomPageText_TextChanged; CustomPageText.Attributes.Add("onkeydown", "return (event.keyCode!=13);"); CustomPageText.AutoPostBack = true; CustomPageText.CssClass = "pgr-custom-txt";
            First = new Button(); First.ID = "First"; First.Click += PageChanged_Click; First.Text = "<<"; First.CssClass = "pgr-first";
            Last = new Button(); Last.ID = "Last"; Last.Click += PageChanged_Click; Last.Text = ">>"; Last.CssClass = "pgr-last";
            TotalPageLabel = new Label(); TotalPageLabel.Text = "/ " + Math.Ceiling(RecordCount / PageSize);

            Repeater = FindControl(Page, this.ControlId);

            PagerElements.Controls.Add(First);
            PagerElements.Controls.Add(Previous);
            PagerElements.Controls.Add(CustomPageText);
            PagerElements.Controls.Add(TotalPageLabel);
            PagerElements.Controls.Add(Next);
            PagerElements.Controls.Add(Last);
            this.Controls.Add(PagerElements);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            PagerElements.RenderControl(writer);
        }
        public void Initialize()
        {
            OnFillPager();
        }
        void CustomPageText_TextChanged(object sender, EventArgs e)
        {
            decimal LastPage = Math.Ceiling(RecordCount / PageSize) - 1;
            int val;
            if (String.IsNullOrEmpty(CustomPageText.Text))
            {
                val = 0;
            }
            else
                val = Convert.ToInt32(CustomPageText.Text) - 1;
            if (val > LastPage)
            {
                CurrentPage = LastPage;
            }
            else if (val < 0)
            {
                CurrentPage = 0;
            }
            else
            {
                CurrentPage = val;
            }
            CustomPageText.Text = (CurrentPage + 1).ToString();
            this.Initialize();
        }
        void PageChanged_Click(object sender, EventArgs e)
        {
            decimal LastPage = Math.Ceiling(RecordCount / PageSize) - 1;
            if ((sender as Button).ID == "Previous")
            {
                if (CurrentPage > 0)
                    CurrentPage = CurrentPage - 1;
            }
            else if ((sender as Button).ID == "Next")
            {
                if (CurrentPage < LastPage)
                    CurrentPage = CurrentPage + 1;
            }
            else if ((sender as Button).ID == "First")
            {
                CurrentPage = 0;
            }
            else if ((sender as Button).ID == "Last")
            {
                CurrentPage = LastPage;
            }
            CustomPageText.Text = (CurrentPage + 1).ToString();
            this.Initialize();
        }
        public event FillPagerEventHandler FillPager;

        public virtual void DataSource(PagerData PagerData)
        {
            var Data = PagerData;
            this.RecordCount = Data.PagerArgs.RecordCount;
            this.CurrentPage = Data.PagerArgs.PageIndex;
            this.Repeater.DataSource = Data.Data;
            this.Repeater.DataBind();
            this.PagerElements.Visible = this.RecordCount > this.PageSize;
            TotalPageLabel.Text = "/ " + Math.Ceiling(RecordCount / PageSize);
        }
        public virtual void OnFillPager()
        {
            if (FillPager != null)
            {
                var Data = FillPager(this, GetPagerArgs());
                this.RecordCount = Data.PagerArgs.RecordCount;
                this.CurrentPage = Data.PagerArgs.PageIndex;
                this.Repeater.DataSource = Data.Data;
                this.Repeater.DataBind();
                this.PagerElements.Visible = this.RecordCount > this.PageSize;
                TotalPageLabel.Text = "/ " + Math.Ceiling(RecordCount / PageSize);
            }
            else
            {
                throw new Exception("OnFillPager event is missing");
            }
        }
        public void RefreshData()
        {
            OnFillPager();
        }
        public PagerArgs GetPagerArgs()
        {
            var x = new PagerArgs(Convert.ToInt32(this.RecordCount), Convert.ToInt32(this.CurrentPage), this.PageSize);
            return x;
        }

        public Repeater FindControl(Control root, string id)
        {
            if (root != null)
            {
                if (root.ID == id) return root as Repeater;

                var foundControl = root.FindControl(id);
                if (foundControl != null) return foundControl as Repeater;

                foreach (Control childControl in root.Controls)
                {
                    foundControl = (Repeater)FindControl(childControl, id);
                    if (foundControl != null) return foundControl as Repeater;

                }
            }
            return null;
        }
    }
    public delegate PagerData FillPagerEventHandler(object sender, PagerArgs Args);
