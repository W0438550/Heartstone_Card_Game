namespace HCD_Classic.Model
{
    public class CardClass
    {

        //Attributes
        public string cardId { get; set; }
        public string dbfId { get; set; }
        public string name { get; set; }
        public string cardSet { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string artist { get; set; }
        public string playerClass { get; set; }
        public string locale { get; set; }
        public Mechanic[] mechanics { get; set; }
        public int cost { get; set; }
        public string img { get; set; }
        public string rarity { get; set; }
        public string flavor { get; set; }
        public bool collectible { get; set; }
        public string faction { get; set; }
        public int attack { get; set; }
        public int health { get; set; }
        public string race { get; set; }
        public bool elite { get; set; }
        public string spellSchool { get; set; }
        public int durability { get; set; }
        public string imgGold { get; set; }
    }

    public class Mechanic
    {

        public string name { get; set; }
    }

}
