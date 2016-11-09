    [Serializable]
    public class PagerArgs
    {
        public PagerArgs() { }
        public PagerArgs(int RecordCount, int PageIndex, int PageSize)
        {
            this.RecordCount = RecordCount;
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
        }
        public int RecordCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public static class PagerArgsExtensions
    {
        public static PagerData ApplyArgs<T>(this PagerArgs Args, IQueryable<T> candidates)
        {
            Args.RecordCount = candidates.Count();
            candidates = candidates.Skip(Args.PageIndex * Args.PageSize).Take(Args.PageSize);
            PagerData data = new PagerData();
            data.PagerArgs = Args;
            try
            {
                data.Data = candidates.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return data;
        }
    }
