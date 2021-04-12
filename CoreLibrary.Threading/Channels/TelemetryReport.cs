using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreLibrary.Threading.Channels
{
    [DataContract, Serializable]
    public class TelemetryReport
    {
        [DataMember, JsonProperty("RECEIVER")]
        public QueueTelemetryReport ReceiverReport { set; get; }

        [DataMember, JsonProperty("SNAPSHOT")]
        public SnapshotTime SnapshotTime { set; get; }

        [DataMember, JsonProperty("DEVICE")]
        public QueueHandlerTelemetryReport DeviceReport { set; get; }

        [DataMember, JsonProperty("DEV_DATA")]
        public QueueHandlerTelemetryReport DeviceDataReport { set; get; }

        [DataMember, JsonProperty("OUT_DATA")]
        public QueueHandlerTelemetryReport BackgroundReport { set; get; }

        [DataMember, JsonProperty("EVT")]
        public QueueHandlerTelemetryReport EventsReport { set; get; }

        [DataMember, JsonProperty("MSG")]
        public QueueHandlerTelemetryReport EventsMessages { set; get; }

        [DataMember, JsonProperty("STORE")]
        public QueueHandlerTelemetryReport StoreReport { set; get; }

        [DataMember, JsonProperty("CPU")]
        public double CpuUsage { set; get; }
    }

    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            base.DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        }
    }

    [DataContract, Serializable]
    public class SnapshotTime
    {
        [DataMember, JsonProperty("FROM", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? From { set; get; }

        [DataMember, JsonProperty("TO"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime To { set; get; }
    }

    [DataContract, Serializable]
    public class QueueTelemetryReport
    {
        [DataMember, JsonProperty("CONSUMERS")]
        public int ConsumersCount { set; get; }

        [DataMember, JsonProperty("AVERAGE_TIME")]
        public long AverageTimePerOperationMs { set; get; }
        
        [DataMember, JsonProperty("DROPPED")]
        public long DroppedItems { set; get; }

        [DataMember, JsonProperty("LENGTH")]
        public int Length { get; set; }

        [DataMember, JsonProperty("TOTAL_PENDING")]
        public long TotalPendingTimeMs { get; set; }
        
        [DataMember, JsonProperty("MAX_TIME")]
        public long MaxTimeMs { get; set; }

        [DataMember, JsonProperty("MIN_TIME")]
        public long MinTimeMs { get; set; }

        [DataMember, JsonProperty("OPERATIONS")]
        public long OperationCount { get; set; }
    }

    [DataContract, Serializable]
    public class HandlerTelemetryReport
    {
        [DataMember, JsonProperty("OP", NullValueHandling = NullValueHandling.Ignore)]
        public OperationTelemetryData Operation { set; get; }

        [DataMember, JsonProperty("HANDLERS")]
        public int HandlersCount { set; get; }
        
    }

    [DataContract, Serializable]
    public class QueueHandlerTelemetryReport : QueueTelemetryReport
    {
        [DataMember, JsonProperty("HANDLER", NullValueHandling = NullValueHandling.Ignore)]
        public HandlerTelemetryReport HandlerTelemetryReport { set; get; }
    }




    [DataContract, Serializable]
    public class OperationTelemetryData
    {
        [DataMember, JsonProperty("COUNT")]
        public long Count { set; get; }

        [DataMember, JsonProperty("TIME", NullValueHandling = NullValueHandling.Ignore)]
        public TimeTelemetryData Time { set; get; }
    }

    [DataContract, Serializable]
    public class TimeTelemetryData
    {
        [DataMember, JsonProperty("AVG")]
        public long Average { set; get; }

        [DataMember, JsonProperty("MAX")]
        public long Max { set; get; }

        [DataMember, JsonProperty("MIN")]
        public long Min { set; get; }

        [DataMember, JsonProperty("TOTAL")]
        public double TotalTime { set; get; }
    }
}
