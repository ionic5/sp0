using Sample.SP0.Client.Core.View;

namespace Sample.SP0.Client.Core
{
    public class MainPanelControllerFactory
    {
        private readonly DailyCandleStickRepository _dailyCandleStickRepository;
        private readonly MaPointRepository _maPointRepository;
        private readonly BBPointRepository _bbPointRepository;
        private readonly RsiPointRepository _rsiPointRepository;
        private readonly MfiPointRepository _mfiPointRepository;
        private readonly ObvPointRepository _obvPointRepository;
        private readonly MacdPointRepository _macdPointRepository;
        private readonly StockItemInfoRepository _stockItemInfoRepository;

        public MainPanelControllerFactory(DailyCandleStickRepository dailyCandlestickRepository, MaPointRepository maPointRepository, BBPointRepository bbPointRepository, RsiPointRepository rsiPointRepository, MfiPointRepository mfiPointRepository, ObvPointRepository obvPointRepository, MacdPointRepository macdPointRepository, StockItemInfoRepository stockItemInfoRepository)
        {
            _dailyCandleStickRepository = dailyCandlestickRepository;
            _maPointRepository = maPointRepository;
            _bbPointRepository = bbPointRepository;
            _rsiPointRepository = rsiPointRepository;
            _mfiPointRepository = mfiPointRepository;
            _obvPointRepository = obvPointRepository;
            _macdPointRepository = macdPointRepository;
            _stockItemInfoRepository = stockItemInfoRepository;
        }

        public MainPanelController Create(IMainPanel mainPanel)
        {
            return new MainPanelController(mainPanel, _dailyCandleStickRepository,
                _maPointRepository, _bbPointRepository, _rsiPointRepository, _mfiPointRepository,
                _obvPointRepository, _macdPointRepository, _stockItemInfoRepository);
        }
    }
}
