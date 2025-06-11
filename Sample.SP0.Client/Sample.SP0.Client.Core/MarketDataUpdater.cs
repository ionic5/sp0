using HtmlAgilityPack;
using Sample.SP0.Client.Core.Entities;
using Sample.SP0.Client.Core.KiwoomApi;
using Sample.SP0.Client.Core.View;
using Sample.SP0.Client.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core
{
    public class MarketDataUpdater
    {
        private readonly IMainScene _mainScene;
        private readonly StockItemInfoRepository _stockItemInfoRepository;
        private readonly ILogger _logger;
        private readonly TokenStore _tokenStore;
        private readonly RestApiClient _restApiClient;
        private readonly UrlSet _urlSet;
        private readonly TypeTransformer _typeTransformer;
        private readonly UpdateLogRepository _updateLogRepository;

        private readonly DailyCandleStickRepository _dailyCandleStickRepository;
        private readonly MaPointRepository _maPointRepository;
        private readonly MfiPointRepository _mfiPointRepository;
        private readonly MacdPointRepository _macdPointRepository;
        private readonly BBPointRepository _bbPointRepository;
        private readonly RsiPointRepository _rsiPointRepository;
        private readonly ObvPointRepository _obvPointRepository;

        private readonly MainPanelControllerFactory _mainPanelControllerFactory;

        public MarketDataUpdater(IMainScene mainScene, StockItemInfoRepository stockItemInfoRepository, ILogger logger,
            TokenStore tokenStore, RestApiClient restApiClient, UrlSet urlSet, TypeTransformer typeTransformer,
            UpdateLogRepository updateLogRepository, DailyCandleStickRepository dailyCandleStickRepository,
            MaPointRepository maPointRepository, MfiPointRepository mfiPointRepository, MacdPointRepository macdPointRepository,
            BBPointRepository bbPointRepository, RsiPointRepository rsiPointRepository, ObvPointRepository obvPointRepository, MainPanelControllerFactory mainPanelControllerFactory)
        {
            _mainScene = mainScene;
            this._stockItemInfoRepository = stockItemInfoRepository;
            _logger = logger;
            this._tokenStore = tokenStore;
            this._restApiClient = restApiClient;
            this._urlSet = urlSet;
            this._typeTransformer = typeTransformer;
            _updateLogRepository = updateLogRepository;
            _dailyCandleStickRepository = dailyCandleStickRepository;
            _maPointRepository = maPointRepository;
            _mfiPointRepository = mfiPointRepository;
            _macdPointRepository = macdPointRepository;
            _bbPointRepository = bbPointRepository;
            _rsiPointRepository = rsiPointRepository;
            _obvPointRepository = obvPointRepository;
            _mainPanelControllerFactory = mainPanelControllerFactory;
        }

        public async void UpdateMarketData()
        {
            var loadingPanel = _mainScene.ShowLoadingPanel();

            loadingPanel.SetMessage("Update stock item information.");
            await TryUpdateStockItemInfoRepository();

            loadingPanel.SetMessage("Update daily candle stick.");
            await TryUpdateDailyCandleStickRepository();

            loadingPanel.SetMessage("Update MA.");
            await TryUpdateMaPointRepository();

            loadingPanel.SetMessage("Update MFI.");
            await TryUpdateMfiPointRepository();

            loadingPanel.SetMessage("Update MACD.");
            await TryUpdateMacdPointRepository();

            loadingPanel.SetMessage("Update bollinger bands.");
            await TryUpdateBollingerBandPointRepository();

            loadingPanel.SetMessage("Update RSI.");
            await TryUpdateRsiPointRepository();

            loadingPanel.SetMessage("Update OBV.");
            await TryUpdateObvPointRepository();

            var mainPanel = _mainScene.ShowMainPanel();

            var ctrl = _mainPanelControllerFactory.Create(mainPanel);
            ctrl.Setup();
        }


        private async Task TryUpdateDailyCandleStickRepository()
        {
            var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
            foreach (var item in stockItemInfos)
            {
                var stockCode = item.StockCode;

                var candlesticks = await GetDailyCandlestacks(stockCode, DateTime.Now.AddDays(-1), 120);

                var newRrows = new List<Entities.DailyCandlestick>();
                foreach (var candlestick in candlesticks)
                {
                    newRrows.Add(new Entities.DailyCandlestick
                    {
                        StockCode = stockCode,
                        TradeDate = _typeTransformer.ConvertToDateTime(candlestick.Dt),
                        OpenPrice = Convert.ToDecimal(candlestick.OpenPric),
                        ClosePrice = Convert.ToDecimal(candlestick.CurPrc),
                        HighPrice = Convert.ToDecimal(candlestick.HighPric),
                        LowPrice = Convert.ToDecimal(candlestick.LowPric),
                        Volume = Convert.ToDecimal(candlestick.TrdeQty)
                    });
                }

                var rows = _dailyCandleStickRepository.SelectAll(stockCode);
                newRrows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                _dailyCandleStickRepository.Insert(newRrows);
            }
        }

        public async Task<IEnumerable<KiwoomApi.DailyCandlestick>> GetDailyCandlestacks(string stockCode, DateTime date, int count = 1)
        {
            var response = await RequestDailyCandlesticks(stockCode, date, string.Empty);
            if (response.Body == null)
                return new List<KiwoomApi.DailyCandlestick>();

            var lastNextKey = string.Empty;
            var result = response.Body.StkDtPoleChartQry.ToList();
            while (result.Count < count)
            {
                if (response.Headers == null)
                    break;

                var contYN = response.Headers.FirstOrDefault(h => h.Key == "cont-yn").Value.FirstOrDefault();
                if (string.IsNullOrEmpty(contYN) || contYN.ToLower() != "y")
                    break;

                var nextKey = response.Headers.FirstOrDefault(h => h.Key == "next-key").Value.FirstOrDefault();
                if (string.IsNullOrEmpty(nextKey) || lastNextKey == nextKey)
                {
                    lastNextKey = string.Empty;
                    break;
                }
                lastNextKey = nextKey;

                response = await RequestDailyCandlesticks(stockCode, date, nextKey);
                if (response.Body == null)
                    break;

                if (response.Body.StkDtPoleChartQry.Count() > 0)
                    result.AddRange(response.Body.StkDtPoleChartQry);
            }

            return result.OrderByDescending(item => item.Dt).Take(count);
        }

        private async Task<Response<DailyCandlestickBody>> RequestDailyCandlesticks(string stockCode, DateTime date, string nextKey)
        {
            var headers = new Dictionary<string, string>();
            headers["authorization"] = $"{_tokenStore.TokenType} {_tokenStore.Token}";
            headers["api-id"] = "ka10081";
            if (!string.IsNullOrEmpty(nextKey))
            {
                headers["next-key"] = nextKey;
                headers["cont-yn"] = "Y";
            }

            var requestBody = new
            {
                stk_cd = stockCode,
                base_dt = date.ToString("yyyyMMdd"),
                upd_stkpc_tp = "1"
            };
            var response = await _restApiClient.SendRequestAsync<DailyCandlestickBody>(_urlSet.Chart,
                HttpMethod.Post, requestBody, headers);

            return response;
        }

        private async Task TryUpdateMaPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRrows = new List<MaPoint>();
                    for (int i = 0; i < candleSticks.Count; i++)
                    {
                        var candleStick = candleSticks[i];
                        var entity = new MaPoint();
                        entity.TradeDate = candleStick.TradeDate;
                        entity.StockCode = candleStick.StockCode;
                        entity.Ma5 = CalcMA(5, candleSticks, i);
                        entity.Ma20 = CalcMA(20, candleSticks, i);
                        entity.Ma60 = CalcMA(60, candleSticks, i);

                        newRrows.Add(entity);
                    }

                    var rows = _maPointRepository.SelectAll(stockCode);
                    newRrows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _maPointRepository.Insert(newRrows);
                }
            });
        }

        private decimal CalcMA(int dayCount, List<Entities.DailyCandlestick> candleSticks, int index)
        {
            var dayIndex = dayCount - 1;
            return index >= dayIndex ? candleSticks.Skip(index - dayIndex).Take(dayCount).Average(e => e.ClosePrice) : 0;
        }

        private async Task TryUpdateMfiPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRrows = CreateMfiPioints(candleSticks);

                    var rows = _mfiPointRepository.SelectAll(stockCode);
                    newRrows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _mfiPointRepository.Insert(newRrows);
                }
            });
        }

        private List<MfiPoint> CreateMfiPioints(List<Entities.DailyCandlestick> candleSticks)
        {
            int period = 14;
            var newRrows = new List<MfiPoint>();

            for (int i = period; i < candleSticks.Count; i++)
            {
                decimal positiveFlow = 0;
                decimal negativeFlow = 0;

                for (int j = i - period; j < i; j++)
                {
                    decimal typicalPrice = (candleSticks[j].HighPrice + candleSticks[j].LowPrice + candleSticks[j].ClosePrice) / 3;
                    decimal moneyFlow = typicalPrice * candleSticks[j].Volume;

                    if (j > 0)
                    {
                        decimal prevTypicalPrice = (candleSticks[j - 1].HighPrice + candleSticks[j - 1].LowPrice + candleSticks[j - 1].ClosePrice) / 3;

                        if (typicalPrice > prevTypicalPrice)
                            positiveFlow += moneyFlow;
                        else if (typicalPrice <= prevTypicalPrice)
                            negativeFlow += moneyFlow;
                    }
                }

                decimal moneyFlowRatio = (negativeFlow == 0) ? 100 : positiveFlow / negativeFlow;
                var mfi = (100 - (100 / (1 + moneyFlowRatio)));

                var candleStick = candleSticks[i];

                var entity = new MfiPoint();
                entity.TradeDate = candleStick.TradeDate;
                entity.StockCode = candleStick.StockCode;
                entity.Mfi = mfi;

                newRrows.Add(entity);
            }

            return newRrows;
        }

        private async Task TryUpdateMacdPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRows = CreateMacdPoints(candleSticks);

                    var rows = _macdPointRepository.SelectAll(stockCode);
                    newRows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _macdPointRepository.Insert(newRows);
                }
            });
        }

        List<MacdPoint> CreateMacdPoints(List<Entities.DailyCandlestick> dailyCandleSticks)
        {
            const int shortPeriod = 12;
            const int longPeriod = 26;
            const int signalPeriod = 9;

            List<MacdPoint> macdPoints = new List<MacdPoint>();

            if (dailyCandleSticks == null || dailyCandleSticks.Count < longPeriod)
                return macdPoints;

            decimal shortMultiplier = 2m / (shortPeriod + 1);
            decimal longMultiplier = 2m / (longPeriod + 1);
            decimal signalMultiplier = 2m / (signalPeriod + 1);

            decimal shortEma = dailyCandleSticks.Take(shortPeriod).Average(c => c.ClosePrice);
            decimal longEma = dailyCandleSticks.Take(longPeriod).Average(c => c.ClosePrice);
            decimal signalEma = 0;

            foreach (var candle in dailyCandleSticks.Skip(longPeriod))
            {
                decimal closePrice = candle.ClosePrice;

                shortEma = (closePrice - shortEma) * shortMultiplier + shortEma;
                longEma = (closePrice - longEma) * longMultiplier + longEma;

                decimal macd = shortEma - longEma;
                signalEma = (macd - signalEma) * signalMultiplier + signalEma;
                decimal histogram = macd - signalEma;

                macdPoints.Add(new MacdPoint
                {
                    StockCode = candle.StockCode,
                    TradeDate = candle.TradeDate,
                    Macd = macd,
                    Signal = signalEma,
                    Histogram = histogram
                });
            }

            return macdPoints;
        }

        private async Task TryUpdateBollingerBandPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRows = CreateBollingerBandPoints(candleSticks);

                    var rows = _bbPointRepository.SelectAll(stockCode);
                    newRows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _bbPointRepository.Insert(newRows);
                }
            });
        }

        List<BBPoint> CreateBollingerBandPoints(List<Entities.DailyCandlestick> dailyCandlesticks)
        {
            const int period = 20;
            List<BBPoint> bollingerBandPoints = new List<BBPoint>();

            if (dailyCandlesticks == null || dailyCandlesticks.Count < period)
                return bollingerBandPoints;

            Queue<decimal> priceQueue = new Queue<decimal>();

            foreach (var candle in dailyCandlesticks)
            {
                decimal closePrice = candle.ClosePrice;

                if (priceQueue.Count == period)
                    priceQueue.Dequeue();
                priceQueue.Enqueue(closePrice);

                if (priceQueue.Count < period)
                    continue;

                decimal mean = priceQueue.Average();
                decimal standardDeviation = (decimal)Math.Sqrt(priceQueue.Select(p => Math.Pow((double)(p - mean), 2)).Average());

                decimal upperBand = mean + (standardDeviation * 2);
                decimal lowerBand = mean - (standardDeviation * 2);

                bollingerBandPoints.Add(new BBPoint
                {
                    StockCode = candle.StockCode,
                    TradeDate = candle.TradeDate,
                    Middle = mean,
                    Upper = upperBand,
                    Lower = lowerBand
                });
            }

            return bollingerBandPoints;
        }

        private async Task TryUpdateRsiPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRrows = CreateRsiPoints(candleSticks);

                    var rows = _rsiPointRepository.SelectAll(stockCode);
                    newRrows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _rsiPointRepository.Insert(newRrows);
                }
            });
        }

        private List<RsiPoint> CreateRsiPoints(List<Entities.DailyCandlestick> candlesticks)
        {
            const int period = 14;
            List<RsiPoint> rsiPoints = new List<RsiPoint>();

            if (candlesticks == null || candlesticks.Count < period)
                return rsiPoints;

            decimal avgGain = 0;
            decimal avgLoss = 0;

            for (int i = 1; i < period; i++)
            {
                decimal change = candlesticks[i].ClosePrice - candlesticks[i - 1].ClosePrice;
                if (change > 0)
                    avgGain += change;
                else
                    avgLoss -= change; // 음수 값을 양수로 변환
            }

            avgGain /= period;
            avgLoss /= period;

            for (int i = period; i < candlesticks.Count; i++)
            {
                decimal change = candlesticks[i].ClosePrice - candlesticks[i - 1].ClosePrice;
                decimal gain = change > 0 ? change : 0;
                decimal loss = change < 0 ? -change : 0;

                avgGain = ((avgGain * (period - 1)) + gain) / period;
                avgLoss = ((avgLoss * (period - 1)) + loss) / period;

                decimal rs = avgLoss == 0 ? 100 : avgGain / avgLoss;
                decimal rsi = 100 - (100 / (1 + rs));

                rsiPoints.Add(new RsiPoint
                {
                    StockCode = candlesticks[i].StockCode,
                    TradeDate = candlesticks[i].TradeDate,
                    Rsi = rsi
                });
            }

            return rsiPoints;
        }

        private async Task TryUpdateObvPointRepository()
        {
            await Task.Run(() =>
            {
                var stockItemInfos = _stockItemInfoRepository.SelectTop100ByMarketCap();
                foreach (var item in stockItemInfos)
                {
                    var stockCode = item.StockCode;
                    var candleSticks = _dailyCandleStickRepository.SelectAll(stockCode).ToList();

                    var newRrows = CreateObvPoints(candleSticks);

                    var rows = _obvPointRepository.SelectAll(stockCode);
                    newRrows.RemoveAll(entry => rows.Any(row => row.TradeDate == entry.TradeDate));

                    _obvPointRepository.Insert(newRrows);
                }
            });
        }

        private List<ObvPoint> CreateObvPoints(List<Entities.DailyCandlestick> candlesticks)
        {
            List<ObvPoint> obvPoints = new List<ObvPoint>();

            if (candlesticks == null || candlesticks.Count == 0)
                return obvPoints;

            decimal previousObv = 0;

            foreach (var candle in candlesticks)
            {
                ObvPoint obvPoint = new ObvPoint
                {
                    StockCode = candle.StockCode,
                    TradeDate = candle.TradeDate
                };

                if (obvPoints.Count > 0)
                {
                    var previousCandle = candlesticks[obvPoints.Count - 1];

                    if (candle.ClosePrice > previousCandle.ClosePrice)
                    {
                        previousObv += candle.Volume;
                    }
                    else if (candle.ClosePrice < previousCandle.ClosePrice)
                    {
                        previousObv -= candle.Volume;
                    }
                }
                else
                {
                    previousObv = candle.Volume;
                }

                obvPoint.Obv = previousObv;
                obvPoints.Add(obvPoint);
            }

            return obvPoints;
        }

        private async Task TryUpdateStockItemInfoRepository()
        {
            if (_stockItemInfoRepository.IsEmpty() || IsUpdateRequired(RepositoryName.StockItemInfo))
                await UpdateStockItemInfoRepository();
        }

        private async Task UpdateStockItemInfoRepository()
        {
            var stockNameToCodeMap = await GetStockNameToCodeMap();

            string url = "https://stock.richinfohub.com/kospi200info/";

            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            var rows = doc.DocumentNode.SelectNodes("//table//tr");

            var entities = new List<Entities.StockItemInfo>();
            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns != null && columns.Count > 1)
                {
                    var companyName = columns[1].InnerText.Trim();
                    if (!stockNameToCodeMap.ContainsKey(companyName))
                    {
                        _logger.Info($"Failed to find stock code for {companyName}.");
                        continue;
                    }

                    var marketCap = _typeTransformer.ConvertToNumber(columns[4].InnerText.Trim());
                    var entity = new Entities.StockItemInfo();
                    entity.CompanyName = companyName;
                    entity.StockCode = stockNameToCodeMap[companyName];
                    entity.MarketCap = marketCap;
                    entities.Add(entity);
                }
            }

            _stockItemInfoRepository.Update(entities);
            _updateLogRepository.Update(new UpdateLog(RepositoryName.StockItemInfo, DateTime.Now));

            _logger.Info("StockItemInfo updated.");
        }

        private async Task<Dictionary<string, string>> GetStockNameToCodeMap()
        {
            var headers = new Dictionary<string, string>();
            headers["authorization"] = $"{_tokenStore.TokenType} {_tokenStore.Token}";
            headers["api-id"] = "ka10099";
            var requestBody = new
            {
                mrkt_tp = "0"
            };
            var response = await _restApiClient.SendRequestAsync<StockItemInfoListBody>(_urlSet.StockInfo,
                HttpMethod.Post, requestBody, headers);

            var result = new Dictionary<string, string>();
            foreach (var item in response.Body.StockItemInfos)
                result[item.Name] = item.Code;
            return result;
        }

        private bool IsUpdateRequired(RepositoryName repositoryName)
        {
            var entity = _updateLogRepository.Select(repositoryName);
            return entity.LastUpdated.Date < DateTime.Now.Date;
        }
    }
}
