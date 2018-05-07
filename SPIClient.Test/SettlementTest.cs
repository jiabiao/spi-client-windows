using System;
using Newtonsoft.Json.Linq;
using Xunit;
using SPIClient;

namespace Test
{
    public class SettlementTest
    {
        [Fact]
        public void TestParseDate()
        {
            JObject data = new JObject(
                new JProperty("settlement_period_start_time", "05:01"),
                new JProperty("settlement_period_start_date", "05Oct17"),
                
                new JProperty("settlement_period_end_time", "06:02"),
                new JProperty("settlement_period_end_date", "06Nov18"),
                
                new JProperty("settlement_triggered_time", "07:03:45"),
                new JProperty("settlement_triggered_date", "07Dec19")
                );
            var m = new Message("77", "event_y", data, false);

            var r = new Settlement(m);
            
            var startTime = r.GetPeriodStartTime();
            Assert.Equal(new DateTime(2017,10,5,5,1,0), startTime);
            
            var endTime = r.GetPeriodEndTime();
            Assert.Equal(new DateTime(2018,11,6,6,2,0), endTime);

            var trigTime = r.GetTriggeredTime();
            Assert.Equal(new DateTime(2019,12,7,7,3,45), trigTime);
            
        }
    }
}