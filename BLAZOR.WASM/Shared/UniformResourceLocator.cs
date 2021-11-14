namespace BLAZOR.WASM.Shared
{
    public class UniformResourceLocator
    {
        public string Domain { get; set; }

        public string Page { get; set; }

        public void SetBase(string URI)
        {
            string[] Items = URI.Replace("/", "").Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            Domain = Items[0] + "://" + Items[1] + "/";
        }

        public void SetPage(string URI)
        {
            if (URI == "")
            {
                Page = "BLAZOR";
            }
            else
            {
                Page = URI;
            }
        }
    }
}
