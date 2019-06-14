namespace DropBoxApp
{
    internal class MyTable
    {
        public MyTable(double Size, string Name)
        {
            this.Size = Size;
            this.Name = Name;
        }
        public double Size { get; set; }
        public string Name { get; set; }
    }
}