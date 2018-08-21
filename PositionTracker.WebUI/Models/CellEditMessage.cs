namespace PositionTracker.WebUI.Models
{
    public class CellEditMessage
    {
        public string MessageType { get; set; }
        public string Coin { get; set; }
        public string Exchange { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}