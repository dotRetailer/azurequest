using System.Collections.Generic;

namespace AzureQuest.Common
{
    public class OperationResult
    {
        public OperationResult() { }
        public OperationResult(bool success) { this.Success = success; }
        public OperationResult(bool success, string message) : this(success) { this.Message = message; }

        public bool Success { get; set; }
        public string RecordId { get; set; }
        public string Message { get; set; }
        public object Tag { get; set; }
        public int ExecutionTime { get; set; }
    }

    public class OperationResult<T> : OperationResult where T : class, new()
    {
        public OperationResult() : base() { }
        public OperationResult(T data) : base(true) { this.Data = data; }
        public OperationResult(bool success) : base(success) { }
        public OperationResult(bool success, string message) : base(success, message) { }

        public T Data { get; set; }
    }

    public class OperationResultList<T> : OperationResult where T : class, new()
    {
        public OperationResultList() : base() { }
        public OperationResultList(bool success) : base(success) { }
        public OperationResultList(bool success, string message) : base(success, message) { }
        public OperationResultList(IEnumerable<T> data) : base(true) { this.Data = data; }
        public OperationResultList(IEnumerable<T> data, int dataLength) : this(data) { this.DataLength = dataLength; }

        public IEnumerable<T> Data { get; set; }
        public int DataLength { get; set; }
    }

    public class BasePaginatedRequest 
    {
        public static int DefaultMaxResults = 500;

        private int firstRecord;
        public int FirstRecord
        {
            get { if (firstRecord <= 0) { firstRecord = 0; } return firstRecord; }
            set { firstRecord = value; }
        }

        private int maxRecords;
        public int MaxRecords
        {
            get { if (maxRecords <= 0) { maxRecords = DefaultMaxResults; } return maxRecords; }
            set { maxRecords = value; }
        }

        public List<DataOrderItem> Order { get; set; }
    }

    public class DataOrderItem 
    {
        public DataOrderItem() { }

        public DataOrderItem(string prop, bool asc)
        {
            this.Property = prop;
            this.Asc = asc;
        }

        public string Property { get; set; }
        public bool Asc { get; set; }
    }
}
