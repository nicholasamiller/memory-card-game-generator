using Blazored.LocalStorage;
using KmipCards.Client.Interfaces;
using KmipCards.Client.Services;
using KmipCards.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KmipCards.Client.Services
{
    public class MockDataRepository : ICardRepository
    {   
        private List<CardRecord> _cards = new List<CardRecord>();
        private string _name = "mock";
        public  MockDataRepository()
        {
            var testCardsData = @"一 (yì) one
二 (èr) two
三 (sān) three
人 (rén) a person, human
大 (dà) big
小 (xiǎo) small
个 (gè) measuring word
天( tiān) sky, days
上 (shàng) up, top
下 (xià) down, bottom
四 (sì) four
五 (wǔ) five
六 (liù) six
七 (qī) seven
八 (bā) eight
九 (jiǔ) nine
十 (shi) ten
口 (kǒu) mouth
日 (ri) sun, day
中 (zhōng) middle
长 (cháng) long
手 (shŏu) hand
山 (shān) mountain, hill
木 (mù) wood, tree
水 (shuǐ) water
火 (huǒ) fire
土 (tǔ) soil
石 (shí) rock, stone
月 (yuè) moon, month
云 (yún) cloud
目 (mù) eye
田 (tián) rice field
三角形 (sān jiǎo xíng) triangle
圆形 (yuán xíng) circle
方形 (fāng xíng) square
两个 (liǎng gè) a couple
天气 (tián qī) weather
下雪 (xià xuě) snowing
下雨 (xià yǔ) raining
晴朗 (qíng lǎng) sunny
多云 (duō yún) cloudy
太热了 (tài rè le) too hot
太冷了 (tài lěng le) too cold";

            var mockCardData = CardDataRepository.ParseFromTextLines(testCardsData);            
            
            foreach (var card in mockCardData)
            {
                this.AddCard(card);
            }

            this.CurrentlyLoadedListName = "mockRepo";
        }

        public string CurrentlyLoadedListName { get => _name; set => _name = value; }

        public event System.EventHandler<CardRepositoryChangedEventArgs> RepositoryChanged;

        public Task AddCard(CardRecord cardRecord)
        {
            _cards.Add(cardRecord);
            OnRepositoryChanged(null);
            return Task.CompletedTask;
        }

        public Task<List<CardRecord>> GetAllCards()
        {
            return Task.FromResult(this._cards);
        }

        public virtual void OnRepositoryChanged(CardRepositoryChangedEventArgs args)
        {
            RepositoryChanged?.Invoke(this, args);
        }

        public Task RemoveAllCards()
        {
            _cards = new List<CardRecord>();
            OnRepositoryChanged(null);
            return Task.CompletedTask; 
        }

        public Task RemoveCard(CardRecord cardRecord)
        {
            _cards.Remove(cardRecord);
            OnRepositoryChanged(null);
            return Task.CompletedTask;
        }
    }
}
