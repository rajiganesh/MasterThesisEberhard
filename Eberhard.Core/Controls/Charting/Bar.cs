namespace Eberhard.Core.Controls.Charting
{
    internal struct Bar
    {
        public Bar(double percentage, GraphStatus status)
        {
            Percentage = percentage;
            Status = status;
        }

        public double Percentage { get; set; }

        public GraphStatus Status { get; set; }
    }
}
