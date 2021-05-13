using HCD_Classic.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Media.Core;

namespace HCD_Classic.ViewModel
{
    public class HSViewModel : INotifyPropertyChanged
    {


        /// <summary>
        /// Event handler to follow up changes on the focused element from the ScrollView
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public static List<CardClass> cardSet;
        private CardClass _selectedCard;
        private string _filter;
        public BitmapImage bitmapImage;

        //Default image for the display
        public Uri defaultUri = new Uri("https://static.wikia.nocookie.net/hearthstone/images/c/c4/Card_back-Default.png/revision/latest?cb=20140823204025");

        /// <summary>
        /// Getter/Setter for content display
        /// </summary>
        public string nameLabel { get; set; }
        public string artistLabel { get; set; }
        public string typeLabel { get; set; }
        public int costLabel { get; set; }
        public int healthLabel { get; set; }
        public int attackLabel { get; set; }
        public string textLabel { get; set; }
        public string imgLabel { get; set; }
        public ObservableCollection<CardClass> Cards { get; set; }



        public HSViewModel()
        {
            Cards = new ObservableCollection<CardClass>();

            //Populate the collection with the information from the LIST-API
            CreateAListOfCards();
        }

        /// <summary>
        /// Retrieving information from the API and place it on a List
        /// </summary>
        public static void FetchApi()
        {
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(
                string.Format("https://omgvamp-hearthstone-v1.p.rapidapi.com/cards/sets/Journey%20to%20Un%27Goro")
                );

            webReq.Method = "GET";
            webReq.Headers["x-rapidapi-key"] = "b2594b1a5fmsh1d3977bc30a580ap11e4c0jsnb371aab02166";
            webReq.Headers["x-rapidapi-host"] = "omgvamp-hearthstone-v1.p.rapidapi.com";

            HttpWebResponse WebResp = (HttpWebResponse)webReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }

            cardSet = JsonConvert.DeserializeObject<List<CardClass>>(jsonString);

            //Filter to remove Duplicates
            cardSet = cardSet.GroupBy(x => x.name).Select(x => x.First()).ToList();

            //Debug.WriteLine(cardSet.Count);
        }

        /// <summary>
        /// Getter/Setter of the current selected item from the scrollview to populate it's information on the display page
        /// </summary>
        public CardClass SelectedFile
        {
            get { return _selectedCard; }

            set
            {
                _selectedCard = value;
                if (value != null)
                {
                    nameLabel = value.name ?? "";
                    typeLabel = value.type ?? "";
                    artistLabel = value.artist ?? "";
                    costLabel = value.cost;
                    healthLabel = value.health;
                    attackLabel = value.attack;
                    textLabel = value.text ?? "";

                    if (value.img != null)
                    {
                        bitmapImage = new BitmapImage(new Uri(value.img));
                    }
                    else
                    {
                        bitmapImage = new BitmapImage(defaultUri);
                    }
                };

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("nameLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("artistLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("typeLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("costLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("healthLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("attackLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("textLabel"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("bitmapImage"));
            }
        }

        /// <summary>
        /// Search bar filtering 1/2
        /// </summary>
        public string filter
        {
            get { return _filter; }
            set
            {
                if (value == _filter) { return; }
                _filter = value;
                PerformFiltering();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(filter)));
            }
        }
        /// <summary>
        /// Search bar filtering 2/2 - Populates the scrollview with the correct values
        /// </summary>
        public void PerformFiltering()
        {
            Cards.Clear();

            //If filter is null set it to ""
            if (_filter == null)
            {
                _filter = "";
            }

            // filter the value with lower case and trim string
            var lowerCaseFilter = filter.ToLowerInvariant().Trim();

            //Use LINQ query to get all file names
            var result =
                cardSet.Where(d => d.name.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            //Get list of values in current filtered list that we want to remove
            //(ie. don't meet new filter criteria)
            var toRemove = Cards.Except(result).ToList();

            //Loop to remove items that fail filter
            foreach (var x in toRemove)
            {
                Cards.Remove(x);
            }

            var resultCount = result.Count;

            // Add back in correct order.
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Cards.Count || !Cards[i].Equals(resultItem))
                {
                    Cards.Insert(i, resultItem);
                }
            }
        }

        public void CreateAListOfCards()
        {

            for (int i = 0; i < cardSet.Count; i++)
            {
                CardClass card = (CardClass)cardSet[i];
                Cards.Insert(i, card);
            }

        }

    }
}
